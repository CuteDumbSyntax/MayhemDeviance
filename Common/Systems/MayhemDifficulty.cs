using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MayhemDeviance.Common.Systems
{
    public class MayhemDifficulty : ModSystem
    {
        public static bool MayhemMode;

        public override void OnWorldLoad() {
            MayhemMode = false;
        }

        public override void OnWorldUnload() {
            MayhemMode = false;
        }

        public override void SaveWorldData(TagCompound tag) {
            tag["MayhemMode"] = MayhemMode;
        }

        public override void LoadWorldData(TagCompound tag) {
            MayhemMode = tag.GetBool("MayhemMode");
        }
    }
}
