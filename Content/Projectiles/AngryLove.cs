using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Terraria.Audio;

namespace MayhemDeviance.Content.Projectiles
{
	public class AngryLove : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;

			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;

			Projectile.penetrate = 1;
			Projectile.timeLeft = 1000;

			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
            

if (!player.active || player.dead)
{
	Projectile.Kill();
	return;
}

			// ORBIT MODE
			if (Projectile.ai[0] == 0)
			{
				float orbitRadius = 48f;
				float orbitSpeed = 0.05f;

				float angle =
					Main.GameUpdateCount * orbitSpeed +
					Projectile.ai[1] * MathHelper.TwoPi / 3f;

				Vector2 offset = angle.ToRotationVector2() * orbitRadius;

				Projectile.Center = player.Center + offset;
				Projectile.velocity = Vector2.Zero;
				Projectile.rotation += 0.15f;
			}
			// FLY MODE
			else
			{
				Projectile.rotation += 0.01f;
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.Kill();
			return false;
		}
	}
}
