using System;
using Microsoft.Xna.Framework;
using SFD;
using SFD.Effects;
using SFD.Projectiles;
using SFD.Sounds;
using SFD.Tiles;
using SFR.Helper;
using static SFD.Objects.ObjectStreetsweeper.StreetsweeperPathFindingPackage;

namespace SFR.Projectiles;

internal sealed class ProjectileSplit : Projectile
{
    private float _gravity;
    private float _velocity;
    private float _time;

    internal ProjectileSplit()
    {
        Visuals = new ProjectileVisuals(Textures.GetTexture("ProjectileFlintlock"), Textures.GetTexture("ProjectileFlintlock"));
        Properties = new ProjectileProperties(108, 700f, 50f, 4f, 6f, 0.1f, 3.5f, 20f, 0.5f)
        {
            PowerupBounceRandomAngle = 0f,
            PowerupFireType = ProjectilePowerupFireType.Fireplosion,
            PowerupTotalBounces = 3,
            PowerupFireIgniteValue = 30f
        };
    }

    private ProjectileSplit(ProjectileProperties projectileProperties, ProjectileVisuals projectileVisuals) : base(projectileProperties, projectileVisuals) { }

    public override float SlowmotionFactor => 1f - (1f - GameWorld.SlowmotionHandler.SlowmotionModifier) * 0.5f;

    public override Projectile Copy()
    {
        ProjectileSplit projectile = new(Properties, Visuals);
        projectile.CopyBaseValuesFrom(this);
        return projectile;
    }

    public override void Update(float ms)
    {
        _time -= ms;
        if (GameOwner != GameOwnerEnum.Server)
        {
            _gravity -= ms;
            if (_gravity <= 0f)
            {
                if (Constants.EFFECT_LEVEL_FULL)
                {
                    EffectHandler.PlayEffect("CSW", Position, GameWorld); 
                }

                if (PowerupFireActive)
                {
                    EffectHandler.PlayEffect("TR_F", Position, GameWorld);
                }

                _gravity = Constants.EFFECT_LEVEL_FULL ? 15f : 30f;
            }
        }
        if (_time <= -350f)
        {
            Split();
            EffectHandler.PlayEffect("TR_F", Position, GameWorld);
            GameWorld.SpawnProjectile(101, Position - Direction * 2f, -Direction, 10);
            Remove();
        }
    }


    public override void HitPlayer(Player player, ObjectData playerObjectData)
    {
        if (GameOwner != GameOwnerEnum.Client)
        {
            player.TakeProjectileDamage(this);
            var material = player.GetPlayerHitMaterial() ?? playerObjectData.Tile.Material;
            SoundHandler.PlaySound(material.Hit.Projectile.HitSound, GameWorld);
            EffectHandler.PlayEffect(material.Hit.Projectile.HitEffect, Position, GameWorld);
            SoundHandler.PlaySound("MeleeHitSharp", GameWorld);

        }
        HitFlag = false;
    }

    public override void HitObject(ObjectData objectData, ProjectileHitEventArgs e)
    {
        base.HitObject(objectData, e);
        if (objectData.Destructable)
        {
            HitFlag = false;
            e.CustomHandled = true;
            e.ReflectionStatus = ProjectileReflectionStatus.None;
        }
    }

    public Random random = new();
    public  void Split()
    {
        int num = 6;
        //double ang = random.NextDouble(1d,10d);
       
        for (int i = 0; i < num; i++)
        {
            double angle = 2f * System.Math.PI / num * i ;
            double cosAngle = System.Math.Cos(angle);
            double sinAngle = System.Math.Sin(angle);
            Vector2 vec = new(0, 10);
            Vector2 direction = new((float)(cosAngle * vec.X - sinAngle * vec.Y), (float)(sinAngle * vec.X + cosAngle * vec.Y));

            if (PowerupFireActive)
            {
                GameWorld.SpawnProjectile(101, Position - Direction * 2f, direction, 10,SFDGameScriptInterface.ProjectilePowerup.Fire);
            }
             
            if (PowerupBounceActive)
            {
                GameWorld.SpawnProjectile(101, Position - Direction * 2f, direction, 10, SFDGameScriptInterface.ProjectilePowerup.Bouncing);

            }
            if (!PowerupBounceActive && !PowerupFireActive)
            {
                GameWorld.SpawnProjectile(101, Position - Direction * 2f, direction, 10);
            }


        }
    }


}