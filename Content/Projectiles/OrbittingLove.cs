using System;
using MayhemDeviance.Content.Buffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace MayhemDeviance.Content.Projectiles
{
public class OrbittingLove : ModProjectile
{

    public override void SetDefaults()
    {
        Projectile.width = 20;
        Projectile.height = 20;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;

    }

    public override void AI()
{
    Player player = Main.player[Projectile.owner];

    // Die if player dead or buff missing
    if (!player.active || player.dead || !player.HasBuff(ModContent.BuffType<OrbittingLoveBuff>()))
    {
        Projectile.Kill();
        return;
    }

    // Orbit logic
    float orbitRadius = 100f;
    float orbitSpeed = 0.12f;

    float angle = Projectile.ai[1] + Projectile.ai[0];
    Projectile.Center = player.Center + orbitRadius * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

    Projectile.ai[1] += orbitSpeed;

    // Keep alive
    Projectile.timeLeft = 2;
}

}
}