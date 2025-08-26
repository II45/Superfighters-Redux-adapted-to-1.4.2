using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SFD;
using SFD.Effects;
using SFD.Materials;
using SFD.Objects;
using SFD.Sounds;
using SFR.Helper;
using SFD.Weapons;
using System;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using SFDGameScriptInterface;

namespace SFR.Weapons.Melee;

internal sealed class CodeBat :  MWeapon,IExtendedWeapon
{
    internal CodeBat()
    {
        MWeaponProperties weaponProperties = new(118, LanguageHelper.GetText("weapon.melee.CodeBat"), 20f, 9999f, "MeleeSlash", "MeleeHitBlunt", "HIT_S", "MeleeBlock", "HIT", "MeleeDraw", "WpnCodeBat", true, WeaponCategory.Melee, false)
        {
            MeleeWeaponType = MeleeWeaponTypeEnum.TwoHanded,
            WeaponMaterial = MaterialDatabase.Get("metal"),
            DurabilityLossOnHitObjects = 0f,
            DurabilityLossOnHitPlayers = 0f,
            DurabilityLossOnHitBlockingPlayers = 0f,
            ThrownDurabilityLossOnHitPlayers = 0f,
            ThrownDurabilityLossOnHitBlockingPlayers =0f,
            DamageObjects = 9999f,
            DamagePlayers = 9999f,
            Range = 20f,
            DeflectionDuringBlock =
            {
                DeflectType = DeflectBulletType.Deflect,
                DurabilityLoss = 0f
            },
            DeflectionOnAttack =
            {
                DeflectType = DeflectBulletType.Deflect,
                DurabilityLoss = 0f
            },
            BreakDebris = new[] { "MetalDebris00A", "CrowbarDebris1" }
        };

        MWeaponVisuals weaponVisuals = new();
        weaponVisuals.SetModelTexture("CodeBatM");
        weaponVisuals.SetDrawnTexture("CodeBatD");
        weaponVisuals.SetSheathedTexture("CodeBatM");
        weaponVisuals.AnimBlockUpper = "UpperBlockMeleeFast";
        weaponVisuals.AnimMeleeAttack1 = "UpperMelee1H1Fast";
        weaponVisuals.AnimMeleeAttack2 = "UpperMelee1H2Fast";
        weaponVisuals.AnimMeleeAttack3 = "UpperMelee1H3Fast";
        weaponVisuals.AnimFullJumpAttack = "FullJumpAttackMelee";
        weaponVisuals.AnimDraw = "UpperDrawMeleeSheathed";
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
        weaponProperties.VisualText = LanguageHelper.GetText("weapon.melee.CodeBat");

        SetPropertiesAndVisuals(weaponProperties, weaponVisuals);
    }

    private CodeBat(MWeaponProperties weaponProperties, MWeaponVisuals weaponVisuals)
    {
        SetPropertiesAndVisuals(weaponProperties, weaponVisuals);
    }

    public override MWeapon Copy() => new CodeBat(Properties, Visuals)
    {
        Durability =
        {
            CurrentValue = Durability.CurrentValue
        }
    };
    public void GetDealtDamage(Player player, float damage)
    {

    }
    public void OnHit(Player player, Player target)
    {
        player.HealAmount(999f);
        target.DoTakeDamage(999f);

    }
    public void Update(Player player, float ms, float realMs)
    {


        Microsoft.Xna.Framework.Vector2 vector = player.Position;
        Random random = new();
        vector.X += random.Next(-4, 4);
        vector.Y += random.Next(-2, 2);
        Microsoft.Xna.Framework.Vector2 position = player.GameWorld.GetMouseWorldPosition();
        Microsoft.Xna.Framework.Vector2 direction = new(0f, 0f);
        direction.X += random.NextFloat(-10, 10);
        direction.Y += random.NextFloat(-10f, 10f);
    }

    public void DrawExtra(SpriteBatch spritebatch, Player player, float ms)
    {

    }

    public void OnHitObject(Player player, PlayerHitEventArgs args, SFD.ObjectData obj)
    {
        obj.DealScriptDamage(500, 0);
        obj.DoTakeDamage(500, SFD.ObjectData.DamageType.Fire, 0);
        obj.Body.SetType(Box2D.XNA.BodyType.Dynamic);
        player.HealAmount(999f);
    }
    public override void OnThrowWeaponItem(Player player, ObjectWeaponItem thrownWeaponItem)
    {
        thrownWeaponItem.Body.SetAngularVelocity(thrownWeaponItem.Body.GetAngularVelocity() * 0.9f);
        var linearVelocity = thrownWeaponItem.Body.GetLinearVelocity();
        linearVelocity.X *= 0.9f;
        linearVelocity.Y *= 0.9f;
        thrownWeaponItem.Body.SetLinearVelocity(linearVelocity);
        base.OnThrowWeaponItem(player, thrownWeaponItem);
    }

    public override void Destroyed(Player ownerPlayer)
    {
        SoundHandler.PlaySound("DestroyMetal", ownerPlayer.GameWorld);
        EffectHandler.PlayEffect("DestroyMetal", ownerPlayer.Position, ownerPlayer.GameWorld);
        Microsoft.Xna.Framework.Vector2 center = new(ownerPlayer.Position.X, ownerPlayer.Position.Y + 16f);
        ownerPlayer.GameWorld.SpawnDebris(ownerPlayer.ObjectData, center, 8f, new[] { "MetalDebris00A" });
    }
}