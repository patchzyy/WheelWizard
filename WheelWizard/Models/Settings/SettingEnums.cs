﻿using System.Collections.Generic;

namespace CT_MKWII_WPF.Models.Settings;

public enum DolphinShaderCompilationMode {
    Default = 0,
    ExclusiveUberShaders = 1,
    HybridUberShaders = 2,
    SkipDrawing = 3
}

public static class SettingValues
{
    public static class GFXRenderers
    {
        public const string D3D11 = "D3D11";
        public const string D3D12 = "D3D12";
        public const string Vulkan = "Vulkan";
        public const string OpenGL = "OGL";
        public const string SoftwareRenderer = "Software Renderer";

        // List of all renderer constants
        public static readonly List<string> AllRenderers = new List<string>
        {
            D3D11,
            D3D12,
            Vulkan,
            OpenGL,
            "Software Renderer"
        };
    }
}
   
