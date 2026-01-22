using System;
using MayhemDeviance.Content.Buffs;
using MayhemDeviance.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MayhemDeviance.Content.Items.Accessories
{
public class LoveCrystal : ModItem
{

    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 28;
        Item.accessory = true;
        Item.value = Item.sellPrice(silver: 50);
        Item.rare = ItemRarityID.Pink;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
{
    // Give the buff
    player.AddBuff(ModContent.BuffType<OrbittingLoveBuff>(), 2); // 2 ticks, refresh every frame
    player.lifeRegen += 1;

    // Spawn projectiles
    bool hasProj1 = false;
    bool hasProj2 = false;

    for (int i = 0; i < Main.maxProjectiles; i++)
    {
        Projectile p = Main.projectile[i];
        if (p.active && p.owner == player.whoAmI && p.type == ModContent.ProjectileType<OrbittingLove>())
        {
            if (p.ai[0] == 0f) hasProj1 = true;
            if (p.ai[0] == (float)Math.PI) hasProj2 = true;
        }
    }

    if (!hasProj1)
        Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<OrbittingLove>(), 55, 1f, player.whoAmI, 0f);
    
    if (!hasProj2)
        Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<OrbittingLove>(), 55, 1f, player.whoAmI, (float)Math.PI);
}

}
}