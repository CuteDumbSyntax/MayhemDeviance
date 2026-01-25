using MayhemDeviance.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using MayhemDeviance.Content.Items;
using MayhemDeviance.Content.Items.Weapons;
using Terraria.Audio;
using System;
namespace MayhemDeviance.Common.ReworkedNPCs{
public class MayhemEOC : GlobalNPC
{
    // public override bool AppliesToEntity(NPC npc, bool lateInstantiation)
    // {
    //     return npc.type == NPCID.EyeofCthulhu;
    // }


//custom boss texture
    // public override bool PreDraw(
    //     NPC npc,
    //     SpriteBatch spriteBatch,
    //     Vector2 screenPos,
    //     Color drawColor)
    // {
    //     if (!MayhemDifficulty.MayhemMode)
    //         return true; // draw vanilla texture normally

    //     Texture2D texture = ModContent.Request<Texture2D>(
    //         "MayhemDeviance/Common/ReworkedNPCs/BossTextures/EOCM"
    //     ).Value;

    //     SpriteEffects effects =
    //         npc.spriteDirection == -1
    //             ? SpriteEffects.None
    //             : SpriteEffects.FlipHorizontally;

    //     Vector2 origin = npc.frame.Size() / 2f;

    //     spriteBatch.Draw(
    //         texture,
    //         npc.Center - screenPos,
    //         npc.frame,
    //         drawColor,
    //         npc.rotation,
    //         origin,
    //         npc.scale,
    //         effects,
    //         0f
    //     );

    //     return false; //  prevent vanilla drawing
    // }

         public override bool InstancePerEntity => true;

        private int attackTimer;
        private int attackState;

        public override bool AppliesToEntity(NPC npc, bool lateInstantiation)
            => npc.type == NPCID.EyeofCthulhu;

        public override bool PreAI(NPC npc)
        {
            if (!MayhemDifficulty.MayhemMode)
                return true;

            // -------- ONLY PHASE 2 --------
            if (npc.ai[0] != 2f)
                return true; // vanilla AI runs fully

            Player player = Main.player[npc.target];
            if (!player.active || player.dead)
                return true;

            npc.TargetClosest();

            // Disable vanilla phase 2 AI
            npc.aiStyle = -1;
            npc.damage = npc.defDamage;

            // Proper Eye of Cthulhu rotation (IMPORTANT)
            Vector2 lookDir = player.Center - npc.Center;
            npc.rotation = lookDir.ToRotation() - MathHelper.PiOver2;

            attackTimer++;

            switch (attackState)
            {
                // =========================
                // DASH ATTACK
                // =========================
                case 0:
                    npc.velocity *= 1f;

                    if (attackTimer == 30)
                    {
                        SoundEngine.PlaySound(SoundID.Roar, npc.Center);
                    }

                    if (attackTimer == 45)
                    {
                        Vector2 dashDir = lookDir.SafeNormalize(Vector2.UnitY);
                        npc.velocity = dashDir * 18f;
                    }

                    if (attackTimer > 70)
                        NextAttack();
                    break;

                // =========================
                // CIRCLE + SHOOT
                // =========================
                case 1:
                    float radius = 220f;
                    float speed = 0.05f;

                    float angle = attackTimer * speed;
                    Vector2 offset = new Vector2(
                        (float)Math.Cos(angle),
                        (float)Math.Sin(angle)
                    ) * radius;

                    npc.Center = player.Center + offset;

                    if (attackTimer % 30 == 0)
                    {
                        Vector2 shootDir = lookDir.SafeNormalize(Vector2.UnitY);
                        Projectile.NewProjectile(
                            npc.GetSource_FromAI(),
                            npc.Center,
                            shootDir * 10f,
                            ProjectileID.BloodShot,
                            15,
                            0f
                        );
                    }

                    if (attackTimer > 120)
                        NextAttack();
                    break;
            }

            return false; // stop vanilla phase 2 AI
        }

        private void NextAttack()
        {
            attackTimer = 0;
            attackState++;
            if (attackState > 1)
                attackState = 0;
        }


     public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(
            ItemDropRule.ByCondition(
                new MayhemDifCondition(),
                ModContent.ItemType<TeethOfCthulhu>(),
                chanceDenominator: 1
            )
        );

        
    }
}
}
