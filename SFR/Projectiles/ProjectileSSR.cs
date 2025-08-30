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

internal sealed class ProjectileSSR : Projectile
{
    private float _gravity;
    private int _time;
    private float _lifetime;

    internal ProjectileSSR()
    {
        Visuals = new ProjectileVisuals(Textures.GetTexture("SSR"), Textures.GetTexture("SSR"));
        Properties = new ProjectileProperties(113, 1300f, 150f, 12f, 30f, 0.48f, 20f, 40f, 0.5f)
        {
            PowerupBounceRandomAngle = 0f,
            PowerupFireType = ProjectilePowerupFireType.Default,
            PowerupTotalBounces = 6,
            PowerupFireIgniteValue = 56f
        };
    }

    private ProjectileSSR(ProjectileProperties projectileProperties, ProjectileVisuals projectileVisuals) : base(projectileProperties, projectileVisuals) { }

    public override float SlowmotionFactor => 1f - (1f - GameWorld.SlowmotionHandler.SlowmotionModifier) * 0.5f;

    public override Projectile Copy()
    {
        ProjectileSSR projectile = new(Properties, Visuals);
        projectile.CopyBaseValuesFrom(this);
        return projectile;
    }


}