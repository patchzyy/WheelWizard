// ModDetailViewer.xaml.cs
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CT_MKWII_WPF.Services.GameBanana;
using CT_MKWII_WPF.Services.Installation;
using CT_MKWII_WPF.Services.Launcher;
using System.IO;

namespace CT_MKWII_WPF.Views.Components
{
    public partial class ModDetailViewer : UserControl
    {
        // Define a delegate and event for download requests
        public delegate void DownloadRequestedEventHandler(object sender, DownloadRequestedEventArgs e);
        public event DownloadRequestedEventHandler DownloadRequested;

        public ModDetailViewer()
        {
            InitializeComponent();
            // Initially hide the detail viewer
            this.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Loads the details of the specified mod.
        /// </summary>
        /// <param name="mod">The mod to display.</param>
        public async Task LoadModDetailsAsync(ModRecord mod)
        {
            if (mod == null)
            {
                ClearDetails();
                return;
            }

            try
            {
                // Fetch the mod details using its ID
                var modDetailsResult = await GamebananaSearchHandler.GetModDetailsAsync(mod._idRow);

                if (!modDetailsResult.Succeeded || modDetailsResult.Content == null)
                {
                    MessageBox.Show($"Failed to retrieve mod details: {modDetailsResult.StatusMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var modDetails = modDetailsResult.Content;

                // Load Images
                ImageCarousel.Items.Clear();
                if (modDetails._aPreviewMedia?._aImages != null && modDetails._aPreviewMedia._aImages.Any())
                {
                    foreach (var image in modDetails._aPreviewMedia._aImages)
                    {
                        var fullImageUrl = $"{image._sBaseUrl}/{image._sFile}";
                        ImageCarousel.Items.Add(new { FullImageUrl = fullImageUrl });
                    }
                }

                // Mod Name and Submitter
                ModName.Text = modDetails._sName;
                ModSubmitter.Text = $"By {modDetails._aSubmitter._sName}";

                // Mod Stats
                ModStats.Text = $"Likes: {modDetails._nLikeCount} | Views: {modDetails._nViewCount} | Downloads: {modDetails._nDownloadCount}";

                // Description
                ModDescriptionHtmlPanel.Text = modDetails._sText;

                // Store the current mod for download
                CurrentMod = mod;

                // Make the viewer visible
                this.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while fetching mod details: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Clears the mod details from the viewer.
        /// </summary>
        public void ClearDetails()
        {
            ImageCarousel.Items.Clear();
            ModName.Text = string.Empty;
            ModSubmitter.Text = string.Empty;
            ModStats.Text = string.Empty;
            ModDescriptionHtmlPanel.Text = string.Empty;
            this.Visibility = Visibility.Collapsed;
        }

        // Property to hold the current mod
        private ModRecord CurrentMod { get; set; }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentMod != null)
            {
                // Raise the DownloadRequested event
                DownloadRequested?.Invoke(this, new DownloadRequestedEventArgs { Mod = CurrentMod });
            }
            else
            {
                MessageBox.Show("No mod selected for download.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    // Custom EventArgs to pass the mod information
    public class DownloadRequestedEventArgs : EventArgs
    {
        public ModRecord Mod { get; set; }
    }
}
