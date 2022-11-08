using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace SpeedElems.Library;

/// <summary>
/// Base page
/// </summary>
public abstract class BasePage : ContentPage
{
    protected ILogger? Logger;

    protected BasePage(object? viewModel = null, ILogger? logger = null)
    {
        BindingContext = viewModel;
        Logger = logger;
        Padding = 0;

        SetDynamicResource(BackgroundColorProperty, "AppBackgroundColor");

        if (string.IsNullOrWhiteSpace(Title))
            Title = GetType().Name;

        if (Logger != null)
        {
            Logger.LogInformation(null, $"[ ] Page {this.GetType().Name} created");
            AppDomain.CurrentDomain.UnhandledException += (s, e) => Logger.LogError((Exception)e.ExceptionObject, $"(X) {s.GetType().Name} UnhandledException");
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Debug.WriteLine($"OnAppearing: {Title}");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        Debug.WriteLine($"OnDisappearing: {Title}");
    }
}

/// <summary>
/// Base Page with BindingContext property
/// </summary>
/// <typeparam name="TViewModel"></typeparam>
public abstract class BasePage<TViewModel> : BasePage where TViewModel : ObservableObject
{
    protected BasePage(TViewModel viewModel, ILogger logger = null) : base(viewModel, logger)
    {
    }

    public new TViewModel BindingContext => (TViewModel)base.BindingContext;
}