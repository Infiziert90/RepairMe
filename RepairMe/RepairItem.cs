using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace RepairMe;

public class RepairItem : Item
{
    public override void OnConsumedByCrafting(ItemSlot[] allInputSlots, ItemSlot stackInSlot, GridRecipe gridRecipe, CraftingRecipeIngredient fromIngredient, IPlayer byPlayer, int quantity)
    {
        if (!gridRecipe.Name.Path.Contains("repair"))
        {
            base.OnConsumedByCrafting(allInputSlots, stackInSlot, gridRecipe, fromIngredient, byPlayer, quantity);
            return;
        }
        
        try
        {
            var mouseSlotItem = byPlayer.InventoryManager.MouseItemSlot;
            if (mouseSlotItem.Empty)
            {
                base.OnConsumedByCrafting(allInputSlots, stackInSlot, gridRecipe, fromIngredient, byPlayer, quantity);
            
                api.Logger.Error("Repair has been reset to prevent item duplication.");
                ((ICoreClientAPI) api).TriggerIngameError(sender: this, errorCode: "exceptionShift", text: "Using <hk>shift</hk> while repairing is currently broken, please drag&drop the output instead.");
                
                return;
            }
            
            var changes = CalculateDurabilityChange(mouseSlotItem.Itemstack);
        
            stackInSlot.Itemstack.Collectible.DamageItem(byPlayer.Entity.World, byPlayer.Entity, stackInSlot, changes.WhetstoneDur);
            mouseSlotItem.Itemstack.Item.SetDurability(mouseSlotItem.Itemstack, changes.ItemDur);
        }
        catch (Exception ex)
        {
            
        }
    }

    private (int WhetstoneDur, int ItemDur) CalculateDurabilityChange(ItemStack repairedTool)
    {
        var maxDurability = repairedTool.Item.GetMaxDurability(repairedTool);
        var remainingDurability = repairedTool.Item.GetRemainingDurability(repairedTool);

        var damagedDurability = (maxDurability / 100) * 5;
        var repairedDurability = remainingDurability + ((maxDurability / 100) * 25);
        
        if (repairedDurability > maxDurability)
            repairedDurability = maxDurability;
        
        return (damagedDurability, repairedDurability);
    }
}