using CommunityToolkit.Maui.Views;
using Microsoft.Extensions.Logging;
using SpeedElems.Library;
using SpeedElems.Models;
using SpeedElems.ViewModels;

namespace SpeedElems.Pages;

/// <summary>
/// Home Page
/// </summary>
public partial class HomePage : BasePage<HomePageViewModel>
{
    private double titleCharHeighth, titleCharWidth;
    private float verticalBackgroundRatio, horizontalBackgroundRatio;
    private StoppableTimer fireStoppableTimer;
    private StoppableTimer waterStoppableTimer;
    private StoppableTimer electricityStoppableTimer;

    public HomePage(HomePageViewModel viewModel, ILogger<GamePage> logger) : base(viewModel, logger)
    {
        //Initialize game sizes after loading HomePage
        SizesManager.Initialize();

        InitializeComponent();

        //Initialize de component animés
        InitializeComponentAsync();

        Loaded += OnPageLoaded;
    }

    public async void InitializeComponentAsync()
    {
        var random = new Random();
        titleCharWidth = SizesManager.ScreenWidth / 11;
        titleCharHeighth = SizesManager.ScreenHeight / 5;
        verticalBackgroundRatio = (float)SizesManager.ScreenHeight / 360;
        horizontalBackgroundRatio = (float)SizesManager.ScreenWidth / 640;
        var titleCharMargin = new Thickness(-20 * verticalBackgroundRatio);

        //[ ][ ][ ][E][L][E][M][S][ ][ ][ ]
        //Increased the size of each of the letters by 1.5 so that they overlap(relative to their positions)-> [ ][ ][[]E[]L[]E[]M[]S[]][ ][ ]
        var displayTitleCharWidth = titleCharWidth * 1.5;
        //Shift to the left of 0.25 to recenter them + placement
        AbsoluteLayout.SetLayoutBounds(titleE1char, new Rect(2.75 * titleCharWidth, titleCharWidth * -1.5, displayTitleCharWidth, displayTitleCharWidth));
        AbsoluteLayout.SetLayoutBounds(titleLchar, new Rect(3.75 * titleCharWidth, titleCharWidth * -1.5, displayTitleCharWidth, displayTitleCharWidth));
        AbsoluteLayout.SetLayoutBounds(titleE2char, new Rect(4.75 * titleCharWidth, titleCharWidth * -1.5, displayTitleCharWidth, displayTitleCharWidth));
        AbsoluteLayout.SetLayoutBounds(titleMchar, new Rect(5.75 * titleCharWidth, titleCharWidth * -1.5, displayTitleCharWidth, displayTitleCharWidth));
        AbsoluteLayout.SetLayoutBounds(titleSchar, new Rect(6.75 * titleCharWidth, titleCharWidth * -1.5, displayTitleCharWidth, displayTitleCharWidth));

        //Shift to the left of Speed with Wind
        AbsoluteLayout.SetLayoutBounds(titleSpeedStackLayout, new Rect(-titleCharWidth * 5, titleCharHeighth * 0.75, titleCharWidth * 5, titleCharHeighth));
        windElemControl.WidthRequest = SizesManager.ElemControlSize;
        windElemControl.Status = ElemControlStatus.Loaded;

        //ElemControls
        AbsoluteLayout.SetLayoutBounds(bioElemControl,
            new Rect(
                horizontalBackgroundRatio * 30,
                verticalBackgroundRatio * 310,
                SizesManager.ElemControlSize,
                SizesManager.ElemControlSize
            )
        );
        AbsoluteLayout.SetLayoutBounds(groundElemControl,
            new Rect(
                6.75 * titleCharWidth + displayTitleCharWidth / 2 - SizesManager.ElemControlSize / 2,
                -SizesManager.ElemControlSize,
                SizesManager.ElemControlSize,
                SizesManager.ElemControlSize
            )
        );
        AbsoluteLayout.SetLayoutBounds(electricityElemControl1,
            new Rect(
                SizesManager.ScreenWidth / 2 - SizesManager.ElemControlSize * 3,
                SizesManager.ScreenHeight / 2 + SizesManager.ElemControlSize * 1,
                SizesManager.ElemControlSize,
                SizesManager.ElemControlSize
            )
        );
        AbsoluteLayout.SetLayoutBounds(electricityElemControl2,
            new Rect(
                SizesManager.ScreenWidth / 2 + SizesManager.ElemControlSize * 2,
                SizesManager.ScreenHeight / 2 + SizesManager.ElemControlSize * 2,
                SizesManager.ElemControlSize,
                SizesManager.ElemControlSize
            )
        );
        AbsoluteLayout.SetLayoutBounds(electricityLink, new Rect(
                SizesManager.ScreenWidth / 2 - SizesManager.ElemControlSize * 2.7,
                SizesManager.ScreenHeight / 2 + SizesManager.ElemControlSize * 1.5,
                SizesManager.ElemControlSize * 5.3,
                SizesManager.ElemControlSize * 6
            )
        );
        electricityLink.Rotation = 15d;

        //Fire Animation
        fireElemControl.IsInHomePage = true;
        fireStoppableTimer = new StoppableTimer(
            async () =>
            {
                var column = random.Next(0, 2) == 0 ? random.Next(0, 3) : random.Next(11, 14);
                var row = random.Next(0, 6);
                fireElemControl.IsVisible = true;
                if (fireElemControl.Status == ElemControlStatus.Exploded)
                    fireElemControl.Resurrect();
                fireElemControl.Status = ElemControlStatus.Created;
                fireElemControl.Status = ElemControlStatus.Loaded;
                AbsoluteLayout.SetLayoutBounds(fireElemControl, new Rect(column * SizesManager.ElemControlSize + SizesManager.ElemControlDecalX, row * SizesManager.ElemControlSize + SizesManager.ElemControlDecalY, SizesManager.ElemControlSize, SizesManager.ElemControlSize));
                await Task.Delay(random.Next(3000, 8000));

                if (Shell.Current.CurrentPage is HomePage)
                {
                    fireElemControl.IsPressed = true;
                    fireElemControl.Release();
                }
            }
        );

        //Water Animation
        waterStoppableTimer = new StoppableTimer(
            async () =>
            {
                await Task.Delay(random.Next(2000, 5000));
                waterElemControl.IsVisible = true;
                var column = random.Next(0, 2) == 0 ? random.Next(0, 3) : random.Next(11, 14);
                AbsoluteLayout.SetLayoutBounds(waterElemControl, new Rect(column * SizesManager.ElemControlSize + SizesManager.ElemControlDecalX, -SizesManager.ElemControlSize, SizesManager.ElemControlSize, SizesManager.ElemControlSize));
                waterElemControl.TranslationY = 0;
                await waterElemControl.TranslateTo(0, SizesManager.ScreenHeight + SizesManager.ElemControlSize, 1000);
            }
        );

        //Electricity Animation
        electricityStoppableTimer = new StoppableTimer(
            async () =>
            {
                await electricityLink.FadeTo(1, 100);
                await Task.Delay(100);
                if (random.Next(0, 2) == 1)
                {
                    await electricityLink.FadeTo(0, 100);
                    await Task.Delay(100);
                    await electricityLink.FadeTo(1, 100);
                    await Task.Delay(100);
                }
                await electricityLink.FadeTo(0, 700);
            }
        );

        //Title chars
        await Task.Delay(1000);
        await titleE1char.TranslateTo(0, titleCharHeighth * 3, 250);
        await titleLchar.TranslateTo(0, titleCharHeighth * 3, 250);
        await titleE2char.TranslateTo(0, titleCharHeighth * 3, 250);
        await titleMchar.TranslateTo(0, titleCharHeighth * 3, 250);
        await Task.WhenAll(
            titleSchar.TranslateTo(0, titleCharHeighth * 5, 400),
            groundElemControl.TranslateTo(0, titleCharHeighth * 5, 400)
        );
        await Task.Delay(1000);
        await Task.WhenAll(
            titleSchar.TranslateTo(0, titleCharHeighth * 4.9, 100),
            groundElemControl.TranslateTo(0, titleCharHeighth * 4.9, 100)
        );
        await Task.WhenAll(
            titleSchar.TranslateTo(0, titleCharHeighth * 5, 100),
            groundElemControl.TranslateTo(0, titleCharHeighth * 5, 100)
        );
        await Task.Delay(1000);
        await Task.WhenAll(
            titleSchar.TranslateTo(0, titleCharHeighth * 3, 150),
            groundElemControl.TranslateTo(0, verticalBackgroundRatio * 310 + SizesManager.ElemControlSize, 150)
        );

        //----> TitleSpeedStackLayout
        await titleSpeedStackLayout.TranslateTo(titleCharWidth * 8.4, 0, 250);
        await titleSpeedStackLayout.TranslateTo(titleCharWidth * 8.2, 0, 50);

        //testButton
        AbsoluteLayout.SetLayoutBounds(testButton,
            new Rect(
                horizontalBackgroundRatio * 30,
                verticalBackgroundRatio * 310,
                SizesManager.ElemControlSize,
                SizesManager.ElemControlSize
            )
        );
    }

    private void OnPageLoaded(object sender, EventArgs e)
    {
        AudioPlayerManager.BackgroundMediaElement = backgroundMediaElement;
        if (Preferences.Get("Parameters.IsMusicActive", true))
            AudioPlayerManager.BackgroundMediaElement.Play();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        fireStoppableTimer.Stop();
        waterStoppableTimer.Stop();
        electricityStoppableTimer.Stop();
        electricityElemControl1.StopSecondaryAnimation();
        electricityElemControl2.StopSecondaryAnimation();

        fireElemControl.StopAnimation();
        waterElemControl.StopAnimation();
        groundElemControl.StopAnimation();
        windElemControl.StopAnimation();
        electricityElemControl1.StopAnimation();
        electricityElemControl2.StopAnimation();
        bioElemControl.StopAnimation();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        fireStoppableTimer.StartLoop(10000, true);
        waterStoppableTimer.StartLoop(7000);
        electricityStoppableTimer.StartLoop(4000);
        electricityElemControl1.StartSecondaryAnimation();
        electricityElemControl2.StartSecondaryAnimation();

        fireElemControl.StartAnimation();
        waterElemControl.StartAnimation();
        groundElemControl.StartAnimation();
        windElemControl.StartAnimation();
        electricityElemControl1.StartAnimation();
        electricityElemControl2.StartAnimation();
        bioElemControl.StartAnimation();
    }

    private async void WindElemControlButton_Clicked(object sender, EventArgs e)
    {
        await titleSpeedStackLayout.TranslateTo(2000, 0, 250);
        await Task.Delay(1000);
        titleSpeedStackLayout.TranslationX = -1000;
        await titleSpeedStackLayout.TranslateTo(titleCharWidth * 8.4, 0, 250);
        await titleSpeedStackLayout.TranslateTo(titleCharWidth * 8.2, 0, 50);
    }

    private async void TitleSchar_Clicked(object sender, EventArgs e)
    {
        await Task.WhenAll(
          titleSchar.TranslateTo(0, titleCharHeighth * 5, 200),
          groundElemControl.TranslateTo(0, titleCharHeighth * 5, 200)
        );
        await Task.Delay(1000);
        await Task.WhenAll(
            titleSchar.TranslateTo(0, titleCharHeighth * 4.9, 100),
            groundElemControl.TranslateTo(0, titleCharHeighth * 4.9, 100)
        );
        await Task.WhenAll(
            titleSchar.TranslateTo(0, titleCharHeighth * 5, 100),
            groundElemControl.TranslateTo(0, titleCharHeighth * 5, 100)
        );
        await Task.Delay(1000);
        await Task.WhenAll(
            titleSchar.TranslateTo(0, titleCharHeighth * 3, 150),
            groundElemControl.TranslateTo(0, verticalBackgroundRatio * 310 + SizesManager.ElemControlSize, 150)
        );
    }
}