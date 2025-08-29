using System;
using System.Threading;
using Box2D.XNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SFD;
using SFD.Effects;
using SFD.Objects;
using SFD.Projectiles;
using SFD.Sounds;
using SFD.Tiles;
using SFR.Fighter;
using SFR.Helper;
using SFR.Objects;
using SFR.Sync.Generic;

namespace SFR.Projectiles;

internal sealed class ProjectileCrossBowBoom : Projectile, IExtendedProjectile
{
    private float _gravity;
    private float _velocity;

    internal ProjectileCrossBowBoom()
    {
        Visuals = new ProjectileVisuals(Textures.GetTexture("CrossbowBoomBolt011"), Textures.GetTexture("CrossbowBoomBolt011"));
        Properties = new ProjectileProperties(103, 800f, 50f, 20f, 100f, 0.01f, 30f, 40f, 0.5f)
        {
            PowerupBounceRandomAngle = 0f,
            PowerupFireType = ProjectilePowerupFireType.Fireplosion,
            PowerupTotalBounces = 8,
            CritDamage = 40,
            PowerupFireIgniteValue = 56f,
            DodgeChance = 2f
        };
    }

    private ProjectileCrossBowBoom(ProjectileProperties projectileProperties, ProjectileVisuals projectileVisuals) : base(projectileProperties, projectileVisuals) { }

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
        Projectile projectile = new ProjectileCrossBowBoom(Properties, Visuals);
        projectile.CopyBaseValuesFrom(this);
        return projectile;
    }

    public override void Update(float ms)
    {
        _velocity += ms;
        float scaleFactor = System.Math.Min(_velocity / 500f, 1f);
        Velocity -= Vector2.UnitY * ms * 0.66f * scaleFactor;
        if (GameWorld.GameOwner != GameOwnerEnum.Server)
        {
            _gravity -= ms;
            if (_gravity < 0&&!PowerupFireActive)
            {
                EffectHandler.PlayEffect("TR_F", Position, GameWorld);
                _gravity = Constants.EFFECT_LEVEL_FULL ? 10f : 20f;
            }
            if (PowerupFireActive)
            {
                EffectHandler.PlayEffect("FIRE", Position, GameWorld);
                _gravity = Constants.EFFECT_LEVEL_FULL ? 10f : 20f;
            }

        }
        Properties.CritChance += ms/10000;


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
            if (PowerupFireActive)
            {
                Boom();
            }
            else
            {
                var data = (ObjectCrossbowBoomBolt)GameWorld.IDCounter.NextObjectData("CrossbowBoomBolt011");
                SpawnObjectInformation spawnObject = new(data, Position, -GetAngle(), 1, Vector2.Zero, 0);
                data.Timer = GameWorld.ElapsedTotalGameTime + 10000;
                var body = GameWorld.CreateTile(spawnObject);
                body.SetType(BodyType.Static);
                data.ApplyPlayerBolt(player);
                data.EnableUpdateObject();
                if (GameOwner == GameOwnerEnum.Server)
                {
                    GenericData.SendGenericDataToClients(new GenericData(DataType.Crossbow, new[] { SyncFlag.NewObjects }, data.ObjectID, player.ObjectID, data.Timer));
                }
            }
            Remove();
            
        }
    }

    public override void HitObject(ObjectData objectData, ProjectileHitEventArgs e)
    {
        base.HitObject(objectData, e);
        if (GameOwner != GameOwnerEnum.Client && !PowerupBounceActive && !objectData.IsPlayer && objectData.GetCollisionFilter().AbsorbProjectile)
        {
            Remove();
            if (PowerupFireActive)
            {
                Boom();
            }
            else
            {
                var data = (ObjectCrossbowBoomBolt)ObjectData.CreateNew(new(GameWorld.IDCounter.NextID(), 0, 0, "CrossbowBoomBolt000", GameOwner));
                _ = GameWorld.CreateTile(new(data, Position, -GetAngle(), 1, objectData.LocalRenderLayer, objectData.GetLinearVelocity(), 0));
                data.Timer = GameWorld.ElapsedTotalGameTime + 15000;
                data.EnableUpdateObject();
                data.FilterObjectId = objectData.BodyID;
                if (objectData.IsStatic)
                {
                    data.ChangeBodyType(BodyType.Static);
                }

            }
        }
    }
    public void Boom()
    {
        GameWorld.TriggerExplosion(Position, 25f, true);
    }
}