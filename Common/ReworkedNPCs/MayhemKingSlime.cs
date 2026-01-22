using MayhemDeviance.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using MayhemDeviance.Content.Items;
namespace MayhemDeviance.Common.ReworkedNPCs{
public class MayhemKingSlime : GlobalNPC
{
    public override bool AppliesToEntity(NPC npc, bool lateInstantiation)
    {
        return npc.type == NPCID.KingSlime;
    }

    // public override bool PreDraw(
    //     NPC npc,
    //     SpriteBatch spriteBatch,
    //     Vector2 screenPos,
    //     Color drawColor)
    // {
    //     if (!MayhemDifficulty.MayhemMode)
    //         return true; // draw vanilla texture normally

    //     Texture2D texture = ModContent.Request<Texture2D>(
    //         "MayhemDeviance/Common/ReworkedNPCs/BossTextures/KingSlimeM"
    //     ).Value;

    //     SpriteEffects effects =
    //         npc.spriteDirection == -1
    //             ? SpriteEffects.None
    //             : SpriteEffects.FlipHorizontally;

    //     Vector2 origin = npc.frame.Size() / 2f;

    //     spriteBatch.Draw(
    //         texture,
    //         npc.Center - screenPos,
    //         npc.frame,
    //         drawColor,
    //         npc.rotation,
    //         origin,
    //         npc.scale,
    //         effects,
    //         0f
    //     );

    //     return false; //  prevent vanilla drawing
    // }

     public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(
            ItemDropRule.ByCondition(
                new MayhemDifCondition(),
                ModContent.ItemType<Ilyasrage>(),
                chanceDenominator: 1
            )
        );
    }
}
}