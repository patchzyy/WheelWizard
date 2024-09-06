namespace CT_MKWII_WPF.Models.Settings;

public enum DolphinShaderCompilationMode {
    Default = 0,
    ExclusiveUberShaders = 1,
    HybridUberShaders = 2,
    SkipDrawing = 3
}

public static class SettingValues
{
public static string[] GFXRenderers = { "D3D11", "D3D12", "Vulkan", "OGL", "Software Renderer" };
}
   
