using SpeedElems.Library;
using SpeedElems.Models;

namespace SpeedElems.Controls;

/// <summary>
/// Electricity Elem Control
/// </summary>
public class ElectricityElemControl : ElemControl
{
    #region Privates

    protected Image SecondaryImage;
    private IDispatcherTimer faceSecondaryImageTimer;

    #endregion Privates

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    public ElectricityElemControl()
    {
        PrincipalImage.Source = ImagesManager.Get(ElemType.Electricity, ElemImageType.Principal);
        SecondaryImage = new() { Source = ImagesManager.Get(ElemType.Electricity, ElemImageType.Secondary) };

        FaceNormalImageSource = ImagesManager.Get(ElemType.Electricity, ElemImageType.FaceNormal);
        FaceWinImageSource = ImagesManager.Get(ElemType.Electricity, ElemImageType.FaceWin);
        FaceLooseImageSource = ImagesManager.Get(ElemType.Electricity, ElemImageType.FaceLoose);

        FaceImage.Source = FaceNormalImageSource;

        faceSecondaryImageTimer = Dispatcher.CreateTimer();
        faceSecondaryImageTimer.Interval = TimeSpan.FromMilliseconds(1000);
        faceSecondaryImageTimer.Tick += async (sender, e) =>
        {
            SecondaryImage.Opacity = 0.8;
            await Task.Delay(200);
            SecondaryImage.Opacity = 0.3;
            await Task.Delay(200);
            SecondaryImage.Opacity = 0.5;
            await Task.Delay(200);
            SecondaryImage.Opacity = 1;
            await Task.Delay(200);
            SecondaryImage.Opacity = 0.4;
        };

        ((Grid)Content).Children.Insert(1, SecondaryImage);
        StartSecondaryAnimation();

        this.Opacity = 0.2;
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
            FaceImage.Source = FaceWinImageSource;
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
                FaceImage.Source = FaceNormalImageSource;
                Status = ElemControlStatus.Loaded;
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

    public async void Explode()
    {
        if (IsPressed && Status != ElemControlStatus.Released)
        {
            Status = ElemControlStatus.Released;

            StopSecondaryAnimation();

            await Task.WhenAll(
                FaceImage.FadeTo(0, 300),
                PrincipalImage.FadeTo(0, 300)
            );

            await Task.Delay(500);
            Status = ElemControlStatus.Exploded;
        }
    }

    public void StopSecondaryAnimation()
    {
        SecondaryImage.IsVisible = false;
        faceSecondaryImageTimer.Stop();
    }

    public void StartSecondaryAnimation()
    {
        SecondaryImage.IsVisible = true;
        faceSecondaryImageTimer.Start();
    }

    #endregion Methods
}