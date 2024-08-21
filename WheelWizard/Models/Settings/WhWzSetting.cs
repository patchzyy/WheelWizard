using CT_MKWII_WPF.Services.Settings;
using System;
using System.Text.Json;

namespace CT_MKWII_WPF.Models.Settings;

public class WhWzSetting : Setting
{
    public WhWzSetting(Type type, string name, object defaultValue) : base(type, name, defaultValue)
    {
        WhWzSettingManager.Instance.RegisterSetting(this);
    }
    
    public override bool Set(object value, bool skipSave = false)
    {
        if (value.GetType() != ValueType)
            return false;
        
        if (Value?.Equals(value) == true) 
            return true;
        
        var oldValue = Value;
        Value = value;
        var newIsValid = SaveEvenIfNotValid || IsValid();
        if (newIsValid && !skipSave) 
            WhWzSettingManager.Instance.SaveSettings(this);
        else
            Value = oldValue;
     
        return newIsValid;
    }
    
    public override object Get() => Value;
    public override bool IsValid() => ValidationFunc == null || ValidationFunc(Value);
    
   public void Set(JsonElement value, bool skipSave = false)
   {
       switch (ValueType)
       {
           case { } t when t == typeof(bool):     Set(value.GetBoolean(), skipSave); break;
           case { } t when t == typeof(int):      Set(value.GetInt32(), skipSave); break;
           case { } t when t == typeof(long):     Set(value.GetInt64(), skipSave); break;
           case { } t when t == typeof(float):    Set((float)value.GetDouble(), skipSave); break;
           case { } t when t == typeof(double):   Set(value.GetDouble(), skipSave); break;
           case { } t when t == typeof(string):   Set(value.GetString()!, skipSave); break;
           case { } t when t == typeof(DateTime): Set(value.GetDateTime(), skipSave); break;
           case { IsEnum: true } t: Set(Enum.ToObject(t, value.GetInt32()), skipSave); break;
           case { IsArray: true } t: SetArray(value, t.GetElementType()!, skipSave); break;
           default:
               throw new InvalidOperationException($"Unsupported type: {ValueType.Name}");
       }
   }
   
   private void SetArray(JsonElement value, Type elementType, bool skipSave = false)
   {
       var json = value.GetRawText();
       var arrayType = Array.CreateInstance(elementType, 0).GetType();
       var array = (Array)JsonSerializer.Deserialize(json, arrayType)!;
       Set(array, skipSave);
   }
}
