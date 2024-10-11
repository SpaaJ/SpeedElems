using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpeedElems.Library;
using SpeedElems.Pages;

namespace SpeedElems.ViewModels;

/// <summary>
/// Home Page ViewModel
/// </summary>
public partial class HomePageViewModel : ObservableObject
{
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

    #region Commands

    [RelayCommand]
    private void PushTheGround() => CurrentAchievementMessage = AchievementsManager.Complete("Tricks_PushTheGround");

    [RelayCommand]
    private void PushTheWind() => CurrentAchievementMessage = AchievementsManager.Complete("Tricks_SpeedFastWithWind");

    [RelayCommand]
    private Task GoToParametersPage() => Shell.Current.GoToAsync(nameof(ParametersPage), false);

    [RelayCommand]
    private Task GoToAchievementsPage() => Shell.Current.GoToAsync(nameof(AchievementsPage), false);

    [RelayCommand]
    private Task GoToLevelsPage() => Shell.Current.GoToAsync(nameof(LevelsPage), false);

    [RelayCommand]
    private Task GoToTestsPage()
    {
        testCount++;
        if (testCount == 10)
            return Shell.Current.GoToAsync(nameof(TestsPage), false);
        else
            return Task.CompletedTask;
    }

    [RelayCommand]
    private void PageAppearing() => testCount = 0;

    private int testCount;

    #endregion Commands
}