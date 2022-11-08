using System.Windows.Input;
using SpeedElems.Library;
using SpeedElems.Models;

namespace SpeedElems.Controls;

/// <summary>
/// LevelStatus Control
/// </summary>
public partial class LevelStatusControl : ContentView
{
    #region Properties

    #region CurrentElemsLevel Property

    /// <summary>
    /// CurrentElemsLevel Property
    /// </summary>
    public static readonly BindableProperty CurrentElemsLevelProperty = BindableProperty.Create(
        propertyName: nameof(CurrentElemsLevel),
        returnType: typeof(ElemsLevel),
        declaringType: typeof(LevelStatusControl),
        defaultValue: null,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: CurrentElemsLevelPropertyChanged,
        propertyChanging: null
    );

    /// <summary>
    /// CurrentElemsLevel
    /// </summary>
    public ElemsLevel CurrentElemsLevel
    {
        get { return (ElemsLevel)GetValue(CurrentElemsLevelProperty); }
        set { SetValue(CurrentElemsLevelProperty, value); }
    }

    private static async void CurrentElemsLevelPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var levelStatusControl = (LevelStatusControl)bindable;
        var elemsLevel = (ElemsLevel)newValue;

        //When the CurrentElemsLevel is set by the GamePage ViewModel on PageAppearing
        await levelStatusControl.LoadingAnimation();
    }

    #endregion CurrentElemsLevel Property

    #region LevelStatus Property

    public static readonly BindableProperty LevelStatusProperty = BindableProperty.Create(
        propertyName: nameof(LevelStatus),
        returnType: typeof(LevelStatus),
        declaringType: typeof(LevelStatusControl),
        defaultValue: LevelStatus.Pend,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: LevelStatusPropertyChanged,
        propertyChanging: null
    );

    public LevelStatus LevelStatus
    {
        get { return (LevelStatus)GetValue(LevelStatusProperty); }
        set { SetValue(LevelStatusProperty, value); }
    }

    private static async void LevelStatusPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (LevelStatusControl)bindable;
        var status = (LevelStatus)newValue;

        switch (status)
        {
            case LevelStatus.Pend:
                control.IsVisible = false;
                break;

            case LevelStatus.Load:
                //not run LoadingAnimation on PageAppearing when CurrentElemsLevel is not setted)
                if (control.CurrentElemsLevel is not null)
                    await control.LoadingAnimation();
                break;

            case LevelStatus.Run:
                control.IsVisible = false;
                break;

            case LevelStatus.Win:
                control.StatusImage.IsVisible = true;
                control.StatusImage.Source = LocalizationManager.Current.Images["Win" + new Random().Next(3)];
                control.IsVisible = true;
                await Task.Delay(1000);
                if (control.CurrentElemsLevel.ID < 250)
                    control.NextImageButton.IsVisible = true;
                else
                    control.FinishedImage.IsVisible = true;

                break;

            case LevelStatus.Missed:
                control.StatusImage.IsVisible = true;
                control.StatusImage.Source = LocalizationManager.Current.Images["Missed"];
                control.IsVisible = true;
                await Task.Delay(1000);
                control.RestartImageButton.IsVisible = true;
                break;

            case LevelStatus.Abused:
                control.StatusImage.IsVisible = true;
                control.StatusImage.Source = LocalizationManager.Current.Images["Abused"];
                control.IsVisible = true;
                await Task.Delay(1000);
                control.RestartImageButton.IsVisible = true;
                break;
        }
    }

    #endregion LevelStatus Property

    #region NextLevelCommand Property

    public static readonly BindableProperty NextLevelCommandProperty = BindableProperty.Create(
        propertyName: nameof(NextLevelCommand),
        returnType: typeof(ICommand),
        declaringType: typeof(LevelStatusControl),
        defaultValue: null,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: null,
        propertyChanging: null
    );

    public ICommand NextLevelCommand
    {
        get { return (ICommand)GetValue(NextLevelCommandProperty); }
        set { SetValue(NextLevelCommandProperty, value); }
    }

    #endregion NextLevelCommand Property

    #endregion Properties

    #region Constructor

    public LevelStatusControl()
    {
        InitializeComponent();
    }

    #endregion Constructor

    #region LoadingAnimation

    /// <summary>
    /// Play a loading animation 3..2..1...GO
    /// </summary>
    /// <returns></returns>
    private async Task LoadingAnimation()
    {
        IsVisible = true;
        LevelHorizontalStackLayout.IsVisible = true;
        StatusImage.IsVisible = true;
        RestartImageButton.IsVisible = false;
        NextImageButton.IsVisible = false;

        int levelID = CurrentElemsLevel.ID;
        DigitOneImage.Source = levelID > 99 ? "digit_" + levelID.ToString("000")[0] + ".png" : null;
        DigitTwoImage.Source = levelID > 9 ? "digit_" + levelID.ToString("000")[1] + ".png" : null;
        DigitThreeImage.Source = "digit_" + levelID.ToString("000")[2] + ".png";

        StatusImage.Source = "three.png";
        await Task.Delay(500);
        StatusImage.Source = "two.png";
        await Task.Delay(500);
        StatusImage.Source = "one.png";
        await Task.Delay(500);
        StatusImage.Source = LocalizationManager.Current.Images["start"];
        await Task.Delay(500);

        LevelHorizontalStackLayout.IsVisible = false;
        StatusImage.IsVisible = false;

        LevelStatus = LevelStatus.Run;
    }

    #endregion LoadingAnimation

    #region RestartImageButton Clicked

    private void RestartImageButton_Clicked(object sender, EventArgs e)
    {
        LevelStatus = LevelStatus.Load;
    }

    #endregion RestartImageButton Clicked

    #region NextImageButton Clicked

    private void NextImageButton_Clicked(object sender, EventArgs e)
    {
        NextLevelCommand.Execute(null);
        LevelStatus = LevelStatus.Load;
    }

    #endregion NextImageButton Clicked
}