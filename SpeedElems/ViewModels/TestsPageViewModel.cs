using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SpeedElems.ViewModels;

/// <summary>
/// Tests Page ViewModel
/// </summary>
public partial class TestsPageViewModel : ObservableObject
{
    #region Privates

    private string[] files;

    #endregion Privates

    #region Properties

    [ObservableProperty]
    private string metroLogs;

    [ObservableProperty]
    private int standardID;

    [ObservableProperty]
    private int easyID;

    #endregion Properties

    #region Commands

    [RelayCommand]
    private Task GoBack() => Shell.Current.GoToAsync("..", false);

    [RelayCommand]
    private void Clear()
    {
        files.ToList().ForEach(path => File.WriteAllText(path, String.Empty));
        MetroLogs = string.Empty;
    }

    [RelayCommand]
    private void Delete()
    {
        files.ToList().ForEach(path => File.Delete(path));
        MetroLogs = string.Empty;
    }

    [RelayCommand]
    private void PageAppearing() => LoadSettings();

    [RelayCommand]
    private void UpdateSettings()
    {
        Preferences.Set($"Levels.StandardID", StandardID);
        Preferences.Set($"Levels.EasyID", EasyID);
    }

    [RelayCommand]
    private void ResetAchievements()
    {
        // Kill
        Preferences.Set("Achievements.Elems_Fire", 0); // 1 / 7 / 13 / 205
        Preferences.Set("Achievements.Elems_Ground", 0); // 31 / 39 / 45 / 237
        Preferences.Set("Achievements.Elems_Wind", 0); // 61 / 73 / 85 / 209
        Preferences.Set("Achievements.Elems_Water", 0); // 101 / 120
        Preferences.Set("Achievements.Elems_Electricity", 0); // 131 / 221
        Preferences.Set("Achievements.Elems_Bio", 0); // 161 / 171 / 181 / ?
        Preferences.Set("Achievements.Elems_Ice", 0); // 191
        Preferences.Set("Achievements.Elems_TwoDifferent", false); // 41
        Preferences.Set("Achievements.Elems_ThreeDifferent", false); // 80
        Preferences.Set("Achievements.Elems_FourDifferent", false); //Lvl ?

        //Tricks
        Preferences.Set("Achievements.Tricks_PushTheGround", false); //S on Ground startscreen
        Preferences.Set("Achievements.Tricks_SpeedFastWithWind", false); //Click Wind startscreen
        Preferences.Set("Achievements.Tricks_CheckBox", false); //Click on itself
        Preferences.Set("Achievements.Tricks_FireCoal", false); // 1 - double click on Fire
        Preferences.Set("Achievements.Tricks_3ParchedGround", false); // 40
        Preferences.Set("Achievements.Tricks_Wind5cm", false); // 60
        Preferences.Set("Achievements.Tricks_NoFireForWater", false); // 131
        Preferences.Set("Achievements.Tricks_5Kills", false); //218 kill 5 Fire in 1 round
        Preferences.Set("Achievements.Tricks_GroundWet", false); //128 -  water on ground
        Preferences.Set("Achievements.Tricks_4Pressed", false); //4 controls pressed simultaneously  ??? (Lvl 130)
    }

    #endregion Commands

    #region Methods

    public void LoadSettings()
    {
        files = Directory.GetFiles(FileSystem.CacheDirectory + '/' + "MetroLogs");
        foreach (var file in files)
            MetroLogs += File.ReadAllText(file);

        StandardID = Preferences.Get($"Levels.StandardID", 1);
        EasyID = Preferences.Get($"Levels.EasyID", 1);
    }

    #endregion Methods
}