using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SFD;
using SFD.Effects;
using SFD.Objects;
using SFD.Projectiles;
using SFD.Sounds;
using SFD.Tiles;
using SFR.Fighter;
using SFR.Helper;
using static SFD.Objects.ObjectStreetsweeper.StreetsweeperPathFindingPackage;

namespace SFR.Projectiles;

internal sealed class ProjectilesPulse : Projectile
{
    private float _gravity;
    private float _velocity = random.Next(1,4);
    private int _time;
    private float _lifetime;

    internal ProjectilesPulse()
    {
        Visuals = new ProjectileVisuals(Textures.GetTexture("Pulse"), Textures.GetTexture("Pulse"));
        Properties = new ProjectileProperties(109, 1800f, 150f, 25f, 85f, 0.58f, 39f, 70f, 0.5f)
        {
            PowerupBounceRandomAngle = 0f,
            PowerupFireType = ProjectilePowerupFireType.Default,
            PowerupTotalBounces = 45,
            PowerupFireIgniteValue = 56f
        };
    }
    
    private ProjectilesPulse(ProjectileProperties projectileProperties, ProjectileVisuals projectileVisuals) : base(projectileProperties, projectileVisuals) { }

    public override float SlowmotionFactor => 1f - (1f - GameWorld.SlowmotionHandler.SlowmotionModifier) * 0.5f;

    public override Projectile Copy()
    {
        ProjectilesPulse projectile = new(Properties, Visuals);
        projectile.CopyBaseValuesFrom(this);
        return projectile;
    }

    public override void Draw(SpriteBatch spriteBatch, float ms)
    {
        float frameDuration = 50f; 
        int frameCount = 4; 
        int currentFrame = (int)(_lifetime / frameDuration) % frameCount;
        _time = currentFrame; 
        if (_lifetime >= frameCount * frameDuration)
        {
            _lifetime %= frameCount * frameDuration; //循环播放
        }
        if (!this.m_lastHitState || !base.HitFlag)
            {
                this.m_lastHitState = base.HitFlag;
                spriteBatch.Draw(
                    base.Visuals.BulletTraceTexture,
                    Camera.ConvertWorldToScreen(base.Position),
                    new Rectangle?(new Rectangle((int)_time*93, 0, 93, 20)),
                    Constants.COLORS.GRAY,
                    base.GetAngle(),
                    new Vector2(2f, (float)(base.Visuals.BulletTraceTexture.Height / 2)),
                    Camera.Zoom*0.3f,
                    SpriteEffects.None,
                    0f
                );
            } 
    }
    public override void Update(float ms)
    {
        _lifetime += ms;
        EffectHandler.PlayEffect("GLM", Position, GameWorld);
        if (PowerupFireActive)
        {
            EffectHandler.PlayEffect("TR_F", Position, GameWorld);
            _gravity = Constants.EFFECT_LEVEL_FULL ? 10f : 20f;
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
            player.Fall();
            player.Energy.CurrentValue = -100f;
            player.Energy.Fullness = -100f;
            player.DropWeaponItem(player.CurrentWeaponDrawn,true);
            EffectHandler.PlayEffect("Electric", Position, GameWorld);
            HitFlag = false;
        }
    }

    public override void HitObject(ObjectData objectData, ProjectileHitEventArgs e)
    {
        base.HitObject(objectData, e);
        objectData.SetMaxFire();
       
        objectData.DealScriptDamage(99f);
        if (objectData.Destructable)
        {
            HitFlag = false;
            e.CustomHandled = true;
            e.ReflectionStatus = ProjectileReflectionStatus.None;
        }
        EffectHandler.PlayEffect("Electric", Position, GameWorld);
        int num = random.Next(1, 51);
        if (!objectData.Destructable && num == 7)
        {
            objectData.Destroy();
        }
    }

    public static Random random = new();


}