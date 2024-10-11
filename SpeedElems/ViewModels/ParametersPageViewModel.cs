using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpeedElems.Library;

namespace SpeedElems.ViewModels;

/// <summary>
/// Parameters Page ViewModel
/// </summary>
public partial class ParametersPageViewModel : ObservableObject
{
    #region Properties

    #region IsMusicActive Property

    private bool isMusicActive;

    public bool IsMusicActive
    {
        get { return isMusicActive; }
        set
        {
            isMusicActive = value;
            Preferences.Set("Parameters.IsMusicActive", value);

            if (AudioPlayerManager.Background.IsPlaying && !value)
                AudioPlayerManager.Background.Stop();
            else if (!AudioPlayerManager.Background.IsPlaying && value)
                AudioPlayerManager.Background.Play();

            OnPropertyChanged();
        }
    }

    #endregion IsMusicActive Property

    #region IsEffectsActive Property

    private bool isEffectsActive;

    public bool IsEffectsActive
    {
        get { return isEffectsActive; }
        set
        {
            isEffectsActive = value;
            Preferences.Set("Parameters.IsEffectsActive", value);
            OnPropertyChanged();
        }
    }

    #endregion IsEffectsActive Property

    #region IsEnglishChecked Property

    private bool isEnglishChecked;

    /// <summary>
    /// IsEnglishChecked Property
    /// </summary>
    public bool IsEnglishChecked
    {
        get => isEnglishChecked;
        set
        {
            if (value)
            {
                if (isEnglishChecked != value && Preferences.Get("Parameters.CurrentCulture", "en") != "en")
                {
                    LocalizationManager.Current.SetCulture("en");
                    Preferences.Set("Parameters.CurrentCulture", "en");
                }
            }

            isEnglishChecked = value;
            OnPropertyChanged();
        }
    }

    #endregion IsEnglishChecked Property

    #region IsFrenchChecked Property

    private bool _isFrenchChecked;

    /// <summary>
    /// IsFrenchChecked Property
    /// </summary>
    public bool IsFrenchChecked
    {
        get => _isFrenchChecked;
        set
        {
            if (value)
            {
                if (_isFrenchChecked != value && Preferences.Get("Parameters.CurrentCulture", "en") != "fr")
                {
                    LocalizationManager.Current.SetCulture("fr");
                    Preferences.Set("Parameters.CurrentCulture", "fr");
                }
            }

            _isFrenchChecked = value;
            OnPropertyChanged();
        }
    }

    #endregion IsFrenchChecked Property

    [ObservableProperty]
    private string version = AppInfo.Current.VersionString;

    #endregion Properties

    #region Commands

    [RelayCommand]
    private Task GoBack() => Shell.Current.GoToAsync("..", false);

    #endregion Commands

    #region Constructor

    public ParametersPageViewModel()
    {
        IsMusicActive = Preferences.Get("Parameters.IsMusicActive", true);
        IsEffectsActive = Preferences.Get("Parameters.IsEffectsActive", true);
        switch (Preferences.Get("Parameters.CurrentCulture", "en"))
        {
            case "en": IsEnglishChecked = true; break;
            case "fr": IsFrenchChecked = true; break;
        }
    }

    #endregion Constructor
}