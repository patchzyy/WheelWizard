using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace CT_MKWII_WPF.Utils;

public class RiivolutionInstaller
{
    public static bool IsRiivolutionInstalled()
    {
        //if RR is in the riivolution folder, then go check Riivolution-OG
        var LoadPath = SettingsUtils.GetLoadPathLocation();
        bool RiiAndRR = Path.Exists(Path.Combine(LoadPath, "Riivolution", "RetroRewind6"));
        var PathToCheck = Path.Combine(LoadPath, "Riivolution");
        if (RiiAndRR)
        {
            PathToCheck = Path.Combine(LoadPath, "Riivolution-OG");
        }
        return Directory.Exists(PathToCheck);

              
    }
    
    static string RiivolutionURL = "https://aerialx.github.io/rvlution.net/riivolution.zip";
    
    public static void InstallRiivolution()
    {
        if (DirectoryHandler.isRRActive())
        {
            DirectoryHandler.BackupRR();
        }
        
        //since Riivolution has not had an update since 2013, i will NOT be implementing any update logic
        //check if riivolution exists
        if (IsRiivolutionInstalled())
        {
            MessageBox.Show("Riivolution is already installed");
            return;
        }
        //check if RR is in the active Directory, if it is remove it
        //else create the folder
        Directory.CreateDirectory(Path.Combine(SettingsUtils.GetLoadPathLocation(), "Riivolution"));
        //download the zip
        using (var client = new HttpClient())
        {
            var response = client.GetAsync(RiivolutionURL).Result;
            if (response.IsSuccessStatusCode)
            {
                using (var stream = response.Content.ReadAsStream())
                {
                    using (var fileStream = new FileStream(Path.Combine(SettingsUtils.GetLoadPathLocation(), "Riivolution", "riivolution.zip"), FileMode.Create, FileAccess.Write))
                    {
                        stream.CopyTo(fileStream);
                    }
                }
            }
            else
            {
                MessageBox.Show("Failed to download Riivolution");
                return;
            }
            //extract the zip
            System.IO.Compression.ZipFile.ExtractToDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dolphin Emulator", "Load", "Riivolution", "riivolution.zip"), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dolphin Emulator", "Load", "Riivolution"));
            //delete the zip
            File.Delete(Path.Combine(SettingsUtils.GetLoadPathLocation(), "Riivolution", "riivolution.zip"));
            //message box confirming the installation
            MessageBox.Show("Riivolution has been installed");
                
        }
        
        
        
        
    }

}