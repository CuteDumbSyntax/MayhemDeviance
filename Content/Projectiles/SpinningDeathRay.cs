using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
namespace MayhemDeviance.Content.Projectiles
{
public class SpinningDeathRay : ModProjectile
{

    public override void SetDefaults()
    {
        Projectile.width = 30;
        Projectile.height = 10;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.hide = false; // IMPORTANT
        Projectile.timeLeft = 360;
    }

   public override void AI()
{
    NPC boss = Main.npc[(int)Projectile.ai[0]];
    if (!boss.active)
    {
        Projectile.Kill();
        return;
    }

    Projectile.Center = boss.Center;

    // ai[1] = current angle
    if (Projectile.localAI[0] == 0f)
    {
        Projectile.localAI[0] = 1f; // init flag
        Projectile.rotation = Projectile.ai[1] - MathHelper.PiOver2;
    }

    float spinSpeed = 0.02f; // radians per tick (adjust!)
    Projectile.rotation += spinSpeed;
}


    // laser hitbox
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
{
    float rayLength = 3000f;
    float collisionWidth = 22f * 3f;

    Vector2 start = Projectile.Center;
    Vector2 end = start + Projectile.rotation
        .ToRotationVector2()
        .RotatedBy(MathHelper.PiOver2) // FIX
        * rayLength;

    float _ = 0f;
    return Collision.CheckAABBvLineCollision(
        targetHitbox.TopLeft(),
        targetHitbox.Size(),
        start,
        end,
        collisionWidth,
        ref _
    );
}


    public override bool PreDraw(ref Color lightColor)
{
    Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

    Vector2 start = Projectile.Center - Main.screenPosition;
    float length = 3000f;

    Rectangle source = new Rectangle(0, 0, texture.Width, texture.Height);

    // top center origin
    Vector2 origin = new Vector2(texture.Width / 2f, 0f);

    Vector2 scale = new Vector2(
        3f,
        length / texture.Height
    );

    Main.EntitySpriteDraw(
        texture,
        start,
        source,
        Color.Red,
        Projectile.rotation,
        origin,
        scale,
        SpriteEffects.None,
        0
    );

    return false;
}

}
}