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
    internal sealed class ProjectileShotBow : Projectile, IExtendedProjectile
    {

        public float lifetime;
        public float m_nextTraceSpawn;
        private float rotation;
        private int m_frame;

        internal ProjectileShotBow()
        {
            Visuals = new ProjectileVisuals(Textures.GetTexture("CrossbowBoomBolt011"), Textures.GetTexture("CrossbowBoomBolt011"));
            Properties = new ProjectileProperties(107, 600f, 50f, 25f, 30f, 0f, 30f, 40f, 0.5f)
            {
                PowerupBounceRandomAngle = 0f,
                PowerupFireType = ProjectilePowerupFireType.Fireplosion,
                PowerupTotalBounces = 8,
                CritDamage = 40,
                PowerupFireIgniteValue = 56f

            };
        }

        private ProjectileShotBow(ProjectileProperties projectileProperties, ProjectileVisuals projectileVisuals) : base(projectileProperties, projectileVisuals) { }

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
            Projectile projectile = new ProjectileShotBow(Properties, Visuals);
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

                    if ( PowerupFireActive)
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
            if (GameOwner != GameOwnerEnum.Client)
            {
                player.TakeProjectileDamage(this);
                Material material = player.GetPlayerHitMaterial();
                if (material == null)
                {
                    material = playerObjectData.Tile.Material;
                }
                SoundHandler.PlaySound(material.Hit.Projectile.HitSound, GameWorld);
                EffectHandler.PlayEffect(material.Hit.Projectile.HitEffect, Position, GameWorld);
                SoundHandler.PlaySound("MeleeHitSharp", GameWorld);
                player.Fall();
                Destroy();
                Remove();

            }
        }

        public override void HitObject(ObjectData objectData, ProjectileHitEventArgs e)
        {
            base.HitObject(objectData, e);
            if (GameOwner != GameOwnerEnum.Client && !PowerupBounceActive && !objectData.IsPlayer && objectData.GetCollisionFilter().AbsorbProjectile)
            {
                Destroy();

            }
        }
        public void Destroy()
        {
            Vector2 vector = new(1, 60);
            Vector2 vector2 = new(-21, 60);
            Vector2 vector3 = new(19, 60);
            Vector2 direction = new(0, -1);
            Vector2 direction2 = new(1f, -3f);
            Vector2 direction3 = new(-1, -3);

            if (PowerupFireActive)
            {
                GameWorld.SpawnProjectile(110, Position + vector, direction, 0, SFDGameScriptInterface.ProjectilePowerup.Fire);
                GameWorld.SpawnProjectile(110, Position + vector2, direction2, 0, SFDGameScriptInterface.ProjectilePowerup.Fire);
                GameWorld.SpawnProjectile(110, Position + vector3, direction3, 0, SFDGameScriptInterface.ProjectilePowerup.Fire);
            }
            else
            {
                GameWorld.SpawnProjectile(110, Position + vector, direction);
                GameWorld.SpawnProjectile(110, Position + vector2, direction2);
                GameWorld.SpawnProjectile(110, Position + vector3, direction3);
            }
        }
    }
}
