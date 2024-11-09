using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Models.Settings;
using CT_MKWII_WPF.Resources.Languages;
using CT_MKWII_WPF.Services;
using CT_MKWII_WPF.Services.Installation;
using CT_MKWII_WPF.Services.Launcher;
using CT_MKWII_WPF.Services.UrlProtocol;
using CT_MKWII_WPF.Views.Popups;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace CT_MKWII_WPF.Views.Pages
{
    public partial class ModsPage : Page, INotifyPropertyChanged
    {
        public ModManager ModManager => ModManager.Instance;
        public ObservableCollection<Mod> Mods => ModManager.Mods;

        private bool _toggleAll = true;
        private Point _startPoint;

        public ModsPage()
        {
            InitializeComponent();
            DataContext = this;
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            try
            {
                await ModManager.LoadModsAsync();
                foreach (var mod in Mods)
                {
                    mod.PropertyChanged += Mod_PropertyChanged;
                }
            }
            catch (Exception ex)
            {
                new YesNoWindow().SetMainText("Failed to load mods: " + ex.Message);
            }
            UpdateEmptyListMessageVisibility();
        }

        private void UpdateEmptyListMessageVisibility()
        {
            PageWithoutMods.Visibility = Mods.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            TopBarButtons.Visibility = Mods.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            PageWithMods.Visibility = Mods.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Mod_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Mod.IsEnabled) || e.PropertyName == nameof(Mod.Title) || e.PropertyName == nameof(Mod.Author) || e.PropertyName == nameof(Mod.ModID) || e.PropertyName == nameof(Mod.Priority))
                ModManager.SaveModsAsync();
            UpdateEmptyListMessageVisibility();
        }

        public void AskModImportType(object sender, RoutedEventArgs routedEventArgs)
        {
            var yesNoWindow = new YesNoWindow()
                .SetMainText("Would you like to open the Mod Browser or import mods manually?")
                .SetButtonText("Open Mod Browser", "Import Manually");

            var openModManager = yesNoWindow.AwaitAnswer();

            if (openModManager)
                openPopUp(null, null); // Open the Mod Manager window
            
            else
                ImportMod_Click(null, null); // Start the mod import process
            
        }

        private void ImportMod_Click(object sender, RoutedEventArgs e)
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
                ProcessModFiles(selectedFiles, singleMod: true);
            else
            {
                var result = new YesNoWindow().SetMainText(Phrases.PopupText_ModCombineQuestion)
                    .SetExtraText(Phrases.PopupText_MultipleFilesSelected).AwaitAnswer();
                if (!result) ProcessModFiles(selectedFiles, singleMod: false);
                else if (result) ProcessModFiles(selectedFiles, singleMod: true);
            }
        }

        private async void ProcessModFiles(string[] filePaths, bool singleMod)
        {
            ShowLoading(true);
            try
            {
                if (singleMod)
                    await CombineFilesIntoSingleMod(filePaths);
                else await InstallEachFileAsMod(filePaths);
            }
            catch (Exception ex)
            {
                MessageBoxWindow.Show($"Failed to process mod files: {ex.Message}");
            }

            ShowLoading(false);
        }

        private async Task InstallEachFileAsMod(string[] filePaths)
        {
            for (var i = 0; i < filePaths.Length; i++)
            {
                UpdateProgress(i + 1, filePaths.Length);
                var modName = Path.GetFileNameWithoutExtension(filePaths[i]);

                if (ModInstallation.ModExists(Mods, modName))
                {
                    MessageBoxWindow.Show(Humanizer.ReplaceDynamic(Phrases.PopupText_ModNameExists, modName));
                    continue;
                }
                var modDirectory = ModInstallation.GetModDirectoryPath(modName);
                CreateDirectory(modDirectory);

                // Pass default author and ModID
                await ModInstallation.InstallModFromFileAsync(filePaths[i], author: "-1", modID: -1);
            }
        }

        private async Task CombineFilesIntoSingleMod(string[] filePaths)
        {
            var modName = new TextInputPopup("Enter Mod Name").ShowDialog();
            if (!IsValidName(modName)) return;

            var modDirectory = ModInstallation.GetModDirectoryPath(modName);
            CreateDirectory(modDirectory);

            for (var i = 0; i < filePaths.Length; i++)
            {
                UpdateProgress(i + 1, filePaths.Length);
                // Pass default author and ModID
                await ModInstallation.InstallModFromFileAsync(filePaths[i], author: "-1", modID: -1);
            }

            var newMod = new Mod
            {
                IsEnabled = true, // Default to enabled
                Title = modName,
                Author = "-1",
                ModID = -1,
                Priority = 0 // Default priority; can be adjusted as needed
            };
            ModManager.AddMod(newMod);
        }

        private void UpdateProgress(int current, int total)
        {
            Dispatcher.Invoke(() =>
            {
                ProgressBar.Value = (double)current / total * 100;
                StatusTextBlock.Text = Humanizer.ReplaceDynamic(Phrases.PopupText_ProcessingXofY,
                                                                current, total);
            }, DispatcherPriority.Background);
        }

        private static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        private void ShowLoading(bool isLoading)
        {
            Dispatcher.Invoke(() =>
            {
                ProgressBar.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
                StatusTextBlock.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
            });
        }

        private void EnableClick(object sender, RoutedEventArgs e)
        {
            foreach (var mod in Mods)
            {
                mod.IsEnabled = _toggleAll;
            }

            _toggleAll = !_toggleAll;
            EnableDisableButton.Text = !_toggleAll ? Common.Action_DisableAll : Common.Action_EnableAll;
        }

        private bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBoxWindow.Show(Phrases.PopupText_ModNameEmpty);
                return false;
            }

            if (!ModInstallation.ModExists(Mods, name)) return true;

            MessageBoxWindow.Show(Humanizer.ReplaceDynamic(Phrases.PopupText_ModNameExists, name));
            return false;
        }

        private void RenameMod_Click(object sender, RoutedEventArgs e)
        {
            var selectedMod = ModsListView.GetCurrentContextItem<Mod>();
            if (selectedMod == null) return;

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

                ModManager.SaveModsAsync();
            }
            catch (IOException ex)
            {
                MessageBoxWindow.Show($"Failed to rename mod directory: {ex.Message}");
            }
        }

        private void DeleteMod_Click(object sender, RoutedEventArgs e)
        {
            var selectedMod = ModsListView.GetCurrentContextItem<Mod>();
            if (selectedMod == null) return;
            var areTheySure = new YesNoWindow().SetMainText(Humanizer.ReplaceDynamic(Phrases.PopupText_SureDeleteQuestion, selectedMod.Title)).AwaitAnswer();
            if (!areTheySure) return;

            ModManager.RemoveMod(selectedMod);
            try
            {
                var modDirectory = ModInstallation.GetModDirectoryPath(selectedMod.Title);
                if (Directory.Exists(modDirectory))
                    Directory.Delete(modDirectory, true);
            }
            catch (IOException)
            {
                MessageBoxWindow.Show($"Failed to delete mod directory. It may be that this file is read only?");
            }
            ModManager.SaveModsAsync();
            UpdateEmptyListMessageVisibility();
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            var selectedMod = ModsListView.GetCurrentContextItem<Mod>();
            if (selectedMod == null) return;

            var modDirectory = ModInstallation.GetModDirectoryPath(selectedMod.Title);
            if (Directory.Exists(modDirectory))
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = modDirectory,
                    UseShellExecute = true,
                    Verb = "open"
                });
            else
            {
                MessageBoxWindow.Show(Phrases.PopupText_NoModFolder);
            }
        }

        private void ModsListView_OnOnItemsReorder(ListViewItem movedItem, int newIndex)
        {
            var mySelectedMod = (Mod)movedItem.DataContext;
            Mods.Move(Mods.IndexOf(mySelectedMod), newIndex);
            // Update priority based on new order
            for (int i = 0; i < Mods.Count; i++)
            {
                Mods[i].Priority = i;
            }
            ModManager.SaveModsAsync();
        }

        private void CheckIfSet(object sender, RoutedEventArgs e)
        {
            bool isRegistered = UrlProtocolManager.IsCustomSchemeRegistered(UrlProtocolManager.ProtocolName);
            MessageBoxWindow.Show(isRegistered ? "The URL scheme is registered" : "The URL scheme is not registered");
        }

        private void RemoveUrlScheme(object sender, RoutedEventArgs e)
        {
            UrlProtocolManager.RemoveCustomScheme(UrlProtocolManager.ProtocolName);
        }

        private void openPopUp(object sender, RoutedEventArgs e)
        {
            var modPopup = new Views.Popups.ModPopupWindow();
            modPopup.Show();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
