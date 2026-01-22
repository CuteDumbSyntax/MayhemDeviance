using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using MayhemDeviance.Content.Buffs;
using MayhemDeviance.Content.Projectiles;
using Terraria.Audio;
namespace MayhemDeviance.Content.Projectiles.Minions
{
    public class IceFairyMinion : ModProjectile
    {
        private const int ShootCooldown = 25; // 1 second

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;

            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            Main.projPet[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18000;
        }

        public override bool? CanCutTiles() => false;
        public override bool MinionContactDamage() => false;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!CheckActive(owner))
                return;

            Animate();

            Vector2 idlePos = GetIdlePosition(owner);
            SearchForTarget(owner, out bool foundTarget, out NPC target);

            if (foundTarget) {
                AttackBehavior(target);
                Projectile.spriteDirection = target.Center.X > Projectile.Center.X ? 1 : -1;
            }else{
                IdleBehavior(idlePos);
                Projectile.spriteDirection = Main.player[Projectile.owner].direction;
            }
            AvoidOtherMinions();
        }

        // ---------------- ACTIVE CHECK ----------------

        private bool CheckActive(Player owner)
        {
            if (!owner.active || owner.dead)
            {
                owner.ClearBuff(ModContent.BuffType<IceFairyStaffBuff>());
                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<IceFairyStaffBuff>()))
                Projectile.timeLeft = 2;

            return true;
        }

        // ---------------- IDLE ----------------

        private Vector2 GetIdlePosition(Player owner)
        {
            Vector2 pos = owner.Center;
            pos.Y -= 48f;
            pos.X += (-40f * owner.direction) - Projectile.minionPos * 30f;
            return pos;
        }

        private void IdleBehavior(Vector2 idlePos)
        {
            // Projectile.spriteDirection = Main.player[Projectile.owner].direction;
            float speed = 6f;
            float inertia = 40f;

            Vector2 toIdle = idlePos - Projectile.Center;
            float distance = toIdle.Length();

            if (distance > 2000f)
            {
                Projectile.Center = idlePos;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }

            if (distance > 20f)
            {
                toIdle.Normalize();
                toIdle *= speed;
                Projectile.velocity = (Projectile.velocity * (inertia - 1) + toIdle) / inertia;
            }
        }

        // ---------------- TARGET SEARCH ----------------

        private void SearchForTarget(Player owner, out bool foundTarget, out NPC target)
        {
            foundTarget = false;
            target = null;
            float maxDistance = 700f;

            if (owner.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[owner.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy() &&
                    Vector2.Distance(Projectile.Center, npc.Center) < maxDistance)
                {
                    foundTarget = true;
                    target = npc;
                    return;
                }
            }

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.CanBeChasedBy())
                    continue;

                float dist = Vector2.Distance(Projectile.Center, npc.Center);
                if (dist < maxDistance && Collision.CanHitLine(
                    Projectile.position, Projectile.width, Projectile.height,
                    npc.position, npc.width, npc.height))
                {
                    maxDistance = dist;
                    foundTarget = true;
                    target = npc;
                }
            }
        }

        // ---------------- ATTACK ----------------

        private void AttackBehavior(NPC target)
        {
            // Orbit logic
            
            float orbitRadius = 90f + Projectile.minionPos * 6f;
            float orbitSpeed = 0.04f;

            float indexOffset = Projectile.minionPos * MathHelper.TwoPi / 6f;
            Projectile.ai[0] += orbitSpeed;
            Vector2 orbitOffset = new Vector2(
            (float)Math.Cos(Projectile.ai[0] + indexOffset),
            (float)Math.Sin(Projectile.ai[0] + indexOffset)
            ) * orbitRadius;

            
            Vector2 desiredPos = target.Center + orbitOffset;
            Vector2 move = desiredPos - Projectile.Center;

            float speed = 8f;
            Projectile.velocity = Vector2.Lerp(
                Projectile.velocity,
                move.SafeNormalize(Vector2.Zero) * speed,
                0.1f
            );

            // Shooting
            Projectile.ai[1]++;
            if (Projectile.ai[1] >= ShootCooldown)
            {
                Projectile.ai[1] = 0;

                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 shootDir = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitY);
                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        shootDir * 10f,
                        ModContent.ProjectileType<SharpIce>(),
                        Projectile.damage,
                        Projectile.knockBack,
                        Projectile.owner
                    );
                    SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
                    

                }
            }

            
        }

        // ---------------- VISUALS ----------------

        private void Animate()
        {
            Projectile.rotation = Projectile.velocity.X * 0.05f;

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 0;
            }
        }

        private void AvoidOtherMinions()
{
    float push = 0.12f; // stronger than idle

    foreach (Projectile other in Main.ActiveProjectiles)
    {
        if (other.whoAmI == Projectile.whoAmI)
            continue;

        if (other.owner == Projectile.owner && other.minion)
        {
            float dist = Vector2.Distance(Projectile.Center, other.Center);

            if (dist < Projectile.width)
            {
                Vector2 pushAway = Projectile.Center - other.Center;
                if (pushAway != Vector2.Zero)
                {
                    pushAway.Normalize();
                    Projectile.velocity += pushAway * push;
                }
            }
        }
    }
}

    }
}
