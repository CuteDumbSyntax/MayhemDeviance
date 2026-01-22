
using MayhemDeviance.Common.Configs;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static MayhemDeviance.Content.Items.Accessories.NinjasFury;


namespace MayhemDeviance.Content.Items.Accessories
{
	[AutoloadEquip(EquipType.Wings)]
	public class NinjasWings : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) {
			return ModContent.GetInstance<aModConfig>().WingsToggle;
		}

		public override void SetStaticDefaults() {

			ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(35, 5f, 0.9f);
		}

		public override void SetDefaults() {
			Item.width = 22;
			Item.height = 20;
			Item.value = 10000;
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;
            
		}
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
            dashPlayer.DashAccessoryEquipped = true;
        }

		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
			ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend) {
			ascentWhenFalling = 0.85f; // Falling glide speed
			ascentWhenRising = 0.15f; // Rising speed
			maxCanAscendMultiplier = 1f;
			maxAscentMultiplier = 3f;
			constantAscend = 0.135f;
		}

}


		
    }



