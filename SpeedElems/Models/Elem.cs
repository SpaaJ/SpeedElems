using SpeedElems.Library;

namespace SpeedElems.Models;

/// <summary>
/// Entité de stockage des données d'un Elem
/// </summary>
public class Elem
{
    public bool Isfrozen { get; set; }

    public int Round { get; set; }

    public ElemType Type { get; set; }

    public double Column { get; set; }

    public double Row { get; set; }

    public Point Position
    {
        get { return new Point(Column * SizesManager.ElemControlSize, Row * SizesManager.ElemControlSize); }
        set
        {
            Column = value.X / SizesManager.ElemControlSize;
            Row = value.Y / SizesManager.ElemControlSize;
        }
    }
}