using Microsoft.Extensions.Logging;
using SpeedElems.Library;
using SpeedElems.ViewModels;

namespace SpeedElems.Pages;

/// <summary>
/// Parameters Page
/// </summary>
public partial class ParametersPage : BasePage<ParametersPageViewModel>
{
    public ParametersPage(ParametersPageViewModel viewModel, ILogger<GamePage> logger) : base(viewModel, logger)
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
}