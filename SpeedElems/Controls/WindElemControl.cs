using SpeedElems.Library;
using SpeedElems.Models;

namespace SpeedElems.Controls;

/// <summary>
/// Wind Elem Control
/// </summary>
public class WindElemControl : ElemControl
{
    #region Privates

    private Point pressedTranslation;
    private Point pressPoint;

    #endregion Privates

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    public WindElemControl()
    {
        PrincipalImageSource = ImagesManager.Get(ElemType.Wind, ElemImageType.Principal);

        FaceNormalImageSource = ImagesManager.Get(ElemType.Wind, ElemImageType.FaceNormal);
        FaceWinImageSource = ImagesManager.Get(ElemType.Wind, ElemImageType.FaceWin);
        FaceLooseImageSource = ImagesManager.Get(ElemType.Wind, ElemImageType.FaceLoose);

        PrincipalImage.Source = PrincipalImageSource;
        FaceImage.Source = FaceNormalImageSource;

        this.Opacity = 0.2;
        this.FadeTo(1, 300);
    }

    #endregion Constructor

    #region Methods

    public override void Press(Point location)
    {
        if (!IsPressed && Status != ElemControlStatus.Released)
        {
            IsPressed = true;
            LayoutBounds = AbsoluteLayout.GetLayoutBounds(this);
            pressedTranslation = new(TranslationX, TranslationY);
            pressPoint = new(location.X - LayoutBounds.X - pressedTranslation.X, location.Y - LayoutBounds.Y - pressedTranslation.Y);
            FaceImage.Source = FaceWinImageSource;
            Status = ElemControlStatus.Pressed;
        }
    }

    public override async void Move(Point location)
    {
        if (IsPressed && Status != ElemControlStatus.Released)
        {
            //Translate du control
            var movePoint = new Point(location.X - LayoutBounds.X, location.Y - LayoutBounds.Y);
            TranslationX = movePoint.X - pressPoint.X;
            TranslationY = movePoint.Y - pressPoint.Y;

            //Achievement
            double fullDistance = Math.Sqrt((Math.Pow(TranslationX, 2) + Math.Pow(TranslationY, 2)));
            if (fullDistance > SizesManager.ElemControlSize * 5)
                CompleteAchievement("Tricks_Wind5cm");

            ////Calcul de la distance parcouru
            double distance = Math.Sqrt((Math.Pow(TranslationX - pressedTranslation.X, 2) + Math.Pow(TranslationY - pressedTranslation.Y, 2)));

            ////Calcul de la position dans l'AbsoluteLayout
            var position = new Point(LayoutBounds.X + TranslationX, LayoutBounds.Y + TranslationY);

            //if distance > 100 or if position in screen is out
            if (distance > SizesManager.ElemControlSize * 2.5 ||
                position.X < SizesManager.ElemControlSize ||
                position.Y < SizesManager.ElemControlSize * 0.5 ||
                position.X > SizesManager.ScreenWidth - SizesManager.ElemControlSize * 2 ||
                position.Y > SizesManager.ScreenHeight - SizesManager.ElemControlSize * 1.5)
            {
                Status = ElemControlStatus.Released;

                await Task.WhenAll(
                    FaceImage.FadeTo(0, 500),
                    PrincipalImage.FadeTo(0, 500),
                    Task.Delay(800)
                );

                Status = ElemControlStatus.Exploded;
            }
        }
    }

    public override void Release()
    {
        if (IsPressed && Status != ElemControlStatus.Released)
        {
            IsPressed = false;
            FaceImage.Source = FaceNormalImageSource;
            Status = ElemControlStatus.Loaded;
        }
    }

    #endregion Methods
}