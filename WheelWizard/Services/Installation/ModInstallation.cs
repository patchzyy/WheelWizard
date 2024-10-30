using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Models.Settings;
using CT_MKWII_WPF.Resources.Languages;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Common;
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
            var extension = Path.GetExtension(file).ToLowerInvariant();

            if (!Directory.Exists(destinationDirectory))
                Directory.CreateDirectory(destinationDirectory);

            try
            {
                // Determine the archive type and extract accordingly
                using (var archive = OpenArchive(file, extension))
                {
                    if (archive == null)
                        throw new Exception($"Unsupported archive format: {extension}");

                    foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                    {
                        // Construct the full path for the entry's destination
                        var entryDestinationPath = Path.Combine(destinationDirectory, entry.Key);
                
                        // Ensure the entry destination path is within the destination directory
                        if (!entryDestinationPath.StartsWith(destinationDirectory, StringComparison.OrdinalIgnoreCase))
                        {
                            throw new UnauthorizedAccessException("Entry is attempting to extract outside of the destination directory.");
                        }

                        entry.WriteToDirectory(destinationDirectory, new ExtractionOptions
                        {
                            ExtractFullPath = true,
                            Overwrite = true
                        });
                    }
                }
            }
            catch (IOException exp)
            {
                throw new IOException("You already have a mod with this name. " + exp.Message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to extract archive file: {ex.Message}");
            }
        }
        
        private static IArchive OpenArchive(string filePath, string extension)
        {
            switch (extension)
            {
                case ".zip":
                case ".7z":
                case ".rar":
                    return ArchiveFactory.Open(filePath);
                default:
                    return null;
            }
        }
        public static async Task InstallModFromFileAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException("File not found.", filePath);
                var extension = Path.GetExtension(filePath).ToLowerInvariant();
                if (extension != ".zip" && extension != ".7z" && extension != ".rar")
                {
                    throw new InvalidOperationException($"Unsupported file type: {extension}. Only .zip, .7z, and .rar files are supported.");
                }
                
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
