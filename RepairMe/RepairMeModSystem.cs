using Vintagestory.API.Common;

namespace RepairMe;

public class RepairMeModSystem : ModSystem
{
    public override void Start(ICoreAPI api)
    {
        base.Start(api);
        api.RegisterItemClass("repair", typeof(RepairItem));
    }
}