using HarmonyLib;
using StardewValley;
using StardewValley.Menus;

namespace StardewPanHat.Patches;

internal static class InventoryPagePatches
{
    //Unwrap the wrapper whenever it is grabbed in the inventory
    private static void setHeldItem_UnwrapHatWrapper_Prefix(ref Item item)
    {
        if (item is not HatWrapper wrapper) return;
        wrapper.onDetachedFromParent();
        item = wrapper.InternalHat;
    }

    public static void PatchAll(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.Method(typeof(InventoryPage), "setHeldItem"),
            prefix: new(typeof(InventoryPagePatches), nameof(setHeldItem_UnwrapHatWrapper_Prefix))
        );
    }
}