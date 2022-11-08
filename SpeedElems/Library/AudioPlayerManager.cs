using System.Reflection;

namespace SpeedElems.Library;

/// <summary>
/// AudioPlayer Manager
/// </summary>
public class AudioPlayerManager
{
    private static readonly Lazy<IAudioPlayer> backgroundAudioPlayer = new Lazy<IAudioPlayer>(() => Create(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

    public static IAudioPlayer Background => backgroundAudioPlayer.Value;

    public static Dictionary<Type, IAudioPlayer> TypeDictionnary { get; set; } = new Dictionary<Type, IAudioPlayer>(5);

    public static IAudioPlayer Create()
    {
#if __ANDROID__
        return new AndroidAudioPlayer();
#else
        //todo: Windows/iOS AudioPlayer
        return null;
#endif
    }

    public static void Create(Type type, string fileName, double volume = 0.5)
    {
#if __ANDROID__
        var audioStream = GetAudioStream(fileName);
        var audioPlayer = new AndroidAudioPlayer();
        audioPlayer.Load(audioStream);
        audioPlayer.Volume = volume;
        TypeDictionnary.Add(type, audioPlayer);
#endif
    }

    public static void CreateBackground(string fileName, double volume = 0.3)
    {
#if __ANDROID__
        var audioStream = GetAudioStream(fileName);
        Background.Load(audioStream);
        Background.Volume = volume;
        Background.Loop = true;
#endif
    }

    private static Stream GetAudioStream(string filename)
    {
        var assembly = typeof(App).GetTypeInfo().Assembly;
        var assemblyName = assembly.GetName().Name;
        var stream = assembly.GetManifestResourceStream($"{assemblyName}.Resources.Audio.{filename}");
        return stream;
    }
}

/// <summary>
/// Interface for IAudioPlayer
/// </summary>
public interface IAudioPlayer : IDisposable
{
    ///<Summary>
    /// Raised when audio playback completes successfully
    ///</Summary>
    event EventHandler PlaybackEnded;

    ///<Summary>
    /// Length of audio in seconds
    ///</Summary>
    double Duration { get; }

    ///<Summary>
    /// Current position of audio playback in seconds
    ///</Summary>
    double CurrentPosition { get; }

    ///<Summary>
    /// Playback volume 0 to 1 where 0 is no-sound and 1 is full volume
    ///</Summary>
    double Volume { get; set; }

    ///<Summary>
    /// Balance left/right: -1 is 100% left : 0% right, 1 is 100% right : 0% left, 0 is equal volume left/right
    ///</Summary>
    double Balance { get; set; }

    ///<Summary>
    /// Indicates if the currently loaded audio file is playing
    ///</Summary>
    bool IsPlaying { get; }

    ///<Summary>
    /// Continously repeats the currently playing sound
    ///</Summary>
    bool Loop { get; set; }

    ///<Summary>
    /// Indicates if the position of the loaded audio file can be updated
    ///</Summary>
    bool CanSeek { get; }

    ///<Summary>
    /// Load wav or mp3 audio file as a stream
    ///</Summary>
    bool Load(Stream audioStream);

    ///<Summary>
    /// Load wav or mp3 audio file from local path
    ///</Summary>
    bool Load(string fileName);

    ///<Summary>
    /// Begin playback or resume if paused
    ///</Summary>
    void Play();

    ///<Summary>
    /// Pause playback if playing (does not resume)
    ///</Summary>
    void Pause();

    ///<Summary>
    /// Stop playack and set the current position to the beginning
    ///</Summary>
    void Stop();

    ///<Summary>
    /// Set the current playback position (in seconds)
    ///</Summary>
    void Seek(double position);
}