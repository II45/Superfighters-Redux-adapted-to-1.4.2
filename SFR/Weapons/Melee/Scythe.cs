using SFD;
using SFD.Effects;
using SFD.Materials;
using SFD.Objects;
using SFD.Sounds;
using SFD.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SFD.Fire;
using static SFD.GUIConstants;
using static SFD.Objects.ObjectStreetsweeper.StreetsweeperPathFindingPackage;

using static SFD.NetMessage;
using System;
using SFD.Core;
using SFD.Tiles;
using System.Threading;

namespace SFR.Weapons.Melee;

internal sealed class Scythe : MWeapon,IExtendedWeapon
{
    internal Scythe()
    {
        MWeaponProperties weaponProperties = new(108, LanguageHelper.GetText("weapon.melee.Scythe"), 15f, 0f, "MeleeSlash", "MeleeHitSharp", "", "MeleeBlock", "HIT", "MeleeDrawMetal", "WpnScythe", false, WeaponCategory.Melee, false)
        {
            MeleeWeaponType = MeleeWeaponTypeEnum.TwoHanded,
            WeaponMaterial = MaterialDatabase.Get("metal"),
            DurabilityLossOnHitObjects = 8f,
            DurabilityLossOnHitPlayers = 8f,
            DurabilityLossOnHitBlockingPlayers = 4f,
            ThrownDurabilityLossOnHitPlayers = 15f,
            ThrownDurabilityLossOnHitBlockingPlayers = 10f,
            DeflectionDuringBlock =
            {
                DeflectType = DeflectBulletType.Deflect,
                DurabilityLoss = 4f
            },
            DeflectionOnAttack =
            {
                DeflectType = DeflectBulletType.Deflect,
                DurabilityLoss = 4f
            },

            BreakDebris = new[] { "ScytheDebris1", "MetalDebris00A" },
            Handling = MeleeHandlingType.Custom
        };
        MWeaponVisuals weaponVisuals = new();
        weaponVisuals.SetModelTexture("ScytheM");
        weaponVisuals.SetDrawnTexture("ScytheD");
        weaponVisuals.SetSheathedTexture("ScytheS");
        weaponVisuals.SetHolsterTexture("ScytheH");
        weaponVisuals.AnimBlockUpper = "UpperBlockMelee2H";
        weaponVisuals.AnimMeleeAttack1 = "UpperMelee2H1";
        weaponVisuals.AnimMeleeAttack2 = "UpperMelee2H2";
        weaponVisuals.AnimMeleeAttack3 = "UpperMelee2H3";
        weaponVisuals.AnimFullJumpAttack = "FullJumpAttackMelee";
        weaponVisuals.AnimDraw = "UpperDrawMelee";
        weaponVisuals.AnimCrouchUpper = "UpperCrouchMelee2H";
        weaponVisuals.AnimIdleUpper = "UpperIdleMelee2H";
        weaponVisuals.AnimJumpKickUpper = "UpperJumpKickMelee";
        weaponVisuals.AnimJumpUpper = "UpperJumpMelee2H";
        weaponVisuals.AnimJumpUpperFalling = "UpperJumpFallingMelee2H";
        weaponVisuals.AnimKickUpper = "UpperKickMelee2H";
        weaponVisuals.AnimStaggerUpper = "UpperStagger";
        weaponVisuals.AnimRunUpper = "UpperRunMelee2H";
        weaponVisuals.AnimWalkUpper = "UpperWalkMelee2H";
        weaponVisuals.AnimFullLand = "FullLandMelee";
        weaponVisuals.AnimToggleThrowingMode = "UpperToggleThrowing";
        weaponProperties.VisualText = LanguageHelper.GetText("weapon.melee.Scythe");
        SetPropertiesAndVisuals(weaponProperties, weaponVisuals);
        CacheDrawnTextures(new[] { "Extended", "Curved" ,"Shock"});

    }
    public void GetDealtDamage(Player player, float damage)
    {
        TriggerExplosion(player);
    }
    public void OnHit(Player player, Player target)
    {
        if (target.Health.MaxValue > 100f)
        {
            target.DoTakeDamage(target.Health.MaxValue / 5f);
        }
        else
        {
            target.DoTakeDamage(20f);
        }
    
        
    }

    public void OnHitObject(Player player, PlayerHitEventArgs args, SFD.ObjectData obj) 
    {
        obj.DealScriptDamage(50, 0);
        obj.DoTakeDamage(50, SFD.ObjectData.DamageType.Fire, 0);
        obj.Body.SetType(Box2D.XNA.BodyType.Dynamic);
    }

    public static bool Shock;
    public static float time;
    public void Update(Player player, float ms, float realMs) 
    {
        time -= ms;
        if (time<=-30000)
        {
            Shock = true;
        }
    }
    public void DrawExtra(SpriteBatch spritebatch, Player player, float ms)
    {
    }

    public override Texture2D GetDrawnTexture(ref GetDrawnTextureArgs args)
    {
        if (Shock)
        {
            args.Postfix = "Shock";
        }
        return base.GetDrawnTexture(ref args);
    }

    public override Texture2D GetSheathedTexture(ref GetDrawnTextureArgs args)
    {
        if (Shock)
        {
            args.Postfix = "Shock";
        }
        return base.GetSheathedTexture(ref args);
    }
    public static void TriggerExplosion(Player ownerPlayer)
    {
        ownerPlayer.TakeFireDamage(3f);
        if (Shock)
        {
            ownerPlayer.HealAmount(6);
            Shock = false;
            time = 0;
        }
      

    }
    public void BeforeHit(Player player, Player target) { }

    private Scythe(MWeaponProperties rwp, MWeaponVisuals rwv)
    {
        SetPropertiesAndVisuals(rwp, rwv);
    }

    public override MWeapon Copy() => new Scythe(Properties, Visuals)
    {
        Durability =
        {
            CurrentValue = Durability.CurrentValue
        }
    };


    public override bool CustomHandlingOnAttackKey(Player player, bool onKeyEvent) => onKeyEvent && player.CurrentAction is PlayerAction.MeleeAttack1 or PlayerAction.MeleeAttack2 or PlayerAction.MeleeAttack3;
    public override void OnThrowWeaponItem(Player player, ObjectWeaponItem thrownWeaponItem)
    {
        thrownWeaponItem.Body.SetAngularVelocity(thrownWeaponItem.Body.GetAngularVelocity() * 0.9f);
        var linearVelocity = thrownWeaponItem.Body.GetLinearVelocity();
        linearVelocity.X *= 0.7f;
        linearVelocity.Y *= 0.7f;
        thrownWeaponItem.Body.SetLinearVelocity(linearVelocity);
        base.OnThrowWeaponItem(player, thrownWeaponItem);

    }

    public override void Destroyed(Player ownerPlayer)
    {
        SoundHandler.PlaySound("DestroySmall", ownerPlayer.GameWorld);
        EffectHandler.PlayEffect("DestroyWood", ownerPlayer.Position, ownerPlayer.GameWorld);
        Vector2 center = new(ownerPlayer.Position.X, ownerPlayer.Position.Y + 16f);
        ownerPlayer.GameWorld.SpawnDebris(ownerPlayer.ObjectData, center, 8f, new[] { "ScytheDebris1", "MetalDebris00A" });


    }
}