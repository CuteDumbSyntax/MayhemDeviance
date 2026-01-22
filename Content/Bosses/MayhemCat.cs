using MayhemDeviance.Common.Systems;
using MayhemDeviance.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.Utilities;

namespace MayhemDeviance.Content.Bosses
{
		[AutoloadBossHead]
	public class MayhemCat : ModNPC
	{
		

		public override void SetStaticDefaults()
{
    Main.npcFrameCount[NPC.type] = 3;
}


		public override void SetDefaults() {
			NPC.width = 18;
			NPC.height = 40;
			NPC.damage = 14;
			NPC.defense = 6;
            NPC.boss = true;
            NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.lifeMax = 1000;
			NPC.HitSound = SoundID.Meowmere;
			NPC.DeathSound = SoundID.NPCDeath2;
			NPC.value = 60f;
			NPC.knockBackResist = 0.5f;
			NPC.aiStyle = -1;

			if (!Main.dedServ) {
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/MayhemCat");
				}

		}

        const int CHAOTIC_SHOOT = 0;

        const int TELEPORTBARRAGE = 1;

        const int DASH_LINE = 2;
        const int LASER_BEAM = 3;
        const int CIRCLE_ARENA = 4;

        public override void FindFrame(int frameHeight)
{
    NPC.frameCounter++;

    if (NPC.frameCounter >= 6)
    {
        NPC.frameCounter = 0;
        NPC.frame.Y += frameHeight;
    }

    if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
    {
        NPC.frame.Y = 0;
    }
}

private void DrawRedLine(Vector2 start, Vector2 direction)
{
    for (int i = 0; i < 2000; i += 10)
    {
        Vector2 pos = start + direction * i;
        Dust d = Dust.NewDustPerfect(pos, DustID.PinkTorch);
        d.noGravity = true;
        d.velocity = Vector2.Zero;
        d.scale = 1.2f;
    }
}

public override void AI()
{


    Player player = Main.player[NPC.target];

if (!player.active || player.dead)
{
    NPC.TargetClosest(false);

    // Fly up and fade out
    NPC.velocity.Y -= 0.2f;

    NPC.timeLeft = 10;

    if (NPC.timeLeft <= 1)
    {
        NPC.active = false;
    }
    return;
}

NPC.TargetClosest(true);
    // Phase check
    NPC.ai[3] = NPC.life < NPC.lifeMax / 2 ? 1 : 0;

    switch ((int)NPC.ai[0])
    {
        case CHAOTIC_SHOOT:
            ChaoticShoot(player);
            break;

        case TELEPORTBARRAGE:
            LaserBeam(player);
            break;

        case DASH_LINE:
            DashAttack(player);
            break;

        case LASER_BEAM:
            LaserBeam(player);
            break;

        case CIRCLE_ARENA:
            CircleArena(player);
            break;
    }
    if (player.Center.X > NPC.Center.X)
{
    NPC.spriteDirection = -1;
}
else
{
    NPC.spriteDirection = 1; 
}
}

void ChaoticShoot(Player player)
{
    NPC.ai[1]++;

    Vector2 targetPos = player.Center + Main.rand.NextVector2Circular(300, 300);
    NPC.velocity = Vector2.Lerp(NPC.velocity,
        (targetPos - NPC.Center).SafeNormalize(Vector2.Zero) * 8f, 0.08f);

    if (NPC.ai[1] % 40 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
    {
        Vector2 dir = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero);
        int p = Projectile.NewProjectile(
            NPC.GetSource_FromAI(),
            NPC.Center,
            dir * 10f,
            ModContent.ProjectileType<FlyingHeart>(),
            20, 0f);
        Main.projectile[p].timeLeft = 300;
    }

    if (NPC.ai[1] > 300)
        NextAttack();
}



void DashAttack(Player player)
{
    NPC.ai[1]++;

    Vector2 dir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);

    if (NPC.ai[1] < 120)
    {
        NPC.velocity *= 0.9f;
        DrawRedLine(NPC.Center, dir);
    }
    else if (NPC.ai[1] == 120)
    {
        NPC.velocity = dir * 24f;
    }
    else if (NPC.ai[1] < 160)
    {
        if (NPC.ai[1] % 6 == 0)
            ShootAtPlayer(player);
    }
    else
    {
        NextAttack();
    }
}

void LaserBeam(Player player)
{
    NPC.velocity = Vector2.Zero;
    NPC.ai[1]++;
    

    Vector2 dir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);

    // TELEGRAPH
    if (NPC.ai[1] < 60)
    {
        DrawRedLine(NPC.Center, dir);
        return;
    }

    // FIRE LASER
    if (NPC.ai[1] % 2 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
    {
        int p = Projectile.NewProjectile(
            NPC.GetSource_FromAI(),
            NPC.Center,
            dir * 16f,
            ModContent.ProjectileType<FlyingHeart>(),
            25, 0f
        );

        Projectile proj = Main.projectile[p];
        proj.ai[1] = -1;        // NORMAL PROJECTILE
        proj.timeLeft = 90;
        proj.tileCollide = false;
        proj.penetrate = -1;
        
    }
    SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
    // END ATTACK
    if (NPC.ai[1] > 180)
    {
        NextAttack();
    }
}


void CircleArena(Player player)
{
    NPC.velocity = Vector2.Zero;
    NPC.ai[1]++;

    float radius = 420f;

    // SPAWN CIRCLE ONCE
    if (NPC.ai[1] == 1 && Main.netMode != NetmodeID.MultiplayerClient)
    {
        for (int i = 0; i < 24; i++)
        {
            int p = Projectile.NewProjectile(
                NPC.GetSource_FromAI(),
                NPC.Center,
                Vector2.Zero,
                ModContent.ProjectileType<AngryHeart>(),
                25, 0f);

            Main.projectile[p].ai[0] = MathHelper.TwoPi * i / 24f; // angle
            Main.projectile[p].ai[1] = NPC.whoAmI;                // owner
            Main.projectile[p].timeLeft = 300;
            Main.projectile[p].tileCollide = false;
            Main.projectile[p].netUpdate = true;
        }
    }

    // SLOW SPIRAL (LIMITED TIME)
    if (NPC.ai[1] % 8 == 0 &&
        NPC.ai[1] < 360 &&
        Main.netMode != NetmodeID.MultiplayerClient)
    {
        float angle = NPC.ai[1] * 0.05f;
        int p = Projectile.NewProjectile(
            NPC.GetSource_FromAI(),
            NPC.Center,
            Vector2.UnitX.RotatedBy(angle) * 6f,
            ModContent.ProjectileType<FlyingHeart>(),
            25, 0f);

        Main.projectile[p].timeLeft = 300;
        Main.projectile[p].tileCollide = false;
        SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
    }

    // OUTSIDE CIRCLE DAMAGE
    if (Vector2.Distance(player.Center, NPC.Center) > radius)
    {
        player.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), 20, 0);
    }

    // END ATTACK
    if (NPC.ai[1] > 300)
    {
        NextAttack();
    }
}



void ShootAtPlayer(Player player)
{
    Vector2 dir = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero);
    int p = Projectile.NewProjectile(
        NPC.GetSource_FromAI(),
        NPC.Center,
        dir * 10f,
        ModContent.ProjectileType<FlyingHeart>(),
        20, 0f);
    Main.projectile[p].timeLeft = 300;
    SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
}

void NextAttack()
{
    NPC.ai[1] = 0;
    NPC.ai[2] = 0;

    // PHASE CHECK
    bool phase2 = NPC.life < NPC.lifeMax / 2;

    if (!phase2)
    {
        // Phase 1 attacks: 0 → 1 → 2 → 0
        NPC.ai[0]++;
        if (NPC.ai[0] > 2)
            NPC.ai[0] = 0;
    }
    else
    {
        // ENTER PHASE 2 ONCE
        if (NPC.ai[0] < 3)
            NPC.ai[0] = 3;
        else
        {
            // Phase 2 attacks: 3 ↔ 4
            NPC.ai[0]++;
            if (NPC.ai[0] > 4)
                NPC.ai[0] = 3;
        }
    }

    NPC.netUpdate = true;
}


public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
{
    Texture2D texture;

    bool throwing =
        NPC.ai[0] == CHAOTIC_SHOOT ||
        NPC.ai[0] == LASER_BEAM;

    if (throwing)
    {
        texture = ModContent.Request<Texture2D>("MayhemDeviance/Content/Bosses/MayhemCat_Throwing").Value;
    }
    else
    {
        texture = TextureAssets.Npc[NPC.type].Value;
    }

    SpriteEffects effects = NPC.spriteDirection == -1
        ? SpriteEffects.FlipHorizontally
        : SpriteEffects.None;

    spriteBatch.Draw(
        texture,
        NPC.Center - screenPos,
        NPC.frame,
        drawColor,
        NPC.rotation,
        NPC.frame.Size() / 2,
        NPC.scale,
        effects,
        0f
    );

    return false; // VERY IMPORTANT
}
public override void OnKill()
{
    DownedBossSystem.MaxBoss = true;
    for (int i = 0; i < Main.maxProjectiles; i++)
    {
        Projectile p = Main.projectile[i];

        if (!p.active)
            continue;

        if (p.type == ModContent.ProjectileType<AngryHeart>())
        {
            p.Kill();
        }
    }
    for(int i = 0; i<15; i++)
            {
                Projectile.NewProjectile(
                    NPC.GetSource_FromAI(),
                    NPC.Center + Main.rand.NextVector2Circular(200, 200),
                    Vector2.Zero,
                    ModContent.ProjectileType<FakeHeart>(),
                    15, 0f);
            }
            }
}



	}
