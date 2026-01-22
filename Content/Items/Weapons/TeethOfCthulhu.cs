using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MayhemDeviance.Content.Items.Weapons
{
	public class TeethOfCthulhu : ModItem
	{
		public override void SetDefaults() {
			// Alter any of these values as you see fit, but you should probably keep useStyle on 1, as well as the noUseGraphic and noMelee bools

			// Common Properties
			Item.rare = ItemRarityID.Red;
			Item.value = Item.sellPrice(silver: 5);
			Item.maxStack = 1;

			// Use Properties
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useAnimation = 9;
			Item.useTime = 9;
			Item.UseSound = SoundID.Item1;
			// Weapon Properties			
			Item.damage = 34;
			Item.knockBack = 5f;
			Item.DamageType = DamageClass.Melee;

			// Projectile Properties
			Item.shootSpeed = 20f;
			Item.shoot = ModContent.ProjectileType<Projectiles.TeethProj>(); // The projectile that will be thrown
		}
	}
}