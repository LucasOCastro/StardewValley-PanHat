using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using StardewValley.Objects;
using StardewValley.Tools;
// ReSharper disable InconsistentNaming

namespace StardewPanHat.Patches;

public static class PanPatches
{
    //Store index in a dictionary to ensure it is compatible with any mod that adds extra attachment slots as well
    public static bool TryGetAttachment(Pan pan, [NotNullWhen(true)] out Hat? hatWrapper)
    {
        hatWrapper = null;
        if (!Indexes.TryGetValue(pan, out var index)) return false;
        if (index < 0 || index >= pan.attachments.Count) return false;
        if (pan.attachments[index] is not HatWrapper wrapper) return false;
        hatWrapper = wrapper.InternalHat;
        return true;
    }
    
    /// <returns>The hat attachment slot index or -1 if for some reason it is not registered.</returns>
    public static int GetAttachmentIndex(Pan pan) => Indexes.GetValueOrDefault(pan, -1);
    private static readonly Dictionary<Pan, int> Indexes = new();
    
    private static void Constructor_SetSlotCount_Postfix(Pan __instance)
    {
        Indexes[__instance] = __instance.AttachmentSlotsCount++; 
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