using System.IO;
using System.Windows;

namespace CT_MKWII_WPF.Utils;

public class RiivolutionLauncher
{
    public static void Launch_pre_requisits()
    {
        //we have 2 ways of starting, depending on the current structure of the folders
        //if the load folder only contains a Riivolution folder, we have to check if this riivolution folder is a RR folder or a Riivolution folder
        // if riivolution contains RetroRewind6 then it is a RR folder, otherwise for now just assume its a riivolution folder
        //if it is a RR folder, then we take out RR and put it in Riivolution-RR
        
        //first check if RR is active
        
        if (DirectoryHandler.isRRActive())
        {
            DirectoryHandler.BackupRR();
        }
        //now check if Riivolution is active
        if (!DirectoryHandler.isRiivolutionActive())
        {
            MessageBox.Show("Riivolution is not active, error");
        }
    }
}