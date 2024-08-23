using CT_MKWII_WPF.Services.Settings;
using System;

namespace CT_MKWII_WPF.Models.Settings;

public class DolphinSetting : Setting
{
    public string FileName { get; private set; }
    public string Section { get; private set; }
    
    public DolphinSetting(Type type, (string,string,string) location, object defaultValue) : base(type, location.Item3, defaultValue)
    {
        FileName = location.Item1;
        Section = location.Item2;
        // name/key = location.Item3
        
        DolphinSettingManager.Instance.RegisterSetting(this);
    }
    
    
    public override bool Set(object value, bool forceSave = false)
    {
        if (value.GetType() != ValueType)
            return false;
        
        if (Value?.Equals(value) == true) 
            return true;

        
        throw new NotImplementedException();
    }

    public override object Get() => Value;
    public override bool IsValid()  => ValidationFunc == null || ValidationFunc(Value);
}
