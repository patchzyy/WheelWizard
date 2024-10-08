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

namespace CT_MKWII_WPF.Services.Installation;

public static class ModInstallation
{
    private static readonly string _configFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "CT-MKWII", "Mods", "modconfig.json");
    public static void ProcessFile(string file, string destinationDirectory)
        {
            if (Path.GetExtension(file).Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                if (!Directory.Exists(destinationDirectory))
                    Directory.CreateDirectory(destinationDirectory);
                try
                {
                    var zipFileName = Path.GetFileNameWithoutExtension(file);
                    //now we check if there isn't already a folder with the same name as the zip file, if so... cancel
                    var modName = Path.Combine(destinationDirectory, zipFileName);
                    if (Directory.Exists(modName))
                    {
                        MessageBox.Show(Humanizer.ReplaceDynamic(Phrases.PopupText_ModNameExists, modName), 
                                        Common.Term_Error, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    ZipFile.ExtractToDirectory(file, destinationDirectory);
                }
                catch (IOException)
                {
                    //if file already exists, we catch the exception and show a message
                    MessageBox.Show($"You already have a mod with this name", Common.Term_Error,
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
                    MessageBox.Show($"Failed to copy file: {ex.Message}", Common.Term_Error,
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    
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
    
}
