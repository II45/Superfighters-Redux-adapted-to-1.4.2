using Microsoft.Xna.Framework;
using SFD;
using SFD.Effects;
using SFD.Objects;
using SFD.Sounds;
using SFD.Weapons;

namespace SFR.Weapons.Rifles;

internal sealed class SplitGun : RWeapon
{
    private bool _reload;

    internal SplitGun()
    {
        RWeaponProperties rWeaponProperties = new(117, LanguageHelper.GetText("weapon.Rifles.Splitgun"), 1, 25, 4, 2, -1, 95, 0, 1, 108, "ShellSmall", 0.15f, new Vector2(10f, -2f), "MuzzleFlashShotgun", "TommyGun", "TommyGunDraw", "TommyGunReload", "OutOfAmmoHeavy", "WpnSplitGun", false, WeaponCategory.Primary)
        {
            BurstCooldown = 70,
            CooldownAfterPostAction = 150,
            ExtraAutomaticCooldown = 150,
            CursorAimOffset = new Vector2(0f, 1f),
            LazerPosition = new Vector2(12f, -0.5f),
            AimStartSoundID = "PistolAim",
            BreakDebris = new[] { "MetalDebris00A", "ItemDebrisStockWood00", "ItemDebrisShiny00" },
            SpecialAmmoBulletsRefill = 25
        };

        RWeaponVisuals weaponVisuals = new();
        weaponVisuals.SetModelTexture("SplitGunM");
        weaponVisuals.SetDrawnTexture("SplitGunD");
        weaponVisuals.SetSheathedTexture("SplitGunS");
        weaponVisuals.SetThrowingTexture("SplitGunThrowing");
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
        weaponVisuals.AnimDraw = "UpperDrawShotgun";
        weaponVisuals.AnimManualAim = "ManualAimRifle";
        weaponVisuals.AnimManualAimStart = "ManualAimRifleStart";
        weaponVisuals.AnimReloadUpper = "UpperReloadShell";
        weaponVisuals.AnimFullLand = "FullLandHandgun";
        weaponVisuals.AnimToggleThrowingMode = "UpperToggleThrowing";
        rWeaponProperties.VisualText = LanguageHelper.GetText("weapon.Rifles.Splitgun");

        SetPropertiesAndVisuals(rWeaponProperties, weaponVisuals);
        CacheDrawnTextures(new[] { "Pump" });
    }

    private SplitGun(RWeaponProperties weaponProperties, RWeaponVisuals weaponVisuals)
    {
        SetPropertiesAndVisuals(weaponProperties, weaponVisuals);
    }



    public override void OnRecoilEvent(Player player)
    {
        if (player.GameOwner != GameOwnerEnum.Server)
        {
            if (Properties.ShellID != string.Empty && Constants.EFFECT_LEVEL_FULL)
            {
                SpawnUnsyncedShell(player, Properties.ShellID);
            }

            SoundHandler.PlaySound(Properties.BlastSoundID, player.Position, player.GameWorld);
        }
    }


    public override void OnSubAnimationEvent(Player player, AnimationEvent animationEvent, AnimationData animationData, int currentFrameIndex)
    {
        if (player.GameOwner != GameOwnerEnum.Server && animationEvent == AnimationEvent.EnterFrame && animationData.Name == "UpperDrawRifle")
        {
            switch (currentFrameIndex)
            {
                case 1:
                    SoundHandler.PlaySound("Draw1", player.GameWorld);
                    break;
                case 6:
                    SoundHandler.PlaySound("TommyGunDraw", player.GameWorld);
                    break;
            }
        }
    }


    public override void OnReloadAnimationFinished(Player player)
    {
        player.AnimationUpperOverride = new PlayerEmptyBoltActionAnimation();
        _reload = false;
    }
    public override void OnReloadAnimationEvent(Player player, AnimationEvent animEvent, SubAnimationPlayer subAnim)
    {
        if (player.GameOwner != GameOwnerEnum.Server && animEvent == AnimationEvent.EnterFrame)
        {
            if (subAnim.GetCurrentFrameIndex() == 3)
            {
                SoundHandler.PlaySound("ShotgunReload", player.GameWorld);
            }
        }
    }

    public override RWeapon Copy()
    {
        SplitGun SplitGun = new(Properties, Visuals);
        SplitGun.CopyStatsFrom(this);
        SplitGun._reload = _reload;
        return SplitGun;
    }

    public override void OnThrowWeaponItem(Player player, ObjectWeaponItem thrownWeaponItem)
    {
        thrownWeaponItem.Body.SetAngularVelocity(thrownWeaponItem.Body.GetAngularVelocity() * 0.8f);
        var linearVelocity = thrownWeaponItem.Body.GetLinearVelocity();
        linearVelocity.X *= 0.85f;
        linearVelocity.Y *= 0.85f;
        thrownWeaponItem.Body.SetLinearVelocity(linearVelocity);
        base.OnThrowWeaponItem(player, thrownWeaponItem);
    }
}