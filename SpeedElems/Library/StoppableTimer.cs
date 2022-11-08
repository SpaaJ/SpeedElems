namespace SpeedElems.Library;

/// <summary>
/// Stoppable Timer
/// </summary>
public class StoppableTimer
{
    private TimeSpan timespan;
    private readonly Action callback;
    private CancellationTokenSource cancellation;

    public StoppableTimer(Action callback)
    {
        this.callback = callback;
        this.cancellation = new CancellationTokenSource();
    }

    public void StartLoop(int durationInMilliseconds, bool directly = false)
    {
        timespan = TimeSpan.FromMilliseconds(durationInMilliseconds);
        if (directly)
            callback.Invoke();
        Start(true);
    }

    public void StartOnce(int durationInMilliseconds)
    {
        timespan = TimeSpan.FromMilliseconds(durationInMilliseconds);
        Start(false);
    }

    public void Stop()
    {
        Interlocked.Exchange(ref this.cancellation, new CancellationTokenSource()).Cancel();
    }

    private void Start(bool continuous)
    {
#pragma warning disable CS0612, CS0618
        CancellationTokenSource cts = this.cancellation;
        Device.StartTimer(timespan,
            () =>
            {
                if (cts.IsCancellationRequested)
                    return false;

                callback.Invoke();
                return continuous;
            });
#pragma warning restore CS0612, CS0618
    }
}