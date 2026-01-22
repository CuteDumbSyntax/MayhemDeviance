using Terraria;
using Terraria.ModLoader;

namespace MayhemDeviance.Content.Buffs
{
public class OrbittingLoveBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.buffNoTimeDisplay[Type] = true; // hide timer
        Main.buffNoSave[Type] = true; // don't save
    }
}
}