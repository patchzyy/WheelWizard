﻿using System;
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
    
    public static class GFXRenderers
    {
        public const string D3D11 = "D3D11";
        public const string D3D12 = "D3D12";
        public const string Vulkan = "Vulkan";
        public const string OpenGL = "OGL";
        public const string SoftwareRenderer = "Software Renderer";

        // Dictionary to map display names to actual values
        public static readonly Dictionary<string, string> RendererMapping = new Dictionary<string, string>
        {
            { "DirectX 11 (Default)", D3D11 },
            { "DirectX 12", D3D12 },
            { "Vulkan", Vulkan },
            { "OpenGL", OpenGL },
            { "Software Renderer", SoftwareRenderer }
        };
        
        public static readonly List<string> AllRenderers = new List<string>(RendererMapping.Keys);
    }

    public static class Languages
    {
        public const int English = 0;
        public const int Japanese = 1;
        public const int French = 2;
        public const int German = 3;

        // Dictionary to map display names to actual values
        public static readonly Dictionary<string, int> LanguageMapping = new Dictionary<string, int>
        {
            { "English", English },
            { "Japanese", Japanese },
            { "French", French },
            { "German", German }
        };

        public static readonly List<string> AllLanguages = new List<string>(LanguageMapping.Keys);
    }
}
   