using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Models.Settings;
using CT_MKWII_WPF.Resources.Languages;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace CT_MKWII_WPF.Services.Installation
{
    public static class ModInstallation
    {
        private static readonly string _configFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "CT-MKWII", "Mods", "modconfig.json");

        public static async Task<ObservableCollection<Mod>> LoadModsAsync()
        {
            var mods = new ObservableCollection<Mod>();
            try
            {
                if (File.Exists(_configFilePath))
                {
                    var json = await File.ReadAllTextAsync(_configFilePath);
                    mods = JsonSerializer.Deserialize<ObservableCollection<Mod>>(json) ?? new ObservableCollection<Mod>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load mods: {ex.Message}");
            }

            return mods;
        }

        public static async Task SaveModsAsync(ObservableCollection<Mod> mods)
        {
            try
            {
                var json = JsonSerializer.Serialize(mods);
                var directory = Path.GetDirectoryName(_configFilePath);

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                await File.WriteAllTextAsync(_configFilePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save mods: {ex.Message}");
            }
        }

        public static string GetModDirectoryPath(string modName) =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CT-MKWII", "Mods", modName);

        public static bool ModExists(ObservableCollection<Mod> mods, string modName) =>
            mods.Any(mod => mod.Title.Equals(modName, StringComparison.OrdinalIgnoreCase));

        public static void ProcessFile(string file, string destinationDirectory)
        {
            if (Path.GetExtension(file).Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                if (!Directory.Exists(destinationDirectory))
                    Directory.CreateDirectory(destinationDirectory);
                try
                {
                    var zipFileName = Path.GetFileNameWithoutExtension(file);
                    var modName = Path.Combine(destinationDirectory, zipFileName);
                    if (Directory.Exists(modName))
                        throw new IOException("Mod with the same name already exists.");

                    ZipFile.ExtractToDirectory(file, destinationDirectory);
                }
                catch (IOException exp)
                {
                    throw new IOException("You already have a mod with this name." + exp.Message);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to extract zip file: {ex.Message}");
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
                    throw new Exception($"Failed to copy file: {ex.Message}");
                }
            }
        }
        
        public static async Task InstallModFromFileAsync(string filePath)
        {
            try
            {
                if (!Path.GetExtension(filePath).Equals(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Only .zip files are supported for mod installation.");
                }

                // Get the mod name from the .zip file name
                var modName = Microsoft.VisualBasic.Interaction
                    .InputBox(Phrases.PopupText_EnterModName, Common.Attribute_ModName, "New Mod");

                // Check if a mod with the same name already exists
                if (ModManager.Instance.Mods.Any(mod => mod.Title.Equals(modName, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show($"Mod with name '{modName}' already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                var modDirectory = GetModDirectoryPath(modName);
                if (!Directory.Exists(modDirectory))
                {
                    Directory.CreateDirectory(modDirectory);
                }
                await Task.Run(() => ProcessFile(filePath, modDirectory));
                var newMod = new Mod
                {
                    IsEnabled = false,
                    Title = modName
                };
                ModManager.Instance.AddMod(newMod);
                MessageBox.Show($"Mod '{modName}' installed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to install mod: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
