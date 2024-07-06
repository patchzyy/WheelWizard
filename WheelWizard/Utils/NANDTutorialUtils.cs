using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Management;
using System.Windows.Media.Animation;
using FlowDirection = System.Windows.Forms.FlowDirection;
using MessageBox = System.Windows.MessageBox;


namespace CT_MKWII_WPF.Utils;

public class NANDTutorialUtils
{
    public static void RunNANDTutorial()
    {
        bool AlreadyhasNAND = MessageBox.Show("Make sure to read every message carefully to avoid any  confusion.\nDo you already have your NAND setup in Dolphin? ", "NAND Setup", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        
        if (AlreadyhasNAND)
        {
            return;
        }
        //ask if the user has a wii on standby
        bool hasWii = MessageBox.Show("Do you have a Wii or Wii-U ready to go?", "NAND Setup", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        if (!hasWii)
        {
            MessageBox.Show("To get a NAND to play online, you need your own Wii or Wii-U, You can still play offline without NAND!", "NAND Setup", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        bool hasSDcard = MessageBox.Show("Do you have an SD card?", "NAND Setup", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        if (!hasSDcard)
        {
            MessageBox.Show("You need an SD card to get your NAND, you can still play offline without NAND!", "NAND Setup", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        bool wantSDcardFormattingHelp = MessageBox.Show("Do you need help formatting your SD card to the proper File System?\nThis will remove ALL files from your SD card.", "NAND Setup", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        if (wantSDcardFormattingHelp)
        {
            MessageBox.Show("Please plug in your SD card and click OK once your computer recognizes it", "NAND Setup", MessageBoxButton.OK, MessageBoxImage.Information);
            MessageBox.Show("Please select your SD drive" , "NAND Setup", MessageBoxButton.OK, MessageBoxImage.Information);
            //pop up to ask where the SD card is
            FolderBrowserDialog folderbrowser = new FolderBrowserDialog();
            folderbrowser.Description = "Select your SD card";
            folderbrowser.RootFolder = Environment.SpecialFolder.MyComputer;

            if (folderbrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string folderPath = folderbrowser.SelectedPath;
                //check to see if they have only selected a drive, the output is something like "D:\"
                if (folderPath.Length > 4)
                {
                    MessageBox.Show("You have selected a drive, not a folder, please select your SD card", "NAND Setup", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                //check to see if its not bigger then 32GB
                DriveInfo drive = new DriveInfo(folderPath);
                if (drive.TotalSize > 32000000000)
                {
                    MessageBox.Show("Your SD card is too big, You may have selected a actual hard drive, For safety reasons i will not accept drives bigger then 32GB", "NAND Setup", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                MessageBox.Show("Selected Drive: " + folderPath, "NAND Setup", MessageBoxButton.OK, MessageBoxImage.Information);
                //check the current format of the SD card to see if its FAT32
                if (drive.DriveFormat != "FAT32")
                {
                    var format = MessageBox.Show("Your SD card is NOT formatted to FAT32. For this to work it needs to be FAT32\n Do you want me to format it for you? Keep in mind, you will lose ALL data on the SD card", "NAND Setup", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (format == MessageBoxResult.No)
                    {
                        MessageBox.Show("Cancelling", "NAND Setup", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    //format the drive to only its letter
                    FormatDriveToFAT32(drive.Name);
                    MessageBox.Show("Your drive has been formatted to FAT32", "NAND Setup", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Your SD card is already formatted to FAT32. you may continue", "NAND Setup", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("NO Drive selected, cancelling", "NAND Setup", MessageBoxButton.OK, MessageBoxImage.Information);
            }


        }
        
        NANDTutorialWindow tutorialWindow = new NANDTutorialWindow();
        tutorialWindow.ShowDialog();
        
        // MessageBox.Show("Step 1:\n Launch your wii and go to your settings. \nClick OK to continue", "Step 1 NAND Setup", MessageBoxButton.OK, MessageBoxImage.Information);
        // MessageBox.Show("Step 2:\n Go to Internet -> Connection Settings ->  Connection 1/2/3 (whichever your currently using", "Step 2 NAND", MessageBoxButton.YesNo, MessageBoxImage.Information);
        // MessageBox.Show("Step 3:\n Click on Change Settings -> Click on the right arrow until you see Auto-Obtain DNS -> Click No -> Click Advanced Settings", "Step 3 NAND", MessageBoxButton.YesNo, MessageBoxImage.Information);
        // MessageBox.Show("Step 4:\n Click on Primary DNS", "Step 4 NAND", MessageBoxButton.YesNo, MessageBoxImage.Information);
        // MessageBox.Show("Step 5:\n Fill in the following DNS: 18.188.135.9", "Step 5 NAND", MessageBoxButton.YesNo, MessageBoxImage.Information);
        // MessageBox.Show("Step 6:\n Do the same for the Secondary DNS: 18.188.135.9", "Step 6 NAND", MessageBoxButton.YesNo, MessageBoxImage.Information);
        // MessageBox.Show("Click Confirm and save. \n Click OK to Continue and wait for your internet to be tested.\nIf it asks you to update, click NO", "Step 7 NAND", MessageBoxButton.YesNo, MessageBoxImage.Information);
        // MessageBox.Show("Step 8:\n Go back to your internet settings and click User Agreements\n It will ask you to use WiiConnect24.\n Click YES and NEXT and DO NOT DO ANYTHING\ndo NOT click ACCEPT or NOT ACCEPT\n Wait untill your wii starts showing crazy colors", "Step 8 NAND", MessageBoxButton.YesNo, MessageBoxImage.Information);
        // MessageBox.Show("Step 9:\n You should now see a message saying this software must NOT be sold. \n Wait here untill 'Press 1 to continue' shows up", "Step 9 NAND", MessageBoxButton.YesNo, MessageBoxImage.Information);
        // MessageBox.Show("Step 10:\n You should have now reached the HackMii Installer. \n Press A to continue and go to the BootMii Menu, and click 'Install BootMii as IOS' and then 'Yes, continue' and then again 'Yes, Continue'.", "Step 10 NAND", MessageBoxButton.YesNo, MessageBoxImage.Information);
        // MessageBox.Show("Step 11:\n Once you have installed BootMii as IOS, go back to the main menu and click 'Exit'.", "Step 11 NAND", MessageBoxButton.YesNo, MessageBoxImage.Information);
        // MessageBox.Show("Step 12:\n You should now be in the Homebrew channel. Press the HOME button and click 'Launch BootMii'.", "Step 12 NAND", MessageBoxButton.YesNo, MessageBoxImage.Information);
        // MessageBox.Show("Step 13:\n Once you have launched BootMii, Your controller will not work anymore. Use the POWER button to move and your RESET button to select on your WII device", "Step 13 NAND", MessageBoxButton.YesNo, MessageBoxImage.Information);
        // MessageBox.Show("Step 14:\n Click the power button 4 times to reach the GEARS. and then press reset to select. then select the SD card with the GREEN arrow\n You should now see your NAND being backed up in real time", "Step 14 NAND", MessageBoxButton.YesNo, MessageBoxImage.Information);
        // MessageBox.Show("Step 15:\n After all the blocks turned green you are finished \n if some blocks are black. just wait, it will come back around and try and fix those. it may take a while", "Step 15 NAND", MessageBoxButton.YesNo, MessageBoxImage.Information);
        // MessageBox.Show("Step 16:\n Once its finished click the power button, it will bring you back a menu, Just click the power button to reach the back arrow, and click reset to select it to go back, and go back to the homebrew channel", "Step 16 NAND", MessageBoxButton.YesNo, MessageBoxImage.Information);
        // MessageBox.Show("Step 17:\n You are all done, you may take out your SD card and put it in your computer.\n Please click continue once you have the file on your computer", "Step 17 NAND", MessageBoxButton.YesNo, MessageBoxImage.Information);
    }

    public static void FormatDriveToFAT32(string DriveLetter)
    {
        try
        {
            //double check drive is not bigger then 32GB
            DriveInfo drive = new DriveInfo(DriveLetter);
            if (drive.TotalSize > 32000000000)
            {
                MessageBox.Show("Your SD card is too big, You may have selected a actual hard drive, For safety reasons i will not accept drives bigger then 32GB", "NAND Setup", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var userSure = MessageBox.Show("Are you sure you want to format drive: " + DriveLetter + " to FAT32? This will erase all data on the drive", "NAND Setup", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (userSure == MessageBoxResult.No)
            {
                MessageBox.Show("Cancelling", "NAND Setup", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            ManagementObject disk = new ManagementObject("Win32_DiskDrive.DeviceID=\"" + DriveLetter + "\"");
            ManagementBaseObject inputArgs = disk.GetMethodParameters("FormatEx");
            
            inputArgs["FileSystem"] = "FAT32";
            inputArgs["QuickFormat"] = true;
            inputArgs["ClusterSize"] = 0; //i think this means default
            inputArgs["EnableCompression"] = false;
            
            ManagementBaseObject outArgs = disk.InvokeMethod("FormatEx", inputArgs, null);
            
            if(outArgs != null)
            {
                uint retVal = (uint)outArgs.Properties["ReturnValue"].Value;
                if (retVal == 0)
                {
                    MessageBox.Show("Format Successful", "NAND Setup", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    Console.WriteLine($"Failed to format drive {DriveLetter} to FAT32. Error code: {retVal}");
                }
            }
        }
        catch (Exception e)
        {
            MessageBox.Show("Failed to format drive to FAT32. Error: " + e.Message, "NAND Setup", MessageBoxButton.OK, MessageBoxImage.Error);
            throw;
        }
    }

}