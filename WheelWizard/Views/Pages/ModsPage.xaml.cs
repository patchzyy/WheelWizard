using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Windows.Threading;
using CT_MKWII_WPF.Utils;


namespace CT_MKWII_WPF.Views.Pages
{
    public partial class ModsPage : Page
    {
        private readonly string configFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "CT-MKWII", "Mods", "modconfig.json");
        public ObservableCollection<Mod> Mods { get; set; }

        private bool _toggleAll = true;
        private Point startPoint;
        public ModsPage()
        {
            InitializeComponent();
            LoadMods();
            
            ModsListView.DataContext = this; 
        }
        
        private void LoadMods()
        {
            try
            {
                if (File.Exists(configFilePath))
                {
                    var json = File.ReadAllText(configFilePath);
                    Mods = JsonSerializer.Deserialize<ObservableCollection<Mod>>(json) ?? new ObservableCollection<Mod>();
                }
                else Mods = new ObservableCollection<Mod>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load mods: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Mods = new ObservableCollection<Mod>();
            }

            foreach (var mod in Mods)  
                mod.PropertyChanged += Mod_PropertyChanged;
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
            if (e.PropertyName == nameof(Mod.IsEnabled))
                SaveMods();
            UpdateEmptyListMessageVisibility();
        }
        
        private void SaveMods()
        {
            try
            {
                var json = JsonSerializer.Serialize(Mods);
                var directory = Path.GetDirectoryName(configFilePath);
                
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                File.WriteAllText(configFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save mods: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void ImportMod_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Mod files (*.zip;*.brstm;*.szs)|*.zip;*.brstm;*.szs|All files (*.*)|*.*",
                Title = "Select Mod File",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var selectedFiles = openFileDialog.FileNames;

                if (selectedFiles.Length == 1) 
                    ProcessModFiles(selectedFiles, singleMod: true);
                else
                {
                    var result = MessageBox.Show("Do you want to combine all files into 1 mod?", 
                        "Multiple Files Selected", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                    if (result == MessageBoxResult.No) ProcessModFiles(selectedFiles, singleMod: false);
                    else if (result == MessageBoxResult.Yes) ProcessModFiles(selectedFiles, singleMod: true);
                }
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
                MessageBox.Show($"Failed to process mod files: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            ShowLoading(false);
        }


        private bool ModExists(string modName) =>
            Mods.Any(mod => mod.Title.Equals(modName, StringComparison.OrdinalIgnoreCase));
        
        private async Task InstallEachFileAsMod(string[] filePaths)
        {
            for (int i = 0; i < filePaths.Length; i++)
            {
                UpdateProgress(i + 1, filePaths.Length);
                var modName = Path.GetFileNameWithoutExtension(filePaths[i]);

                if (ModExists(modName))
                {
                    MessageBox.Show($"A mod with the name '{modName}' already exists.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    continue;
                }

                var modDirectory = GetModDirectoryPath(modName);
                CreateDirectory(modDirectory);
                await Task.Run(() => ProcessFile(filePaths[i], modDirectory));
                AddMod(modName);
            }
        }
        
        private async Task CombineFilesIntoSingleMod(string[] filePaths)
        {
            string modName = Microsoft.VisualBasic.Interaction
                .InputBox("Enter mod name:", "Mod Name", "New Mod");
            if (!IsValidName(modName)) return;
            var modDirectory = GetModDirectoryPath(modName);
            CreateDirectory(modDirectory);
            for (int i = 0; i < filePaths.Length; i++)
            {
                UpdateProgress(i + 1, filePaths.Length);
                await Task.Run(() => ProcessFile(filePaths[i], modDirectory));
            }
            AddMod(modName);
        }
        
        private void AddMod(string modName)
        {
            if (!Mods.Any(mod => mod.Title.Equals(modName, StringComparison.OrdinalIgnoreCase)))
            {
                var mod = new Mod
                {
                    IsEnabled = false,
                    Title = modName,
                };
                mod.PropertyChanged += Mod_PropertyChanged;
                Mods.Add(mod);
                SaveMods();
            }
            UpdateEmptyListMessageVisibility();
        }
        
        private void ProcessFile(string file, string destinationDirectory)
        {
            if (Path.GetExtension(file).Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                if (!Directory.Exists(destinationDirectory)) 
                    Directory.CreateDirectory(destinationDirectory);
                try
                {
                    // Extract the zip file to the destination directory
                    //get name of the zip file
                    string zipFileName = Path.GetFileNameWithoutExtension(file);
                    //now we check if there isn't already a folder with the same name as the zip file, if so... cancel
                    if (Directory.Exists(Path.Combine(destinationDirectory, zipFileName)))
                    {
                        MessageBox.Show($"You already have a mod with this name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    ZipFile.ExtractToDirectory(file, destinationDirectory);
                }
                catch (IOException)
                {
                    //if file already exists, we catch the exception and show a message
                    MessageBox.Show($"You already have a mod with this name", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to extract zip file.\nThis is most likely because there is " +
                                    $"an invalid folder name. Or the ZIP might be password protected\n {ex.Message}", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                try
                {
                    if (!Directory.Exists(destinationDirectory)) 
                        Directory.CreateDirectory(destinationDirectory);
                    
                    var destFile = Path.Combine(destinationDirectory, Path.GetFileName(file));
                    File.Copy(file, destFile, overwrite: true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to copy file: {ex.Message}", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
        private void UpdateProgress(int current, int total)
        {
            Dispatcher.Invoke(() =>
            {
                ProgressBar.Value = (double)current / total * 100;
                StatusTextBlock.Text = $"Processing {current} of {total} files...";
            }, DispatcherPriority.Background);
        }
        
        private static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path)) 
                Directory.CreateDirectory(path);
        }
        
        private static string GetModDirectoryPath(string modName) => 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                "CT-MKWII", "Mods", modName);
        
        
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
            //bool selectAll = Mods.Any(mod => !mod.IsEnabled);
            foreach (var mod in Mods)
                mod.IsEnabled = _toggleAll;
            _toggleAll = !_toggleAll;
            if (!_toggleAll)
                EnableDisableButton.Text = "Disable All";
            else EnableDisableButton.Text = "Enable All";
        }

        private bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show($"Mod name can't be empty", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (Mods.Any(mod => mod.Title.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show($"A mod with the name '{name}' already exists.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        } 
        
        private void RenameMod_Click(object sender, RoutedEventArgs e)
        {
            var selectedMod = ModsListView.GetCurrentContextItem<Mod>();
            if (selectedMod == null) return;
            
            string newTitle = Microsoft.VisualBasic.Interaction
                .InputBox("Enter new title:", "Rename Mod", selectedMod.Title);
            if (!IsValidName(newTitle)) return;
            
            try
            {
                string oldDirectoryName = GetModDirectoryPath(selectedMod.Title);
                string newDirectoryName = GetModDirectoryPath(newTitle);
                Directory.Move(oldDirectoryName, newDirectoryName);
                selectedMod.Title = newTitle; // rename only after the directory, just in case that fails, then 
                // this renaming also does not run anymore
                SaveMods();
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Failed to rename mod directory: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteMod_Click(object sender, RoutedEventArgs e)
        {
            var selectedMod = ModsListView.GetCurrentContextItem<Mod>();
            if (selectedMod == null) return;
            var areTheySure = MessageBox.Show($"Are you sure you want to delete {selectedMod.Title}?",
                "Delete Mod", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
            if (!areTheySure) return;
    
            Mods.Remove(selectedMod);
            try
            {
                string modDirectory = GetModDirectoryPath(selectedMod.Title);
                if (Directory.Exists(modDirectory))
                    Directory.Delete(modDirectory, true);
            }
            catch (IOException)
            {
                MessageBox.Show($"Failed to delete mod directory. It may be that this file is read only?", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            SaveMods();
            
            UpdateEmptyListMessageVisibility();
        }
        
        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            var selectedMod = ModsListView.GetCurrentContextItem<Mod>();
            if (selectedMod == null) return;
            
            string modDirectory = GetModDirectoryPath(selectedMod!.Title);
            if (Directory.Exists(modDirectory))
                System.Diagnostics.Process.Start("explorer", modDirectory);
            else MessageBox.Show("Mod folder does not exist", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ModsListView_OnOnItemsReorder(ListViewItem movedItem, int newIndex)
        {
            var mySelectedMod = (Mod)movedItem.DataContext;
            // This is also done inside the component, but that does not translate outside of the component
            // for now SaveMods uses the Mods property, so therefore we also have to update it here
            Mods.Remove(mySelectedMod);  
            Mods.Insert(newIndex, mySelectedMod); 
            
            SaveMods();
        }
    }
}
