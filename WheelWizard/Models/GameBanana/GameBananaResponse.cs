using System;
using System.Collections.Generic;

public class GameBananaResponse
{
    // Metadata for the API response (e.g., total records, pagination)
    public Metadata _aMetadata { get; set; }

    // List of records representing mods or other GameBanana content
    public List<ModRecord> _aRecords { get; set; }
}

public class Metadata
{
    // Total number of records found in the query
    public int _nRecordCount { get; set; }

    // Number of records per page returned by the API
    public int _nPerpage { get; set; }

    // Whether the response includes all results (false means pagination is needed)
    public bool _bIsComplete { get; set; }
}

public class ModRecord
{
    // Unique ID for the mod or content item
    public int _idRow { get; set; }

    // Model name/type (e.g., "Mod", "WiP" (Work in Progress))
    public string _sModelName { get; set; }

    // Singular title for the content (e.g., "Mod", "WiP")
    public string _sSingularTitle { get; set; }

    // Name of the mod or content item (e.g., the title given by the submitter)
    public string _sName { get; set; }

    // URL to the mod’s profile page on GameBanana
    public string _sProfileUrl { get; set; }

    // Date when the mod was first added (timestamp)
    public long _tsDateAdded { get; set; }

    // Date when the mod was last modified (timestamp)
    public long _tsDateModified { get; set; }

    // Indicates whether the mod has files associated with it
    public bool _bHasFiles { get; set; }

    // Tags assigned to the mod (e.g., "Stable", "In Development")
    public List<string> _aTags { get; set; }

    // Contains media information like images and screenshots for the mod
    public PreviewMedia _aPreviewMedia { get; set; }

    // Information about the user who submitted the mod
    public Submitter _aSubmitter { get; set; }

    // Information about the game the mod is for (e.g., Mario Kart, Counter-Strike)
    public Game _aGame { get; set; }

    // Category information for the mod (e.g., "Characters", "Maps")
    public RootCategory _aCategory { get; set; }

    // Development state of the mod (e.g., "In Development", "Final/Stable")
    public string _sDevelopmentState { get; set; }

    // Percentage of completion if it's still being developed (0–100%)
    public int _iCompletionPercentage { get; set; }

    // Number of likes the mod has received
    public int _nLikeCount { get; set; }

    // Number of views the mod page has received
    public int _nViewCount { get; set; }

    // URL of the first image
    public string FirstImageUrl
    {
        get
        {
            if (_aPreviewMedia?._aImages != null && _aPreviewMedia._aImages.Count > 0)
            {
                return $"{_aPreviewMedia._aImages[0]._sBaseUrl}/{_aPreviewMedia._aImages[0]._sFile}";
            }
            return string.Empty;
        }
    }

    // License details for the mod
    public License _aLicense { get; set; }

    // Trash info if the mod is trashed
    public TrashInfo _aTrashInfo { get; set; }

    // Files associated with the mod
    public List<ModFile> _aFiles { get; set; }

    // Embedded media like YouTube videos
    public List<string> _aEmbeddedMedia { get; set; }

    // Mod description or text (from _sText or _sDescription)
    public string _sText { get; set; }
    
    // Some mods may also use _sDescription, so you can handle both
    public string _sDescription { get; set; }
}

public class PreviewMedia
{
    // List of images or other preview media associated with the mod
    public List<Image> _aImages { get; set; }
}

public class Image
{
    // Type of media (e.g., "screenshot")
    public string _sType { get; set; }

    // Base URL for accessing the image
    public string _sBaseUrl { get; set; }

    // Filename of the image (can be appended to the base URL)
    public string _sFile { get; set; }

    // Width and height for a 100px version of the image
    public int _wFile100 { get; set; }
    public int _hFile100 { get; set; }
}

public class Submitter
{
    // Unique ID of the user who submitted the mod
    public int _idRow { get; set; }

    // Display name of the submitter
    public string _sName { get; set; }

    // URL to the submitter's profile page on GameBanana
    public string _sProfileUrl { get; set; }

    // URL to the submitter's avatar image
    public string _sAvatarUrl { get; set; }
}

public class Game
{
    // Unique ID for the game this mod is made for
    public int _idRow { get; set; }

    // Name of the game (e.g., "Counter-Strike: Global Offensive")
    public string _sName { get; set; }

    // URL to the game's profile page on GameBanana
    public string _sProfileUrl { get; set; }

    // URL to the game's icon image
    public string _sIconUrl { get; set; }
}

public class RootCategory
{
    // Category name (e.g., "Maps", "Characters")
    public string _sName { get; set; }

    // URL to the category's profile page on GameBanana
    public string _sProfileUrl { get; set; }

    // URL to the category's icon image
    public string _sIconUrl { get; set; }
}

public class License
{
    // URL to the Creative Commons license details
    public string _sLicenseUrl { get; set; }

    // Details on what users can or cannot do with the mod
    public LicenseChecklist _aLicenseChecklist { get; set; }
}

public class LicenseChecklist
{
    // Actions allowed without asking for permission
    public List<string> yes { get; set; }

    // Actions requiring permission from the mod author
    public List<string> ask { get; set; }

    // Actions that are not allowed at all
    public List<string> no { get; set; }
}

public class TrashInfo
{
    // Reason why the mod was trashed
    public string _sReason { get; set; }

    // Date when the mod was trashed (timestamp)
    public long _tsTrashDate { get; set; }
}

public class ModFile
{
    // Unique ID for the file
    public int _idRow { get; set; }

    // Name of the file
    public string _sFile { get; set; }

    // Size of the file in bytes
    public int _nFilesize { get; set; }

    // URL to download the file
    public string _sDownloadUrl { get; set; }

    // Analysis result of the file (e.g., "clean", "contains_exe")
    public string _sAnalysisResult { get; set; }

    // Indicates whether the file contains an executable
    public bool _bContainsExe { get; set; }

    // MD5 checksum of the file
    public string _sMd5Checksum { get; set; }
}
