using HarmonyLib;
using StardewModdingAPI;
using StardewPanHat.Patches;

namespace StardewPanHat;

public class ModEntry : Mod
{
    public static IMonitor MonitorSingleton { get; private set; } = null!;
    private static string _keyQualifier = null!;

    /// <summary> Adds the unique mod ID to the beginning of a key, making it unique. </summary>
    public static string QualifyKey(string key) => _keyQualifier + key; 
    
    public override void Entry(IModHelper helper)
    {
        MonitorSingleton = Monitor;
        _keyQualifier = ModManifest.UniqueID + '/';
        Harmony harmony = new(ModManifest.UniqueID);
        
        PanPatches.PatchAll(harmony);
        ToolPatches.PatchAll(harmony);
        UtilityPatches.PatchAll(harmony);
        InventoryMenuPatches.PatchAll(harmony);
        InventoryPagePatches.PatchAll(harmony);
    }
}