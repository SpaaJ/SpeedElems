using SpeedElems.Library;
using SpeedElems.Models;

namespace SpeedElems.Controls;

public partial class RoundProgressBar : ContentView
{
    #region Private

    private readonly ImageSource FaceNormalImageSource = ImagesManager.Get(ElemType.Block, ElemImageType.FaceNormal);
    private readonly ImageSource FaceWinImageSource = ImagesManager.Get(ElemType.Block, ElemImageType.FaceWin);
    private readonly ImageSource FaceLooseImageSource = ImagesManager.Get(ElemType.Block, ElemImageType.FaceLoose);

    #endregion Private

    #region Properties

    #region FaceImageSource Property

    public static readonly BindableProperty FaceImageSourceProperty = BindableProperty.Create(
        propertyName: nameof(FaceImageSource),
        returnType: typeof(ImageSource),
        declaringType: typeof(RoundProgressBar),
        defaultValue: null,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: null,
        propertyChanging: null
    );

    public ImageSource FaceImageSource
    {
        get { return (ImageSource)GetValue(FaceImageSourceProperty); }
        set { SetValue(FaceImageSourceProperty, value); }
    }

    #endregion FaceImageSource Property

    #region Status Property

    public static readonly BindableProperty StatusProperty = BindableProperty.Create(
        propertyName: nameof(Status),
        returnType: typeof(RoundStatus),
        declaringType: typeof(RoundProgressBar),
        defaultValue: RoundStatus.Pend,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: StatusPropertyChanged,
        propertyChanging: null
    );

    public RoundStatus Status
    {
        get { return (RoundStatus)GetValue(StatusProperty); }
        set { SetValue(StatusProperty, value); }
    }

    private static void StatusPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (RoundProgressBar)bindable;

        var status = (RoundStatus)newValue;
        switch (status)
        {
            case RoundStatus.Pend:
                control.IsVisible = false;
                break;

            case RoundStatus.Run:
                control.Animate("TimerBarProgress", arg => control.Progress = 1 - arg, 10, (uint)control.RoundDelay, Easing.Linear);
                control.FaceImageSource = control.FaceNormalImageSource;
                control.IsVisible = true;
                break;

            case RoundStatus.Win:
                control.FaceImageSource = control.FaceWinImageSource;
                control.AbortAnimation("TimerBarProgress");
                break;

            case RoundStatus.Loose:
                control.FaceImageSource = control.FaceLooseImageSource;
                control.AbortAnimation("TimerBarProgress");
                break;
        }
    }

    #endregion Status Property

    #region Progress Property

    public static readonly BindableProperty ProgressProperty = BindableProperty.Create(
        propertyName: nameof(Progress),
        returnType: typeof(double),
        declaringType: typeof(RoundProgressBar),
        defaultValue: 0d,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: null,
        propertyChanging: null
    );

    public double Progress
    {
        get { return (double)GetValue(ProgressProperty); }
        set { SetValue(ProgressProperty, value); }
    }

    #endregion Progress Property

    #region RoundDelay Property

    public static readonly BindableProperty RoundDelayProperty = BindableProperty.Create(
        propertyName: nameof(RoundDelay),
        returnType: typeof(int),
        declaringType: typeof(RoundProgressBar),
        defaultValue: 0,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: null,
        propertyChanging: null
    );

    public int RoundDelay
    {
        get { return (int)GetValue(RoundDelayProperty); }
        set { SetValue(RoundDelayProperty, value); }
    }

    #endregion RoundDelay Property

    #region Message Property

    public static readonly BindableProperty MessageProperty = BindableProperty.Create(
        propertyName: nameof(Message),
        returnType: typeof(string),
        declaringType: typeof(RoundProgressBar),
        defaultValue: null,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: null,
        propertyChanging: null
    );

    public string Message
    {
        get { return (string)GetValue(MessageProperty); }
        set { SetValue(MessageProperty, value); }
    }

    #endregion Message Property

    #region ErrorMessage Property

    public static readonly BindableProperty ErrorMessageProperty = BindableProperty.Create(
        propertyName: nameof(ErrorMessage),
        returnType: typeof(string),
        declaringType: typeof(RoundProgressBar),
        defaultValue: null,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: null,
        propertyChanging: null
    );

    public string ErrorMessage
    {
        get { return (string)GetValue(ErrorMessageProperty); }
        set { SetValue(ErrorMessageProperty, value); }
    }

    #endregion ErrorMessage Property

    #endregion Properties

    #region Constructor

    public RoundProgressBar()
    {
        InitializeComponent();

        var grid = (Grid)Content;
        grid.WidthRequest = 14 * SizesManager.ElemControlSize;

        IsVisible = false;
    }

    #endregion Constructor
}