using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.ServerMods;

namespace RepairMe;

public class RecipeGeneration : ModSystem
{
    private static Harmony HarmonyInstance;
    
    public override bool ShouldLoad(EnumAppSide forSide) => forSide.IsServer();
    public override double ExecuteOrder() => 1.02;
    
    public override void AssetsLoaded(ICoreAPI api)
    {
        var gridRecipeLoader = api.ModLoader.GetModSystem<GridRecipeLoader>();

        Item[] whetstones = [
            api.World.GetItem(new AssetLocation("repairme", "whetstone-flint")), 
            api.World.GetItem(new AssetLocation("repairme", "whetstone-lapislazuli")),
            api.World.GetItem(new AssetLocation("repairme", "whetstone-peridot")),
            api.World.GetItem(new AssetLocation("repairme", "whetstone-amethyst")),
            api.World.GetItem(new AssetLocation("repairme", "whetstone-emerald")),
            api.World.GetItem(new AssetLocation("repairme", "whetstone-diamond")),
        ];
        
        foreach (var item in api.World.Items)
        {
            if (item.Code is null)
                continue;

            // Air has durability 1 for whatever reason
            if (item.Durability < 2 || item.ItemClass != EnumItemClass.Item)
                continue;
            
            // Prevent repair of the whetstone with a whetstone
            if (whetstones.Any(i => i.ItemId == item.ItemId))
                continue;
            
            var recipe = new GridRecipe();
            recipe.IngredientPattern = "W,T";
            recipe.Width = 1;
            recipe.Height = 2;
            recipe.Shapeless = true;
            recipe.AverageDurability = false;
            recipe.Name = new AssetLocation($"repairme:{item.Code.Path} use-whetstone");
            recipe.CopyAttributesFrom = "T";
            recipe.RecipeGroup = 1;
            recipe.Output = new CraftingRecipeIngredient { Type = EnumItemClass.Item, Code = item.Code, };

            recipe.Ingredients = new Dictionary<string, CraftingRecipeIngredient>()
            {
                { "W", new CraftingRecipeIngredient { Type = EnumItemClass.Item, Code = "repairme:whetstone-*", IsTool = true } },
                { "T", new CraftingRecipeIngredient { Type = EnumItemClass.Item, Code = item.Code } }
            };

            gridRecipeLoader.LoadRecipe(new AssetLocation($"repairme:{item.Code.Path} use-whetstone"), recipe);
        }

        Patch();
    }

    private void Patch()
    {
        if (HarmonyInstance != null) 
            return;
        
        HarmonyInstance = new Harmony(Mod.Info.ModID);
        Mod.Logger.Debug("Patching...");
        
        HarmonyInstance.PatchCategory("whetstoneRepairPatch");
        Mod.Logger.Debug("Patched Whetstone Repair...");
    }

    public override void Dispose()
    {
        Mod.Logger.VerboseDebug("Unpatching...");
        HarmonyInstance?.UnpatchAll();
        HarmonyInstance = null;
    }
}