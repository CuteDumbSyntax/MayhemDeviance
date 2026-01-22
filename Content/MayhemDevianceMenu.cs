using MayhemDeviance.Backgrounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MayhemDeviance.Content
{
	public class MayhemDevianceMenu : ModMenu
	{
		private const string menuAssetPath = "MayhemDeviance/Assets/Textures/Menu"; // Creates a constant variable representing the texture path, so we don't have to write it out multiple times

		private Asset<Texture2D> sunTexture;
		private Asset<Texture2D> moonTexture;

        private Asset<Texture2D> logoTexture;

		public override void Load() {
			sunTexture = ModContent.Request<Texture2D>($"{menuAssetPath}/ExampleSun");
			moonTexture = ModContent.Request<Texture2D>($"{menuAssetPath}/ExampliumMoon");
            logoTexture = ModContent.Request<Texture2D>($"{menuAssetPath}/Logo");
		}

		public override Asset<Texture2D> Logo => logoTexture;

		public override Asset<Texture2D> SunTexture => sunTexture;

		public override Asset<Texture2D> MoonTexture => moonTexture;

		/*
		In ExampleMod we preload all "extra" textures, as recommended in https://github.com/tModLoader/tModLoader/wiki/Assets#asset-loading-timing.
		It is possible to load textures on demand instead, which might be useful in rare situations such as rarely used large textures. That would look like this:
		private Asset<Texture2D> moonTexture;
		public override Asset<Texture2D> MoonTexture => moonTexture ??= ModContent.Request<Texture2D>($"{menuAssetPath}/ExampliumMoon");
		*/

		public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Ilyasragequit");

		public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<SurfaceBackgroundStyle>();

		public override string DisplayName => "Mayhem Deviance";

		public override void OnSelected() {
			SoundEngine.PlaySound(SoundID.Thunder); // Plays a thunder sound when this ModMenu is selected
		}

		public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor) {
			drawColor = Main.DiscoColor; // Changes the draw color of the logo
			return true;
		}
	}
}