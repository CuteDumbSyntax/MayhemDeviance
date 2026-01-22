using Terraria;
using Terraria.ModLoader;
using MayhemDeviance.Common.Systems;
using MayhemDeviance.Content.Projectiles;
using Microsoft.Xna.Framework;
using System.Net.Http.Headers;
using Terraria.GameContent.Bestiary;
using System;


namespace MayhemDeviance.Common.GlobalNPCs
{
    public class MayhemGlobalNPC : GlobalNPC
    {
        public override void SetDefaults(NPC npc) {
            if (MayhemDifficulty.MayhemMode && npc.friendly == false) {
                npc.lifeMax *= 3;
                npc.damage *= 2;
                
            }
        }

        public override void OnKill(NPC npc)
        {
            
            if(MayhemDifficulty.MayhemMode && npc.friendly == false) {
                Projectile.NewProjectile(
                    npc.GetSource_FromAI(),
                    npc.Center + Main.rand.NextVector2Circular(2, 2),
                    Vector2.Zero,
                    ModContent.ProjectileType<FakeHeart>(),
                    15, 0f);
            
            }
        }
        
     }
            
}

