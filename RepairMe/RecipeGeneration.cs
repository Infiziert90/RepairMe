using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.ServerMods;

namespace RepairMe;

public class RecipeGeneration : ModSystem
{
    public override bool ShouldLoad(EnumAppSide forSide) => forSide.IsServer();
    public override double ExecuteOrder() => 1.02;
    
    public override void AssetsLoaded(ICoreAPI api)
    {
        var gridRecipeLoader = api.ModLoader.GetModSystem<GridRecipeLoader>();

        var whetstone = api.World.GetItem(new AssetLocation("repairme", "whetstone"));
        
        foreach (var item in api.World.Items)
        {
            if (item.Code is null)
                continue;

            // Air has durability 1 for whatever reason
            if (item.Durability < 2 || item.ItemClass != EnumItemClass.Item)
                continue;
            
            // Prevent repair of the whetstone with a whetstone
            if (item.ItemId == whetstone.ItemId)
                continue;
            
            var recipe = new GridRecipe();
            recipe.IngredientPattern = "W,T";
            recipe.Width = 1;
            recipe.Height = 2;
            recipe.Shapeless = true;
            recipe.AverageDurability = false;
            recipe.Name = new AssetLocation("repairme", $"{item.Code.Path} repair");
            recipe.CopyAttributesFrom = "T";
            recipe.RecipeGroup = 1;
            recipe.Output = new CraftingRecipeIngredient { Type = EnumItemClass.Item, Code = item.Code, };

            recipe.Ingredients = new Dictionary<string, CraftingRecipeIngredient>()
            {
                { "W", new CraftingRecipeIngredient { Type = EnumItemClass.Item, Code = whetstone.Code, IsTool = true } },
                { "T", new CraftingRecipeIngredient { Type = EnumItemClass.Item, Code = item.Code } }
            };

            gridRecipeLoader.LoadRecipe(new AssetLocation($"repairme:{item.Code.Path} repair"), recipe);
        }
    }
}