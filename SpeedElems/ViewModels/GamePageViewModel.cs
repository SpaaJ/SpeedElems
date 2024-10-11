using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpeedElems.Library;
using SpeedElems.Models;

namespace SpeedElems.ViewModels;

/// <summary>
/// Game Page ViewModel
/// </summary>
public partial class GamePageViewModel : ObservableObject, IQueryAttributable
{
    #region Properties

    [ObservableProperty]
    private int levelID;

    [ObservableProperty]
    private ElemsLevel currentElemsLevel;

    #region LevelStatus

    [ObservableProperty]
    private LevelStatus levelStatus;

    partial void OnLevelStatusChanged(LevelStatus value)
    {
        Debug.WriteLine("--------LevelStatus: " + value);

        if (value == LevelStatus.Win)
        {
            if (Preferences.Get("Parameters.IsLevelsEasy", false))
            {
                if (CurrentElemsLevel.ID + 1 > Preferences.Get("Levels.EasyID", 1))
                {
                    Preferences.Set("Levels.EasyID", CurrentElemsLevel.ID + 1);
                    MessagingCenter.Send(this, "LevelStatus.Win");
                }
            }
            else
            {
                if (CurrentElemsLevel.ID + 1 > Preferences.Get("Levels.StandardID", 1))
                {
                    Preferences.Set("Levels.StandardID", CurrentElemsLevel.ID + 1);
                    MessagingCenter.Send(this, "LevelStatus.Win");

                    if (CurrentElemsLevel.ID + 1 > Preferences.Get("Levels.EasyID", 1))
                        Preferences.Set("Levels.EasyID", CurrentElemsLevel.ID + 1);
                }
            }
        }
    }

    #endregion LevelStatus

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

    #region IQueryAttributable

    public void ApplyQueryAttributes(IDictionary<string, object> query) => LevelID = (int)query["LevelID"];

    #endregion IQueryAttributable

    #region Commands

    [RelayCommand]
    private Task GoBack() => Shell.Current.GoToAsync($"..", false);

    [RelayCommand]
    private async Task PageAppearing() => CurrentElemsLevel = await LevelsManager.GetLevelAsync(levelID);

    [RelayCommand]
    private async Task NextLevel()
    {
        levelID = CurrentElemsLevel.ID + 1;
        CurrentElemsLevel = await LevelsManager.GetLevelAsync(levelID);
    }

    #endregion Commands
}