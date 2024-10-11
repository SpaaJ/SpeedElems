global using System.Diagnostics;
using CommunityToolkit.Maui.Views;
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
        AudioPlayerManager.CreateBackgroundMediaElement();

        AudioPlayerManager.CreateMediaElement(typeof(FireElemControl), "fire.wav");
        AudioPlayerManager.CreateMediaElement(typeof(GroundElemControl), "ground.wav");
        AudioPlayerManager.CreateMediaElement(typeof(WindElemControl), "wind.wav");
        AudioPlayerManager.CreateMediaElement(typeof(ElectricityElemControl), "electricity.wav");
        AudioPlayerManager.CreateMediaElement(typeof(WaterElemControl), "water.wav");
        AudioPlayerManager.CreateMediaElement(typeof(BioElemControl), "bio.wav");
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