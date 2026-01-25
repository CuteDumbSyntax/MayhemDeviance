using Terraria;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.ModLoader.Utilities;
using System;
using MayhemDeviance.Content.Projectiles;
using Terraria.DataStructures;
using MayhemDeviance.Content.Items.Consumables;
using MayhemDeviance.Common.Systems;
using Terraria.Graphics.CameraModifiers;
using System.Collections.Generic;


namespace MayhemDeviance.Content.Bosses
{
	[AutoloadBossHead]
	public class Argun : ModNPC
	{

        

        bool phase2Started;
        Vector2 arenaCenter;

		public override void SetStaticDefaults() {
			Main.npcFrameCount[Type] = 3;

			NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers() { // Influences how the NPC looks in the Bestiary
				Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
		}

		public override void SetDefaults() {
			NPC.width = 18;
			NPC.height = 40;
			NPC.damage = 14;
			NPC.defense = 6;
            NPC.boss = true;
            NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.lifeMax = 2000;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2;
			NPC.value = 1000f;
			NPC.knockBackResist = 0.5f;
			NPC.aiStyle = -1;

			if (!Main.dedServ) {
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/BossPlaceHolder");

				}
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("Servant of Ilya...")
            });
        }
    
            
			
		

        public override void ModifyNPCLoot(NPCLoot npcLoot) {
						LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
			npcLoot.Add(notExpertRule);
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<ArgunBag>()));
			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.Furniture.ArgunRelic>()));

		}

		private const int ATTACK_CIRCLE = 0;
    private const int ATTACK_AIM = 1;
    private const int ATTACK_DASH = 2;
    private const int ATTACK_ENRAGED = 3;
    private const int ATTACK_DEATH = 4;

    public override void AI()
    {

        Player player = Main.player[NPC.target];

if (!player.active || player.dead)
{
    NPC.TargetClosest(false);
    player = Main.player[NPC.target];

    if (!player.active || player.dead)
    {
        DespawnBoss();
        return;
    }
}

        if (player.Center.X > NPC.Center.X)
{
    NPC.spriteDirection = 1;   // facing right
}
else
{
    NPC.spriteDirection = -1;  // facing left
}


        bool phase2 = NPC.life < NPC.lifeMax / 2;

        NPC.ai[1]++; // timer

        switch ((int)NPC.ai[0])
        {
            case ATTACK_CIRCLE:
                CircleAttack(player);
                break;

            case ATTACK_AIM:
                AimAndShoot(player);
                break;

            case ATTACK_DASH:
                DashAttack(player, phase2);
                break;

            case ATTACK_ENRAGED:
                EnragedArena(player);
                break;

            case ATTACK_DEATH:
                DeathSequence();
                break;
        }

        if (phase2 && !phase2Started)
{
    phase2Started = true;
    NextAttack(ATTACK_ENRAGED);

    if(phase2Started == true)
                {
                    SoundEngine.PlaySound(SoundID.Roar, player.position);
                    Main.NewText("I WILL KILL YOU!",200, 50, 50);
                    Main.instance.CameraModifiers.Add(
                    new PunchCameraModifier(
                    NPC.Center,
                    Main.rand.NextVector2CircularEdge(1f, 1f),
                    15f,
                    8f,
                    150,
                    1200f
        )
    );
                }
}

    }

        void CircleAttack(Player player)
{
    float radius = 300f;
    float speed = 0.04f;

    NPC.ai[2] += speed;
    Vector2 offset = new Vector2((float)Math.Cos(NPC.ai[2]), (float)Math.Sin(NPC.ai[2])) * radius;
    NPC.Center = player.Center + offset;

    if (NPC.ai[1] % 40 == 0)
    {
        Vector2 dir = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero);
        Projectile.NewProjectile(NPC.GetSource_FromAI(),
            NPC.Center,
            dir * 8f,
            ModContent.ProjectileType<RockProjectile>(),
            25, 1f);
            SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
        // camera shake
    //         Main.instance.CameraModifiers.Add(
    //     new PunchCameraModifier(
    //         NPC.Center,
    //         Main.rand.NextVector2CircularEdge(1f, 1f),
    //         15f,
    //         8f,
    //         25,
    //         1200f
    //     )
    // );
    }

    if (NPC.ai[1] > 300)
        NextAttack(ATTACK_AIM);
}

void AimAndShoot(Player player)
{
    NPC.velocity *= 0.9f;

    if (NPC.ai[1] % 25 == 0)
    {
        Vector2 dir = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero);
        Projectile.NewProjectile(NPC.GetSource_FromAI(),
            NPC.Center,
            dir * 10f,
            ModContent.ProjectileType<RockProjectile>(),
            30, 1f);
            SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
    }
    
    if (NPC.ai[1] > 180)
        NextAttack(ATTACK_DASH);
}

void DashAttack(Player player, bool fast)
{
    int aimTime = fast ? 60 : 180;

    if (NPC.ai[1] <= aimTime)
    {
        Vector2 dir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
        DrawRedLine(NPC.Center, dir);
        NPC.velocity *= 0.95f;
        return;
    }

    if (NPC.ai[1] == aimTime + 1)
    {
        Vector2 dashDir = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero);
        NPC.velocity = dashDir * (fast ? 25f : 18f);
    }

    if (NPC.ai[1] > aimTime + 30)
    {
        NPC.ai[2]++;
        NPC.ai[1] = 0;

        if (NPC.ai[2] >= 5)
        {
            NPC.ai[2] = 0;
            NextAttack(ATTACK_CIRCLE);
        }
    }
}

private void DrawRedLine(Vector2 start, Vector2 direction)
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

void KillAllRocks()
{
    for (int i = 0; i < Main.maxProjectiles; i++)
    {
        Projectile p = Main.projectile[i];
        if (p.active && p.type == ModContent.ProjectileType<RockProjectile>())
        {
            p.Kill();
        }
    }
}
void EnragedArena(Player player)
{
    float arenaRadius = 500f;
    NPC.velocity = Vector2.Zero;
    NPC.Center = NPC.Center; // hard-lock position
    if (NPC.ai[1] == 1)
    {
        arenaCenter = NPC.Center;
        for (int i = 0; i < 40; i++)
        {
            Vector2 pos = NPC.Center + Vector2.UnitX.RotatedBy(MathHelper.TwoPi * i / 40f) * arenaRadius;
            Projectile.NewProjectile(NPC.GetSource_FromAI(),
                pos, Vector2.Zero,
                ModContent.ProjectileType<SharpRockProjectile>(),
                0, 0f);
        }
        NPC.velocity = Vector2.Zero;
        NPC.Center = arenaCenter;

    }

    if (Vector2.Distance(player.Center, NPC.Center) > arenaRadius)
    {
        player.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI),50, 0);
    }

    if (NPC.ai[1] % 15 == 0)
    {
        Vector2 dir = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero);
        Projectile.NewProjectile(NPC.GetSource_FromAI(),
            NPC.Center,
            dir * 12f,
            ModContent.ProjectileType<RockProjectile>(),
            35, 1f);
            SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
           
    }
}

void DeathSequence()
{
    NPC.velocity = Vector2.Zero;
    NPC.dontTakeDamage = true;

    if (NPC.ai[1] == 1)
    {
        KillAllRocks();
        Main.NewText("STOP BEING TOXIC!!",200, 50, 50);
    }

    if (NPC.ai[1] == 120)
        Main.NewText("He will...");

    if (NPC.ai[1] == 240)
        Main.NewText("He will kill you!");

    if (NPC.ai[1] > 300)
    {
    DownedBossSystem.ArgunBoss = true;


        NPC.life = -1;
        NPC.HitEffect();
        NPC.checkDead();
        
    }

    NPC.ai[1]++;
}




public override bool CheckDead()
{
    if (NPC.ai[0] == ATTACK_DEATH)
        return true;

    NPC.life = 1;
    NPC.ai[0] = ATTACK_DEATH;
    NPC.ai[1] = 0;
    NPC.velocity = Vector2.Zero;
    NPC.dontTakeDamage = true;
    NPC.netUpdate = true;
    return false;
}


void NextAttack(int attack)
{
    NPC.ai[0] = attack;
    NPC.ai[1] = 0;
    NPC.ai[2] = 0;
    NPC.netUpdate = true;
}

void DespawnBoss()
{
    KillAllRocks(); // remove arena + projectiles

    NPC.velocity.Y -= 0.4f; // fly upward
    NPC.timeLeft = 10;      // force despawn
    NPC.netUpdate = true;
}

public override void FindFrame(int frameHeight)
{
    NPC.frameCounter++;

    if (NPC.frameCounter >= 5) // animation speed
    {
        NPC.frameCounter = 0;
        NPC.frame.Y += frameHeight;

        if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
        {
            NPC.frame.Y = 0;
        }
    }
}



		
		

    }
}