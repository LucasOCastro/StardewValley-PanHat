using HarmonyLib;
using StardewPanHat.HatStuff;
using StardewValley.Tools;
// ReSharper disable InconsistentNaming

namespace StardewPanHat.Patches;

internal static class PanPatches
{
    private static void Constructor_SetSlotCount_Postfix(Pan __instance)
    {
        PanAttachmentSlots.SetupAttachmentSlot(__instance);
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