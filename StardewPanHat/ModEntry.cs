using HarmonyLib;
using StardewModdingAPI;
using StardewPanHat.Patches;

namespace StardewPanHat;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        Harmony harmony = new(ModManifest.UniqueID);
        
        PanPatches.PatchAll(harmony);
        ToolPatches.PatchAll(harmony);
        UtilityPatches.PatchAll(harmony);
        InventoryMenuPatches.PatchAll(harmony);
        InventoryPagePatches.PatchAll(harmony);
    }
}