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
using WheelWizard.Views.Popups.Generic;

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
        if (ModInstallation.ModExists(Mods, mod.Title)) return;

        mod.PropertyChanged += Mod_PropertyChanged;
        Mods.Add(mod);
        SortModsByPriority();
        _ = SaveModsAsync(); //this calls async method without awaiting it and no warning :)
        ModsChanged?.Invoke();
    }

    public void RemoveMod(Mod mod)
    {
        if (!Mods.Contains(mod)) return;

        Mods.Remove(mod);
        _ = SaveModsAsync();
        ModsChanged?.Invoke();
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
            // SortModsByPriority();
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
        await ProcessModFilesAsync(selectedFiles);

    }

    private async Task ProcessModFilesAsync(string[] filePaths)
    {
        ShowProcessing(true);
        try
        {
            await CombineFilesIntoSingleModAsync(filePaths);
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke($"Failed to process mod files: {ex.Message}");
        }
        ShowProcessing(false);
    }

    private async Task CombineFilesIntoSingleModAsync(string[] filePaths)
    {
        var modName = new WPFViews.Popups.Generic.TextInputPopup("Enter Mod Name").ShowDialog();
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

    public async void RenameMod(Mod selectedMod)
    {
        var newTitle = await new TextInputWindow().setLabelText("Enter Mod Title").ShowDialog();
        if (!IsValidName(newTitle)) return;

        var oldTitle = selectedMod.Title;
        var oldDirectoryName = ModInstallation.GetModDirectoryPath(oldTitle);
        var newDirectoryName = ModInstallation.GetModDirectoryPath(newTitle);

        // Construct the .ini paths
        var oldIniPath = Path.Combine(oldDirectoryName, $"{oldTitle}.ini"); // Path before directory rename
    
        try
        {
            Directory.Move(oldDirectoryName, newDirectoryName);
            var updatedOldIniPath = Path.Combine(newDirectoryName, $"{oldTitle}.ini"); // Path after directory rename
            var newIniPath = Path.Combine(newDirectoryName, $"{newTitle}.ini");
            if (File.Exists(updatedOldIniPath))
            {
                File.Move(updatedOldIniPath, newIniPath);
            }

            selectedMod.Title = newTitle;
            SaveModsAsync(); // Ideally: await SaveModsAsync();
            ModsChanged?.Invoke();
        }
        catch (IOException ex)
        {
            ErrorOccurred?.Invoke($"Failed to rename mod directory: {ex.Message}");
        }
    }


    public async void DeleteMod(Mod selectedMod)
    {
        var areTheySure = await new YesNoWindow().SetMainText(Humanizer.ReplaceDynamic(Phrases.PopupText_SureDeleteQuestion, selectedMod.Title)).AwaitAnswer();
        if (!areTheySure) return;

        var modDirectory = ModInstallation.GetModDirectoryPath(selectedMod.Title);
        Console.WriteLine($"Attempting to delete mod directory: {modDirectory}");

        if (!Directory.Exists(modDirectory))
        {
            Console.WriteLine($"Mod directory not found: {modDirectory}");
            RemoveMod(selectedMod);
            return;
        }
        try
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            var di = new DirectoryInfo(modDirectory);
            di.Attributes &= ~FileAttributes.ReadOnly; 
            foreach (var file in di.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                file.Attributes &= ~FileAttributes.ReadOnly;
            }
            Directory.Delete(modDirectory, true); // true for recursive deletion
            RemoveMod(selectedMod);
        }
        catch (Exception ex) // Catch a more general exception
        {
            ErrorOccurred?.Invoke($"Failed to delete mod directory: {ex.Message}");
        }
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

