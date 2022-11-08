using SkiaSharp.Extended.UI.Controls;
using SpeedElems.Library;
using SpeedElems.Models;

namespace SpeedElems.Controls;

/// <summary>
/// Ground Elem Control
/// </summary>
public class GroundElemControl : ElemControl
{
    #region Privates

    private ImageSource secondaryImageSource;
    private SKLottieView animationView;

    #endregion Privates

    public bool IsHorrified { get; private set; }

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    public GroundElemControl()
    {
        PrincipalImageSource = ImagesManager.Get(ElemType.Ground, ElemImageType.Principal);
        secondaryImageSource = ImagesManager.Get(ElemType.Ground, ElemImageType.Secondary);

        FaceNormalImageSource = ImagesManager.Get(ElemType.Ground, ElemImageType.FaceNormal);
        FaceWinImageSource = ImagesManager.Get(ElemType.Ground, ElemImageType.FaceWin);
        FaceLooseImageSource = ImagesManager.Get(ElemType.Ground, ElemImageType.FaceLoose);

        PrincipalImage.Source = PrincipalImageSource;
        FaceImage.Source = FaceNormalImageSource;

        animationView = new SKLottieView
        {
            IsAnimationEnabled = false,
            HeightRequest = SizesManager.ElemControlSize,
            WidthRequest = SizesManager.ElemControlSize
        };
        ((Grid)Content).Children.Add(animationView);

        this.Rotation = new Random().Next(2) == 0 ? 360 : -360;
        this.Opacity = 0.2;
        this.RotateTo(0, 300);
        this.FadeTo(1, 300);
    }

    #endregion Constructor

    #region Methods

    public override void Press(Point location = default)
    {
        if (!IsPressed && Status != ElemControlStatus.Released)
        {
            IsPressed = true;
            LayoutBounds = AbsoluteLayout.GetLayoutBounds(this);
            FaceImage.Source = IsHorrified ? FaceNormalImageSource : FaceWinImageSource;
            Status = ElemControlStatus.Pressed;
        }
    }

    public override void Move(Point location)
    {
        if (IsPressed && Status != ElemControlStatus.Released)
        {
            if (location.X < LayoutBounds.X - SizesManager.TwentyPercentElemControlSize ||
                location.Y < LayoutBounds.Y - SizesManager.TwentyPercentElemControlSize ||
                location.X > LayoutBounds.X + SizesManager.ElemControlSize + SizesManager.TwentyPercentElemControlSize ||
                location.Y > LayoutBounds.Y + SizesManager.ElemControlSize + SizesManager.TwentyPercentElemControlSize)
            {
                IsPressed = false;
                FaceImage.Source = !IsHorrified ? FaceNormalImageSource : FaceWinImageSource;
                Status = ElemControlStatus.Loaded;
            }
        }
    }

    public override async void Release()
    {
        if (IsPressed && Status != ElemControlStatus.Released)
        {
            if (!IsHorrified)
            {
                IsHorrified = true;
                Status = ElemControlStatus.Loaded;
                PrincipalImage.Source = secondaryImageSource;

                IsPressed = false;
            }
            else
            {
                IsHorrified = false;
                Status = ElemControlStatus.Released;

                animationView.Source = new SKFileLottieImageSource { File = "ground_explosion.json" };// set here because IsVisible dont work :(
                animationView.IsAnimationEnabled = true;
                await Task.WhenAll(
                    FaceImage.FadeTo(0, 300),
                    PrincipalImage.FadeTo(0, 300),
                    Task.Delay(700)
                );
                animationView.IsAnimationEnabled = false;

                await Task.Delay(100);

                Status = ElemControlStatus.Exploded;
            }
        }
    }

    public void CompleteGroundWetAchievement()
    {
        PrincipalImage.Source = "ground_wet.png";
        CompleteAchievement("Tricks_GroundWet");
    }

    #endregion Methods
}