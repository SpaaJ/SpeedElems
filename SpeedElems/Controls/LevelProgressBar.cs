using SpeedElems.Library;

namespace SpeedElems.Controls;

/// <summary>
/// Level Progress Bar
/// </summary>
public class LevelProgressBar : StackLayout
{
    #region Privates

    private Color separatorColor;

    #endregion Privates

    #region Properties

    #region Rounds Property

    public static readonly BindableProperty RoundsProperty = BindableProperty.Create(
        propertyName: nameof(Rounds),
        returnType: typeof(int),
        declaringType: typeof(LevelProgressBar),
        defaultValue: null,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: RoundsPropertyChanged,
        propertyChanging: null
    );

    public int Rounds
    {
        get { return (int)GetValue(RoundsProperty); }
        set { SetValue(RoundsProperty, value); }
    }

    private static void RoundsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (LevelProgressBar)bindable;
        var rounds = (int)newValue;

        control.Children.Clear();

        for (int i = 0; i < rounds; i++)
        {
            var layout = new ContentView()
            {
                Content = new Image()
                {
                    Source = "block.png",
                    ClassId = $"{i + 1}",
                    Margin = new Thickness(-3),
                    WidthRequest = SizesManager.ElemControlSize / 2d,
                    HeightRequest = SizesManager.ElemControlSize / 2d
                }
            };

            control.Children.Add(layout);

            var imagesWidth = SizesManager.ElemControlSize / 2d - 6d;
            var remainingWidth = control.WidthRequest - (imagesWidth * (double)rounds);

            if (i < rounds - 1)
            {
                var separatorLine = new BoxView()
                {
                    HeightRequest = 6,
                    WidthRequest = remainingWidth / (double)(rounds - 1),
                    VerticalOptions = LayoutOptions.Center,
                    ClassId = $"{i + 1}"
                };
                control.Children.Add(separatorLine);
            }
        }
    }

    #endregion Rounds Property

    #region RoundSelected Property

    public static readonly BindableProperty RoundSelectedProperty = BindableProperty.Create(
        propertyName: nameof(RoundSelected),
        returnType: typeof(int),
        declaringType: typeof(LevelProgressBar),
        defaultValue: null,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: RoundSelectedPropertyChanged,
        propertyChanging: null
    );

    public int RoundSelected
    {
        get { return (int)GetValue(RoundSelectedProperty); }
        set { SetValue(RoundSelectedProperty, value); }
    }

    private static void RoundSelectedPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (LevelProgressBar)bindable;
        var rounds = (int)newValue;

        foreach (var child in control.Children.OfType<ContentView>().Select(l => l.Content))
            child.IsVisible = Convert.ToInt32(child.ClassId) <= rounds;

        foreach (var child in control.Children.OfType<BoxView>())
            child.Color = Convert.ToInt32(child.ClassId) < rounds ? control.separatorColor : Colors.Transparent;
    }

    #endregion RoundSelected Property

    #endregion Properties

    #region Contructor

    public LevelProgressBar()
    {
        HeightRequest = SizesManager.ElemControlSize;
        WidthRequest = 14 * SizesManager.ElemControlSize;

        Orientation = StackOrientation.Horizontal;
        HorizontalOptions = LayoutOptions.Center;
        Padding = new(0);
        Spacing = 0;
        separatorColor = Color.FromArgb("#99509714");
    }

    #endregion Contructor
}