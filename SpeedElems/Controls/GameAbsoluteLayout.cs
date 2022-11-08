using Microsoft.Maui.Layouts;
using SpeedElems.Library;

namespace SpeedElems.Controls;

/// <summary>
/// Game AbsoluteLayout
/// </summary>
public class GameAbsoluteLayout : AbsoluteLayout
{
    #region Privates

    private AbsoluteLayoutManager absoluteLayoutManager;

    #endregion Privates

    #region Properties

    public IEnumerable<ElemControl> ElemControls => Children.OfType<ElemControl>();

    public IEnumerable<FireElemControl> FireElemControls => Children.OfType<FireElemControl>();

    public IEnumerable<GroundElemControl> GroundElemControls => Children.OfType<GroundElemControl>();

    public IEnumerable<WindElemControl> WindElemControls => Children.OfType<WindElemControl>();

    public IEnumerable<WaterElemControl> WaterElemControls => Children.OfType<WaterElemControl>();

    public IEnumerable<BioElemControl> BioElemControls => Children.OfType<BioElemControl>();

    public IEnumerable<ElectricityElemControl> ElectricityElemControls => Children.OfType<ElectricityElemControl>();

    #endregion Properties

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    public GameAbsoluteLayout()
    {
        absoluteLayoutManager = new AbsoluteLayoutManager(this);
    }

    #endregion Constructor

    #region Methods

    /// <summary>
    /// Arrange Children
    /// Update all the children in the AbsoluteLayout
    /// </summary>
    public void ArrangeChildren()
    {
        absoluteLayoutManager.ArrangeChildren(new Rect(0, 0, SizesManager.ScreenWidth, SizesManager.ScreenHeight));
    }

    #endregion Methods
}