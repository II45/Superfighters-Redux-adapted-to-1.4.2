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


using Microsoft.Xna.Framework.Graphics;
using static SFD.GUIConstants;
using SFR.Helper;
using SFD.Weapons;
using System.Runtime;

namespace SFR.Projectiles;

internal sealed class Projectileelectromagnetic_guns : Projectile, IExtendedProjectile
{
    private const float MaxLaserDistance = 500f;
    private const float EffectSpacing = 2.5f; // 粒子间距
    private float _gravity;
    private float _velocity;
    private const float BaseFireIntensity = 3.2f;
    private const int MaxFireLayers = 5;
    private static readonly Vector2[] ExplosionDirections =
    {
    new(-40, 0),  // Left
    new(40, 0),   // Right
    new(0, 40),   // Up
    new(0, -40)   // Down
};
    internal Projectileelectromagnetic_guns()
    {
        Visuals = new ProjectileVisuals(Textures.GetTexture("electromagnetic_guns0"), Textures.GetTexture("electromagnetic_guns1"));
        Properties = new ProjectileProperties(104, 1000f, 50f, 10f, 1000f, 0f, 30f, 40f, 50f)
        {
            PowerupBounceRandomAngle = 0f,
            PowerupFireType = ProjectilePowerupFireType.Fireplosion,
            PowerupTotalBounces = 0,
            PowerupFireIgniteValue = 56f,
            DodgeChance = 0f
            
        };
    }
    private Projectileelectromagnetic_guns(ProjectileProperties projectileProperties, ProjectileVisuals projectileVisuals) : base(projectileProperties, projectileVisuals) { }

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
        Projectile projectile = new Projectileelectromagnetic_guns(Properties, Visuals);
        CopyBaseValuesFrom(this);
        return projectile;
    }
    public float time;

    public override void Draw(SpriteBatch spriteBatch, float ms)
    {
        if (!this.m_lastHitState || !base.HitFlag)
        {
            this.m_lastHitState = base.HitFlag;
            spriteBatch.Draw(base.Visuals.BulletTraceTexture, Camera.ConvertWorldToScreen(base.Position), null, Constants.COLORS.GRAY, base.GetAngle(), new Vector2(2f, (float)(base.Visuals.BulletTraceTexture.Height / 2)), Camera.Zoom*0.0001f, SpriteEffects.None, 0f);

        }
        UpdateLaser();
    }


    public override void Update(float ms)
    {
        _velocity += ms;
        float scaleFactor = System.Math.Min(_velocity / 500f, 1f);
        //Velocity -= Vector2.UnitY * ms * 0.66f * scaleFactor;
        time -= ms;
        if (GameOwner != GameOwnerEnum.Server)
        {
            _gravity -= ms;
            if (_gravity < 0)
            {
            //    EffectHandler.PlayEffect("Electric", Position, GameWorld, Direction.X, Direction.Y); //TR_F
                _gravity = Constants.EFFECT_LEVEL_FULL ? 10f : 20f;
            }
            if (PowerupFireActive)
            {
                EffectHandler.PlayEffect("TR_F", Position, GameWorld);
            //    EffectHandler.PlayEffect("Electric", Position, GameWorld);
                _gravity = Constants.EFFECT_LEVEL_FULL ? 10f : 20f;
            }
        }

        var dir = Direction;
        dir.Normalize();
        var pos = Position;
        var pos2 = pos + dir * 500;
        float num = (int)Vector2.Distance(pos, pos2);
        GameWorld.RayCastResult rays = GameWorld.RayCast(pos, dir, PlayerOwner.AimVector().GetAngle(),num, new GameWorld.RayCastFixtureCheck(PlayerOwner.LazerRayCastCollision), new GameWorld.RayCastPlayerCheck(PlayerOwner.LazerRayCastPlayerCollision));
        if (!rays.TunnelCollision)
        {
            Position = rays.EndPosition;
        }
        if (HitFlag)
        {
            pos2 = Position;
        }
    } 

    public override void HitPlayer(Player player, ObjectData playerObjectData)
    {
        Exp();
        bool flag = base.GameOwner != GameOwnerEnum.Client;
        bool flag2 = flag;
        bool flag3 = flag2;
        bool flag4 = flag3;
        if (flag4)
        {
            base.HitPlayer(player, playerObjectData);
            player.TakeProjectileDamage(this);
            player.Gib();
            Material material = player.GetPlayerHitMaterial() ?? playerObjectData.Tile.Material;
            SoundHandler.PlaySound(material.Hit.Projectile.HitSound, base.GameWorld);
            EffectHandler.PlayEffect(material.Hit.Projectile.HitEffect, base.Position, base.GameWorld);
            SoundHandler.PlaySound("MeleeHitSharp", base.GameWorld);
            base.Remove();
            EffectHandler.PlayEffect("CAM_S", Position, GameWorld, 20f, 600f, false);
            this.Fire(playerObjectData);
            SoundHandler.PlaySound("Boom", Position, GameWorld);
        }
    }

    public override void HitObject(ObjectData objectData, ProjectileHitEventArgs e)
    {
        base.HitObject(objectData, e);
        bool flag = base.GameOwner != GameOwnerEnum.Client && !base.PowerupBounceActive && !objectData.IsPlayer && objectData.GetCollisionFilter().AbsorbProjectile;
        bool flag2 = flag;
        bool flag3 = flag2;
        bool flag4 = flag3;
        if (flag4)
        {
            objectData.ChangeBodyType(BodyType.Dynamic);
            base.HitObject(objectData, e);
            Exp();
            
            Fire(objectData);
            
            EffectHandler.PlayEffect("CAM_S", Position, GameWorld, 20f, 600f, false);
            SoundHandler.PlaySound("Boom", Position, GameWorld);
        }
    }
    private void UpdateLaser()
    {

        var direction = Vector2.Normalize(Direction);
        var startPos = Position;


        var rayResult = GameWorld.RayCast(
            startPos,
            direction,
            PlayerOwner.AimVector().GetAngle(),
            MaxLaserDistance,
            new GameWorld.RayCastFixtureCheck(PlayerOwner.LazerRayCastCollision),
            new GameWorld.RayCastPlayerCheck(PlayerOwner.LazerRayCastPlayerCollision));

        var endPos = rayResult.TunnelCollision ? rayResult.EndPosition : startPos + direction * MaxLaserDistance;


        GenerateLaserEffects(startPos, endPos);
    }

    private void GenerateLaserEffects(Vector2 start, Vector2 end)
    {

        var distance = Vector2.Distance(start, end);
        var effectCount = (int)(distance / EffectSpacing);
        var step = (end - start) / effectCount;


        for (int i = 0; i < effectCount; i++)
        {
            var pos = start + step * i;


            if (i % 2 == 0)
            {
                EffectHandler.PlayEffect("TR_B", pos, GameWorld);
            }
            else
            {
                EffectHandler.PlayEffect("GLM", pos, GameWorld);
            }


            if (i == effectCount - 1)
            {
                EffectHandler.PlayEffect("Electric", pos, GameWorld);
            }
        }
    }
    public void Fire(ObjectData target)
    {
        var velocity = target.Body.GetAverageLinearVelocity();
        var velocityDir = Vector2.Normalize(velocity);
        var fireOrigin = target.Body.GetPosition() - velocityDir * 0.3f;
        var scaledVelocity = velocity * 0.04f;

        for (int layer = 1; layer <= MaxFireLayers; layer++)
        {
            var (radius, intensity, nodes) = GetFireParameters(layer);
            GameWorld.FireGrid.AddFireNodes(
                fireOrigin,
                radius,
                intensity,
                nodes,
                FireNodeTypeEnum.Molotov,
                scaledVelocity);
        }
    }

    private (float radius, float intensity, int nodes) GetFireParameters(int layer)
    {
        return layer switch
        {
            1 => (1f, 0.5f, 20),
            2 => (2f, 1f, 20),
            3 => (3f, 2f, 30),
            4 => (4f, 3f, 40),
            5 => (5f, BaseFireIntensity, 50),
            _ => throw new ArgumentOutOfRangeException(nameof(layer))
        };
    }
    public void Exp()
    {

        GameWorld.TriggerExplosion(Position - Direction * 2f, 100f, true);

        foreach (var dir in ExplosionDirections)
        {
            GameWorld.TriggerExplosion(Position + dir, 100f, true);
        }
    }
}