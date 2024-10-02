using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Models;
using CT_MKWII_WPF.Services.GameBanana;

namespace CT_MKWII_WPF.Views.Popups
{
    public partial class ModPopupWindow : PopupContent
    {
        private ObservableCollection<ModRecord> Mods { get; set; } = new ObservableCollection<ModRecord>();
        private int CurrentPage { get; set; } = 1;
        private const int ModsPerPage = 20;
        private string CurrentSearchTerm = "";

        public ModPopupWindow() : base(true, false,false, "Mod Browser", new Vector(800, 800))
        {
            InitializeComponent();
            ModListView.ItemsSource = Mods;
            LoadMods(CurrentPage);
        }

        private async void LoadMods(int page, string searchTerm = "")
        {
            try
            {
                var result = await GamebananaSearchHandler.SearchModsAsync(searchTerm, page, ModsPerPage);
                
                if (result.Succeeded && result.Content != null)
                {
                    Mods.Clear();
                    foreach (var mod in result.Content._aRecords)
                    {
                        Mods.Add(mod);
                    }
                }
                else
                {
                    MessageBox.Show("Failed to load mods: " + result.StatusMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                LoadMods(CurrentPage, CurrentSearchTerm);
            }
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage++;
            LoadMods(CurrentPage, CurrentSearchTerm);
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            CurrentSearchTerm = SearchTextBox.Text;
            CurrentPage = 1;
            LoadMods(CurrentPage, CurrentSearchTerm);
        }

        private void ModListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModListView.SelectedItem is ModRecord selectedMod)
            {
                UpdateModDetails(selectedMod);
            }
        }

        private void UpdateModDetails(ModRecord mod)
        {
            if (mod._aPreviewMedia?._aImages?.Count > 0)
            {
                ModImage.Source = new BitmapImage(new Uri(mod._aPreviewMedia._aImages[0]._sBaseUrl));
            }
            else
            {
                ModImage.Source = null;
            }

            ModName.Text = mod._sName;
            ModSubmitter.Text = $"By {mod._aSubmitter._sName}";
            ModStats.Text = $"Likes: {mod._nLikeCount} | Views: {mod._nViewCount}";
            ModDescription.Text = $"Development State: {mod._sDevelopmentState}\nCompletion: {mod._iCompletionPercentage}%";
        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            if (ModListView.SelectedItem is ModRecord selectedMod)
            {
                MessageBox.Show($"Downloading mod: {selectedMod._sName}\nThis feature is not yet implemented.", "Download", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
