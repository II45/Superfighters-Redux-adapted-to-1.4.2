using Box2D.XNA;
using Microsoft.Xna.Framework.Graphics;
using SFD;
using SFD.Effects;
using SFD.Materials;
using SFR.Helper;
using SFD.Sounds;
using SFD.Projectiles;
using Microsoft.Xna.Framework;
using SFD.Weapons;
using SFR.Projectiles;
using static SFD.GUIConstants;
using static SFD.Objects.ObjectStreetsweeper.StreetsweeperPathFindingPackage;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using System;
using SFD.Tiles;

namespace SFR.Weapons.Melee;

internal sealed class Vampireknife : MWeapon, IExtendedWeapon
{

    internal Vampireknife()
    {
        MWeaponProperties weaponProperties = new(114, LanguageHelper.GetText("weapon.melee.Vampireknife"), 9f, 5f, "MeleeSlash", "MeleeHitSharp", "HIT_S", "MeleeBlock", "HIT", "MeleeDrawMetal", "WpnVampireknife", false, WeaponCategory.Melee, false)
        {
            MeleeWeaponType = MeleeWeaponTypeEnum.OneHanded,
            WeaponMaterial = MaterialDatabase.Get("metal"),
            DurabilityLossOnHitObjects = 5f,
            DurabilityLossOnHitPlayers = 9f,
            DurabilityLossOnHitBlockingPlayers = 9f,
            ThrownDurabilityLossOnHitPlayers = 9f,
            ThrownDurabilityLossOnHitBlockingPlayers = 0f,
            VisualText = LanguageHelper.GetText("weapon.melee.Vampireknife"),
            DeflectionDuringBlock =
            {
                DeflectType = DeflectBulletType.Deflect,
                DurabilityLoss = 10f
            },
            DeflectionOnAttack =
            {
                DeflectType = DeflectBulletType.Deflect,
                DurabilityLoss = 10f
            },
            BreakDebris = new[]
            {
                "VampireknifeDebris1",
                "VampireknifeDebris2"
            },
            AI_DamageOutput = DamageOutputType.High

        };

        MWeaponVisuals weaponVisuals = new()
        {
            AnimBlockUpper = "UpperBlockMelee",
            AnimMeleeAttack1 = "UpperMelee1H1",
            AnimMeleeAttack2 = "UpperMelee1H2",
            AnimMeleeAttack3 = "UpperMelee1H3",
            AnimFullJumpAttack = "FullJumpAttackMelee",
            AnimDraw = "UpperDrawMelee",
            AnimCrouchUpper = "UpperCrouchMelee",
            AnimIdleUpper = "UpperIdleMelee",
            AnimJumpKickUpper = "UpperJumpKickMelee",
            AnimJumpUpper = "UpperJumpMelee",
            AnimJumpUpperFalling = "UpperJumpFallingMelee",
            AnimKickUpper = "UpperKickMelee",
            AnimStaggerUpper = "UpperStagger",
            AnimRunUpper = "UpperRunMelee",
            AnimWalkUpper = "UpperWalkMelee",
            AnimFullLand = "FullLandMelee",
            AnimToggleThrowingMode = "UpperToggleThrowing"
        };

        weaponVisuals.SetModelTexture("VampireknifeM");
        weaponVisuals.SetDrawnTexture("VampireknifeD");
        weaponVisuals.SetSheathedTexture("VampireknifeS");
        SetPropertiesAndVisuals(weaponProperties, weaponVisuals);
    }

    private Vampireknife(MWeaponProperties weaponProperties, MWeaponVisuals weaponVisuals) => SetPropertiesAndVisuals(weaponProperties, weaponVisuals);
    public void GetDealtDamage(Player player, float damage)
    {
    }    

    public void OnHit(Player player, Player target)
    {
        TriggerExplosion(player);
    }

    public void OnHitObject(Player player, PlayerHitEventArgs args, ObjectData obj) { }
    public float time;
    public void Update(Player player, float ms, float realMs)
    {   
        time -= ms; 
        if (time <= -2500)
        {
            Vector2 vector2 = new(-1, 9);
            player.GameWorld.SpawnProjectile(106, player.GetGrabWorldPosition()+ vector2, player.AimVector(), 0);
            time = 0;
        }

    }
    public void DrawExtra(SpriteBatch spritebatch, Player player, float ms) 
    {
    }

    public void BeforeHit(Player player, Player target) { }

    public override MWeapon Copy() => new Vampireknife(Properties, Visuals)
    {
        Durability =
        {
            CurrentValue = Durability.CurrentValue
        }
    };

    public static void TriggerExplosion(Player ownerPlayer)
    {
            ownerPlayer.HealAmount(3);
        
        if (ownerPlayer.Health.MaxValue >= 80)
        {
            ownerPlayer.Health.MaxValue -= 2;
        }
        EffectHandler.PlayEffect("BLD", ownerPlayer.Position, ownerPlayer.GameWorld);
    }
    public override void Destroyed(Player player)
    {
        SoundHandler.PlaySound("DestroyMetal", player.GameWorld);
        EffectHandler.PlayEffect("DestroyMetal", player.Position, player.GameWorld);
        Vector2 center = new(player.Position.X, player.Position.Y + 16f);
        player.GameWorld.SpawnDebris(player.ObjectData, center, 8f, new[] { "VampireknifeDebris1", "VampireknifeDebris2" });
        TriggerExplosion(player);
    }
}