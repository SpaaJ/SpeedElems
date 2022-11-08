using SpeedElems.Library;
using SpeedElems.Models;

namespace SpeedElems.Controls;

/// <summary>
/// Bio Elem Control
/// </summary>
public class BioElemControl : ElemControl
{
    #region Privates

    private double pressedDistance;

    #endregion Privates

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    public BioElemControl()
    {
        PrincipalImage.Source = ImagesManager.Get(ElemType.Bio, ElemImageType.Principal);
        FaceNormalImageSource = ImagesManager.Get(ElemType.Bio, ElemImageType.FaceNormal);
        FaceWinImageSource = ImagesManager.Get(ElemType.Bio, ElemImageType.FaceWin);
        FaceLooseImageSource = ImagesManager.Get(ElemType.Bio, ElemImageType.FaceLoose);

        FaceImage.Source = FaceNormalImageSource;

        this.Scale = 0;
        this.ScaleTo(1, 300);
    }

    #endregion Constructor

    #region Methods

    public override void Press(Point location)
    {
        if (!IsPressed && Status != ElemControlStatus.Released)
        {
            IsPressed = true;
            pressedDistance = location.X;
            LayoutBounds = AbsoluteLayout.GetLayoutBounds(this);
            FaceImage.Source = FaceWinImageSource;
            Status = ElemControlStatus.Pressed;
        }
    }

    public override async void Move(Point location)
    {
        var actualDistance = location.X;
        var movedDistance = pressedDistance - actualDistance;

        //Scale animation
        Scale = Math.Min(1d, 1d * (100d - 100d * movedDistance / SizesManager.ElemControlSize) / 100d);

        if (movedDistance > SizesManager.ElemControlSize * 0.75)
        {
            Scale = 0;
            Status = ElemControlStatus.Released;

            await Task.Delay(500);
            Status = ElemControlStatus.Exploded;
        }
    }

    public override void Release()
    {
        if (IsPressed && Status != ElemControlStatus.Released)
        {
            IsPressed = false;
            Scale = 1;
            FaceImage.Source = FaceNormalImageSource;
            Status = ElemControlStatus.Loaded;
        }
    }

    #endregion Methods
}