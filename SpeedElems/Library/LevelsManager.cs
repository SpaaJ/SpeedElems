//using Newtonsoft.Json;
using System.Text.Json;
using SpeedElems.Models;

namespace SpeedElems.Library;

/// <summary>
/// Levels Manager
/// </summary>
public static class LevelsManager
{
    public static async Task<ElemsLevel> GetLevelAsync(int levelNumber)
    {
        var stream = await FileSystem.OpenAppPackageFileAsync($"Maps\\{levelNumber.ToString("000")}.json");
        var level = JsonSerializer.Deserialize<ElemsLevel>(stream, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        level.ID = levelNumber;
        return level;
    }
}