using SpeedElems.Models;

namespace SpeedElems.Library;

public static class AchievementsManager
{
    public static string Complete(string name)
    {
        if (!Preferences.Get("Achievements." + name, false))
        {
            Preferences.Set("Achievements." + name, true);
            return LocalizationManager.Current.Resources[name];
        }
        else
            return null;
    }

    public static string CompleteElement(ElemType elemType, int count)
    {
        if (count > Preferences.Get("Achievements.Elems_" + elemType.ToString(), 0))
        {
            Preferences.Set("Achievements.Elems_" + elemType.ToString(), count);
            var elementCount = LocalizationManager.Current.Resources["ElementCount" + count];
            var elementType = LocalizationManager.Current.Resources[elemType.ToString()];
            if (elementType.StartsWith("E") || elementType.StartsWith("É")) // Eau || Éléctrique
                elementCount = elementCount.Replace("de ", "d'");
            return elementCount + elementType;
        }
        else
            return null;
    }
}

public class AchievementEventArgs : EventArgs
{
    public string AchievementName { get; set; }

    public AchievementEventArgs(string achievementName)
    {
        AchievementName = achievementName;
    }
}

public class AchievementManager
{
    public delegate void AchievementEventHandler(object sender, AchievementEventArgs args);
}