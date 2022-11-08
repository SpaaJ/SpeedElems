using SpeedElems.Models;

namespace SpeedElems.Library;

/// <summary>
/// Images Manager
/// </summary>
public static class ImagesManager
{
    public static string Get(ElemType type, ElemImageType imageType)
    {
        return type.ToString().ToLower() + '_' + imageType.ToString().ToLower() + ".png";
    }
}