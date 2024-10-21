namespace SpeedElems;

using CommunityToolkit.Maui;
using MetroLog.MicrosoftExtensions;
using Plugin.Maui.Audio;
using SkiaSharp.Views.Maui.Controls.Hosting;
using SpeedElems.Library;
using SpeedElems.Pages;
using SpeedElems.TouchTracking;
using SpeedElems.ViewModels;

/// <summary>
/// Maui Program
/// </summary>
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseSkiaSharp()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton(AudioManager.Current);
        AudioPlayerManager.AudioManager = AudioManager.Current;

        builder.ConfigureEffects(effects =>
        {
#if __ANDROID__
            effects.Add<TouchRoutingEffect, TouchPlatformEffect>();
#else
            //todo: Windows/iOS touch effects
#endif
        });

        //Logging
        builder.Logging.AddStreamingFileLogger(options =>
        {
            options.RetainDays = 2;
            options.FolderPath = Path.Combine(FileSystem.CacheDirectory, "MetroLogs");
        });

        RegisterPages(builder.Services);
        RegisterViewModels(builder.Services);

        return builder.Build();
    }

    private static void RegisterPages(in IServiceCollection services)
    {
        services.AddTransient<AchievementsPage>();
        services.AddTransient<GamePage>();
        services.AddTransient<HomePage>();
        services.AddTransient<LevelsPage>();
        services.AddTransient<ParametersPage>();
        services.AddTransient<TestsPage>();
    }

    private static void RegisterViewModels(in IServiceCollection services)
    {
        services.AddTransient<AchievementsPageViewModel>();
        services.AddTransient<GamePageViewModel>();
        services.AddTransient<HomePageViewModel>();
        services.AddTransient<LevelsPageViewModel>();
        services.AddTransient<ParametersPageViewModel>();
        services.AddTransient<TestsPageViewModel>();
    }
}