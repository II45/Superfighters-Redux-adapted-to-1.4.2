using System;
using System.Threading;
using Box2D.XNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SFD;
using SFD.Effects;
using SFD.Materials;
using SFD.Objects;
using SFD.Projectiles;
using SFD.Sounds;
using SFD.Tiles;
using SFR.Helper;
using SFR.Objects;
using SFR.Sync.Generic;
using static SFD.Objects.ObjectStreetsweeper.StreetsweeperPathFindingPackage;

namespace SFR.Projectiles
{
    internal sealed class ProjectileBowFix : Projectile, IExtendedProjectile
    {

        public float lifetime;
        public float m_nextTraceSpawn;
        private float rotation;
        private int m_frame;

        internal ProjectileBowFix()
        {
            Visuals = new ProjectileVisuals(Textures.GetTexture("BowArrow"), Textures.GetTexture("BowArrow"));
            Properties = new ProjectileProperties(110, 500f, 50f, 10f, 30f, 0f, 30f, 40f, 0.5f)
            {
                PowerupBounceRandomAngle = 0f,
                PowerupFireType = ProjectilePowerupFireType.Fireplosion,
                PowerupTotalBounces = 8,
                CritDamage = 40,
                PowerupFireIgniteValue = 56f

            };
        }

        private ProjectileBowFix(ProjectileProperties projectileProperties, ProjectileVisuals projectileVisuals) : base(projectileProperties, projectileVisuals) { }

        public override float SlowmotionFactor => 1f - (1f - GameWorld.SlowmotionHandler.SlowmotionModifier) * 0.5f;

        public bool OnHit(Projectile projectile, ProjectileHitEventArgs e, ObjectData objectData) => true;

        public bool OnExplosiveHit(Projectile projectile, ProjectileHitEventArgs e, ObjectExplosive objectData)
        {
            ObjectDataMethods.ApplyProjectileHitImpulse(objectData, projectile, e);
            return false;
        }

        public bool OnExplosiveBarrelHit(Projectile projectile, ProjectileHitEventArgs e, ObjectBarrelExplosive objectData)
        {
            ObjectDataMethods.ApplyProjectileHitImpulse(objectData, projectile, e);
            return false;
        }

        public override Projectile Copy()
        {
            Projectile projectile = new ProjectileBowFix(Properties, Visuals);
            projectile.CopyBaseValuesFrom(this);
            return projectile;
        }

        public override void Update(float ms)
        {
            lifetime += ms;
            float num = System.Math.Min(lifetime / 500f, 1f);
            Velocity -= Vector2.UnitY * ms * 0.66f * num;
            if (GameOwner != GameOwnerEnum.Server)
            {

                if (PowerupFireActive)
                {
                    EffectHandler.PlayEffect("TR_S", Position, GameWorld);
                    EffectHandler.PlayEffect("TR_F", Position, GameWorld);
                }
                EffectHandler.PlayEffect("F_S", Position, GameWorld);
                m_nextTraceSpawn = (Constants.EFFECT_LEVEL_FULL ? 10f : 20f);

            }
        }

        public override void HitPlayer(Player player, ObjectData playerObjectData)
        {
            if (base.GameOwner != GameOwnerEnum.Client)
            {
                player.TakeProjectileDamage(this);
                Material material = player.GetPlayerHitMaterial();
                if (material == null)
                {
                    material = playerObjectData.Tile.Material;
                }
                SoundHandler.PlaySound(material.Hit.Projectile.HitSound, base.GameWorld);
                EffectHandler.PlayEffect(material.Hit.Projectile.HitEffect, base.Position, base.GameWorld);
                SoundHandler.PlaySound("MeleeHitSharp", base.GameWorld);
            }
        }

        public override void HitObject(ObjectData objectData, ProjectileHitEventArgs e)
        {
            base.HitObject(objectData, e);
            if (GameOwner != GameOwnerEnum.Client && !PowerupBounceActive && !objectData.IsPlayer && objectData.GetCollisionFilter().AbsorbProjectile)
            {

                HitFlag = false;
                e.CustomHandled = true;
                e.ReflectionStatus = ProjectileReflectionStatus.None;
            }
        }

    }
}
