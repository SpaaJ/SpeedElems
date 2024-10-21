global using System.Diagnostics;
using SpeedElems.Library;

namespace SpeedElems;

/// <summary>
/// App
/// </summary>
public partial class App : Application
{
    public App()
    {
        //Init CurrentCulture
        if (!Preferences.ContainsKey("Parameters.CurrentCulture"))
        {
            string lang = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            if (lang == "fr")
                Preferences.Set("Parameters.CurrentCulture", "fr");
            else
                Preferences.Set("Parameters.CurrentCulture", "en");
        }

        //Set Culture
        LocalizationManager.Current.SetCulture(Preferences.Get("Parameters.CurrentCulture", "en"));

        InitializeComponent();

        MainPage = new AppShell();
    }

    protected override void OnResume()
    {
        base.OnResume();
        if (Preferences.Get("Parameters.IsMusicActive", true))
            AudioPlayerManager.BackgroundMediaElement.Play();
    }

    protected override void OnSleep()
    {
        base.OnSleep();
        if (Preferences.Get("Parameters.IsMusicActive", true))
            AudioPlayerManager.BackgroundMediaElement.Pause();
    }
}