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
using static SFD.GUIConstants;
using static SFD.Objects.ObjectStreetsweeper.StreetsweeperPathFindingPackage;

namespace SFR.Projectiles
{
    internal sealed class ProjectilAtomicgun : Projectile, IExtendedProjectile
    {

        public float lifetime;
        public float m_nextTraceSpawn;
        private float rotation;
        private int m_frame;

        internal ProjectilAtomicgun()
        {
            Visuals = new ProjectileVisuals(Textures.GetTexture("BulletBarrett"), Textures.GetTexture("BulletBarrett"));
            Properties = new ProjectileProperties(111, 250f, 50f, 1f, 20f, 0f, 0f, 0.3f, 0.6f)
            {
                PowerupBounceRandomAngle = 0f,
                PowerupFireType = ProjectilePowerupFireType.Fireplosion,
                PowerupTotalBounces = 8,
                DodgeChance = 0f,
                PowerupFireIgniteValue = 56f

            };
        }

        private ProjectilAtomicgun(ProjectileProperties projectileProperties, ProjectileVisuals projectileVisuals) : base(projectileProperties, projectileVisuals) { }

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
            Projectile projectile = new ProjectilAtomicgun(Properties, Visuals);
            projectile.CopyBaseValuesFrom(this);
            return projectile;
        }

        public override void Update(float ms)
        {
            lifetime += ms;
            if (GameOwner != GameOwnerEnum.Server)
            {

                if (PowerupFireActive)
                {
                    EffectHandler.PlayEffect("TR_S", Position, GameWorld);
                    EffectHandler.PlayEffect("TR_F", Position, GameWorld);
                }
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
            Exp();
            Effect();
        }

        public override void HitObject(ObjectData objectData, ProjectileHitEventArgs e)
        {
            base.HitObject(objectData, e);
            if (GameOwner != GameOwnerEnum.Client && !PowerupBounceActive && !objectData.IsPlayer && objectData.GetCollisionFilter().AbsorbProjectile)
            {

                Exp();
                Effect();
            }

        }
        public void Exp()
        {
            GameWorld.TriggerExplosion(Position - Direction * 2f, 1f, true);
            GameWorld.TriggerExplosion(Position - Direction * 2f, 1f, true);
            GameWorld.TriggerExplosion(Position - Direction * 2f, 1f, true);

        }
        public void Effect()
        {
            Vector2 Up = new(0, 30);
            Vector2 Left = new(-30, 0);
            Vector2 Down = new(0, -30);
            Vector2 Right = new(30, 0);
            EffectHandler.PlayEffect("Electric",Position , GameWorld);
            EffectHandler.PlayEffect("Electric", Position + Up, GameWorld);
            EffectHandler.PlayEffect("Electric", Position + Left, GameWorld);
            EffectHandler.PlayEffect("Electric", Position + Down, GameWorld);
            EffectHandler.PlayEffect("Electric", Position + Right, GameWorld);
        }
        
    }
}
