using System;

namespace RepairMe;

[Serializable]
public class RepairConfig
{
    public int WhetstoneDamageAsPercent = 5;
    public int WhetstoneMinimumDamage = 10;
    public int ToolRestoreAsPercent = 25;

    // public bool ApplyDurabilityLoss = true;
    // public int DurabilityLossAsPercent = 2;
}