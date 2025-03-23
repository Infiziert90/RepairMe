using System;
using Vintagestory.API.Common;

namespace RepairMe;

public class RepairItem : Item
{
    public override void OnConsumedByCrafting(ItemSlot[] allInputSlots, ItemSlot stackInSlot, GridRecipe gridRecipe, CraftingRecipeIngredient fromIngredient, IPlayer byPlayer, int quantity)
    {
        if (!gridRecipe.Name.Path.Contains("useWhetstone"))
        {
            base.OnConsumedByCrafting(allInputSlots, stackInSlot, gridRecipe, fromIngredient, byPlayer, quantity);
            return;
        }
        
        try
        {
            var changes = CalculateDurabilityChange(gridRecipe.Output.ResolvedItemstack);
            stackInSlot.Itemstack.Item.DamageItem(byPlayer.Entity.World, byPlayer.Entity, stackInSlot, changes.WhetstoneDur);
        }
        catch (Exception ex)
        {
            api.Logger.Error(ex);
        }
    }

    public static (int WhetstoneDur, int ItemDur) CalculateDurabilityChange(ItemStack repairedTool)
    {
        var maxDurability = repairedTool.Item.GetMaxDurability(repairedTool);
        var remainingDurability = repairedTool.Item.GetRemainingDurability(repairedTool);
        
        var damagedDurability = (maxDurability / 100.0f) * 5;
        var repairedDurability = remainingDurability + ((maxDurability / 100.0f) * 25.0f);

        // Minimum durability damage for the whetstone is 10
        if (damagedDurability < 10)
            damagedDurability = 10;
        
        // Ensure that we don't go above maxDurability
        if (repairedDurability > maxDurability)
            repairedDurability = maxDurability;
        
        return ((int)damagedDurability, (int)repairedDurability);
    }
}