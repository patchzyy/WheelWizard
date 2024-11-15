using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using WheelWizard.Helpers;
using WheelWizard.Models.Settings;
using WheelWizard.Resources.Languages;
using WheelWizard.Services.Installation;
using WheelWizard.Services.Launcher;
using WheelWizard.Views.Popups;

namespace WheelWizard.Services;

public class ModManager : INotifyPropertyChanged
{
    private static readonly Lazy<ModManager> _instance = new(() => new ModManager());
    public static ModManager Instance => _instance.Value;

    private ObservableCollection<Mod> _mods;
    public ObservableCollection<Mod> Mods
    {
        get => _mods;
        private set
        {
            _mods = value;
            OnPropertyChanged();
        }
    }

    private bool _isProcessing;

    private ModManager()
    {
        _mods = new ObservableCollection<Mod>();
        LoadModsAsync();
    }

    // Events to communicate with the frontend
    public event Action? ModsLoaded;
    public event Action? ModsChanged;
    public event Action? ModProcessingStarted;
    public event Action? ModProcessingCompleted;
    public event Action<int, int, string>? ModProcessingProgress;
    public event Action<string>? ErrorOccurred;

    public bool IsModInstalled(int modID)
    {
        return Mods.Any(mod => mod.ModID == modID);
    }

    public async void InitializeAsync()
    {
        await LoadModsAsync();
        ModsLoaded?.Invoke();
    }

    public async Task LoadModsAsync()
    {
        try
        {
            Mods = await ModInstallation.LoadModsAsync();
            foreach (var mod in Mods)
            {
                mod.PropertyChanged += Mod_PropertyChanged;
            }
            OnPropertyChanged(nameof(Mods));
            ModsChanged?.Invoke();
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke($"Failed to load mods: {ex.Message}");
        }
    }

    public async Task SaveModsAsync()
    {
        try
        {
            await ModInstallation.SaveModsAsync(Mods);
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke($"Failed to save mods: {ex.Message}");
        }
    }

    public void AddMod(Mod mod)
    {
        if (!ModInstallation.ModExists(Mods, mod.Title))
        {
            mod.PropertyChanged += Mod_PropertyChanged;
            Mods.Add(mod);
            SortModsByPriority();
            _ = SaveModsAsync(); //this calls async method without awaiting it and no warning :)
            ModsChanged?.Invoke();
        }
    }

    public void RemoveMod(Mod mod)
    {
        if (Mods.Contains(mod))
        {
            Mods.Remove(mod);
            _ = SaveModsAsync();
            ModsChanged?.Invoke();
        }
    }

    private void Mod_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Mod.IsEnabled) ||
            e.PropertyName == nameof(Mod.Title) ||
            e.PropertyName == nameof(Mod.Author) ||
            e.PropertyName == nameof(Mod.ModID) ||
            e.PropertyName == nameof(Mod.Priority))
        {
            _ = SaveModsAsync();
            SortModsByPriority();
            ModsChanged?.Invoke();
        }
    }

    private void SortModsByPriority()
    {
        var sortedMods = new ObservableCollection<Mod>(Mods.OrderBy(m => m.Priority));
        Mods = sortedMods;
        OnPropertyChanged(nameof(Mods));
    }

    public async void ImportMods()
    {
        var joinedExtensions = string.Join(";", ModsLaunchHelper.AcceptedModExtensions);
        joinedExtensions += ";*.zip";
        var openFileDialog = new OpenFileDialog
        {
            Filter = $"Mod files ({joinedExtensions})|{joinedExtensions}|All files (*.*)|*.*",
            Title = "Select Mod File",
            Multiselect = true
        };

        if (openFileDialog.ShowDialog() != true) return;

        var selectedFiles = openFileDialog.FileNames;

        if (selectedFiles.Length == 1)
            await ProcessModFilesAsync(selectedFiles, singleMod: true);
        else
        {
            var result = new YesNoWindow().SetMainText(Phrases.PopupText_ModCombineQuestion)
                .SetExtraText(Phrases.PopupText_MultipleFilesSelected).AwaitAnswer();
            if (!result)
                await ProcessModFilesAsync(selectedFiles, singleMod: false);
            else
                await ProcessModFilesAsync(selectedFiles, singleMod: true);
        }
    }

    private async Task ProcessModFilesAsync(string[] filePaths, bool singleMod)
    {
        ShowProcessing(true);
        try
        {
            if (singleMod)
                await CombineFilesIntoSingleModAsync(filePaths);
            else
                await InstallEachFileAsModAsync(filePaths);
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke($"Failed to process mod files: {ex.Message}");
        }
        ShowProcessing(false);
    }

    private async Task InstallEachFileAsModAsync(string[] filePaths)
    {
        ModProcessingStarted?.Invoke();
        var total = filePaths.Length;
        for (var i = 0; i < filePaths.Length; i++)
        {
            var modName = Path.GetFileNameWithoutExtension(filePaths[i]);

            if (ModInstallation.ModExists(Mods, modName))
            {
                ErrorOccurred?.Invoke(Humanizer.ReplaceDynamic(Phrases.PopupText_ModNameExists, modName));
                continue;
            }

            var modDirectory = ModInstallation.GetModDirectoryPath(modName);
            CreateDirectory(modDirectory);

            await ModInstallation.InstallModFromFileAsync(filePaths[i], modName,author: "-1", modID: -1);

            ModProcessingProgress?.Invoke(i + 1, total, Humanizer.ReplaceDynamic(Phrases.PopupText_ProcessingXofY, i + 1, total));
        }
        await LoadModsAsync();
        ModProcessingCompleted?.Invoke();
    }

    private async Task CombineFilesIntoSingleModAsync(string[] filePaths)
    {
        var modName = new TextInputPopup("Enter Mod Name").ShowDialog();
        if (!IsValidName(modName)) return;
        var tempZipPath = Path.Combine(Path.GetTempPath(), $"{modName}.zip");
        try
        {
            using (var zipArchive = ZipFile.Open(tempZipPath, ZipArchiveMode.Create))
            {
                foreach (var filePath in filePaths)
                {
                    var entryName = Path.GetFileName(filePath);
                    zipArchive.CreateEntryFromFile(filePath, entryName, CompressionLevel.Optimal);
                }
            }
            await ModInstallation.InstallModFromFileAsync(tempZipPath, modName, author: "-1", modID: -1);
            if (File.Exists(tempZipPath))
                File.Delete(tempZipPath);
            ModProcessingCompleted?.Invoke();
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke($"Failed to combine and install mod: {ex.Message}");
        }
    }

    public void ToggleAllMods(bool enable)
    {
        foreach (var mod in Mods)
        {
            mod.IsEnabled = enable;
        }
        _isProcessing = !_isProcessing;
        ModsChanged?.Invoke();
    }

    public void RenameMod(Mod selectedMod)
    {
        var newTitle = new TextInputPopup("Enter Mod Title").ShowDialog();
        if (!IsValidName(newTitle)) return;

        try
        {
            var oldDirectoryName = ModInstallation.GetModDirectoryPath(selectedMod.Title);
            var newDirectoryName = ModInstallation.GetModDirectoryPath(newTitle);
            Directory.Move(oldDirectoryName, newDirectoryName);
            selectedMod.Title = newTitle; // Trigger property changed

            // Rename INI file
            var oldIniPath = Path.Combine(oldDirectoryName, $"{selectedMod.Title}.ini");
            var newIniPath = Path.Combine(newDirectoryName, $"{newTitle}.ini");
            if (File.Exists(oldIniPath))
            {
                File.Move(oldIniPath, newIniPath);
            }

            SaveModsAsync();
            ModsChanged?.Invoke();
        }
        catch (IOException ex)
        {
            ErrorOccurred?.Invoke($"Failed to rename mod directory: {ex.Message}");
        }
    }

    public void DeleteMod(Mod selectedMod)
    {
        var areTheySure = new YesNoWindow().SetMainText(Humanizer.ReplaceDynamic(Phrases.PopupText_SureDeleteQuestion, selectedMod.Title)).AwaitAnswer();
        if (!areTheySure) return;

        RemoveMod(selectedMod);
        try
        {
            var modDirectory = ModInstallation.GetModDirectoryPath(selectedMod.Title);
            if (Directory.Exists(modDirectory))
                Directory.Delete(modDirectory, true);
        }
        catch (IOException)
        {
            ErrorOccurred?.Invoke($"Failed to delete mod directory. It may be that this file is read only?");
        }
        SaveModsAsync();
        ModsChanged?.Invoke();
    }

    public void OpenModFolder(Mod selectedMod)
    {
        var modDirectory = ModInstallation.GetModDirectoryPath(selectedMod.Title);
        if (Directory.Exists(modDirectory))
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = modDirectory,
                UseShellExecute = true,
                Verb = "open"
            });
        }
        else
        {
            ErrorOccurred?.Invoke(Phrases.PopupText_NoModFolder);
        }
    }
    
    public void DeleteModById(int modId)
    {
        var modToDelete = Mods.FirstOrDefault(mod => mod.ModID == modId);
    
        if (modToDelete == null)
        {
            ErrorOccurred?.Invoke($"No mod found with ID: {modId}");
            return;
        }
        DeleteMod(modToDelete);
    }


    public void ReorderMod(Mod movedMod, int newIndex)
    {
        var oldIndex = Mods.IndexOf(movedMod);
        if (oldIndex < 0 || newIndex < 0 || newIndex >= Mods.Count) return;

        Mods.Move(oldIndex, newIndex);
        for (var i = 0; i < Mods.Count; i++)
        {
            Mods[i].Priority = i;
        }
        SaveModsAsync();
        ModsChanged?.Invoke();
    }

    // Helper Methods

    private bool IsValidName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ErrorOccurred?.Invoke(Phrases.PopupText_ModNameEmpty);
            return false;
        }

        if (!ModInstallation.ModExists(Mods, name)) return true;

        ErrorOccurred?.Invoke(Humanizer.ReplaceDynamic(Phrases.PopupText_ModNameExists, name));
        return false;
    }

    private static void CreateDirectory(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    private void ShowProcessing(bool isProcessing)
    {
        if (isProcessing)
            ModProcessingStarted?.Invoke();
        else
            ModProcessingCompleted?.Invoke();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

