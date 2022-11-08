global using System.Diagnostics;
using SpeedElems.Controls;
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

    //On start load Audio files
    protected override void OnStart()
    {
        //Load Audio Players
        AudioPlayerManager.Create(typeof(FireElemControl), "fire.wav");
        AudioPlayerManager.Create(typeof(GroundElemControl), "ground.wav");
        AudioPlayerManager.Create(typeof(WindElemControl), "wind.wav");
        AudioPlayerManager.Create(typeof(ElectricityElemControl), "electricity.wav");
        AudioPlayerManager.Create(typeof(WaterElemControl), "water.wav");
        AudioPlayerManager.Create(typeof(BioElemControl), "bio.wav");

        AudioPlayerManager.CreateBackground("music.wav");
        if (Preferences.Get("Parameters.IsMusicActive", true))
            AudioPlayerManager.Background.Play();
    }

    protected override void OnResume()
    {
        base.OnResume();
        if (Preferences.Get("Parameters.IsMusicActive", true))
            AudioPlayerManager.Background.Play();
    }

    protected override void OnSleep()
    {
        base.OnSleep();
        if (Preferences.Get("Parameters.IsMusicActive", true))
            AudioPlayerManager.Background.Pause();
    }
}