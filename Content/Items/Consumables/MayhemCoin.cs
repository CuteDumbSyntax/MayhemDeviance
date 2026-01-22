using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MayhemDeviance.Common.Systems;
using MayhemDeviance.Content.NPCs;
using Terraria.Audio;

namespace MayhemDeviance.Content.Items.Consumables
{
    public class MayhemCoin : ModItem
    {
        public override void SetDefaults() {
            Item.width = 20;
            Item.height = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.consumable = true;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Red;
        }

        public override bool CanUseItem(Player player) {
            return !MayhemDifficulty.MayhemMode;
        }

        public override bool? UseItem(Player player) {
            SoundEngine.PlaySound(SoundID.Roar, player.position);
            MayhemDifficulty.MayhemMode = true;

            if (Main.netMode != NetmodeID.MultiplayerClient) {
                NPC.NewNPC(
                    player.GetSource_ItemUse(Item),
                    (int)player.Center.X,
                    (int)(player.Center.Y - 800),
                    ModContent.NPCType<Content.NPCs.Nazar>()
                );
            }

            Main.NewText("Good luck!!", 200, 50, 50);
            return true;
        }
    }
}
