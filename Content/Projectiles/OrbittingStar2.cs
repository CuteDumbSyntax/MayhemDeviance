using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MayhemDeviance.Content.Projectiles
{
	
	public class OrbittingStar2 : ModProjectile
	{

		public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 600;
    }

    public override void AI()
{
    int bossID = (int)Projectile.ai[0];
    if (bossID < 0 || bossID >= Main.maxNPCs)
    {
        Projectile.Kill();
        return;
    }

    NPC boss = Main.npc[bossID];
    if (!boss.active)
    {
        Projectile.Kill();
        return;
    }

    // ai[1] = angle (already set by boss on spawn)
    Projectile.ai[1] += 0.05f;

    // OPTIONAL: radius via localAI
    if (Projectile.localAI[0] == 0f)
        Projectile.localAI[0] = 700f; // default radius

    float angle = Projectile.ai[1];
    float radius = Projectile.localAI[0];

    Projectile.Center = boss.Center + angle.ToRotationVector2() * radius;
}

	}
}

