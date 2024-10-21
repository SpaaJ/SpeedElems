using System.Globalization;
using CommunityToolkit.Maui.Converters;

namespace SpeedElems.Library.Converters;

public class ZeroToBoolConverter : BaseConverterOneWay<int, bool>
{
    public override bool DefaultConvertReturnValue { get; set; }

    public override bool ConvertFrom(int value, CultureInfo? culture = null) => value is 0;
}

public class GreaterOrEqualThanParameterToBoolConverter : BaseConverterOneWay<int, bool, string>
{
    public override bool DefaultConvertReturnValue { get; set; }

    public override bool ConvertFrom(int value, string parameter, CultureInfo? culture = null) => value >= int.Parse(parameter);
}