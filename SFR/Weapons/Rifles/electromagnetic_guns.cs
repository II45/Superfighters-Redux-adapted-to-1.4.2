using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SFD;
using SFD.Objects;
using SFD.Sounds;
using SFD.Weapons;

namespace SFR.Weapons.Rifles;

internal sealed class electromagnetic_guns : RWeapon
{
    private const int RevUpRounds = 50;


    private int _revUpCurrent;


    internal electromagnetic_guns()
    {
        RWeaponProperties weaponProperties = new(111, LanguageHelper.GetText("weapon.Rifles.elecromagnetic") , 1, 1, 0, 1, -1, 400, 0, 1, 104, "ShellSmall", 0.0001f, new Vector2(15f, 1f), "MuzzleFlashL", "GLauncher", "GLauncherDraw", "GLauncherReload", "OutOfAmmoHeavy", "WpnElectromagneticGuns", false, WeaponCategory.Primary)
        {
            CursorAimOffset = new Vector2(0f, 1f),
            LazerPosition = new Vector2(14f, -0.5f),
            AimStartSoundID = "PistolAim",
            BreakDebris = new[]
            {
                "MetalDebris00A",
                "ItemDebrisShiny00"
            },
            SpecialAmmoBulletsRefill = 1,
            AI_DamageOutput = DamageOutputType.High,
            AI_EffectiveRange = 80,
            AI_MaxRange = 200,
            CanUseBouncingBulletsPowerup = false
            
        };

        RWeaponVisuals weaponVisuals = new();
        weaponVisuals.SetModelTexture("electromagnetic_gunsM");
        weaponVisuals.SetDrawnTexture("electromagnetic_gunsD");
        weaponVisuals.SetSheathedTexture("electromagnetic_gunsS");
        weaponVisuals.SetThrowingTexture("electromagnetic_gunsThrowing");
        weaponVisuals.AnimIdleUpper = "UpperIdleRifle";
        weaponVisuals.AnimCrouchUpper = "UpperCrouchRifle";
        weaponVisuals.AnimJumpKickUpper = "UpperJumpKickRifle";
        weaponVisuals.AnimJumpUpper = "UpperJumpRifle";
        weaponVisuals.AnimJumpUpperFalling = "UpperJumpFallingRifle";
        weaponVisuals.AnimKickUpper = "UpperKickRifle";
        weaponVisuals.AnimStaggerUpper = "UpperStaggerHandgun";
        weaponVisuals.AnimRunUpper = "UpperRunRifle";
        weaponVisuals.AnimWalkUpper = "UpperWalkRifle";
        weaponVisuals.AnimUpperHipfire = "UpperHipfireRifle";
        weaponVisuals.AnimFireArmLength = 2f;
        weaponVisuals.AnimDraw = "UpperDrawRifle";
        weaponVisuals.AnimManualAim = "ManualAimRifle";
        weaponVisuals.AnimManualAimStart = "ManualAimRifleStart";
        weaponVisuals.AnimReloadUpper = "UpperReload";
        weaponVisuals.AnimFullLand = "FullLandHandgun";
        LazerUpgrade = 1;
        weaponVisuals.AnimToggleThrowingMode = "UpperToggleThrowing";
        weaponProperties.VisualText = LanguageHelper.GetText("weapon.Rifles.elecromagnetic");

        SetPropertiesAndVisuals(weaponProperties, weaponVisuals);
        CacheDrawnTextures(new[] { "H" });
    }

    private electromagnetic_guns(RWeaponProperties weaponProperties, RWeaponVisuals weaponVisuals)
    {
        SetPropertiesAndVisuals(weaponProperties, weaponVisuals);
    }
    public override RWeapon Copy()
    {
        electromagnetic_guns electromagnetic_guns = new(Properties, Visuals);
        electromagnetic_guns.CopyStatsFrom(this);
        return electromagnetic_guns;
    }

    public override void OnReloadAnimationEvent(Player player, AnimationEvent animEvent, SubAnimationPlayer subAnim)
    {
        if (player.GameOwner != GameOwnerEnum.Server && animEvent == AnimationEvent.EnterFrame)
        {
            if (subAnim.GetCurrentFrameIndex() == 1)
            {
                SpawnMagazine(player, "MagDrum", new Vector2(-8f, -3f));
                SoundHandler.PlaySound("MagnumReloadEnd", player.Position, player.GameWorld);
            }
            else if (subAnim.GetCurrentFrameIndex() == 4)
            {
                SoundHandler.PlaySound("PistolReload", player.Position, player.GameWorld);
            }
        }
    }

    public override void OnSubAnimationEvent(Player player, AnimationEvent animationEvent, AnimationData animationData, int currentFrameIndex)
    {
        if (player.GameOwner != GameOwnerEnum.Server && animationEvent == AnimationEvent.EnterFrame && animationData.Name == "UpperDrawRifle")
        {
            if (currentFrameIndex == 1)
            {
                SoundHandler.PlaySound("Draw1", player.GameWorld);
            }
            if (currentFrameIndex == 6)
            {
                SoundHandler.PlaySound("GLauncherDraw", player.GameWorld);
            }
        }
    }

    public override void OnThrowWeaponItem(Player player, ObjectWeaponItem thrownWeaponItem)
    {
        thrownWeaponItem.Body.SetAngularVelocity(thrownWeaponItem.Body.GetAngularVelocity() * 0.8f);
        var linearVelocity = thrownWeaponItem.Body.GetLinearVelocity();
        linearVelocity.X *= 0.7f;
        linearVelocity.Y *= 0.7f;
        thrownWeaponItem.Body.SetLinearVelocity(linearVelocity);
        base.OnThrowWeaponItem(player, thrownWeaponItem);
    }
}