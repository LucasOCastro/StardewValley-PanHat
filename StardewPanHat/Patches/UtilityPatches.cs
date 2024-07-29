using HarmonyLib;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
// ReSharper disable InconsistentNaming

namespace StardewPanHat.Patches;

public static class UtilityPatches
{
    private static string PanIDModDataKey => ModEntry.QualifyKey("HatPanId"); 
    
    private static bool PerformSpecialItemPlaceReplacement_ChangeHat_Prefix(ref Item __result, Item placedItem)
    {
        if (placedItem is not Pan pan || pan.attachments[PanPatches.GetAttachmentIndex(pan)] is not HatWrapper wrapper) 
            return true;
        
        var hat = wrapper.InternalHat;
        hat.modData[PanIDModDataKey] = pan.QualifiedItemId;
            
        //TODO copying mod data and enchantments may cause conflicts if the hat already has any
        hat.modData.CopyFrom(pan.modData);
        hat.enchantments.AddRange(pan.enchantments);
        hat.previousEnchantments.AddRange(pan.previousEnchantments);
            
        __result = hat;
        return false;
    }

    private static bool PerformSpecialItemGrabReplacement_RestorePan_Prefix(ref Item __result, Item heldItem)
    {
        if (heldItem is not Hat hat || !hat.modData.TryGetValue(PanIDModDataKey, out var panId)) 
            return true;
        
        var pan = ItemRegistry.Create<Pan>(panId);
        pan.attach(new HatWrapper(hat));
        hat.modData.Remove(PanIDModDataKey);
            
        //TODO Clearing the mod data and enchantments may clear stuff from other mods!
        pan.modData.CopyFrom(hat.modData);
        hat.modData.Clear();
        pan.enchantments.AddRange(hat.enchantments);
        hat.enchantments.Clear();
        pan.previousEnchantments.AddRange(hat.previousEnchantments);
        hat.previousEnchantments.Clear();
            
        __result = pan;
        return false;
    }

    public static void PatchAll(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.Method(typeof(Utility), nameof(Utility.PerformSpecialItemPlaceReplacement)),
            prefix: new(typeof(UtilityPatches), nameof(PerformSpecialItemPlaceReplacement_ChangeHat_Prefix))
        );
        harmony.Patch(
            original: AccessTools.Method(typeof(Utility), nameof(Utility.PerformSpecialItemGrabReplacement)),
            prefix: new(typeof(UtilityPatches), nameof(PerformSpecialItemGrabReplacement_RestorePan_Prefix))
        );
    }
}