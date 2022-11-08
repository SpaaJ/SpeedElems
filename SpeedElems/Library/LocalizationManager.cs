using System.ComponentModel;
using System.Globalization;
using SpeedElems.Resources.Localization;

namespace SpeedElems.Library;

/// <summary>
/// Localization translated Type
/// </summary>
public enum TranslateType
{
    Resource,
    Image
}

/// <summary>
/// Localization Manager for binding
/// </summary>
public class LocalizationManager
{
    private LocalizationManager()
    {
        AppResources.Culture = CultureInfo.CurrentCulture;
    }

    public static LocalizationManager Current { get; } = new();

    public LocalizationResourceManager Resources = new();

    public LocalizationImageManager Images = new();

    public void SetCulture(string name)
    {
        AppResources.Culture = new CultureInfo(name);
        Resources.InvokePropertyChanged();
        Images.InvokePropertyChanged();
    }
}

/// <summary>
/// Localization Resource Manager : search binding in Localization\AppResources
/// </summary>
public class LocalizationResourceManager : INotifyPropertyChanged
{
    public string this[string resourceKey] => (string)AppResources.ResourceManager.GetObject(resourceKey, AppResources.Culture) ?? string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public void InvokePropertyChanged() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
}

/// <summary>
/// Localization Image Manager : search binding in Images
/// Image end with '_fr' or '_en' - exemple: parameters_fr.png and parameters_en.png
/// </summary>
public class LocalizationImageManager : INotifyPropertyChanged
{
    public string this[string imageKey] => $"{imageKey.ToLower()}_{AppResources.Culture.TwoLetterISOLanguageName}.png" ?? string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public void InvokePropertyChanged() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
}

/// <summary>
/// Markup extension for binding :
/// Resource : {library:Translate Difficulty}
/// Image : {library:Translate Parameters, Type=Image}
/// </summary>
[ContentProperty(nameof(Name))]
public class TranslateExtension : IMarkupExtension<BindingBase>
{
    public string? Name { get; set; }
    public TranslateType Type { get; set; } = TranslateType.Resource;

    public BindingBase ProvideValue(IServiceProvider serviceProvider)
    {
        return new Binding
        {
            Mode = BindingMode.OneWay,
            Path = $"[{Name}]",
            Source = Type == TranslateType.Resource ? LocalizationManager.Current.Resources : LocalizationManager.Current.Images
        };
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
    {
        return ProvideValue(serviceProvider);
    }
}