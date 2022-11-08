using Microsoft.Extensions.Logging;
using SpeedElems.Library;
using SpeedElems.ViewModels;

namespace SpeedElems.Pages;

/// <summary>
/// Game Page
/// </summary>
public partial class GamePage : BasePage<GamePageViewModel>
{
    public GamePage(GamePageViewModel viewModel, ILogger<GamePage> logger) : base(viewModel, logger)
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

#if __ANDROID__
        GameControl.Effects.Add(GameControl.GameTouchPlatformEffect);
#else
        //todo: Windows/iOS touch effects
#endif
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
#if __ANDROID__
        GameControl.Effects.Remove(GameControl.GameTouchPlatformEffect);
#else
        //todo: Windows/iOS touch effects
#endif
    }
}