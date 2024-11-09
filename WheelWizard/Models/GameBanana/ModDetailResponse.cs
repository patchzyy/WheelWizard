using System.Collections.Generic;

namespace CT_MKWII_WPF.Models.GameBanana;

public class ModDetailResponse
{
    public string? OverrideDownloadUrl { get; set; }
    public int _idRow { get; set; }
    public string _nStatus { get; set; }
    public bool _bIsPrivate { get; set; }
    public long _tsDateModified { get; set; }
    public long _tsDateAdded { get; set; }
    public string _sProfileUrl { get; set; }
    public PreviewMedia _aPreviewMedia { get; set; }
    public string _sCommentsMode { get; set; }
    public bool _bAccessorIsSubmitter { get; set; }
    public bool _bIsTrashed { get; set; }
    public bool _bIsWithheld { get; set; }
    public string _sName { get; set; }
    public int _nUpdatesCount { get; set; }
    public bool _bHasUpdates { get; set; }
    public long _tsDateUpdated { get; set; }
    public int _nAllTodosCount { get; set; }
    public bool _bHasTodos { get; set; }
    public int _nPostCount { get; set; }
    // public Dictionary<string, string> _aAttributes { get; set; }
    // public List<string> _aTags { get; set; }
    public bool _bCreatedBySubmitter { get; set; }
    public bool _bIsPorted { get; set; }
    public int _nThanksCount { get; set; }
    public string _sInitialVisibility { get; set; }
    public string _sDownloadUrl { get; set; }
    public int _nDownloadCount { get; set; }
    public List<FileDetail> _aFiles { get; set; }
    public int _nSubscriberCount { get; set; }
    public List<object> _aContributingStudios { get; set; }
    public string _sLicense { get; set; }
    public LicenseChecklist _aLicenseChecklist { get; set; }
    public bool _bGenerateTableOfContents { get; set; }
    public string _sText { get; set; } 
    public bool _bIsObsolete { get; set; }
    public int _nLikeCount { get; set; }
    public int _nViewCount { get; set; }
    public string _sVersion { get; set; }
    public bool _bAcceptsDonations { get; set; }
    public bool _bShowRipePromo { get; set; }
    public Embeddables _aEmbeddables { get; set; }
    public Submitter _aSubmitter { get; set; }
    public Game _aGame { get; set; }
    public Category _aCategory { get; set; }
    public SuperCategory _aSuperCategory { get; set; }
    public List<Credit> _aCredits { get; set; }
    public List<string> _aEmbeddedMedia { get; set; }
}

public class FileDetail
{
    public int _idRow { get; set; }
    public string _sFile { get; set; }
    public int _nFilesize { get; set; }
    public string _sDescription { get; set; }
    public long _tsDateAdded { get; set; }
    public int _nDownloadCount { get; set; }
    public string _sAnalysisState { get; set; }
    public string _sAnalysisResultCode { get; set; }
    public string _sAnalysisResult { get; set; }
    public bool _bContainsExe { get; set; }
    public string _sDownloadUrl { get; set; }
    public string _sMd5Checksum { get; set; }
    public string _sClamAvResult { get; set; }
    public string _sAvastAvResult { get; set; }
}

public class LicenseChecklist
{
    public List<string> yes { get; set; }
    public List<string> ask { get; set; }
    public List<string> only_if_same { get; set; }
    public List<string> no { get; set; }
}

public class Embeddables
{
    public string _sEmbeddableImageBaseUrl { get; set; }
    public List<string> _aVariants { get; set; }
}

public class Category
{
    public int _idRow { get; set; }
    public string _sName { get; set; }
    public string _sModelName { get; set; }
    public string _sProfileUrl { get; set; }
    public string _sIconUrl { get; set; }
}

public class SuperCategory
{
    public int _idRow { get; set; }
    public string _sName { get; set; }
    public string _sModelName { get; set; }
    public string _sProfileUrl { get; set; }
    public string _sIconUrl { get; set; }
}

public class Credit
{
    public string _sGroupName { get; set; }
    public List<Author> _aAuthors { get; set; }
}

public class Author
{
    public int _idRow { get; set; }
    public string _sName { get; set; }
    public string _sProfileUrl { get; set; }
    public bool _bIsOnline { get; set; }
}
