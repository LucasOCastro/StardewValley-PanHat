using HarmonyLib;
using StardewValley.Tools;

namespace StardewPanHat.Patches;

public static class PanPatches
{
    public static int GetAttachmentIndex(Pan pan) => 0;
    
    private static void Constructor_SetSlotCount_Postfix(Pan __instance)
    {
        __instance.AttachmentSlotsCount = 1;
    }

    public static void PatchAll(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.Constructor(typeof(Pan), Array.Empty<Type>()),
            postfix: new(typeof(PanPatches), nameof(Constructor_SetSlotCount_Postfix))
        );
        harmony.Patch(
            original: AccessTools.Constructor(typeof(Pan), new [] { typeof(int) }),
            postfix: new(typeof(PanPatches), nameof(Constructor_SetSlotCount_Postfix))
        );
    }
}