namespace SpeedElems.Models;

/// <summary>
/// Elems Level (for JSON file)
/// </summary>
public class ElemsLevel
{
    public int ID { get; set; }

    public string[] Messages { get; set; }

    public Elem[] Elems { get; set; }
}