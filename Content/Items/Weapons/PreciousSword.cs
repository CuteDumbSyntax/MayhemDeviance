using MayhemDeviance.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MayhemDeviance.Content.Items.Weapons
{
	// This is a weak copy of excalibur
	public class PreciousSword : ModItem
	{
		public override void SetDefaults() {
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useAnimation = 20;
			Item.useTime = 30;
			Item.damage = 34;
			Item.knockBack = 4.5f;
			Item.width = 40;
			Item.height = 40;
			Item.scale = 1f;
			Item.UseSound = SoundID.Item1;
			Item.rare = ItemRarityID.Pink;
			Item.value = Item.buyPrice(gold: 4); // Sell price is 5 times less than the buy price.
			Item.DamageType = DamageClass.Melee;
			Item.shoot = ModContent.ProjectileType<MagicalSwordProjectile>();
			Item.noMelee = true; // This is set the sword itself doesn't deal damage (only the projectile does).
			Item.shootsEveryUse = true; // This makes sure Player.ItemAnimationJustStarted is set when swinging.
			Item.autoReuse = true;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			float adjustedItemScale = player.GetAdjustedItemScale(Item); // Get the melee scale of the player and item.
			Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), type, damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax, adjustedItemScale);
			NetMessage.SendData(MessageID.PlayerControls, number: player.whoAmI); // Sync the changes in multiplayer.

			return base.Shoot(player, source, position, velocity, type, damage, knockback);
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.PlatinumBroadsword, 1);
            recipe.AddIngredient(ItemID.PlatinumBar, 5);
			recipe.AddIngredient(ItemID.PlatinumShortsword, 1);
			recipe.AddIngredient(ItemID.Ruby, 2);
			recipe.AddIngredient(ItemID.Diamond, 2);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();

			Recipe recipe2 = CreateRecipe();
			recipe2.AddIngredient(ItemID.GoldBroadsword, 1);
			recipe2.AddIngredient(ItemID.GoldShortsword, 1);
            recipe2.AddIngredient(ItemID.GoldBar, 5);
			recipe2.AddIngredient(ItemID.Ruby, 2);
			recipe2.AddIngredient(ItemID.Diamond, 2);
			recipe2.AddTile(TileID.Anvils);
			recipe2.Register();
		}
		
		
	}
}