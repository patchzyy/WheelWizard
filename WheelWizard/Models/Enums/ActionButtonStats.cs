namespace CT_MKWII_WPF.Models.Enums;

public enum WheelWizardStatus
{
    NoServer, // no server for retro rewind could be found
    NoDolphin,
    ConfigNotFinished,
    NoRR,
    NoRRActive,
    RRNotReady,
    OutOfDate,
    UpToDate
}