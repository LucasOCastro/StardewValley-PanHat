using HarmonyLib;
using StardewValley;
using StardewValley.Tools;
using Object = StardewValley.Object;
// ReSharper disable InconsistentNaming

namespace StardewPanHat.Patches;

public static class ToolPatches
{
    public static void CanThisBeAttached_AllowHatWrapper_Postfix(Tool __instance, ref bool __result, Object o, int slot)
    {
        if (__instance is Pan pan && slot == PanPatches.GetAttachmentIndex(pan))
            __result = o is HatWrapper;
    }

    public static void PatchAll(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.Method(typeof(Tool), nameof(Tool.canThisBeAttached), new []{typeof(Object), typeof(int)}),
            postfix: new(typeof(ToolPatches), nameof(CanThisBeAttached_AllowHatWrapper_Postfix))
        );
    }
}