using Microsoft.Extensions.Logging;
using SpeedElems.Library;
using SpeedElems.ViewModels;

namespace SpeedElems.Pages;

/// <summary>
/// Levels Page
/// </summary>
public partial class LevelsPage : BasePage<LevelsPageViewModel>
{
    public LevelsPage(LevelsPageViewModel viewModel, ILogger<GamePage> logger) : base(viewModel, logger)
    {
        InitializeComponent();
    }
}