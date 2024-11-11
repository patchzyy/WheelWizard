using CT_MKWII_WPF.Models.Settings;
using CT_MKWII_WPF.Views.Popups;
using SharpCompress.Archives;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CT_MKWII_WPF.Services.Installation;
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
            using var archive = OpenArchive(file, extension);
            if (archive == null)
                throw new Exception($"Unsupported archive format: {extension}");
    
            foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
            {
                // Normalize entry path by removing empty folder segments
                var sanitizedKey = string.Join(Path.DirectorySeparatorChar.ToString(),
                    entry.Key.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                        .Where(segment => !string.IsNullOrWhiteSpace(segment)));
    
                var entryDestinationPath = Path.Combine(destinationDirectory, sanitizedKey);
    
                // Ensure the entry destination path is within the destination directory
                if (!Path.GetFullPath(entryDestinationPath).StartsWith(Path.GetFullPath(destinationDirectory), StringComparison.OrdinalIgnoreCase))
                {
                    throw new UnauthorizedAccessException("Entry is attempting to extract outside of the destination directory.");
                }
    
                // Create directory structure for the file
                var directoryPath = Path.GetDirectoryName(entryDestinationPath);
                if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
    
                // Extract the file
                using var stream = entry.OpenEntryStream();
                using var fileStream = File.Create(entryDestinationPath);
                stream.CopyTo(fileStream);
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
    public static async Task InstallModFromFileAsync(string filePath, string givenModName, string author = "-1", int modID = -1)
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
            if (string.IsNullOrWhiteSpace(givenModName))
            {
                return;
            }

            // Check if a mod with the same name already exists
            if (ModExists(ModManager.Instance.Mods, givenModName))
            {
                MessageBoxWindow.ShowDialog($"Mod with name '{givenModName}' already exists.");
                return;
            }
            
            var modDirectory = GetModDirectoryPath(givenModName);
            if (!Directory.Exists(modDirectory))
            {
                Directory.CreateDirectory(modDirectory);
            }
            await Task.Run(() => ProcessFile(filePath, modDirectory));

            // Create Mod instance
            var newMod = new Mod
            {
                IsEnabled = true, // Default to enabled
                Title = givenModName,
                Author = author,
                ModID = modID,
                Priority = 0 // Default priority; can be adjusted as needed
            };

            // Save INI file
            var iniFilePath = Path.Combine(modDirectory, $"{givenModName}.ini");
            await newMod.SaveToIniAsync(iniFilePath);

            // Add to ModManager
            ModManager.Instance.AddMod(newMod);

            MessageBoxWindow.ShowDialog($"Mod '{givenModName}' installed successfully.", MessageBoxWindow.MessageType.Message);
        }
        catch (Exception ex)
        {
            MessageBoxWindow.ShowDialog($"Failed to install mod: {ex.Message}");
        }
    }
}

