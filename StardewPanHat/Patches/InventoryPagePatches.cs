using HarmonyLib;
using StardewValley;
using StardewValley.Menus;

namespace StardewPanHat.Patches;

public static class InventoryPagePatches
{
    private static void setHeldItem_UnwrapHatWrapper_Prefix(ref Item item)
    {
        if (item is HatWrapper wrapper)
        {
            wrapper.onDetachedFromParent();
            item = wrapper.InternalHat;
        }
    }

    public static void PatchAll(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.Method(typeof(InventoryPage), "setHeldItem"),
            prefix: new(typeof(InventoryPagePatches), nameof(setHeldItem_UnwrapHatWrapper_Prefix))
        );
    }
}