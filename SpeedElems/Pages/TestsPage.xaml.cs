using Microsoft.Extensions.Logging;
using SpeedElems.Library;
using SpeedElems.ViewModels;

namespace SpeedElems.Pages;

/// <summary>
/// Tests Page
/// </summary>
public partial class TestsPage : BasePage<TestsPageViewModel>
{
    public TestsPage(TestsPageViewModel viewModel, ILogger<GamePage> logger) : base(viewModel, logger)
    {
        InitializeComponent();
    }
}