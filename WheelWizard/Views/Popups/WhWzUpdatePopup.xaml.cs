using CT_MKWII_WPF.Models.Github;
using System.Windows;

namespace CT_MKWII_WPF.Views.Popups;

public partial class WhWzUpdatePopup : PopupContent
{
    public bool Result { get; private set; }
    public GithubRelease Update { get; private set; }
    public WhWzUpdatePopup(GithubRelease update,Window owner = null) : base(allowClose: true, allowLayoutInteraction: false, isTopMost: true, title: "Mod Details", owner: owner, size: new Vector(500, 700))
    {
        Update = update;
        InitializeComponent();
        LoadUpdateData();
    }

    private void LoadUpdateData()
    {
        // Set the title to the release's tag name
        ModTitle.Text = $"Wheel Wizard Update - {Update.TagName}";

        // Set the author button's text to the uploader's name (if available)
        if (Update.Assets.Length > 0 && Update.Assets[0].Uploader != null)
        {
            AuthorButton.Text = Update.Assets[0].Uploader.Name;
        }
        else
        {
            AuthorButton.Text = "Unknown Author";
        }

        // Load the release description (HTML body)
        ModDescriptionHtmlPanel.Text = Update.Body;

        // Optionally set the BannerImage or other visual elements
        // BannerImage.Source = new BitmapImage(new Uri("/Resources/Images/test_image.jpg", UriKind.Relative)); // Adjust as needed
    }


    private void AuthorLink_Click(object sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void Update_Click(object sender, RoutedEventArgs e)
    {
        Result = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        Result = false;
        Close();
    }
    
    public bool AwaitAnswer()
    {
        ShowDialog();
        return Result;
    }
}

