using Microsoft.Xna.Framework.Graphics;
using SFD;
using SFD.Objects;
using SFD.Sounds;
using SFD.Tiles;
using SFD.Weapons;
using SFDGameScriptInterface;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace SFR.Weapons.Rifles;

internal sealed class ShotBow : RWeapon
{
    internal ShotBow()
    {

        RWeaponProperties weaponProperties = new(115, LanguageHelper.GetText("weapon.Rifles.ShotBow"), "WpnShotBow", false, WeaponCategory.Primary)
        {
            MaxMagsInWeapon = 1,
            MaxRoundsInMag = 4,
            MaxCarriedSpareMags = 0,
            StartMags = 1,
            
            CooldownAfterPostAction = 1000,
            CooldownBeforePostAction = 0,
            ProjectilesEachBlast = 1,
            ProjectileID = 107,
            ShellID = string.Empty,
            AccuracyDeflection = 0f,
            MuzzlePosition = new Vector2(1f, -0.5f),
            MuzzleEffectTextureID = "",
            BlastSoundID = "BowShoot",
            DrawSoundID = "GrenadeDraw",
            GrabAmmoSoundID = "GrenadeDraw",
            OutOfAmmoSoundID = "BowNoAmmo",
            CursorAimOffset = new Vector2(0f, 2f),
            LazerPosition = new Vector2(1f, -1.5f),
            AimStartSoundID = "BowDrawback",
            AI_ImpactAoERadius = 1.43999994f,
            AI_DamageOutput = DamageOutputType.High,
            AI_EffectiveRange = 80,
            CanRefilAtAmmoStashes = false,
            AI_HasOneShotPotential = true,
            BreakDebris = new[]
            {
                "ItemDebrisShiny00",
                "ItemDebrisShiny01"
            },
            SpecialAmmoBulletsRefill = 4
        };

        RWeaponVisuals weaponVisuals = new();
        weaponVisuals.SetModelTexture("ShotBowM");
        weaponVisuals.SetDrawnTexture("ShotBowD");
        weaponVisuals.SetSheathedTexture("ShotBowS");
        weaponVisuals.SetHolsterTexture("ShotBowH");
        weaponVisuals.SetThrowingTexture("ShotBowT");
        weaponVisuals.AnimIdleUpper = "UpperIdleMelee";
        weaponVisuals.AnimCrouchUpper = "UpperCrouchMelee";
        weaponVisuals.AnimJumpKickUpper = "UpperJumpKickMelee";
        weaponVisuals.AnimJumpUpper = "UpperJumpMelee";
        weaponVisuals.AnimJumpUpperFalling = "UpperJumpFallingMelee";
        weaponVisuals.AnimKickUpper = "UpperKickMelee";
        weaponVisuals.AnimStaggerUpper = "UpperStagger";
        weaponVisuals.AnimRunUpper = "UpperRunMelee";
        weaponVisuals.AnimWalkUpper = "UpperWalkMelee";
        weaponVisuals.AnimUpperHipfire = "UpperHipfireBow";
        weaponVisuals.AnimFireArmLength = 7f;
        weaponVisuals.AnimDraw = "UpperDrawMelee";
        weaponVisuals.AnimManualAim = "ManualAimBow";
        weaponVisuals.AnimManualAimStart = "ManualAimBowStart";
        weaponVisuals.AnimReloadUpper = "UpperReload";
        weaponVisuals.AnimFullLand = "FullLandMelee";
        weaponVisuals.AnimToggleThrowingMode = "UpperToggleThrowing";
        weaponProperties.VisualText = LanguageHelper.GetText("weapon.Rifles.ShotBow");
        SetPropertiesAndVisuals(weaponProperties, weaponVisuals);
        CacheDrawnTextures(new string[] { "DrawBack" });
        m_textureSheathedEmpty = Textures.GetTexture("ShotBowSEmpty");
        m_textureHolsterEmpty = Textures.GetTexture("ShotBowHEmpty");
        m_textureDrawnEmpty = Textures.GetTexture("ShotBowDDrawBackEmpty");
    }

    public static Texture2D m_textureSheathedEmpty;

    public static Texture2D m_textureHolsterEmpty;

    public static Texture2D m_textureDrawnEmpty;
    private ShotBow(RWeaponProperties weaponProperties, RWeaponVisuals weaponVisuals)
    {
        SetPropertiesAndVisuals(weaponProperties, weaponVisuals);
    }

    public override RWeapon Copy()
    {
        ShotBow wpnRCM = new(Properties, Visuals);
        wpnRCM.CopyStatsFrom(this);
        return wpnRCM;
    }


    public override Texture2D GetDrawnTexture(ref GetDrawnTextureArgs args)
    {
        if (args.Player != null && args.Player.CurrentRifleWeapon != null && args.Player.CurrentRifleWeapon.IsEmpty && args.Postfix == "DrawBack")
        {
            return m_textureDrawnEmpty;
        }
        return base.GetDrawnTexture(ref args);
    }

    public override Texture2D GetHolsterTexture(ref GetDrawnTextureArgs args)
    {
        if (args.Player != null && args.Player.CurrentRifleWeapon != null && args.Player.CurrentRifleWeapon.IsEmpty)
        {
            return m_textureHolsterEmpty;
        }
        return base.GetHolsterTexture(ref args);
    }

    public override Texture2D GetSheathedTexture(ref GetDrawnTextureArgs args)
    {
        if (args.Player != null && args.Player.CurrentRifleWeapon != null && args.Player.CurrentRifleWeapon.IsEmpty)
        {
            return m_textureSheathedEmpty;
        }
        return base.GetSheathedTexture(ref args);
    }
    public override void OnSubAnimationEvent(Player player, AnimationEvent animationEvent, AnimationData animationData, int currentFrameIndex)
    {
        if (player.GameOwner != GameOwnerEnum.Server && animationEvent == AnimationEvent.EnterFrame && animationData.Name == "UpperDrawMelee" && currentFrameIndex == 1)
        {
            SoundHandler.PlaySound("Draw1", player.GameWorld);
        }
    }



    public override void OnThrowWeaponItem(Player player, ObjectWeaponItem thrownWeaponItem)
    {
        thrownWeaponItem.Body.SetAngularVelocity(thrownWeaponItem.Body.GetAngularVelocity() * 0.6f);
        var linearVelocity = thrownWeaponItem.Body.GetLinearVelocity() * 0.8f;
        thrownWeaponItem.Body.SetLinearVelocity(linearVelocity);
        base.OnThrowWeaponItem(player, thrownWeaponItem);
    }
}