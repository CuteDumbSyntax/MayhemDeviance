// using MayhemDeviance.Common.Systems;
// using Microsoft.Xna.Framework;
// using Terraria;
// using Terraria.ID;
// using Terraria.ModLoader;
// namespace MayhemDeviance.Common.ReworkedNPCs.BossesGore{
// public class MayhemEyeGore : GlobalNPC
// {
//     public override bool AppliesToEntity(NPC npc, bool lateInstantiation)
//     {
//         return npc.type == NPCID.EyeofCthulhu;
//     }

//     public override void OnKill(NPC npc)
//     {
//         if (!MayhemDifficulty.MayhemMode)
//             return;

//         // ❗ Prevent vanilla gore
//         if (Main.netMode == NetmodeID.Server)
//             return;

//         SpawnMayhemGore(npc);
//     }

//     private void SpawnMayhemGore(NPC npc)
//     {
//         Vector2 pos = npc.Center;

//         Gore.NewGore(
//             npc.GetSource_Death(),
//             pos,
//             npc.velocity,
//             Mod.Find<ModGore>("Eye_Mayhem_Gore1").Type
//         );

//         Gore.NewGore(
//             npc.GetSource_Death(),
//             pos,
//             npc.velocity.RotatedByRandom(0.5f),
//             Mod.Find<ModGore>("Eye_Mayhem_Gore2").Type
//         );
//     }
// }
// }
