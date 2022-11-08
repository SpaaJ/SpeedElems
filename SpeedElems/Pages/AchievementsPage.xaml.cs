using Microsoft.Extensions.Logging;
using SpeedElems.Library;
using SpeedElems.ViewModels;

namespace SpeedElems.Pages;

/// <summary>
/// Achievements Page
/// </summary>
public partial class AchievementsPage : BasePage<AchievementsPageViewModel>
{
    public AchievementsPage(AchievementsPageViewModel viewModel, ILogger<GamePage> logger) : base(viewModel, logger)
    {
        InitializeComponent();
    }
}