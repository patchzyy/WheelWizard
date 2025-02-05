using Avalonia;
using Avalonia.Controls.Primitives;

namespace WheelWizard.Views.Components.WhWzLibrary;

public class ModBrowserListItem : TemplatedControl
{
    public static readonly StyledProperty<string> ModTitleProperty =
        AvaloniaProperty.Register<ModBrowserListItem, string>(nameof(ModTitle));

    public string ModTitle
    {
        get => GetValue(ModTitleProperty);
        set => SetValue(ModTitleProperty, value);
    }
    
    public static readonly StyledProperty<string> ModAuthorProperty =
        AvaloniaProperty.Register<ModBrowserListItem, string>(nameof(ModAuthor));

    public string ModAuthor
    {
        get => GetValue(ModAuthorProperty);
        set => SetValue(ModAuthorProperty, value);
    }
    
    public static readonly StyledProperty<string> DownloadCountProperty =
        AvaloniaProperty.Register<ModBrowserListItem, string>(nameof(DownloadCount));

    public string DownloadCount
    {
        get => GetValue(DownloadCountProperty);
        set => SetValue(DownloadCountProperty, value);
    }
    
    public static readonly StyledProperty<string> ViewCountProperty =
        AvaloniaProperty.Register<ModBrowserListItem, string>(nameof(ViewCount));

    public string ViewCount
    {
        get => GetValue(ViewCountProperty);
        set => SetValue(ViewCountProperty, value);
    }
        
    public static readonly StyledProperty<string> LikeCountProperty =
        AvaloniaProperty.Register<ModBrowserListItem, string>(nameof(LikeCount));

    public string LikeCount
    {
        get => GetValue(LikeCountProperty);
        set => SetValue(LikeCountProperty, value);
    }

}

