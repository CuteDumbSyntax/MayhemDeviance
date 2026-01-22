using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MayhemDeviance.Content.Projectiles
{
	// This projectile showcases advanced AI code. Of particular note is a showcase on how projectiles can stick to NPCs in a manner similar to the behavior of vanilla weapons such as Bone Javelin, Daybreak, Blood Butcherer, Stardust Cell Minion, and Tentacle Spike. This code is modeled closely after Bone Javelin.
	public class FallingLight : ModProjectile
	{

		public override void SetDefaults() {
			Projectile.width = 32; // The width of projectile hitbox
			Projectile.height = 32; // The height of projectile hitbox
			Projectile.aiStyle = 0; // The ai style of the projectile (0 means custom AI). For more please reference the source code of Terraria
			Projectile.friendly = false; // Can the projectile deal damage to enemies?
			Projectile.hostile = true; // Can the projectile deal damage to the player?
			Projectile.DamageType = DamageClass.Melee; // Makes the projectile deal ranged damage. You can set in to DamageClass.Throwing, but that is not used by any vanilla items
			Projectile.penetrate = -1; // How many monsters the projectile can penetrate.
			Projectile.timeLeft = 2; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.tileCollide = false; // Can the projectile collide with tiles?
			Projectile.light = 0.5f;
			
			
			
		}

		private const int GravityDelay = 45;
			public override void AI()
		{
						Projectile.rotation += 0.05f;

		}
				public override bool OnTileCollide(Vector2 oldVelocity) {
			Projectile.penetrate--;
			if (Projectile.penetrate <= 0) {
				Projectile.Kill();
			}
			else {
				Projectile.ai[0] += 0.1f;
				if (Projectile.velocity.X != oldVelocity.X) {
					Projectile.velocity.X = -oldVelocity.X;
				}
				if (Projectile.velocity.Y != oldVelocity.Y) {
					Projectile.velocity.Y = -oldVelocity.Y;
				}
				Projectile.velocity *= 0.75f;
				SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
			}
			return false;
		}

       
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			Projectile.ai[0] += 0.1f;
			Projectile.velocity *= 0.75f;
		}
	}
}
