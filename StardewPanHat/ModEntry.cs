using HarmonyLib;
using StardewModdingAPI;
using StardewPanHat.Patches;

namespace StardewPanHat;

internal class ModEntry : Mod
{
    public const string ModAuthorName = "Malkavian242";
    public static IMonitor MonitorSingleton { get; private set; } = null!;
    private static string _keyQualifier = null!;

    /// <summary> Adds the unique mod ID to the beginning of a key. </summary>
    public static string QualifyKey(string key) => _keyQualifier + key; 
    
    public override void Entry(IModHelper helper)
    {
        MonitorSingleton = Monitor;
        _keyQualifier = ModManifest.UniqueID + '/';
        HandleHarmonyPatches();
        RegisterSerializableTypes();
    }

    private void HandleHarmonyPatches()
    {
        Harmony harmony = new(ModManifest.UniqueID);
        PanPatches.PatchAll(harmony);
        ToolPatches.PatchAll(harmony);
        UtilityPatches.PatchAll(harmony);
        InventoryMenuPatches.PatchAll(harmony);
        InventoryPagePatches.PatchAll(harmony);
    }

    private void RegisterSerializableTypes()
    {
        var api = Helper.ModRegistry.GetApi<ISpaceCoreApi>("spacechase0.SpaceCore");
        if (api == null) throw new("Missing SpaceCore!");
        api.RegisterSerializerType(typeof(HatWrapper));
    }
}