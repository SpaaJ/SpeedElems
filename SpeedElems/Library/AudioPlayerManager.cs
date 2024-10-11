using CommunityToolkit.Maui.Views;

namespace SpeedElems.Library;

/// <summary>
/// AudioPlayer Manager
/// </summary>
public class AudioPlayerManager
{
    public static MediaElement BackgroundMediaElement;

    public static Dictionary<Type, MediaElement> TypeMediaElements = new Dictionary<Type, MediaElement>();

    public static void CreateBackgroundMediaElement()
    {
        BackgroundMediaElement = new()
        {
            Source = MediaSource.FromResource("music.wav"),
            ShouldAutoPlay = false,
            ShouldLoopPlayback = true,
            ShouldShowPlaybackControls = false,
            IsVisible = false
        };
    }

    public static void CreateMediaElement(Type type, string filename)
    {
        MediaElement mediaElement = new()
        {
            Source = MediaSource.FromResource(filename),
            ShouldAutoPlay = false,
            ShouldLoopPlayback = false,
            ShouldShowPlaybackControls = false,
            IsVisible = false
        };

        TypeMediaElements.Add(type, mediaElement);
    }
}