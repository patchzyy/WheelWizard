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
    
    public override bool Set(object newValue, bool skipSave = false)
    {
        if (!base.Set(newValue, skipSave))
            return false;
        
        if (Value?.Equals(newValue) == true) 
            return true;

        var oldValue = Value;
        Value = newValue;
        var newIsValid = SaveEvenIfNotValid || IsValid();
        if (newIsValid)
        {
            if (!skipSave)
                DolphinSettingManager.Instance.SaveSettings(this);
        }
        else
            Value = oldValue;
        
        return newIsValid;
    }
    
    public override object Get() => Value;
    public override bool IsValid()  => ValidationFunc == null || ValidationFunc(Value);

    public string GetStringValue()
    {
        if (ValueType.IsEnum)
            return ((int)Value).ToString();
        
        return Value?.ToString() ?? "null";
    }
    
    public bool SetFromString(string newValue, bool skipSave = false)
    {
        // That these are the only types currently supported does not mean that these are all the Dolphin settings types
        // feel free to add more types if you find them
        return ValueType switch
        {
            { } t when t == typeof(string) => Set(newValue, skipSave),
            { } t when t == typeof(int)    => Set(int.Parse(newValue), skipSave),
            { } t when t == typeof(long)   => Set(long.Parse(newValue), skipSave),
            { } t when t == typeof(float)  => Set(float.Parse(newValue), skipSave),
            { } t when t == typeof(double) => Set(double.Parse(newValue), skipSave),
            { } t when t == typeof(bool)   => Set(bool.Parse(newValue), skipSave),
            { IsEnum: true } t             => Set(Enum.ToObject(t, int.Parse(newValue)), skipSave),
            _  => throw new InvalidOperationException($"Unsupported type: {ValueType.Name}")
        };
    }
}
