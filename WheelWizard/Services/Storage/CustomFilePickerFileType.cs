using Avalonia.Platform.Storage;

namespace WheelWizard.Services;

public static class CustomFilePickerFileType
{
    public static FilePickerFileType Mods { get; } = new("Mods")
    {
        //i have just added .zip here because the mod loader can load mods from zip files, so it should be able to load them from the file picker too
        //but just keep this in mind for in the future.
        Patterns = new[] { "*.szs", "*.arc", "*.brstm", "*.brsar", "*.thp", "*.zip" }, 
        AppleUniformTypeIdentifiers = new[] { "com.wheelwizard.mods" }, // Honestly no idea how it works
        MimeTypes = new[] { "application/mods" } // Honestly no idea how it works
    };
    
    
}
