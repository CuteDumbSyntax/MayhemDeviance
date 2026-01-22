using MayhemDeviance.Content.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace MayhemDeviance.Content.Bosses
{
    [AutoloadBossHead]
    public class IlyasClone : ModNPC
    {
         private enum BossPhase { Phase1, Phase2 }
        private enum BossAttack
        {
            None,

            // Phase 1
            OrbitDash,
            PredictiveLines,
            DeathRay,
            DashShoot,

            // Phase 2
            SpiralToBoss,
            CircleTrap,
            DashOnly,
            StopAndShoot
        }


        private const float Phase2CircleRadius = 320f;

        private BossPhase phase;
        private BossAttack attack;

        private BossPhase lastPhase;

        private int attackTimer;
        private int attackIndex;

        private List<int> spawnedProjectiles = new();
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 3;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
            {
                Velocity = 1f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 18;
            NPC.height = 40;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;

            NPC.damage = 14;
            NPC.defense = 6;
            NPC.lifeMax = 4000; // mayhem mod = 36000 health

            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;

            NPC.HitSound = SoundID.NPCHit37;
            NPC.DeathSound = SoundID.NPCDeath59;

            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/OverworldHallow");
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(),
                new FlavorTextBestiaryInfoElement("Forgotten clone of Ilya.")
            });
        }

        private void DrawLine(Vector2 start, Vector2 direction)
{
    for (int i = 0; i < 2000; i += 10)
    {
        Vector2 pos = start + direction * i;
        Dust d = Dust.NewDustPerfect(pos, DustID.RedTorch);
        d.noGravity = true;
        d.velocity = Vector2.Zero;
        d.scale = 1.2f;
    }
}

 public override void AI()
        {
            // Ensure valid target FIRST
NPC.TargetClosest(false);
Player player = Main.player[NPC.target];

// Despawn only if player is actually dead or gone
if (!player.active || player.dead)
{
    NPC.velocity.Y -= 0.2f;
    NPC.timeLeft = 10;

    if (NPC.velocity.Y < -10f)
        NPC.velocity.Y = -10f;

    return;
}
    phase = NPC.life < NPC.lifeMax / 2 ? BossPhase.Phase2 : BossPhase.Phase1;

// PHASE TRANSITION FIX
if (phase != lastPhase)
{
    attack = BossAttack.None;
    attackTimer = 0;
    attackIndex = 0;
    NPC.velocity = Vector2.Zero;
}
if (player.Center.X > NPC.Center.X)
{
    NPC.spriteDirection = 1;   // facing right
}
else
{
    NPC.spriteDirection = -1;  // facing left
}

lastPhase = phase;



            phase = NPC.life < NPC.lifeMax / 2 ? BossPhase.Phase2 : BossPhase.Phase1;

            attackTimer++;

            if (attack == BossAttack.None)
                SelectNextAttack();

            switch (phase)
            {
                case BossPhase.Phase1:
                    Phase1AI(player);
                    break;

                case BossPhase.Phase2:
                    Phase2AI(player);
                    break;
            }
        }
        private void Phase1AI(Player player)
        {
            switch (attack)
            {
                case BossAttack.OrbitDash:
                    OrbitDash(player);
                    break;

                case BossAttack.PredictiveLines:
                    PredictiveLines(player);
                    break;

                case BossAttack.DeathRay:
                    DeathRay(player);
                    break;

                case BossAttack.DashShoot:
                    DashShoot(player);
                    break;
            }
        } 
        private void Phase2AI(Player player)
        {
            switch (attack)
            {
                case BossAttack.SpiralToBoss:
                    SpiralToBoss();
                    break;

                case BossAttack.CircleTrap:
                    CircleTrap(player);
                    break;

                case BossAttack.DashOnly:
                    SimpleDash(player);
                    break;

                case BossAttack.StopAndShoot:
                    StopAndShoot(player);
                    break;
            }
        }
         private void OrbitDash(Player player)
        {
            if (attackTimer == 1)
            {
                SpawnOrbitingProjectiles(ModContent.ProjectileType<OrbittingStar>(), 6, 140f);                 // Orbitting Projectile 1
            }

            if (attackTimer % 60 == 0)
            {
                Vector2 dir = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                NPC.velocity = dir * 20f;
                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
            }

            if (attackTimer > 300)
                EndAttack();
        }

        private void PredictiveLines(Player player)
        {
            NPC.velocity = Vector2.Lerp(NPC.velocity,
                (player.Center + player.velocity * 30f - NPC.Center) * 0.01f, 0.08f);

            if (attackTimer % 60 == 0)
            {
                Vector2 predictedDir =
                    (player.Center + player.velocity * 40f - NPC.Center).SafeNormalize(Vector2.UnitX);

                DrawLine(NPC.Center, predictedDir);

                int p = Projectile.NewProjectile(
                    NPC.GetSource_FromAI(),
                    NPC.Center,
                    predictedDir * 14f,
                    ModContent.ProjectileType<FlyingHeart>(),                 // Projectile 1
                    20,
                    0f,
                    Main.myPlayer);

                spawnedProjectiles.Add(p);
                Main.projectile[p].timeLeft = 240;
            }

            if (attackTimer > 360)
                EndAttack();
        }

        private void DeathRay(Player player)
        {
            NPC.velocity *= 0.9f;

            Vector2 dir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);

            if (attackTimer < 120)
                DrawLine(NPC.Center, dir);

            if (attackTimer == 120)
            {
                for (int i = -8; i <= 8; i++)
                {
                    Vector2 spread = dir.RotatedBy(MathHelper.ToRadians(i * 2));
                    int p = Projectile.NewProjectile(
                        NPC.GetSource_FromAI(),
                        NPC.Center,
                        spread * 18f,
                        ModContent.ProjectileType<FlyingHeart>(),                 // Projectile 2
                        30,
                        0f,
                        Main.myPlayer);

                    spawnedProjectiles.Add(p);
                    Main.projectile[p].timeLeft = 120;
                }
            }

            if (attackTimer > 180)
                EndAttack();
        }

        private void DashShoot(Player player)
        {
            if (attackTimer % 50 == 0)
            {
                Vector2 dir = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                NPC.velocity = dir * 22f;
                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
            }

            if (attackTimer % 10 == 0)
            {
                int p = Projectile.NewProjectile(
                    NPC.GetSource_FromAI(),
                    NPC.Center,
                    (player.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 10f,
                    ModContent.ProjectileType<FlyingHeart>(),                 // Projectile 3
                    22,
                    0f,
                    Main.myPlayer);

                spawnedProjectiles.Add(p);
                Main.projectile[p].timeLeft = 180;
            }

            if (attackTimer > 300)
                EndAttack();
        }

        // =========================
        // ATTACKS — PHASE 2
        // =========================

        private void SpiralToBoss()
        {
            NPC.velocity *= 0.95f;
            Main.StartRain();
            if (attackTimer % 5 == 0)
            {
                Vector2 spawnPos = NPC.Center + Main.rand.NextVector2CircularEdge(900, 900);
                Vector2 vel = (NPC.Center - spawnPos).RotatedBy(MathHelper.ToRadians(attackTimer * 4)) * 0.03f;

                int p = Projectile.NewProjectile(
                    NPC.GetSource_FromAI(),
                    spawnPos,
                    vel,
                    ModContent.ProjectileType<FallingLight>(),                 // Projectile 4
                    25,
                    0f,
                    Main.myPlayer);

                spawnedProjectiles.Add(p);
                Main.projectile[p].timeLeft = 240;

            }

            if (attackTimer > 300)
                EndAttack();
        }

        private void CircleTrap(Player player)
        {
            NPC.velocity = Vector2.Zero;

            if (attackTimer == 1)
                SpawnOrbitingProjectiles(ModContent.ProjectileType<OrbittingStar2>(), 120, 700f);                 // Orbitting Projectile 2
            
        
            if (attackTimer % 6 == 0)
            {
                float rot = attackTimer * 0.15f;
                Vector2 dir = new Vector2(1, 0).RotatedBy(rot);

                int p = Projectile.NewProjectile(
                    NPC.GetSource_FromAI(),
                    NPC.Center,
                    dir * 12f,
                    ModContent.ProjectileType<FlyingHeart>(),                 //  Projectile 5
                    26,
                    0f,
                    Main.myPlayer);

                spawnedProjectiles.Add(p);
                Main.projectile[p].timeLeft = 240;
                SoundEngine.PlaySound(SoundID.Item20, NPC.Center);

            }
    
            if (Vector2.Distance(player.Center, NPC.Center) > 700f)
                player.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), 40, 0);

            if (attackTimer > 360)
                EndAttack();
        }

        private void SimpleDash(Player player)
        {
            if (attackTimer % 40 == 0)
            {
                NPC.velocity = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 26f;
                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
            }

            if (attackTimer > 240)
                EndAttack();
        }

        private void StopAndShoot(Player player)
        {
            NPC.velocity *= 0.9f;

            if (attackTimer % 20 == 0)
            {
                int p = Projectile.NewProjectile(
                    NPC.GetSource_FromAI(),
                    NPC.Center,
                    (player.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 12f,
                    ModContent.ProjectileType<FlyingHeart>(),                 // Projectile 3
                    25,
                    0f,
                    Main.myPlayer);

                spawnedProjectiles.Add(p);
                Main.projectile[p].timeLeft = 180;
            }

            if (attackTimer > 240)
                EndAttack();
        }

        // =========================
        // HELPERS
        // =========================

        private void SelectNextAttack()
{
    attackTimer = 0;
    spawnedProjectiles.Clear();

    if (phase == BossPhase.Phase1)
    {
        attackIndex = (attackIndex + 1) % 4;
        attack = (BossAttack)(attackIndex + 1);
    }
    else
    {
        attackIndex = (attackIndex + 1) % 4;
        attack = (BossAttack)(attackIndex + 5);
    }
}


        private void EndAttack()
        {
            attack = BossAttack.None;
            attackTimer = 0;

            foreach (int i in spawnedProjectiles)
            {
                if (i >= 0 && i < Main.maxProjectiles && Main.projectile[i].active)
                    Main.projectile[i].Kill();
            }

            spawnedProjectiles.Clear();
        }

        private void SpawnOrbitingProjectiles(int type, int count, float radius)
        {
            for (int i = 0; i < count; i++)
            {
                float rot = MathHelper.TwoPi / count * i;
                Vector2 pos = NPC.Center + rot.ToRotationVector2() * radius;

                int p = Projectile.NewProjectile(
                    NPC.GetSource_FromAI(),
                    pos,
                    Vector2.Zero,
                    type,
                    25,
                    0f,
                    Main.myPlayer,
                    NPC.whoAmI,
                    rot);

                spawnedProjectiles.Add(p);
            }
        }

        
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
                    NPC.frame.Y = 0;
            }
        }
    }
}
