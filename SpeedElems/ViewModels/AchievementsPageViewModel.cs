using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpeedElems.Library;

namespace SpeedElems.ViewModels;

/// <summary>
/// Achievements Page ViewModel
/// </summary>
public partial class AchievementsPageViewModel : ObservableObject
{
    #region Properties

    [ObservableProperty]
    private int elems_Fire;

    [ObservableProperty]
    private int elems_Ground;

    [ObservableProperty]
    private int elems_Wind;

    [ObservableProperty]
    private int elems_Water;

    [ObservableProperty]
    private int elems_Electricity;

    [ObservableProperty]
    private int elems_Bio;

    [ObservableProperty]
    private int elems_Ice;

    [ObservableProperty]
    private bool elems_TwoDifferent;

    [ObservableProperty]
    private bool elems_ThreeDifferent;

    [ObservableProperty]
    private bool elems_FourDifferent;

    [ObservableProperty]
    private bool tricks_PushTheGround;

    [ObservableProperty]
    private bool tricks_SpeedFastWithWind;

    [ObservableProperty]
    private bool tricks_CheckBox;

    [ObservableProperty]
    private bool tricks_FireCoal;

    [ObservableProperty]
    private bool tricks_3ParchedGround;

    [ObservableProperty]
    private bool tricks_Wind5cm;

    [ObservableProperty]
    private bool tricks_NoFireForWater;

    [ObservableProperty]
    private bool tricks_5Kills;

    [ObservableProperty]
    private bool tricks_GroundWet;

    [ObservableProperty]
    private bool tricks_4Pressed;

    #region CurrentAchievement Message Property

    private string? currentAchievementMessage;

    /// <summary>
    /// CurrentAchievementMessage Property
    /// </summary>
    public string? CurrentAchievementMessage
    {
        get => currentAchievementMessage;
        set
        {
            currentAchievementMessage = value;
            if (value != null)
            {
                OnPropertyChanged();
                Task.Run(async () =>
                {
                    await Task.Delay(2000);
                    currentAchievementMessage = null;
                    OnPropertyChanged();
                });
            }
        }
    }

    #endregion CurrentAchievement Message Property

    #endregion Properties

    #region Commands

    [ICommand]
    private Task GoBack() => Shell.Current.GoToAsync("..", false);

    [ICommand]
    private void PageAppearing() => LoadAchievements();

    #endregion Commands

    #region Methods

    private void LoadAchievements()
    {
        // Kill
        Elems_Fire = Preferences.Get("Achievements.Elems_Fire", 0); // 1 / 7 / 13 / 205
        Elems_Ground = Preferences.Get("Achievements.Elems_Ground", 0); // 31 / 39 / 45 / 237
        Elems_Wind = Preferences.Get("Achievements.Elems_Wind", 0); // 61 / 73 / 85 / 209
        Elems_Water = Preferences.Get("Achievements.Elems_Water", 0); // 101 / 120
        Elems_Electricity = Preferences.Get("Achievements.Elems_Electricity", 0); // 131 / 221
        Elems_Bio = Preferences.Get("Achievements.Elems_Bio", 0); // 161 / 171 / 181 / ?
        Elems_Ice = Preferences.Get("Achievements.Elems_Ice", 0); // 191
        Elems_TwoDifferent = Preferences.Get("Achievements.Elems_TwoDifferent", false); // 41
        Elems_ThreeDifferent = Preferences.Get("Achievements.Elems_ThreeDifferent", false); // 80
        Elems_FourDifferent = Preferences.Get("Achievements.Elems_FourDifferent", false); //Lvl ?

        //Tricks
        Tricks_PushTheGround = Preferences.Get("Achievements.Tricks_PushTheGround", false); //S on Ground startscreen
        Tricks_SpeedFastWithWind = Preferences.Get("Achievements.Tricks_SpeedFastWithWind", false); //Click Wind startscreen
        Tricks_CheckBox = Preferences.Get("Achievements.Tricks_CheckBox", false); //Click on itself
        Tricks_FireCoal = Preferences.Get("Achievements.Tricks_FireCoal", false); // 1 - double click on Fire
        Tricks_3ParchedGround = Preferences.Get("Achievements.Tricks_3ParchedGround", false); // 40
        Tricks_Wind5cm = Preferences.Get("Achievements.Tricks_Wind5cm", false); // 60
        Tricks_NoFireForWater = Preferences.Get("Achievements.Tricks_NoFireForWater", false); // 131
        Tricks_5Kills = Preferences.Get("Achievements.Tricks_5Kills", false); //218 kill 5 Fire in 1 round
        Tricks_GroundWet = Preferences.Get("Achievements.Tricks_GroundWet", false); //128 -  water on ground
        Tricks_4Pressed = Preferences.Get("Achievements.Tricks_4Pressed", false); //4 controls pressed simultaneously  ??? (Lvl 130)

        PropertyChanged += AchievementsPageViewModel_PropertyChanged;
    }

    private void AchievementsPageViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tricks_CheckBox))
            CurrentAchievementMessage = AchievementsManager.Complete("Tricks_CheckBox");
    }

    #endregion Methods
}