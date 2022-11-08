using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace SpeedElems;

/// <summary>
/// Main Activity
/// </summary>
[Activity(
    Label = "Speed Elems",
    Theme = "@style/SpeedElems.SplashTheme",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density,
    ScreenOrientation = ScreenOrientation.SensorLandscape
)
]
public class MainActivity : MauiAppCompatActivity
{
    public MainActivity()
    {
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        Platform.Init(this, savedInstanceState);

        SetWindowLayout();
    }

    private void SetWindowLayout()
    {
        if (Window != null)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
            {
#pragma warning disable CA1416

                //Remove right/left header for Camera hole area
                Window.AddFlags(WindowManagerFlags.LayoutNoLimits);
                Window.Attributes.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.ShortEdges;

#pragma warning restore CA1416
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
            {
#pragma warning disable CA1416

                //Remove Header (Datetime + Infos)
                Window.SetDecorFitsSystemWindows(false);
                Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

                IWindowInsetsController wicController = Window.InsetsController;
                if (wicController != null)
                {
                    wicController.Hide(WindowInsets.Type.Ime());
                    wicController.Hide(WindowInsets.Type.NavigationBars());
                }

#pragma warning restore CA1416
            }
            else
            {
#pragma warning disable CS0618

                //Remove Header (Datetime + Infos)
                Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

                Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(SystemUiFlags.Fullscreen |
                                                                            SystemUiFlags.HideNavigation |
                                                                            SystemUiFlags.Immersive |
                                                                            SystemUiFlags.ImmersiveSticky |
                                                                            SystemUiFlags.LayoutHideNavigation |
                                                                            SystemUiFlags.LayoutStable |
                                                                            SystemUiFlags.LowProfile);
#pragma warning restore CS0618
            }
        }
    }
}