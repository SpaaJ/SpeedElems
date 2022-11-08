using System.Collections.ObjectModel;
using System.Numerics;
using SpeedElems.Library;
using SpeedElems.Models;
using SpeedElems.TouchTracking;

namespace SpeedElems.Controls;

/// <summary>
/// Game Control
/// </summary>
public class GameControl : ContentView
{
    #region Privates

    private int currentRound;
    private readonly bool isLevelsEasy;
    private readonly GameAbsoluteLayout gameAbsoluteLayout;
    private readonly ElemControl[] pressedControls = new ElemControl[10];
    private readonly StoppableTimer roundStoppableTimer;
    private readonly List<ElemControl> explosionList = new();

    //For Water
    private readonly ObservableCollection<ElemControl> fireAndGroundElemControlCollection = new();

    //For Bio
    private BioElemControl? pressedBioElemControl;

    private readonly Dictionary<long, Vector2> aroundBioElemPressedPoints = new();

    //For Electricity
    private Image electricityElemConnectionImage1, electricityElemConnectionImage2, electricityElemConnectionImage3;

    private int electricityElemControlPressed = 0;

    #endregion Privates

    #region Properties

#if __ANDROID__
    public TouchPlatformEffect GameTouchPlatformEffect;
#else
    //todo: Windows/iOS touch effects
#endif

    #region CurrentElemsLevel Property

    /// <summary>
    /// CurrentElemsLevel Property
    /// </summary>
    public static readonly BindableProperty CurrentElemsLevelProperty = BindableProperty.Create(
        propertyName: nameof(CurrentElemsLevel),
        returnType: typeof(ElemsLevel),
        declaringType: typeof(GameControl),
        defaultValue: null,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: CurrentElemsLevelPropertyChanged,
        propertyChanging: null
    );

    /// <summary>
    /// CurrentElemsLevel
    /// </summary>
    public ElemsLevel CurrentElemsLevel
    {
        get { return (ElemsLevel)GetValue(CurrentElemsLevelProperty); }
        set { SetValue(CurrentElemsLevelProperty, value); }
    }

    /// <summary>
    /// CurrentElemsLevel Property Changed
    /// </summary>
    /// <param name="bindable"></param>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private static void CurrentElemsLevelPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var gameControl = (GameControl)bindable;

        //When the CurrentElemsLevel is set by the GamePage ViewModel
        //Load the CurrentElemsLevel
        gameControl.LevelStatus = LevelStatus.Load;
    }

    #endregion CurrentElemsLevel Property

    #region Rounds Property

    public static readonly BindableProperty RoundsProperty = BindableProperty.Create(
        nameof(Rounds),
        typeof(int),
        typeof(GameControl),
        default(int)
    );

    public int Rounds
    {
        get { return (int)GetValue(RoundsProperty); }
        set { SetValue(RoundsProperty, value); }
    }

    #endregion Rounds Property

    #region RoundSelected Property

    public static readonly BindableProperty RoundSelectedProperty = BindableProperty.Create(
        nameof(RoundSelected),
        typeof(int),
        typeof(GameControl),
        default(int)
    );

    public int RoundSelected
    {
        get { return (int)GetValue(RoundSelectedProperty); }
        set { SetValue(RoundSelectedProperty, value); }
    }

    #endregion RoundSelected Property

    #region LevelStatus Property

    public static readonly BindableProperty LevelStatusProperty = BindableProperty.Create(
        propertyName: nameof(LevelStatus),
        returnType: typeof(LevelStatus),
        declaringType: typeof(GameControl),
        defaultValue: LevelStatus.Pend,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: LevelStatusPropertyChanged,
        propertyChanging: null
    );

    public LevelStatus LevelStatus
    {
        get { return (LevelStatus)GetValue(LevelStatusProperty); }
        set { SetValue(LevelStatusProperty, value); }
    }

    private static void LevelStatusPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var gameControl = (GameControl)bindable;
        var levelStatus = (LevelStatus)newValue;

        switch (levelStatus)
        {
            case LevelStatus.Load:
                gameControl.Rounds = 0;
                gameControl.RoundSelected = 0;
                gameControl.RoundStatus = RoundStatus.Pend;
                gameControl.ClearElemsAbsoluteLayout();
                break;

            case LevelStatus.Run:
                gameControl.currentRound = 0;
                gameControl.LoadElemsRound();

                var roundCount = gameControl.CurrentElemsLevel.Elems.Select(e => e.Round).Distinct().Count();
                gameControl.Rounds = roundCount;
                gameControl.RoundSelected = roundCount;
                break;
        }
    }

    #endregion LevelStatus Property

    #region RoundStatus Property

    public static readonly BindableProperty RoundStatusProperty = BindableProperty.Create(
        propertyName: nameof(RoundStatus),
        returnType: typeof(RoundStatus),
        declaringType: typeof(GameControl),
        defaultValue: RoundStatus.Pend,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: null,
        propertyChanging: null
    );

    public RoundStatus RoundStatus
    {
        get { return (RoundStatus)GetValue(RoundStatusProperty); }
        set { SetValue(RoundStatusProperty, value); }
    }

    #endregion RoundStatus Property

    #region RoundDelay Property

    public static readonly BindableProperty RoundDelayProperty = BindableProperty.Create(
        propertyName: nameof(RoundDelay),
        returnType: typeof(int),
        declaringType: typeof(GameControl),
        defaultValue: null,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: null,
        propertyChanging: null
    );

    public int RoundDelay
    {
        get { return (int)GetValue(RoundDelayProperty); }
        set { SetValue(RoundDelayProperty, value); }
    }

    #endregion RoundDelay Property

    #region Message Property

    public static readonly BindableProperty MessageProperty = BindableProperty.Create(
        propertyName: nameof(Message),
        returnType: typeof(string),
        declaringType: typeof(GameControl),
        defaultValue: null,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: null,
        propertyChanging: null
    );

    public string? Message
    {
        get { return (string)GetValue(MessageProperty); }
        set { SetValue(MessageProperty, value); }
    }

    #endregion Message Property

    #region ErrorMessage Property

    public static readonly BindableProperty ErrorMessageProperty = BindableProperty.Create(
        propertyName: nameof(ErrorMessage),
        returnType: typeof(string),
        declaringType: typeof(GameControl),
        defaultValue: null,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: null,
        propertyChanging: null
    );

    public string? ErrorMessage
    {
        get { return (string)GetValue(ErrorMessageProperty); }
        set { SetValue(ErrorMessageProperty, value); }
    }

    #endregion ErrorMessage Property

    #region CurrentAchievementMessage Property

    public static readonly BindableProperty CurrentAchievementMessageProperty = BindableProperty.Create(
        propertyName: nameof(CurrentAchievementMessage),
        returnType: typeof(string),
        declaringType: typeof(GameControl),
        defaultValue: null,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: null,
        propertyChanging: null
    );

    public string CurrentAchievementMessage
    {
        get { return (string)GetValue(CurrentAchievementMessageProperty); }
        set { SetValue(CurrentAchievementMessageProperty, value); }
    }

    #endregion CurrentAchievementMessage Property

    #endregion Properties

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    public GameControl()
    {
        //var grid = new Grid();
        gameAbsoluteLayout = new GameAbsoluteLayout();
        Content = gameAbsoluteLayout;

#if __ANDROID__

        GameTouchPlatformEffect = new() { Capture = true };
        GameTouchPlatformEffect.TouchAction += TouchPlatformEffect_TouchAction;
#else

        //todo: Windows/iOS touch effects

#endif

        isLevelsEasy = Preferences.Get("Parameters.IsLevelsEasy", false);
        roundStoppableTimer = new StoppableTimer(() =>
            {
                ClearPressedControls();
                SetNotReleasedControlsToMissed();
                RoundStatus = RoundStatus.Loose;
                LevelStatus = LevelStatus.Missed;
            }
        );
    }

    #endregion Constructor

    #region clearPressedControls

    /// <summary>
    /// clear Pressed Controls array
    /// </summary>
    private void ClearPressedControls()
    {
        for (int i = 0; i < 10; i++)
            pressedControls[i] = null;
    }

    #endregion clearPressedControls

    #region setNotReleasedControlsToMissed

    /// <summary>
    /// Set NotReleased Controls to Missed
    /// </summary>
    private void SetNotReleasedControlsToMissed()
    {
        foreach (var control in gameAbsoluteLayout.ElemControls.Where(e => e.Status < ElemControlStatus.Released))
            control.Status = ElemControlStatus.Missed;
    }

    #endregion setNotReleasedControlsToMissed

    #region LoadElemsRound

    /// <summary>
    /// LoadElemsRound in gameAbsoluteLayout
    /// </summary>
    private void LoadElemsRound()
    {
        ClearElemsAbsoluteLayout();
        explosionList.Clear();

        //Load all ElemControls in AbsoluteLayout
        var currentRoundElems = CurrentElemsLevel.Elems.Where(e => e.Round == currentRound);
        foreach (var elem in currentRoundElems)
        {
            ElemControl elemControl = elem.Type switch
            {
                ElemType.Fire => new FireElemControl(),
                ElemType.Ground => new GroundElemControl(),
                ElemType.Wind => new WindElemControl(),
                ElemType.Electricity => new ElectricityElemControl(),
                ElemType.Water => new WaterElemControl(elem.Isfrozen),
                ElemType.Bio => new BioElemControl(),
                _ => throw new NotImplementedException()
            };

            gameAbsoluteLayout.Add(elemControl);
            gameAbsoluteLayout.SetLayoutBounds(
                elemControl,
                new Rect(
                    SizesManager.ElemControlSize * elem.Column + SizesManager.ElemControlDecalX,
                    SizesManager.ElemControlSize * elem.Row + SizesManager.ElemControlDecalY,
                    SizesManager.ElemControlSize,
                    SizesManager.ElemControlSize
                )
            );

            elemControl.StatusChanged += ElemControl_StatusChanged;
            elemControl.AchievementEnded += ElemControl_AchievementEnded;
            elemControl.Status = ElemControlStatus.Loaded;
        }

        //If there is WaterElemControls : set FireAndGroundElemControlCollection
        fireAndGroundElemControlCollection.Clear();
        if (gameAbsoluteLayout.WaterElemControls.Any())
        {
            gameAbsoluteLayout.FireElemControls.ToList().ForEach(e => fireAndGroundElemControlCollection.Add(e));
            gameAbsoluteLayout.GroundElemControls.ToList().ForEach(e => fireAndGroundElemControlCollection.Add(e));
            gameAbsoluteLayout.WaterElemControls.ToList().ForEach(e => e.FireAndGroundElemControlCollection = fireAndGroundElemControlCollection);
        }

        //Timer delay
        RoundDelay = isLevelsEasy ? 3000 : 2000;

        //Messages
        if (CurrentElemsLevel.Messages is not null)
        {
            if (CurrentElemsLevel.Messages.Length > currentRound)
            {
                Message = LocalizationManager.Current.Resources[CurrentElemsLevel.Messages[currentRound]];
                RoundDelay = 20000;
            }
            else
            {
                Message = null;
            }
        }
        ErrorMessage = null;

        //Start round
        roundStoppableTimer.StartOnce(RoundDelay);
        RoundStatus = RoundStatus.Run;

        // ArrangeChildren -> Redraw AbsoluteLayout
        gameAbsoluteLayout.ArrangeChildren();
    }

    #endregion LoadElemsRound

    #region ClearElems

    /// <summary>
    /// Clear Elems in gameAbsoluteLayout
    /// </summary>
    private void ClearElemsAbsoluteLayout()
    {
        var elemControls = gameAbsoluteLayout.ElemControls.ToList();
        foreach (var elemControl in elemControls)
        {
            elemControl.StatusChanged -= ElemControl_StatusChanged;
            elemControl.AchievementEnded -= ElemControl_AchievementEnded;
            elemControl.StopAnimation();
            if (elemControl is ElectricityElemControl)
                ((ElectricityElemControl)elemControl).StopSecondaryAnimation();
            gameAbsoluteLayout.Children.Remove(elemControl);
        }
        electricityElemConnectionImage1 = null;
        electricityElemConnectionImage2 = null;
        electricityElemConnectionImage3 = null;
    }

    #endregion ClearElems

    #region TouchPlatformEffect TouchAction

    /// <summary>
    /// TouchPlatformEffect TouchAction
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args">
    /// - The Type of TouchEffect
    /// - The location
    /// - The ID (for multiple touch)
    /// </param>
    private void TouchPlatformEffect_TouchAction(object sender, TouchActionEventArgs args)
    {
        switch (args.Type)
        {
            /////////////////////////////////////////////////////////////////////////////////
            //Pressed
            case TouchActionType.Pressed:

                //All ElemControls
                var controls = gameAbsoluteLayout.ElemControls
                    .Where(c => c.Status == ElemControlStatus.Loaded || (c is FireElemControl && c.Status == ElemControlStatus.Released))
                    .Where(c => c is not BioElemControl)
                    .Where(c =>
                    {
                        var layoutBound = AbsoluteLayout.GetLayoutBounds(c);
                        return layoutBound.X + c.TranslationX - SizesManager.TwentyPercentElemControlSize < args.Location.X &&
                               layoutBound.Y + c.TranslationY - SizesManager.TwentyPercentElemControlSize < args.Location.Y &&
                               layoutBound.X + c.TranslationX + SizesManager.ElemControlSize + SizesManager.TwentyPercentElemControlSize > args.Location.X &&
                               layoutBound.Y + c.TranslationY + SizesManager.ElemControlSize + SizesManager.TwentyPercentElemControlSize > args.Location.Y;
                    });

                //Take the control with the max ZIndex
                var control = controls.SingleOrDefault(e => e.ZIndex == controls.Max(e => e.ZIndex));
                if (control is not null)
                {
                    control.Press(args.Location);
                    pressedControls[args.Id] = control;
                }

                //BioElemControls around pressed Points
                else if (gameAbsoluteLayout.BioElemControls.Where(b => b.Status == ElemControlStatus.Loaded).Any())
                {
                    aroundBioElemPressedPoints.Add(args.Id, new Vector2((float)args.Location.X, (float)args.Location.Y));
                    if (aroundBioElemPressedPoints.Count == 2)
                    {
                        var pointFrom = aroundBioElemPressedPoints.First().Value;
                        var pointTo = aroundBioElemPressedPoints.Last().Value;
                        var distance = Vector2.Distance(pointFrom, pointTo);

                        //Calculate if, for a BioElemControl, the two pressed points are :
                        pressedBioElemControl = gameAbsoluteLayout.BioElemControls
                            .SingleOrDefault(bioElemControl =>
                            {
                                var location = new Vector2((float)(bioElemControl.X + SizesManager.ElemControlSize / 2), (float)(bioElemControl.Y + SizesManager.ElemControlSize / 2));
                                var distanceFromElem = Vector2.Distance(pointFrom, location);
                                var distanceToElem = Vector2.Distance(pointTo, location);

                                //1. At the right distance
                                if (distanceFromElem > SizesManager.ElemControlSize && distanceFromElem < SizesManager.ElemControlSize * 2 && distanceToElem > SizesManager.ElemControlSize && distanceToElem < SizesManager.ElemControlSize * 2)
                                {
                                    //2. through the control (intersection)
                                    var radius = (float)SizesManager.ElemControlSize / 1.5;
                                    var ab = pointTo - pointFrom;
                                    float t = Vector2.Dot(location - pointFrom, ab) / Vector2.Dot(ab, ab);
                                    if (t < 0)
                                        t = 0;
                                    else if (t > 1)
                                        t = 1;
                                    var h = ((ab * t) + pointFrom) - location;
                                    var h2 = Vector2.Dot(h, h);
                                    var intersect = h2 <= (radius * radius);
                                    return intersect;
                                }
                                else
                                    return false;
                            });

                        if (pressedBioElemControl is not null)
                        {
                            var startDistance = Vector2.Distance(aroundBioElemPressedPoints.First().Value, aroundBioElemPressedPoints.Last().Value);
                            pressedBioElemControl.Press(new Point(startDistance, 0));
                        }
                    }
                }

                break;

            /////////////////////////////////////////////////////////////////////////////////
            //Moved
            case TouchActionType.Moved:

                //All ElemControls
                if (pressedControls[args.Id] is not null)
                {
                    pressedControls[args.Id].Move(args.Location);
                }

                //BioElemControls around pressed Points
                else if (aroundBioElemPressedPoints.Any(p => p.Key == args.Id) && pressedBioElemControl is not null && pressedBioElemControl.Status == ElemControlStatus.Pressed)
                {
                    aroundBioElemPressedPoints[args.Id] = new Vector2((float)args.Location.X, (float)args.Location.Y);
                    var actualDistance = Vector2.Distance(aroundBioElemPressedPoints.First().Value, aroundBioElemPressedPoints.Last().Value);
                    pressedBioElemControl.Move(new Point((double)actualDistance, 0));
                }

                break;

            /////////////////////////////////////////////////////////////////////////////////
            //Released
            case TouchActionType.Released:

                ////All ElemControls
                if (pressedControls[args.Id] is not null)
                {
                    pressedControls[args.Id].Release();
                    pressedControls[args.Id] = null;
                }

                //BioElemControls around pressed Points
                else if (aroundBioElemPressedPoints.Any(p => p.Key == args.Id))
                {
                    aroundBioElemPressedPoints.Remove(args.Id);
                    if (pressedBioElemControl is not null)
                    {
                        if (pressedBioElemControl.Status == ElemControlStatus.Pressed)
                            pressedBioElemControl.Release();
                        pressedBioElemControl = null;
                    }
                }
                break;
        }
    }

    #endregion TouchPlatformEffect TouchAction

    #region ElemControl StatusChanged

    /// <summary>
    /// ElemControl Status Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ElemControl_StatusChanged(object? sender, StatusEventArgs e)
    {
        var currentControl = sender as ElemControl;

        switch (e.Status)
        {
            case ElemControlStatus.Loaded:

                //ElectricityElemControl
                if (currentControl is ElectricityElemControl && electricityElemControlPressed > 0)
                {
                    electricityElemControlPressed--;
                }

                //Achievement
                if (currentControl is GroundElemControl && gameAbsoluteLayout.ElemControls.OfType<GroundElemControl>().Where(e => e.IsHorrified).Count() >= 3)
                    CurrentAchievementMessage = AchievementsManager.Complete("Tricks_3ParchedGround");

                break;

            case ElemControlStatus.Pressed:

                //Achievement
                if (gameAbsoluteLayout.ElemControls.Count(e => e.Status == ElemControlStatus.Pressed) >= 4)
                    CurrentAchievementMessage = AchievementsManager.Complete("Tricks_4Pressed");

                //ElectricityElemControl
                if (currentControl is ElectricityElemControl)
                {
                    electricityElemControlPressed++;
                    var allElectricityElemControls = gameAbsoluteLayout.ElectricityElemControls.ToList();
                    if (allElectricityElemControls.All(e => e.IsPressed))
                    {
                        if (allElectricityElemControls.Count == 2)
                        {
                            electricityElemConnectionImage1 = CreateElectricityElemConnection(allElectricityElemControls[0], allElectricityElemControls[1]);
                            allElectricityElemControls.ToList().ForEach(e => e.Explode());
                        }
                        else
                        {
                            electricityElemConnectionImage1 = CreateElectricityElemConnection(allElectricityElemControls[0], allElectricityElemControls[1]);
                            electricityElemConnectionImage2 = CreateElectricityElemConnection(allElectricityElemControls[1], allElectricityElemControls[2]);
                            electricityElemConnectionImage3 = CreateElectricityElemConnection(allElectricityElemControls[2], allElectricityElemControls[0]);
                            allElectricityElemControls.ToList().ForEach(e => e.Explode());
                        }
                    }
                }

                //Update ZIndex of pressed control
                currentControl.ZIndex = gameAbsoluteLayout.ElemControls.Max(e => e.ZIndex) + 1;

                break;

            case ElemControlStatus.Released:

                if (currentControl is ElectricityElemControl)
                {
                    electricityElemControlPressed--;
                }

                //Force release
                if (currentControl is WaterElemControl)
                {
                    var i = pressedControls.ToList().IndexOf(currentControl);
                    pressedControls[i] = null;
                }

                if (currentControl is FireElemControl)
                {
                    //Si on supprime le Fire dispo pour un Water
                    if (fireAndGroundElemControlCollection.OfType<FireElemControl>().Any())
                    {
                        var fires = gameAbsoluteLayout.FireElemControls.Where(e => e.Status < ElemControlStatus.Released).Count();
                        var waters = gameAbsoluteLayout.WaterElemControls.Where(e => e.Status < ElemControlStatus.Released);
                        if (fires < waters.Count(e => !e.IsFrozen) + waters.Count(e => e.IsFrozen) * 2)
                        {
                            Message = null;
                            ErrorMessage = LocalizationManager.Current.Resources["AbusedNoMoreFireForWater"];

                            //Achievements
                            CurrentAchievementMessage = AchievementsManager.Complete("Tricks_NoFireForWater");

                            roundStoppableTimer.Stop();
                            ClearPressedControls();
                            SetNotReleasedControlsToMissed();
                            RoundStatus = RoundStatus.Loose;
                            LevelStatus = LevelStatus.Abused;
                        }
                    }
                }

                //Si c'est le dernier qui start son status released
                if (gameAbsoluteLayout.ElemControls
                    .All(e => e.Status == ElemControlStatus.Released || e.Status == ElemControlStatus.Exploded))
                {
                    roundStoppableTimer.Stop();
                    RoundStatus = RoundStatus.Win;
                }

                break;

            case ElemControlStatus.Exploded:

                //Remove Electricity connections
                if (currentControl is ElectricityElemControl)
                {
                    gameAbsoluteLayout.Children.Remove(electricityElemConnectionImage1);
                    gameAbsoluteLayout.Children.Remove(electricityElemConnectionImage2);
                    gameAbsoluteLayout.Children.Remove(electricityElemConnectionImage3);
                    //Achievements
                    //Kill Type
                    if (electricityElemConnectionImage2 != null)
                        CurrentAchievementMessage = AchievementsManager.CompleteElement(ElemType.Electricity, 3);
                    else
                        CurrentAchievementMessage = AchievementsManager.CompleteElement(ElemType.Electricity, 2);
                }

                //Remove currentControl
                currentControl.StopAnimation();
                currentControl.StatusChanged -= ElemControl_StatusChanged;
                currentControl.AchievementEnded -= ElemControl_AchievementEnded;
                gameAbsoluteLayout.Children.Remove(currentControl);
                explosionList.Add(currentControl);

                //Achievements
                //Kill Type
                if (currentControl is FireElemControl)
                    CurrentAchievementMessage = AchievementsManager.CompleteElement(ElemType.Fire, explosionList.Count(e => e is FireElemControl));
                else if (currentControl is GroundElemControl)
                    CurrentAchievementMessage = AchievementsManager.CompleteElement(ElemType.Ground, explosionList.Count(e => e is GroundElemControl));
                else if (currentControl is WindElemControl)
                    CurrentAchievementMessage = AchievementsManager.CompleteElement(ElemType.Wind, explosionList.Count(e => e is WindElemControl));
                else if (currentControl is WaterElemControl)
                {
                    if (((WaterElemControl)currentControl).WasFrozen)
                        CurrentAchievementMessage = AchievementsManager.CompleteElement(ElemType.Ice, explosionList.OfType<WaterElemControl>().Count(e => e.WasFrozen));
                    else
                        CurrentAchievementMessage = AchievementsManager.CompleteElement(ElemType.Water, explosionList.Count(e => e is WaterElemControl));
                }
                else if (currentControl is BioElemControl)
                    CurrentAchievementMessage = AchievementsManager.CompleteElement(ElemType.Bio, explosionList.Count(e => e is BioElemControl));

                //Distinct Kill
                var distinctType = explosionList.Select(e => e.GetType()).Distinct();
                if (distinctType.Count() == 2)
                    CurrentAchievementMessage = AchievementsManager.Complete("Elems_TwoDifferent");
                else if (distinctType.Count() == 3)
                    CurrentAchievementMessage = AchievementsManager.Complete("Elems_ThreeDifferent");
                else if (distinctType.Count() == 4)
                    CurrentAchievementMessage = AchievementsManager.Complete("Elems_FourDifferent");

                //Number Kill >= 5
                if (explosionList.Count >= 5)
                    CurrentAchievementMessage = AchievementsManager.Complete("Tricks_5Kills");

                //if all exploded : load new round or win game
                if (!gameAbsoluteLayout.ElemControls.Any())
                {
                    roundStoppableTimer.Stop();
                    RoundSelected--;
                    currentRound++;
                    if (CurrentElemsLevel.Elems.Any(e => e.Round == currentRound))
                        LoadElemsRound();
                    else
                        LevelStatus = LevelStatus.Win;
                }
                break;

            case ElemControlStatus.Abused:
                Message = null;
                ErrorMessage = LocalizationManager.Current.Resources["AbusedDoublePressedFire"];

                roundStoppableTimer.Stop();
                ClearPressedControls();
                SetNotReleasedControlsToMissed();
                RoundStatus = RoundStatus.Loose;
                LevelStatus = LevelStatus.Abused;

                //Achievement Errors Fire
                CurrentAchievementMessage = AchievementsManager.Complete("Tricks_FireCoal");

                break;
        }
    }

    #endregion ElemControl StatusChanged

    #region ElemControl AchievementEnded

    private void ElemControl_AchievementEnded(object sender, AchievementEventArgs args)
    {
        CurrentAchievementMessage = AchievementsManager.Complete(args.AchievementName);
    }

    #endregion ElemControl AchievementEnded

    #region CreateElectricityElemConnection

    /// <summary>
    /// Create Electricity Elem Connection
    /// </summary>
    /// <param name="electricity1"></param>
    /// <param name="electricity2"></param>
    /// <returns></returns>
    private Image CreateElectricityElemConnection(ElectricityElemControl electricity1, ElectricityElemControl electricity2)
    {
        double xDiff = electricity2.X - electricity1.X;
        double yDiff = electricity2.Y - electricity1.Y;
        var distance = Math.Sqrt(Math.Pow(electricity2.X - electricity1.X, 2) + Math.Pow(electricity2.Y - electricity1.Y, 2));
        var degrees = Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;

        var image = new Image
        {
            AnchorX = 0d,
            AnchorY = 0.5d,
            Aspect = Aspect.AspectFill,
            Source = "electricity_connection.png",
            Opacity = 1,
            HeightRequest = SizesManager.ElemControlSize,
            WidthRequest = distance,
            Rotation = degrees
        };
        gameAbsoluteLayout.Children.Add(image);

        AbsoluteLayout.SetLayoutBounds(image, new Rect(electricity1.X + SizesManager.ElemControlSize / 2, electricity1.Y, image.WidthRequest, image.HeightRequest));
        image.FadeTo(0, 500);

        return image;
    }

    #endregion CreateElectricityElemConnection
}