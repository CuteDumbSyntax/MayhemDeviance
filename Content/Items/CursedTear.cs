using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MayhemDeviance.Content.Items
{
	public class CursedTear : ModItem
	{
		public override void SetStaticDefaults() {
			// Registers a vertical animation with 4 frames and each one will last 5 ticks (1/12 second)
			ItemID.Sets.ItemIconPulse[Type] = true; // The item pulses while in the player's inventory
			ItemID.Sets.ItemNoGravity[Type] = true; // Makes the item have no gravity

			Item.ResearchUnlockCount = 25; // Configure the amount of this item that's needed to research it in Journey mode.
		}

		public override void SetDefaults() {
			Item.width = 18;
			Item.height = 18;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 1000; // Makes the item worth 1 gold.
			Item.rare = ItemRarityID.Orange;
		}

		public override void PostUpdate() {
			Lighting.AddLight(Item.Center, Color.SkyBlue.ToVector3() * 0.55f * Main.essScale); // Makes this item glow when thrown out of inventory.
		}

		public override Color? GetAlpha(Color lightColor) {
			return new Color(255, 255, 255, 50); // Makes this item render at full brightness.
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		
	}
}