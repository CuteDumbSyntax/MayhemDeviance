using MayhemDeviance.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MayhemDeviance.Content.Bosses
{
    [AutoloadBossHead]
    public class TestBoss : ModNPC
    {
        // This is a test boss. It'll be deleted later, or replaced.
private int attackTimer;
private Vector2 lockedAimDirection;
private int deathRayProj = -1;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 3;
        }

        public override void SetDefaults()
        {
            NPC.width = 124;
			NPC.height = 124;
			NPC.damage = 14;
			NPC.defense = 6;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.lifeMax = 10000;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = -1;

            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/OverworldHallow");
        }


    public override void AI()
{
    Player player = Main.player[NPC.target];
    if (!player.active || player.dead)
    {
        NPC.TargetClosest();
        return;
    }

    NPC.velocity = Vector2.Zero; // Boss does not move at all

    attackTimer++;

    // ===== PHASE 1: AIM (1 second = 60 ticks) =====
    if (attackTimer <= 60)
    {
        Vector2 toPlayer = player.Center - NPC.Center;
        lockedAimDirection = toPlayer.SafeNormalize(Vector2.UnitY);

        NPC.rotation = lockedAimDirection.ToRotation();

        // Optional charging dust
        Dust.NewDustPerfect(
            NPC.Center + lockedAimDirection * 40f,
            DustID.FireworkFountain_Red,
            Vector2.Zero
        );
    }

    // ===== PHASE 2: FIRE =====
    if (attackTimer == 60)
    {
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            deathRayProj = Projectile.NewProjectile(
                NPC.GetSource_FromAI(),
                NPC.Center,
                lockedAimDirection,
                ModContent.ProjectileType<SpinningDeathRay>(),
                80,
                0f,
                Main.myPlayer,
                NPC.whoAmI // pass boss ID
            );
        }

        SoundEngine.PlaySound(
            SoundID.Item33 with { Volume = 1.2f },
            NPC.Center
        );
    }

    // ===== PHASE 3: KEEP RAY ALIVE =====
    if (attackTimer > 60 && attackTimer <= 180)
    {
        if (deathRayProj >= 0 && Main.projectile[deathRayProj].active)
        {
            Projectile ray = Main.projectile[deathRayProj];
            ray.velocity = lockedAimDirection;
        }
    }

    // ===== RESET =====
    if (attackTimer > 180)
    {
        attackTimer = 0;
    }

    }

private void NormalMovement(Player player)
{
    NPC.TargetClosest();
    Vector2 targetPos = player.Center + new Vector2(0, -200f);
    Vector2 move = targetPos - NPC.Center;

    float speed = 6f;
    NPC.velocity = Vector2.Lerp(NPC.velocity, move.SafeNormalize(Vector2.Zero) * speed, 0.05f);

    NPC.ai[1]++;

    // After some time, enter deathray phase
    if (NPC.ai[1] >= 240)
    {
        NPC.ai[1] = 0;
        NPC.ai[0] = 1;
        NPC.netUpdate = true;
    }
}
private void DeathrayCharge(Player player)
{
    NPC.velocity = Vector2.Zero;
    NPC.ai[1]++;

    // Aim while charging
    Vector2 aim = player.Center - NPC.Center;
    float desiredRotation = aim.ToRotation();

    // Smooth aiming (looks MUCH better)
    NPC.rotation = NPC.rotation.AngleLerp(desiredRotation, 0.08f);

    // LOCK aim at the END of charge
    if (NPC.ai[1] == 60)
    {
        NPC.ai[3] = NPC.rotation;
    }

    if (NPC.ai[1] >= 60)
    {
         SoundEngine.PlaySound(SoundID.Zombie104, NPC.Center);
        NPC.ai[1] = 0;
        NPC.ai[0] = 2;

        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            int ray = Projectile.NewProjectile(
                NPC.GetSource_FromAI(),
                NPC.Center,
                Vector2.Zero,
                ModContent.ProjectileType<SpinningDeathRay>(),
                80,
                0f,
                Main.myPlayer,
                NPC.whoAmI,
                NPC.ai[3] // PASS LOCKED ROTATION
            );

            NPC.ai[2] = ray;
        }

        NPC.netUpdate = true;
    }
}

private void DeathrayFire(Player player)
{
    NPC.velocity = Vector2.Zero;

    // Keep facing player
    Vector2 aim = player.Center - NPC.Center;
    NPC.rotation = aim.ToRotation();

    NPC.ai[1]++;

    // Kill deathray after duration
    if (NPC.ai[1] >= 120) // 2 seconds firing
    {
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            if (NPC.ai[2] >= 0 && NPC.ai[2] < Main.maxProjectiles)
            {
                Projectile proj = Main.projectile[(int)NPC.ai[2]];
                if (proj.active)
                    proj.Kill();
            }
        }

        NPC.ai[0] = 0;
        NPC.ai[1] = 0;
        NPC.netUpdate = true;
    }
}
}
}
