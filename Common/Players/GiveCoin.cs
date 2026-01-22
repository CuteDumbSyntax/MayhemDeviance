using Terraria;
using Terraria.ModLoader;
using MayhemDeviance.Content.Items.Consumables;

namespace MayhemDeviance.Common.Players
{
    public class GiveCoin : ModPlayer
    {
        public bool receivedStarterItem;

        public override void OnEnterWorld() {
            if (!receivedStarterItem) {
                Player.QuickSpawnItem(
                    Player.GetSource_Misc("MayhemStarter"),
                    ModContent.ItemType<MayhemCoin>()
                );
                receivedStarterItem = true;
            }
        }

        public override void SaveData(Terraria.ModLoader.IO.TagCompound tag) {
            tag["receivedStarterItem"] = receivedStarterItem;
        }

        public override void LoadData(Terraria.ModLoader.IO.TagCompound tag) {
            receivedStarterItem = tag.GetBool("receivedStarterItem");
        }
    }
}
