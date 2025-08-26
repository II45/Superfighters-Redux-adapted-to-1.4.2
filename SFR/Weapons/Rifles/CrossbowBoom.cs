using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SFD;
using SFD.Objects;
using SFD.Sounds;
using SFD.Tiles;
using SFD.Weapons;
using System;

namespace SFR.Weapons.Rifles;

internal sealed class CrossbowBoom : RWeapon
{
    private static Texture2D _textureSEmpty = Textures.GetTexture("CrossbowBoomSEmpty");
    private static Texture2D _textureDrawEmpty = Textures.GetTexture("CrossbowBoomDDrawBackEmpty");

    internal CrossbowBoom()
    {
        RWeaponProperties weaponProperties = new(110, LanguageHelper.GetText("weapon.Rifles.CrossbowBoom") , 1, 1, 8, 8, -1, 1000, 200, 1, 103, string.Empty, 0.05f, new Vector2(1f, -0.5f), string.Empty, "BowShoot", "GrenadeDraw", "GrenadeDraw", "OutOfAmmoLight", "WpnCrossbowBoom", false, WeaponCategory.Primary)
        {
            SpecialAmmoBulletsRefill = 4,
            AI_DamageOutput = DamageOutputType.High,
            AI_GravityArcingEffect = 0.66f,
            AI_MaxRange = 500f,
            LazerPosition = new Vector2(1f, -1.5f),
            CursorAimOffset = new Vector2(0f, 2f),
            AimStartSoundID = "Draw1",
            BreakDebris = new string[] { "ItemDebrisShiny00", "ItemDebrisShiny01" },
            ExtraAutomaticCooldown = 0,
            CooldownAfterPostAction = 250,
            ReloadPostCooldown = 0f,
            MaxMagsInWeapon = 1,
            MaxCarriedSpareMags = 4,
            StartMags = 4,
            MaxRoundsInMag = 1,
            VisualText = LanguageHelper.GetText("weapon.Rifles.CrossbowBoom")
        };

        RWeaponVisuals weaponVisuals = new()
        {
            AnimIdleUpper = "UpperIdleRifle",
            AnimCrouchUpper = "UpperCrouchRifle",
            AnimJumpKickUpper = "UpperJumpKickRifle",
            AnimJumpUpper = "UpperJumpRifle",
            AnimJumpUpperFalling = "UpperJumpFallingRifle",
            AnimKickUpper = "UpperKickRifle",
            AnimStaggerUpper = "UpperStaggerHandgun",
            AnimRunUpper = "UpperRunRifle",
            AnimWalkUpper = "UpperWalkRifle",
            AnimUpperHipfire = "UpperHipfireRifle",
            AnimFireArmLength = 7f,
            AnimDraw = "UpperDrawRifle",
            AnimManualAim = "ManualAimRifle",
            AnimManualAimStart = "ManualAimRifleStart",
            AnimReloadUpper = "UpperReloadBazooka",
            AnimFullLand = "FullLandHandgun",
            AnimToggleThrowingMode = "UpperToggleThrowing"
        };
        weaponVisuals.SetModelTexture("CrossbowBoomM");
        weaponVisuals.SetDrawnTexture("CrossbowBoomD");
        weaponVisuals.SetSheathedTexture("CrossbowBoomS");
        weaponVisuals.SetThrowingTexture("CrossbowBoomT");
        SetPropertiesAndVisuals(weaponProperties, weaponVisuals);
        CacheDrawnTextures(new[] { "DrawBack", "Reload1", "Reload2" });
    }

    private CrossbowBoom(RWeaponProperties weaponProperties, RWeaponVisuals weaponVisuals)
    {
        SetPropertiesAndVisuals(weaponProperties, weaponVisuals);
    }

    public override RWeapon Copy()
    {
        CrossbowBoom CrossbowBoom = new(Properties, Visuals);
        CrossbowBoom.CopyStatsFrom(this);
        return CrossbowBoom;
    }

    public override Texture2D GetDrawnTexture(ref GetDrawnTextureArgs args)
    {
        switch (args.Player)
        {
            case { CurrentRifleWeapon.RoundReady: false, Reloading: false }:
                return _textureDrawEmpty;
            case { CurrentRifleWeapon.RoundReady: true, Reloading: false }:
                args.Postfix = "DrawBack";
                break;
        }

        if (args.SubAnimation is "UpperReloadBazooka")
        {
            args.Postfix = args.SubFrame switch
            {
                >= 1 and <= 3 => "Reload1",
                > 3 and <= 5 => "Reload2",
                _ => args.Postfix
            };
        }

        return base.GetDrawnTexture(ref args);
    }

    public override Texture2D GetSheathedTexture(ref GetDrawnTextureArgs args) => args.Player is { CurrentRifleWeapon.IsEmpty: true } ? _textureSEmpty : base.GetSheathedTexture(ref args);

    public override void OnSubAnimationEvent(Player player, AnimationEvent animationEvent, AnimationData animationData, int currentFrameIndex)
    {
        if (player.GameOwner != GameOwnerEnum.Server && animationEvent == AnimationEvent.EnterFrame)
        {
            if (animationData.Name == "UpperReloadBazooka" && currentFrameIndex == 1)
            {
                SoundHandler.PlaySound("BowDrawback", player.GameWorld);
            }
            else if (animationData.Name == "UpperDrawRifle" && currentFrameIndex == 1)
            {
                SoundHandler.PlaySound("BowDrawback", player.GameWorld);
            }
        }
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

    public override void OnRecoilEvent(Player player)
    {
        if (player.GameOwner != GameOwnerEnum.Server)
        {
            SoundHandler.PlaySound(Properties.BlastSoundID, player.Position, player.GameWorld);
        }
    }
}