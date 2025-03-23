using Vintagestory.API.Common;

namespace RepairMe;

public class RepairMeModSystem : ModSystem
{
    public static ILogger Logger;

    public override void StartPre(ICoreAPI api)
    {
        Logger = Mod.Logger;
    }
    
    public override void Start(ICoreAPI api)
    {
        base.Start(api);
        api.RegisterItemClass("repair", typeof(RepairItem));
    }
}