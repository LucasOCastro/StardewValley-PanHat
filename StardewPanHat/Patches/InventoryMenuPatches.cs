using System.Reflection.Emit;
using HarmonyLib;
using StardewPanHat.HatStuff;
using StardewValley.Menus;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace StardewPanHat.Patches;

internal static class InventoryMenuPatches
{
    /**
     * One of the conditions to attach an object to a tool is:
     *  (toAddTo == null || toAddTo is Object)
     * I want to add another situation, where 'toAddTo' is a Hat and I create a HatWrapper.
     *  (toAddTo == null || toAddTo is Object || (toAddTo is Hat hat; toAddTo = new HatWrapper(hat)))
     */
    private static IEnumerable<CodeInstruction> rightClick_GenerateHatWrapper_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        bool alreadyPatched = false;
        List<CodeInstruction> list = new(instructions);
        for (int i = 0; i < list.Count; i++)
        {
            if (alreadyPatched)
            {
                yield return list[i];
                continue;
            }

            //Iterate until find the section which verifies (toAddTo is Object)
            if (list[i + 1].opcode != OpCodes.Isinst || !typeof(Object).Equals(list[i + 1].operand))
            {
                yield return list[i];
                continue;
            }
            alreadyPatched = true;

            //If any condition within the OR clause is true, skips to the success label.
            var successSkipLabel = list[i - 1].operand;
            
            //Define and assign the label for the next condition
            var nextConditionLabel = generator.DefineLabel();
            list[i].labels.Add(nextConditionLabel);
            
            //if toAddTo is not Hat, skip to next condition in the OR clause
            yield return new(OpCodes.Ldarg_3);
            yield return new(OpCodes.Isinst, typeof(Hat));
            yield return new(OpCodes.Brfalse, nextConditionLabel);
            //toAdd = new HatWrapper((Hat)toAdd), then end the OR clause
            yield return new(OpCodes.Ldarg_3);
            yield return new(OpCodes.Castclass, typeof(Hat));
            yield return new(OpCodes.Newobj, AccessTools.Constructor(typeof(HatWrapper), new[] { typeof(Hat) }));
            yield return new(OpCodes.Starg, 3);
            yield return new(OpCodes.Br, successSkipLabel);

            yield return list[i];
        }
    }

    public static void PatchAll(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.Method(typeof(InventoryMenu), nameof(InventoryMenu.rightClick)),
            transpiler: new(typeof(InventoryMenuPatches), nameof(rightClick_GenerateHatWrapper_Transpiler))
        );
    }
}