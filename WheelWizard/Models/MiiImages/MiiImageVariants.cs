using System;
using System.Collections.Generic;
using System.Numerics;

namespace WheelWizard.Models.MiiImages;

public static class MiiImageVariants
{
    public enum Variant
    {
        DEFAULT,
        SURPRISED
    }

    private static Dictionary<Variant, Func<string, string>> _variantMap = new()
    {
        [Variant.DEFAULT] = GetMiiImageUrlFromResponse(Expression.NORMAL, new()),
        [Variant.SURPRISED] = GetMiiImageUrlFromResponse(Expression.SURPRISE, new(350,15,355))
    };
    
    
    #region SETUP
    public static Func<string, string> Get(Variant variant) => _variantMap[variant];
    private static Func<string, string> GetMiiImageUrlFromResponse(Expression expression, Vector3 characterRotation)
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
                "cameraXRotate=0",
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
