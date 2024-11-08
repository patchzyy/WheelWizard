using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Models.Settings;
using CT_MKWII_WPF.Resources.Languages;
using CT_MKWII_WPF.Views.Popups;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Common;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using IniParser;
using IniParser.Model;

namespace CT_MKWII_WPF.Services.Installation
{
    public static class ModInstallation
    {
        private static readonly string _configFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "CT-MKWII", "Mods", "modconfig.json");

        private static readonly string _modsDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "CT-MKWII", "Mods");

        public static async Task<ObservableCollection<Mod>> LoadModsAsync()
        {
            var mods = new ObservableCollection<Mod>();
            try
            {
                // Ensure the mods directory exists
                if (!Directory.Exists(_modsDirectory))
                    Directory.CreateDirectory(_modsDirectory);

                // Check for INI files first
                var iniFiles = Directory.GetFiles(_modsDirectory, "*.ini", SearchOption.AllDirectories);
                if (iniFiles.Any())
                {
                    foreach (var iniFile in iniFiles)
                    {
                        
                        var mod = await Mod.LoadFromIniAsync(iniFile);
                        if (!string.IsNullOrWhiteSpace(mod.Title)) // Ensure title exists to avoid empty mod
                        {
                            mods.Add(mod);
                        }
                    }
                }
                else if (File.Exists(_configFilePath))
                {
                    // Backward compatibility: Load from JSON and convert to INI
                    var json = await File.ReadAllTextAsync(_configFilePath);
                    var modDataList = JsonSerializer.Deserialize<ObservableCollection<ModData>>(json) ?? new ObservableCollection<ModData>();

                    foreach (var modData in modDataList)
                    {
                        var mod = new Mod
                        {
                            Title = modData.Title,
                            IsEnabled = modData.IsEnabled,
                            Author = "-1", 
                            ModID = -1,     
                            Priority = 0 
                        };

                        // Create INI file
                        var modDirectory = GetModDirectoryPath(mod.Title);
                        if (!Directory.Exists(modDirectory))
                            Directory.CreateDirectory(modDirectory);

                        var iniFilePath = Path.Combine(modDirectory, $"{mod.Title}.ini");
                        await mod.SaveToIniAsync(iniFilePath);

                        mods.Add(mod);
                    }

                    // Delete the JSON config after conversion
                    File.Delete(_configFilePath);
                }
                else
                {
                    // No mods found; optionally, you can initialize an empty collection
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load mods: {ex.Message}");
            }

            // Sort mods based on priority
            var sortedMods = new ObservableCollection<Mod>(mods.OrderBy(m => m.Priority));
            return sortedMods;
        }

        public static async Task SaveModsAsync(ObservableCollection<Mod> mods)
        {
            try
            {
                foreach (var mod in mods)
                {
                    var modDirectory = GetModDirectoryPath(mod.Title);
                    if (!Directory.Exists(modDirectory))
                        Directory.CreateDirectory(modDirectory);

                    var iniFilePath = Path.Combine(modDirectory, $"{mod.Title}.ini");
                    await mod.SaveToIniAsync(iniFilePath);
                }

                // Optionally, delete the JSON config if it exists to ensure future loads use INI
                if (File.Exists(_configFilePath))
                {
                    File.Delete(_configFilePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save mods: {ex.Message}");
            }
        }

        public static string GetModDirectoryPath(string modName) =>
            Path.Combine(_modsDirectory, modName);

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
                        if (!Path.GetFullPath(entryDestinationPath).StartsWith(Path.GetFullPath(destinationDirectory), StringComparison.OrdinalIgnoreCase))
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
            try
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
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Installs a mod from a file. Sets Author and ModID based on parameters.
        /// </summary>
        public static async Task InstallModFromFileAsync(string filePath, string author = "-1", int modID = -1)
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
                
                // Prompt for mod name if not provided
                var inputPopup = new TextInputPopup("Enter Mod Name");
                var modName = inputPopup.ShowDialog();
                if (string.IsNullOrWhiteSpace(modName))
                {
                    return;
                }

                // Check if a mod with the same name already exists
                if (ModExists(ModManager.Instance.Mods, modName))
                {
                    ErrorMessageWindow.Show($"Mod with name '{modName}' already exists.");
                    return;
                }
                
                var modDirectory = GetModDirectoryPath(modName);
                if (!Directory.Exists(modDirectory))
                {
                    Directory.CreateDirectory(modDirectory);
                }
                await Task.Run(() => ProcessFile(filePath, modDirectory));

                // Create Mod instance
                var newMod = new Mod
                {
                    IsEnabled = true, // Default to enabled
                    Title = modName,
                    Author = author,
                    ModID = modID,
                    Priority = 0 // Default priority; can be adjusted as needed
                };

                // Save INI file
                var iniFilePath = Path.Combine(modDirectory, $"{modName}.ini");
                await newMod.SaveToIniAsync(iniFilePath);

                // Add to ModManager
                ModManager.Instance.AddMod(newMod);

                ErrorMessageWindow.Show($"Mod '{modName}' installed successfully.");
            }
            catch (Exception ex)
            {
                ErrorMessageWindow.Show($"Failed to install mod: {ex.Message}");
            }
        }
    }
}
