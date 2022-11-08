using SkiaSharp.Extended.UI.Controls;
using SpeedElems.Library;
using SpeedElems.Models;

namespace SpeedElems.Controls;

/// <summary>
/// Fire Elem Control
/// </summary>
public class FireElemControl : ElemControl
{
    #region Privates

    private SKLottieView animationView;
    private ImageSource secondaryImageSource;

    #endregion Privates

    #region Public

    public bool IsInHomePage = false;

    #endregion Public

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    public FireElemControl()
    {
        PrincipalImageSource = ImagesManager.Get(ElemType.Fire, ElemImageType.Principal);
        secondaryImageSource = ImagesManager.Get(ElemType.Fire, ElemImageType.Secondary);

        FaceNormalImageSource = ImagesManager.Get(ElemType.Fire, ElemImageType.FaceNormal);
        FaceWinImageSource = ImagesManager.Get(ElemType.Fire, ElemImageType.FaceWin);
        FaceLooseImageSource = ImagesManager.Get(ElemType.Fire, ElemImageType.FaceLoose);

        PrincipalImage.Source = PrincipalImageSource;
        FaceImage.Source = FaceNormalImageSource;

        animationView = new SKLottieView
        {
            IsAnimationEnabled = false,
            Source = new SKFileLottieImageSource { File = "fire_explosion.json" },
            HeightRequest = 2.5 * SizesManager.ElemControlSize,
            WidthRequest = 2.5 * SizesManager.ElemControlSize,
            Rotation = new Random().Next(0, 90)
        };
        ((Grid)Content).Children.Add(animationView);

        this.Opacity = 0.2;
        this.FadeTo(1, 300);
    }

    #endregion Constructor

    #region Methods

    public override void Press(Point location = default)
    {
        if (!IsPressed && Status < ElemControlStatus.Released)
        {
            IsPressed = true;
            LayoutBounds = AbsoluteLayout.GetLayoutBounds(this);
            FaceImage.Source = FaceWinImageSource;
            Status = ElemControlStatus.Pressed;
        }
    }

    public override void Move(Point location)
    {
        if (IsPressed && Status < ElemControlStatus.Released)
        {
            if (location.X < LayoutBounds.X - SizesManager.TwentyPercentElemControlSize ||
                location.Y < LayoutBounds.Y - SizesManager.TwentyPercentElemControlSize ||
                location.X > LayoutBounds.X + SizesManager.ElemControlSize + SizesManager.TwentyPercentElemControlSize ||
                location.Y > LayoutBounds.Y + SizesManager.ElemControlSize + SizesManager.TwentyPercentElemControlSize)
            {
                IsPressed = false;
                FaceImage.Source = FaceNormalImageSource;
                Status = ElemControlStatus.Loaded;
            }
        }
    }

    public override async void Release()
    {
        if (IsPressed)
        {
            if (Status < ElemControlStatus.Released)
            {
                Status = ElemControlStatus.Released;

                animationView.IsAnimationEnabled = true;
                await Task.WhenAll(
                    FaceImage.FadeTo(0, 500),
                    PrincipalImage.FadeTo(0, 500),
                    FaceImage.ScaleTo(0.5, 500),
                    PrincipalImage.ScaleTo(0.5, 500),
                    Task.Delay(700)
                );
                animationView.IsAnimationEnabled = false;
                animationView.Progress = TimeSpan.Zero;

                await Task.Delay(100);

                if (Status != ElemControlStatus.Abused)
                    Status = ElemControlStatus.Exploded;
            }
            // Abused (if Released and Status == ElemControlStatus.Released )
            else
            {
                //For not be Abused if the Release is called by the timer in HomePage
                if (IsInHomePage)
                    return;

                animationView.IsAnimationEnabled = false;
                animationView.Progress = TimeSpan.Zero;
                Status = ElemControlStatus.Abused;
                PrincipalImage.Source = secondaryImageSource;

                await Task.WhenAll(
                   FaceImage.FadeTo(1, 500),
                   PrincipalImage.FadeTo(1, 500),
                   FaceImage.ScaleTo(1, 300),
                   PrincipalImage.ScaleTo(1, 300)
               );
            }
        }
    }

    /// <summary>
    /// Called if a Water Elem touch it
    /// </summary>
    public async void PutOut()
    {
        FaceImage.Source = FaceWinImageSource;
        Status = ElemControlStatus.Released;

        await Task.WhenAll(
            FaceImage.FadeTo(0, 500),
            PrincipalImage.FadeTo(0, 500),
            Task.Delay(700)
        );

        Status = ElemControlStatus.Exploded;
    }

    /// <summary>
    /// Resurrect the control in the HomePage
    /// </summary>
    public void Resurrect()
    {
        FaceImage.Opacity = 1d;
        PrincipalImage.Opacity = 1d;
        FaceImage.Scale = 1d;
        PrincipalImage.Scale = 1d;

        this.Opacity = 0.2;
        this.FadeTo(1, 300);
    }

    #endregion Methods
}