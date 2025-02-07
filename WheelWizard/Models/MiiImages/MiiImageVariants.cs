using System;
using System.Collections.Generic;
using System.Numerics;

namespace WheelWizard.Models.MiiImages;

public static class MiiImageVariants
{
    public enum Variant
    {
        DEFAULT,
        SLIGHT_SIDE_PROFILE
    }

    private static Dictionary<Variant, Func<string, string>> _variantMap = new()
    {
        [Variant.DEFAULT] = GetMiiImageUrlFromResponse(Expression.NORMAL, new(), 0),
        [Variant.SLIGHT_SIDE_PROFILE] = GetMiiImageUrlFromResponse(Expression.SMILE, new(350,15,355),12),
    };
    
    
    #region SETUP
    public static Func<string, string> Get(Variant variant) => _variantMap[variant];
    private static Func<string, string> GetMiiImageUrlFromResponse(Expression expression, Vector3 characterRotation, int cameraTilt)
    {
        return (miiData) =>
        {
            var queryParams = new List<string>
            {
                $"data={miiData}",
                "type=face",
                $"expression={expression.ToString().ToLower()}",
                "width=270",
                "bgColor=FFFFFF00",
                "clothesColor=default",
                "cameraXRotate=12",
                "cameraYRotate=0",
                "cameraZRotate=0",
                $"characterXRotate={(int)characterRotation.X}",
                $"characterYRotate={(int)characterRotation.Y}",
                $"characterZRotate={(int)characterRotation.Z}",
                "lightDirectionMode=none",
                "instanceCount=1",
                "instanceRotationMode=model"
            };
            return string.Join("&", queryParams);
        };
    }

    private enum Expression
    {
        NORMAL,
        SMILE,
        FRUSTATED,
        ANGER,
        BLINK,
        SORROW,
        SURPRISE
    }
    
    #endregion
}
