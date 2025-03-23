using HarmonyLib;
using Vintagestory.API.Common;

namespace RepairMe;

[HarmonyPatch(typeof (CollectibleObject))]
[HarmonyPatchCategory("whetstoneRepairPatch")]
public class ItemPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(CollectibleObject.OnCreatedByCrafting)), HarmonyPriority(Priority.Last)]
    public static void OnCreatedByCrafting(ItemSlot[] allInputslots, ItemSlot outputSlot, GridRecipe byRecipe)
    {
        if (outputSlot.Itemstack == null)
            return;
        
        if (!byRecipe.Name.Path.Contains("useWhetstone"))
            return;
        
        var changes = RepairItem.CalculateDurabilityChange(outputSlot.Itemstack);
        outputSlot.Itemstack.Item.SetDurability(outputSlot.Itemstack, changes.ItemDur);
        outputSlot.MarkDirty();
    }
}