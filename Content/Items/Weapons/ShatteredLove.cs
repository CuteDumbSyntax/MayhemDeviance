using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using MayhemDeviance.Content.Projectiles;
using Terraria.Audio;
using Terraria.DataStructures;

namespace MayhemDeviance.Content.Items.Weapons
{
    public class ShatteredLove : ModItem
    {
        private const int MaxHearts = 3;

        private const int SpawnDelay = 10; // ticks between spawns (~0.33s)

		private int spawnTimer;
        public override void SetDefaults()
        {
            Item.damage = 55;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.noMelee = false;        // IMPORTANT
            Item.shoot = ModContent.ProjectileType<ShatteredLoveSw>();
            Item.noMelee = true; // This is set the sword itself doesn't deal damage (only the projectile does).
			Item.shootsEveryUse = true; // This makes sure Player.ItemAnimationJustStarted is set when swinging.
            Item.knockBack = 6f;
            Item.value = Item.buyPrice(0, 10);
            Item.rare = ItemRarityID.Pink;

            Item.autoReuse = true;


        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			float adjustedItemScale = player.GetAdjustedItemScale(Item); // Get the melee scale of the player and item.
			Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), type, damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax, adjustedItemScale);
			NetMessage.SendData(MessageID.PlayerControls, number: player.whoAmI); // Sync the changes in multiplayer.

			return base.Shoot(player, source, position, velocity, type, damage, knockback);
		}

        public override void HoldItem(Player player)
		{
			if (player.whoAmI != Main.myPlayer)
				return;

			if (player.controlUseItem)
			{
				spawnTimer++;

				if (spawnTimer >= SpawnDelay)
				{
					spawnTimer = 0;

					int type = ModContent.ProjectileType<AngryLove>();

					int count = 0;
					for (int i = 0; i < Main.maxProjectiles; i++)
					{
						Projectile p = Main.projectile[i];
						if (p.active && p.owner == player.whoAmI && p.type == type && p.ai[0] == 0)
							count++;
					}

					if (count < MaxHearts)
					{
						Projectile proj = Projectile.NewProjectileDirect(
							player.GetSource_ItemUse(Item),
							player.Center,
							Vector2.Zero,
							type,
							Item.damage * 2,
							0f,
							player.whoAmI
						);

						proj.ai[1] = count; // orbit index
						proj.netUpdate = true;

						SoundEngine.PlaySound(SoundID.Item4, player.Center);
					}
				}
			}
			else
			{
				spawnTimer = 0;
			}
		}

		public override void UpdateInventory(Player player)
		{
			if (player.whoAmI != Main.myPlayer)
				return;

			if (!player.controlUseItem)
			{
				int type = ModContent.ProjectileType<AngryLove>();

				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					Projectile p = Main.projectile[i];

					if (!p.active || p.owner != player.whoAmI || p.type != type)
						continue;

					if (p.ai[0] == 0)
					{
						Vector2 dir = (Main.MouseWorld - p.Center).SafeNormalize(Vector2.UnitX);
						p.ai[0] = 1;
						p.velocity = dir * 12f;
						p.netUpdate = true;
					}
				}
			}
		}
        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<PreciousSword>());
            recipe.AddIngredient(ItemID.Bone, 5);
			recipe.AddIngredient(ModContent.ItemType<CursedTear>(), 5);
			recipe.AddIngredient(ModContent.ItemType<EssenceOfLove>(), 5);
			recipe.AddIngredient(ItemID.NightsEdge, 1);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
        }
    }
}
