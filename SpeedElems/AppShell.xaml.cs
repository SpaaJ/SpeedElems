using SpeedElems.Pages;

namespace SpeedElems;

/// <summary>
/// App Shell
/// </summary>
public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("//HomePage", typeof(HomePage));
        Routing.RegisterRoute("//HomePage/ParametersPage", typeof(ParametersPage));
        Routing.RegisterRoute("//HomePage/AchievementsPage", typeof(AchievementsPage));
        Routing.RegisterRoute("//HomePage/LevelsPage", typeof(LevelsPage));
        Routing.RegisterRoute("//HomePage/LevelsPage/AchievementsPage", typeof(AchievementsPage));
        Routing.RegisterRoute("//HomePage/LevelsPage/GamePage", typeof(GamePage));
        Routing.RegisterRoute("//HomePage/TestsPage", typeof(TestsPage));

        Navigated += (o, e) => Debug.WriteLine($"Navigated: {Current.CurrentState.Location}");
    }
}