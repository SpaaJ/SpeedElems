using System.Collections.ObjectModel;
using SkiaSharp.Extended.UI.Controls;
using SpeedElems.Library;
using SpeedElems.Models;

namespace SpeedElems.Controls;

/// <summary>
/// Water Elem Control
/// </summary>
public class WaterElemControl : ElemControl
{
    #region Privates

    private Point pressPoint;
    private Point pressedTranslation;
    private ImageSource secondaryImageSource;
    private SKLottieView animationView;

    #endregion Privates

    #region Properties

    public ObservableCollection<ElemControl> FireAndGroundElemControlCollection { get; set; }

    #region IsFrozen Property

    public bool WasFrozen { get; private set; }

    private bool isFrozen;

    public bool IsFrozen
    {
        get { return isFrozen; }
        set
        {
            isFrozen = value;

            if (!isFrozen)
            {
                PrincipalImage.Source = PrincipalImageSource;
            }
            else
            {
                secondaryImageSource = ImagesManager.Get(ElemType.Water, ElemImageType.Secondary);
                PrincipalImage.Source = secondaryImageSource;
            }
        }
    }

    #endregion IsFrozen Property

    #endregion Properties

    #region Constructor

    /// <summary>
    /// Constructor for Xaml
    /// </summary>
    public WaterElemControl() : this(false) { }

    /// <summary>
    /// Constructor
    /// </summary>
    public WaterElemControl(bool isFrozen)
    {
        PrincipalImageSource = ImagesManager.Get(ElemType.Water, ElemImageType.Principal);

        FaceNormalImageSource = ImagesManager.Get(ElemType.Water, ElemImageType.FaceNormal);
        FaceWinImageSource = ImagesManager.Get(ElemType.Water, ElemImageType.FaceWin);
        FaceLooseImageSource = ImagesManager.Get(ElemType.Water, ElemImageType.FaceLoose);

        PrincipalImage.Source = PrincipalImageSource;
        FaceImage.Source = FaceNormalImageSource;

        WasFrozen = IsFrozen = isFrozen;

        animationView = new SKLottieView
        {
            IsAnimationEnabled = false,
            Source = new SKFileLottieImageSource { File = "water_explosion.json" },
            HeightRequest = 1.5 * SizesManager.ElemControlSize,
            WidthRequest = 1.5 * SizesManager.ElemControlSize,
            Rotation = new Random().Next(0, 90)
        };
        ((Grid)Content).Children.Add(animationView);

        this.ScaleY = 0.2;
        this.ScaleYTo(1, 300);
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

            //Calcul de la position dans l'AbsoluteLayout
            var position = new Point(LayoutBounds.X + TranslationX + SizesManager.ElemControlSize / 2, LayoutBounds.Y + TranslationY + SizesManager.ElemControlSize / 2);
            foreach (var fireOrGroundElemControl in FireAndGroundElemControlCollection.Where(e => e.Status == ElemControlStatus.Loaded))
            {
                var al = AbsoluteLayout.GetLayoutBounds(fireOrGroundElemControl);
                if (al.X - SizesManager.TwentyPercentElemControlSize < position.X &&
                   al.X + SizesManager.ElemControlSize + SizesManager.TwentyPercentElemControlSize > position.X &&
                   al.Y - SizesManager.TwentyPercentElemControlSize < position.Y &&
                   al.Y + SizesManager.ElemControlSize + SizesManager.TwentyPercentElemControlSize > position.Y)
                {
                    if (fireOrGroundElemControl is FireElemControl)
                    {
                        var fireElemControl = (FireElemControl)fireOrGroundElemControl;
                        if (!IsFrozen)
                        {
                            //Force release
                            Status = ElemControlStatus.Released;
                            fireElemControl.PutOut();

                            animationView.IsAnimationEnabled = true;
                            await Task.WhenAll(
                                FaceImage.FadeTo(0, 300),
                                PrincipalImage.FadeTo(0, 300),
                                Task.Delay(700)
                            );
                            animationView.IsAnimationEnabled = false;

                            await Task.Delay(100);

                            Status = ElemControlStatus.Exploded;
                            return;
                        }
                        else
                        {
                            PrincipalImage.Source = PrincipalImageSource;
                            IsFrozen = false;

                            fireElemControl.PutOut();
                        }
                    }
                    else
                    {
                        var groundElemControl = (GroundElemControl)fireOrGroundElemControl;
                        groundElemControl.CompleteGroundWetAchievement();
                    }
                }
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