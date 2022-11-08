using SpeedElems.Models;

namespace SpeedElems.Library;

/// <summary>
/// Status EventArgs
/// </summary>
public class StatusEventArgs : EventArgs
{
    public ElemControlStatus Status { get; set; }

    public StatusEventArgs(ElemControlStatus status)
    {
        Status = status;
    }
}