using System;
using System.Collections.Generic;
using System.Numerics;

namespace WheelWizard.Models.MiiImages;

public static class MiiImageVariants
{
    public enum Variant
    {
        DEFAULT,
        SLIGHT_SIDE_PROFILE_DEFAULT,
        SLIGHT_SIDE_PROFILE_SURPISED,
        SLIGHT_SIDE_PROFILE_FRUSTATED,
        FULL_BODY,
    }

    private static Dictionary<Variant, Func<string, string>> _variantMap = new()
    {
        [Variant.DEFAULT] = GetMiiImageUrlFromResponse(Expression.NORMAL, BodyType.FACE, ImageSize.SMALL),
        [Variant.FULL_BODY] = GetMiiImageUrlFromResponse(Expression.NORMAL, BodyType.ALL_BODY, ImageSize.MEDIUM),
        [Variant.SLIGHT_SIDE_PROFILE_DEFAULT] = GetMiiImageUrlFromResponse(Expression.NORMAL, BodyType.FACE, ImageSize.MEDIUM,
            characterRotation: new(350,15,355), cameraTilt: 12),
        [Variant.SLIGHT_SIDE_PROFILE_SURPISED] = GetMiiImageUrlFromResponse(Expression.SMILE, BodyType.FACE, ImageSize.MEDIUM,
            characterRotation: new(350,15,355), cameraTilt: 12),
        
        [Variant.SLIGHT_SIDE_PROFILE_FRUSTATED] = GetMiiImageUrlFromResponse(Expression.FRUSTRATED, BodyType.FACE, ImageSize.MEDIUM,
            characterRotation: new(350,15,355), cameraTilt: 12),
    };
    
    
    #region SETUP
    public static Func<string, string> Get(Variant variant) => _variantMap[variant];
    private static Func<string, string> GetMiiImageUrlFromResponse(
        Expression expression, BodyType type, ImageSize size, 
        Vector3 characterRotation = new(), int cameraTilt = 0)
    {
        return (miiData) =>
        {
            var queryParams = new List<string>
            {
                $"data={miiData}",
                $"type={type.ToString().ToLower()}",
                $"expression={expression.ToString().ToLower()}",
                $"width={(int)size}",
                "bgColor=FFFFFF00",
                "clothesColor=default",
                $"cameraXRotate={cameraTilt}",
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
        FRUSTRATED,
        ANGER,
        BLINK,
        SORROW,
        SURPRISE
    }

    private enum BodyType
    {
        FACE,
        ALL_BODY
    }
    private enum ImageSize
    {
        SMALL = 270,
        MEDIUM = 512,
    }
    
    #endregion
}
