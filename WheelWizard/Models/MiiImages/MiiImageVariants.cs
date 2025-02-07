using System;
using System.Collections.Generic;

namespace WheelWizard.Models.MiiImages;

public static class MiiImageVariants
{
    public enum Variant
    {
        DEFAULT,
        AUSTRALIAN
    }

    private static Dictionary<Variant, Func<string, string>> _variantMap = new()
    {
        [Variant.DEFAULT] = GetMiiImageUrlFromResponse(0),
        [Variant.AUSTRALIAN] = GetMiiImageUrlFromResponse(180)
    };
    
    #region SETUP
    public static Func<string, string> Get(Variant variant) => _variantMap[variant];
    private static Func<string, string> GetMiiImageUrlFromResponse(int camZRot)
    {
        return (miiData) =>
        {
            var queryParams = new List<string>
            {
                $"data={miiData}",
                "type=face",
                "expression=normal",
                "width=270",
                "bgColor=FFFFFF00",
                "clothesColor=default",
                "cameraXRotate=0",
                "cameraYRotate=0",
                $"cameraZRotate={camZRot}",
                "characterXRotate=0",
                "characterYRotate=0",
                "characterZRotate=0",
                "lightDirectionMode=none",
                "instanceCount=1",
                "instanceRotationMode=model"
            };
            return string.Join("&", queryParams);
        };
    }

    #endregion
}
