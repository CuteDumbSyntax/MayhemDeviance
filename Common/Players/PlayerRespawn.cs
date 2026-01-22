using Terraria;
using Terraria.ModLoader;

namespace MayhemDeviance.Common.Players
{    public class PlayerRespawn : ModPlayer
    {
       public override void UpdateDead()
        {
            // change respawn when no bosses nearby
            if (!IsBossAlive())
            {
                if (Player.respawnTimer > 3 * 60)
                {
                    Player.respawnTimer = 3 * 60;
                }
            }
        }
        private bool IsBossAlive() // checks there's boss
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.boss)
                    return true;
            }
            return false;
        }
    }
}
