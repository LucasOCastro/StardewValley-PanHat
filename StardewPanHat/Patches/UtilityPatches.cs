using HarmonyLib;
using StardewModdingAPI;
using StardewPanHat.HatStuff;
using StardewValley;
using StardewValley.Mods;
using StardewValley.Objects;
using StardewValley.Tools;
// ReSharper disable InconsistentNaming

namespace StardewPanHat.Patches;

internal static class UtilityPatches
{
    private static string PanIDModDataKey => ModEntry.QualifyKey("HatPanId");
    private static string StoredModDataKey => ModEntry.QualifyKey("ModData/");
    private static string StoredEnchantmentIndicesKey => ModEntry.QualifyKey("EnchantmentIndices");
    private static string StoredPreviousEnchantmentIndicesKey => ModEntry.QualifyKey("PreviousEnchantmentIndices");
    
    private const char RangeSeparator = '~';
    
    //Changes a pan into a hat when equipping
    private static bool PerformSpecialItemPlaceReplacement_ChangeIntoHat_Prefix(ref Item __result, Item placedItem)
    {
        // If not pan or has no hat attached, skip
        if (placedItem is not Pan pan || !PanAttachmentSlots.TryGetAttachedHat(pan, out var hat))
            return true;

        // Extract the attached hat into the slot and store the og pan's info
        try
        {
            if (hat == null) throw new NullReferenceException("Null hat when trying to change pan into hat.");
            hat.modData[PanIDModDataKey] = pan.QualifiedItemId;

            StoreModData(pan.modData, hat.modData);
            StoreEnchantments(pan.enchantments, hat.enchantments, hat.modData, StoredEnchantmentIndicesKey);
            StoreEnchantments(pan.previousEnchantments, hat.previousEnchantments, hat.modData, StoredPreviousEnchantmentIndicesKey);

            __result = hat;
        }
        catch (Exception)
        {
            ModEntry.MonitorSingleton.Log($"Invalid internal hat stored in {pan.Name}.", LogLevel.Error);
        }
        return false;
    }

    //Changes a hat into a pan when unequipping
    private static bool PerformSpecialItemGrabReplacement_RestorePan_Prefix(ref Item __result, Item heldItem)
    {
        // If is not hat or has no pan info stored, skip
        if (heldItem is not Hat hat || !hat.modData.TryGetValue(PanIDModDataKey, out var panId)) 
            return true;

        // Recreate the old pan from the stored data and fill it with saved data
        try
        {
            var pan = ItemRegistry.Create<Pan>(panId, allowNull: true);
            if (pan == null) throw new NullReferenceException("Null pan when trying to restore pan from hat.");
            
            pan.attach(new HatWrapper(hat));
            hat.modData.Remove(PanIDModDataKey);
            
            RetrieveModData(hat.modData, pan.modData);
            RetrieveEnchantments(hat.enchantments, hat.modData, pan.enchantments, StoredEnchantmentIndicesKey);
            RetrieveEnchantments(hat.previousEnchantments, hat.modData, pan.previousEnchantments, StoredPreviousEnchantmentIndicesKey);
        
            __result = pan;
        }
        catch (Exception)
        {
            ModEntry.MonitorSingleton.Log($"Invalid pan ID {panId} stored in {hat.Name}.", LogLevel.Error);
            return true;
        }
        return false;
    }

    public static void PatchAll(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.Method(typeof(Utility), nameof(Utility.PerformSpecialItemPlaceReplacement)),
            prefix: new(typeof(UtilityPatches), nameof(PerformSpecialItemPlaceReplacement_ChangeIntoHat_Prefix))
        );
        harmony.Patch(
            original: AccessTools.Method(typeof(Utility), nameof(Utility.PerformSpecialItemGrabReplacement)),
            prefix: new(typeof(UtilityPatches), nameof(PerformSpecialItemGrabReplacement_RestorePan_Prefix))
        );
    }
    
    //When a pan turns into a hat, store the pan's info inside the hat
    private static void StoreModData(ModDataDictionary fromPan, ModDataDictionary toHat)
    {
        foreach (var pair in fromPan.FieldDict)
            toHat[StoredModDataKey + pair.Key] = pair.Value.Value;
    }

    private static void StoreEnchantments<T>(
        IList<T> fromPan, IList<T> toHat, ModDataDictionary toHatModData, string storageKey)
    {
        toHatModData[storageKey] = $"{toHat.Count}{RangeSeparator}{toHat.Count + fromPan.Count}";
        foreach (var enchant in fromPan)
            toHat.Add(enchant);
    }

    //When the hat goes back to a pan, retrieve the stored pan info and return it to the pan
    private static void RetrieveModData(ModDataDictionary fromHat, ModDataDictionary toPan)
    {
        var storedModData = fromHat.FieldDict
            .Where(p => p.Key.StartsWith(StoredModDataKey))
            .ToList();
        foreach (var stored in storedModData)
        {
            toPan[stored.Key.Replace(StoredModDataKey, "")] = stored.Value.Value;
            fromHat.Remove(stored.Key);
        }
    }

    private static void RetrieveEnchantments<T>(
        IList<T> fromHat, ModDataDictionary fromHatModData, IList<T> toPan, string storageKey)
    {
        var indexRange = fromHatModData[storageKey].Split(RangeSeparator);
        fromHatModData.Remove(storageKey);
        int min = int.Parse(indexRange[0]);
        int max = int.Parse(indexRange[1]);
        for (int i = max - 1; i >= min; i--)
        {
            toPan.Add(fromHat[i]);
            fromHat.RemoveAt(i);
        }
    }
}