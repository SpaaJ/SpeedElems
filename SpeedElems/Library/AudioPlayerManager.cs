using Plugin.Maui.Audio;

namespace SpeedElems.Library;

/// <summary>
/// AudioPlayer Manager
/// </summary>
public class AudioPlayerManager
{
    public static IAudioManager AudioManager;

    public static IAudioPlayer BackgroundMediaElement;

    public static Dictionary<Type, IAudioPlayer> TypeMediaElements = new();

    public static async Task CreateBackgroundMediaElement()
    {
        var file = await FileSystem.OpenAppPackageFileAsync("music.wav");
        BackgroundMediaElement = AudioManager.CreatePlayer(file);
        BackgroundMediaElement.Loop = true;
    }

    public static async Task CreateMediaElement(Type type, string filename)
    {
        var file = await FileSystem.OpenAppPackageFileAsync(filename);
        var mediaElement = AudioManager.CreatePlayer(file);

        TypeMediaElements.Add(type, mediaElement);
    }
}