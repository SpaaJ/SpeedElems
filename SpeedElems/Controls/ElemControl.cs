using SpeedElems.Library;
using SpeedElems.Models;
using static SpeedElems.Library.AchievementManager;

namespace SpeedElems.Controls;

/// <summary>
/// Elem Control
/// </summary>
public abstract class ElemControl : ContentView
{
    #region Privates

    private IDispatcherTimer faceAnimationTimer;

    #endregion Privates

    #region Protecteds

    protected Image PrincipalImage;
    protected Image FaceImage;
    protected ImageSource PrincipalImageSource;
    protected ImageSource FaceNormalImageSource;
    protected ImageSource FaceWinImageSource;
    protected ImageSource FaceLooseImageSource;
    protected Rect LayoutBounds;

    #endregion Protecteds

    #region Properties

    public bool IsPressed { get; set; }

    #region Status Property

    private ElemControlStatus status;

    public ElemControlStatus Status
    {
        get { return status; }
        set
        {
            var oldValue = status;
            var newValue = value;

            status = value;

            if (newValue != oldValue && StatusChanged is not null)
                StatusChanged(this, new StatusEventArgs(status));

            if (status == ElemControlStatus.Loaded)
            {
                faceAnimationTimer.Start();
            }
            else if (status == ElemControlStatus.Missed || status == ElemControlStatus.Abused)
            {
                faceAnimationTimer.Stop();
                if (this is ElectricityElemControl)
                    ((ElectricityElemControl)this).StopSecondaryAnimation();
                FaceImage.Source = FaceLooseImageSource;
            }
            else if (status == ElemControlStatus.Released)
            {
                faceAnimationTimer.Stop();
                if (Preferences.Get("Parameters.IsEffectsActive", true))
                {
                    var media = AudioPlayerManager.TypeMediaElements[GetType()];
                    media.Stop();
                    media.Play();
                }
            }
            else if (faceAnimationTimer.IsRunning)
            {
                faceAnimationTimer.Stop();
            }
        }
    }

    #endregion Status Property

    #endregion Properties

    #region Events

    public event EventHandler<StatusEventArgs> StatusChanged;

    #endregion Events

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    public ElemControl()
    {
        PrincipalImage = new();
        FaceImage = new();

        Content = new Grid { Children = { PrincipalImage, FaceImage } };

        faceAnimationTimer = Dispatcher.CreateTimer();
        faceAnimationTimer.Interval = TimeSpan.FromMilliseconds(new Random().Next(3000, 5000));
        faceAnimationTimer.Tick += async (sender, e) =>
        {
            FaceImage.Source = FaceLooseImageSource;
            await Task.Delay(200);
            FaceImage.Source = FaceNormalImageSource;
        };
    }

    #endregion Constructor

    #region Methods

    public abstract void Press(Point location = default);

    public abstract void Move(Point location);

    public abstract void Release();

    public void StartAnimation()
    {
        faceAnimationTimer.Start();
    }

    public void StopAnimation()
    {
        faceAnimationTimer.Stop();
    }

    #endregion Methods

    #region AchievementEvent

    public event AchievementEventHandler AchievementEnded;

    protected virtual void CompleteAchievement(string name) => AchievementEnded?.Invoke(this, new AchievementEventArgs(name));

    #endregion AchievementEvent
}