using System;
using Box2D.XNA;
using Microsoft.Xna.Framework;
using SFD;
using SFD.Effects;
using SFD.Fire;
using SFD.Materials;
using SFD.Objects;
using SFD.Projectiles;
using SFD.Sounds;
using SFD.Tiles;
using SFD.Weapons;
using SFR.Helper;
using SFR.Objects;
using SFR.Sync.Generic;

namespace SFR.Projectiles;

internal sealed class ProjectileJianqi : Projectile, IExtendedProjectile
{
    private float _gravity;
    private int _splits = 2;
    private const float SplitDistance = 32;
    internal ProjectileJianqi()
    {
        Visuals = new ProjectileVisuals(ProjectileDatabase.BulletCommonTexture, ProjectileDatabase.BulletCommonSlowmoTexture);
        Properties = new ProjectileProperties(106, velocity, 5f, 5f, 40f, 0.1f, 9f, 0.2f, 0.5f)
        {
            PowerupBounceRandomAngle = 0f,
            PowerupFireType = ProjectilePowerupFireType.Default,
            PowerupTotalBounces = 0,
            PowerupFireIgniteValue = 560f,
            DodgeChance = 0.1f

        };
    }
    public float velocity = 100f;
    private ProjectileJianqi(ProjectileProperties projectileProperties, ProjectileVisuals projectileVisuals) : base(projectileProperties, projectileVisuals) { }


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
        Projectile projectile = new ProjectileJianqi(Properties, Visuals);
        projectile.CopyBaseValuesFrom(this);
        return projectile;
    }

    public override void Update(float ms)
    {
        if (_splits > 1 && TotalDistanceTraveled > SplitDistance)
        {
            if (GameOwner != GameOwnerEnum.Client)
            {
                var newProj = (ProjectileJianqi)GameWorld.CreateProjectile(106, null, Position, Direction.GetRotatedVector(-0.1), 0);
                newProj._splits = _splits - 1;
                newProj = (ProjectileJianqi)GameWorld.CreateProjectile(106, null, Position, Direction.GetRotatedVector(0.1), 0);
                newProj._splits = _splits - 1;
                _splits = 1;
            }
        }
        if (_splits > 0 && TotalDistanceTraveled > SplitDistance+52)
        {
            if (GameOwner != GameOwnerEnum.Client)
            {
                var newProj = (ProjectileJianqi)GameWorld.CreateProjectile(106, null, Position, Direction.GetRotatedVector(-0.5), 0);
                newProj._splits = _splits - 1;
                    newProj = (ProjectileJianqi)GameWorld.CreateProjectile(106, null, Position, Direction.GetRotatedVector(0.15), 0);
                    newProj._splits = _splits - 1;
                _splits = 0;
            }
        }
    }

    public override void HitPlayer(Player player, ObjectData playerObjectData)
    {
        if (GameOwner != GameOwnerEnum.Client)
        {
            HitFlag = false;
            player.TakeProjectileDamage(this);
            var material = player.GetPlayerHitMaterial() ?? playerObjectData.Tile.Material;
            SoundHandler.PlaySound(material.Hit.Projectile.HitSound, GameWorld);
            EffectHandler.PlayEffect(material.Hit.Projectile.HitEffect, Position, GameWorld);
        }
    }

    public override void HitObject(ObjectData objectData, ProjectileHitEventArgs e)
    {
        base.HitObject(objectData, e);

        if (objectData.Destructable)
        {
            HitFlag = false;
            e.CustomHandled = false;
            e.ReflectionStatus = ProjectileReflectionStatus.None;
        }
    }
}
