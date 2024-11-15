namespace WheelWizard.Models.Settings;

public interface ISettingListener
{
    public void OnSettingChanged(Setting setting);
}
