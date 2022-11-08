namespace SpeedElems.Library;

/// <summary>
/// Sizes Manager
/// </summary>
public static class SizesManager
{
    public static double ScreenWidth { get; set; }

    public static double ScreenHeight { get; set; }

    public static double ElemControlSize { get; set; }

    public static double TwentyPercentElemControlSize { get; set; }

    public static double ElemControlSizeMenu { get; set; }

    public static double GameLayoutMargin { get; set; }

    public static double ElemControlDecalX { get; set; }

    public static double ElemControlDecalY { get; set; }

    public static double LevelsWidth { get; set; }

    public static double LevelsHeight { get; set; }

    public static void Initialize()
    {
        var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;

        ScreenWidth = mainDisplayInfo.Width / mainDisplayInfo.Density;
        ScreenHeight = mainDisplayInfo.Height / mainDisplayInfo.Density;

        GameLayoutMargin = 15 * mainDisplayInfo.Density;
        ElemControlSize = Math.Min((ScreenWidth - GameLayoutMargin * 2) / 14, (ScreenHeight - GameLayoutMargin * 2) / 7);
        TwentyPercentElemControlSize = ElemControlSize * 0.20;
        ElemControlSizeMenu = ElemControlSize * 0.9d;
        ElemControlDecalX = (ScreenWidth - ElemControlSize * 14) / 2;
        ElemControlDecalY = (ScreenHeight - ElemControlSize * 7) / 2;
        LevelsWidth = ElemControlSize * 14;
        LevelsHeight = ElemControlSize * 7;
    }
}