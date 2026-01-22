
using MayhemDeviance.Content.Items.Accessories;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MayhemDeviance.Content
{
	// This class contains thoughtful examples of item recipe creation.
	// Recipes are explained in detail on the https://github.com/tModLoader/tModLoader/wiki/Basic-Recipes and https://github.com/tModLoader/tModLoader/wiki/Intermediate-Recipes wiki pages. Please visit the wiki to learn more about recipes if anything is unclear.
	public class Recipes : ModSystem
	{
    public override void AddRecipes()
    {
        Recipe katana = Recipe.Create(ItemID.Katana);
        katana.AddIngredient(ItemID.Wood, 5);
        katana.AddIngredient(ItemID.SilverBar, 10);
        katana.AddIngredient(ItemID.IronBar, 5);
        katana.AddTile(TileID.Anvils);
        katana.Register();

        Recipe ninjasFury = Recipe.Create(ModContent.ItemType<NinjasFury>());
        ninjasFury.AddIngredient(ItemID.Katana, 1);
        ninjasFury.AddIngredient(ItemID.NinjaHood, 1);
        ninjasFury.AddIngredient(ItemID.NinjaShirt, 1);
        ninjasFury.AddIngredient(ItemID.NinjaPants, 1);
        ninjasFury.AddIngredient(ItemID.Silk, 5);
        ninjasFury.AddTile(TileID.Anvils);
        ninjasFury.Register();

        Recipe W1 = Recipe.Create(ItemID.CreativeWings);
        W1.AddIngredient(ItemID.Cloud, 5);
        W1.AddIngredient(ItemID.Feather, 5);
        W1.AddIngredient(ItemID.Silk, 2);
        W1.AddIngredient(ItemID.ManaCrystal, 2);
        W1.AddTile(TileID.WorkBenches);
        W1.Register();
    }


    }
}