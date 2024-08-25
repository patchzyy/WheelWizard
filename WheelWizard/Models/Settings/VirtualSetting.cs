using CT_MKWII_WPF.Services.Settings;
using System;

namespace CT_MKWII_WPF.Models.Settings;

public class VirtualSetting : Setting
{
    private Setting[] _dependencies;
    private Action<object> Setter;
    private Func<object> Getter;
    private bool _acceptsSignals = true;
    
    public VirtualSetting(Type type, object defaultValue, Action<object> setter, Func<object> getter) 
        : base(type, "virtual", defaultValue)
    {
        _dependencies = Array.Empty<Setting>();
        Setter = setter;
        Getter = getter;
        VirtualSettingManager.Instance.RegisterSetting(this);
    }
    
    public override bool Set(object newValue, bool skipSave = false)
    {
        // Its a virtual setting, so skipSave does nothing, since... well... its virtual, there is nowhere to save to
       
        _acceptsSignals = false;
        var result = SetInternal(newValue);
        _acceptsSignals = true;
        
        return result;
    }

    private bool SetInternal(object newValue)
    {
        if (!base.Set(newValue, false))
            return false;

        if (Value?.Equals(newValue) == true) 
            return true;
        
        var oldValue = Value;
        Value = newValue;
        var newIsValid = SaveEvenIfNotValid || IsValid();
        if (newIsValid)
        {
            Setter(newValue);   
            SignalDependents();
            VirtualSettingManager.Instance.SaveSettings(this);
        }
        else
            Value = oldValue;

        return true;
    }
      
    public override object Get() => Value; 
        // We dont have to constantly recalculate the value, since if they didn't change, the value is still the same
        // and they only change when the dependencies change, or when the users sets a new value
    public override bool IsValid()  => ValidationFunc == null || ValidationFunc(Value);
    
    public void SetDependencies(params Setting[] dependencies)
    {
        if (dependencies.Length != 0)
            throw new ArgumentException("Dependencies have already been set once");
        
        _dependencies = dependencies;
        foreach (var dependency in dependencies)
        {
            dependency.RegisterDependentVirtualSetting(this);
        }
    }
    
    public object Recalculate()
    {
        Value = Getter();
        return Get();
    }
    
    public void DependencyChanged()
    {
        if (!_acceptsSignals)
            return;
        
        SignalDependents();
        Recalculate();
    }
}
