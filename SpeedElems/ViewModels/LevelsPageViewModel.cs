using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpeedElems.Library;
using SpeedElems.Models;
using SpeedElems.Pages;

namespace SpeedElems.ViewModels;

/// <summary>
/// Levels Page ViewModel
/// </summary>
public partial class LevelsPageViewModel : ObservableObject
{
    #region Privates

    private List<LevelLink> levelsList;
    private List<LevelLinksGroup> groupsList;
    private CollectionView collectionView;
    private LevelLink currentLevelLink;

    #endregion Privates

    #region Properties

    #region IsLevelsEasy

    public bool IsLevelsEasy
    {
        get { return Preferences.Get("Parameters.IsLevelsEasy", false); }
        set
        {
            if (value != Preferences.Get("Parameters.IsLevelsEasy", false))
            {
                Preferences.Set("Parameters.IsLevelsEasy", value);
                IsActivityIndicatorRunning = true;
                _ = LoadMaps();
                OnPropertyChanged();
            }
        }
    }

    #endregion IsLevelsEasy

    [ObservableProperty]
    private bool isActivityIndicatorRunning = true;

    [ObservableProperty]
    private List<LevelLinksGroup> levelLinksGroups = new();

    #region SelectedItem

    [ObservableProperty]
    public LevelLink selectedItem;

    /// <summary>
    /// Go to GamePage whene SelectedItem property changed
    /// </summary>
    partial void OnSelectedItemChanged(LevelLink value)
    {
        if (value is not null && value.ID <= currentLevelLink.ID)
        {
            Shell.Current.GoToAsync(nameof(GamePage), false, new Dictionary<string, object> { { "LevelID", selectedItem.ID } });
            SelectedItem = null;
        }
    }

    #endregion SelectedItem

    #endregion Properties

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    public LevelsPageViewModel()
    {
        //Update CurrentLevel
        MessagingCenter.Subscribe<GamePageViewModel>(this, "LevelStatus.Win", (sender) =>
        {
            levelsList.Single(l => l.ID == sender.CurrentElemsLevel.ID).UpdateImage();
            if (sender.CurrentElemsLevel.ID + 1 <= 250)
            {
                currentLevelLink = levelsList.Single(l => l.ID == sender.CurrentElemsLevel.ID + 1);
                currentLevelLink.IsEnabled = true;
                collectionView.ScrollTo(levelsList[currentLevelLink.ID - 1], groupsList[(int)currentLevelLink.ElemType - 1], ScrollToPosition.Center, false);
            }
        });
    }

    #endregion Constructor

    #region Commands

    [RelayCommand]
    private async Task PageAppearing() => await LoadMaps();

    [RelayCommand]
    private Task GoBack() => Shell.Current.GoToAsync("..", false);

    [RelayCommand]
    private Task GoToAchievementsPage() => Shell.Current.GoToAsync(nameof(AchievementsPage), false);

    [RelayCommand]
    private void CollectionViewLoaded(CollectionView control) => collectionView = control;

    #endregion Commands

    #region LoadMaps List Methods

    private async Task LoadMaps()
    {
        if (!IsActivityIndicatorRunning)
            return;

        await Task.Delay(500);

        bool isLevelsEasy = Preferences.Get("Parameters.IsLevelsEasy", false);
        int currentLevelID = Preferences.Get($"Levels.{(isLevelsEasy ? "Easy" : "Standard")}ID", 1);

        levelsList = new List<LevelLink>();
        for (int i = 1; i <= 250; i++)
        {
            if (i < currentLevelID)
            {
                var levelLink = new LevelLink(i);
                levelLink.isEnabled = true;
                levelsList.Add(levelLink);
            }
            else if (i > currentLevelID)
            {
                levelsList.Add(new LevelLink(i, "block.png"));
            }
            else
            {
                currentLevelLink = new LevelLink(i, "block.png");
                currentLevelLink.isEnabled = true;
                levelsList.Add(currentLevelLink);
            }
        }

        //if currentLevelID == 251
        if (currentLevelLink == null)
            currentLevelLink = levelsList.Last();

        groupsList = new List<LevelLinksGroup>{
            new LevelLinksGroup(LocalizationManager.Current.Resources["Fire"], ElemType.Fire, "Fire", levelsList.Between(l => l.ID, 1, 30).ToList()),
            new LevelLinksGroup(LocalizationManager.Current.Resources["Ground"], ElemType.Ground, "Ground", levelsList.Between(l => l.ID, 31, 60).ToList()),
            new LevelLinksGroup(LocalizationManager.Current.Resources["Wind"], ElemType.Wind, "Wind", levelsList.Between(l => l.ID, 61, 100).ToList()),
            new LevelLinksGroup(LocalizationManager.Current.Resources["Water"], ElemType.Water, "Water", levelsList.Between(l => l.ID, 101, 130).ToList()),
            new LevelLinksGroup(LocalizationManager.Current.Resources["ElectricityDuo"], ElemType.Electricity, "Electricityduo", levelsList.Between(l => l.ID, 131, 160).ToList()),
            new LevelLinksGroup(LocalizationManager.Current.Resources["Bio"], ElemType.Bio, "Bio", levelsList.Between(l => l.ID, 161, 190).ToList()),
            new LevelLinksGroup(LocalizationManager.Current.Resources["Ice"], ElemType.Ice, "Ice", levelsList.Between(l => l.ID, 191, 220).ToList()),
            new LevelLinksGroup(LocalizationManager.Current.Resources["ElectricityTrio"], ElemType.Electricity, "Electricitytrio", levelsList.Between(l => l.ID, 221, 250).ToList())
        };
        groupsList.ForEach(g =>
            g.ForEach(l =>
            {
                l.ElemType = g.ElemType;
                if (l.StatusImageSource == null)
                    l.UpdateImage();
            }
        ));

        LevelLinksGroups = groupsList;

        collectionView.ScrollTo(levelsList[currentLevelLink.ID - 1], groupsList[(int)currentLevelLink.ElemType - 1], ScrollToPosition.Center, false);
        IsActivityIndicatorRunning = false;
    }

    #endregion LoadMaps List Methods
}

/// <summary>
/// LevelLinks Group in CollectionView
/// </summary>
public class LevelLinksGroup : List<LevelLink>
{
    public string Name { get; set; }

    public ElemType ElemType { get; set; }

    public ImageSource ElemTypeSource { get; }

    public LevelLinksGroup(string name, ElemType elemType, string elemTypeSourceName, List<LevelLink> levels) :
        base(levels)
    {
        Name = name;
        ElemType = elemType;
        ElemTypeSource = $"{elemTypeSourceName}.png";
    }
}

/// <summary>
/// LevelLink in CollectionView
/// </summary>
public partial class LevelLink : ObservableObject
{
    public int ID { get; set; }

    public ElemType ElemType { get; set; }

    [ObservableProperty]
    public ImageSource statusImageSource;

    [ObservableProperty]
    public bool isEnabled; //to force IsEnabled after clean current level

    public LevelLink(int id) => ID = id;

    public LevelLink(int id, ImageSource statusImageSource) : this(id) => StatusImageSource = statusImageSource;

    public void UpdateImage() => StatusImageSource = ElemType.ToString() + ".png";
}