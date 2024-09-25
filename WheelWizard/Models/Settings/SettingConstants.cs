using System;
using System.Collections.Generic;

namespace CT_MKWII_WPF.Models.Settings;

public enum DolphinShaderCompilationMode {
    Default = 0,
    ExclusiveUberShaders = 1,
    HybridUberShaders = 2,
    SkipDrawing = 3
}

public static class SettingValues
{
    public static readonly double[] WindowScales = { 0.7, 0.8, 0.9, 1.0, 1.15, 1.3, 1.45 };
    
    public static readonly ListedSetting<string> GFXRenderers = new("DirectX 11 (Default)", 
            ("DirectX 11 (Default)", "D3D11"),
        ("DirectX 12", "D3D12"),
        ("Vulkan", "Vulkan"),
        ("OpenGL", "OGL")
    );

    public static readonly ListedSetting<int> Languages = new("English",
        ("English", 0),
        ("Japanese", 1),
        ("French", 2),
        ("German", 3),
        ("Dutch", 4)
    ); 
}
   
