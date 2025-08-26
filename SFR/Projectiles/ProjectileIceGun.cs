using System;
using System.Collections.Generic;
using SFD;
using SFD.Effects;
using SFD.Objects;
using SFD.Projectiles;
using SFD.Tiles;
using SFDGameScriptInterface;
using SFR.Helper;
using SFR.Sync.Generic;

namespace SFR.Projectiles;

internal sealed class ProjectileIceGun : Projectile, IExtendedProjectile
{
    private float Time;
    Random Random = new();
    bool ice;
    internal ProjectileIceGun()
    {
        Visuals = new ProjectileVisuals(Textures.GetTexture("BulletCommon"), Textures.GetTexture("BulletCommonSlowmo"));
        Properties = new SFD.Projectiles.ProjectileProperties(112, 1200f, 5f, 2f, 2f, 0f, 2f, 0.1f, 0.0f)
        {
            PowerupBounceRandomAngle = 0f,
            PowerupFireType = ProjectilePowerupFireType.Default,
            PowerupFireIgniteValue = 100f,
            PowerupTotalBounces = 10
        }; 
    }

    private ProjectileIceGun(SFD.Projectiles.ProjectileProperties projectileProperties, ProjectileVisuals projectileVisuals) : base(projectileProperties, projectileVisuals) { }

    public override Projectile Copy()
    {
        ProjectileIceGun projectile = new(Properties, Visuals);
        projectile.CopyBaseValuesFrom(this);
        return projectile;
    }

    public override void Update(float ms)
    {

        
    }
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

    public override void HitPlayer(Player player, ObjectData playerObjectData)
    {
        base.HitPlayer(player, playerObjectData);

        if (!PowerupFireActive)
        {
            var extendedPlayer = player.GetExtension();
            extendedPlayer.Ice = true;
            player.GetHitAndStunned(950f);
        }
        else
            player.Fall();

    }

    public override void HitObject(ObjectData objectData, ProjectileHitEventArgs e)
    {
        base.HitObject(objectData, e);
        int A = Random.Next(1,20) ;
        if (A == 2)
        {
            objectData.SetInitialBodyType(SpawnObjectInformation.SpawnTypeValue.Static);
        }

    }
}