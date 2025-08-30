using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SFD;
using SFD.Effects;
using SFD.Objects;
using SFD.Sounds;
using SFD.Tiles;
using SFD.Weapons;


namespace SFR.Weapons.Rifles
{
    public class MK20_SSR : RWeapon
    {
        public MK20_SSR()
        {
            RWeaponProperties rweaponProperties = new RWeaponProperties(122, LanguageHelper.GetText("weapon.Rifles.SSR"), "WpnMK20_SSR", false, WeaponCategory.Primary)
            {
                MaxMagsInWeapon = 1,
                MaxRoundsInMag = 20,
                MaxCarriedSpareMags = 2,
                StartMags = 1,
                CooldownBeforePostAction = 350,
                CooldownAfterPostAction = 700,
                ExtraAutomaticCooldown = 150,
                ProjectilesEachBlast = 1,
                ShellID = "ShellBig",
                AccuracyDeflection = 0.005f,
                ProjectileID = 113,
                MuzzlePosition = new Vector2(13f, -2.5f),
                CursorAimOffset = new Vector2(0f, 2.5f),
                LazerPosition = new Vector2(5f, -4.5f),
                MuzzleEffectTextureID = "MuzzleFlashS",
                BlastSoundID = "AK47",
                DrawSoundID = "SniperDraw",
                GrabAmmoSoundID = "SniperReload",
                OutOfAmmoSoundID = "OutOfAmmoHeavy",
                AimStartSoundID = "PistolAim",
                AI_DamageOutput = DamageOutputType.High,
                CanRefilAtAmmoStashes = false,
                ReloadPostCooldown = 750f,
                BreakDebris = new string[] { "MetalDebris00C", "ItemDebrisStockDark00", "ItemDebrisShiny01" },
                SpecialAmmoBulletsRefill = 10,
                VisualText = LanguageHelper.GetText("weapon.Rifles.SSR")
            };
            RWeaponVisuals rweaponVisuals = new()
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
                AnimFireArmLength = 2f,
                AnimDraw = "UpperDrawRifle",
                AnimManualAim = "ManualAimRifle",
                AnimManualAimStart = "ManualAimRifleStart",
                AnimReloadUpper = "UpperReload",
                AnimFullLand = "FullLandHandgun",
                AnimToggleThrowingMode = "UpperToggleThrowing"
            };
            rweaponVisuals.SetModelTexture("SSRM");
            rweaponVisuals.SetDrawnTexture("SSRD");
            rweaponVisuals.SetSheathedTexture("SSRS");
            rweaponVisuals.SetThrowingTexture("SSRThrowing");
            SetPropertiesAndVisuals(rweaponProperties, rweaponVisuals);
            LazerUpgrade = 1;
            CacheDrawnTextures(new string[] { "Reload" });
        }

        public override void OnRecoilEvent(Player player)
        {
            if (player.GameOwner != GameOwnerEnum.Server)
            {
                SoundHandler.PlaySound(Properties.BlastSoundID, player.Position, player.GameWorld);
                EffectHandler.PlayEffect("MZLED", Vector2.Zero, player.GameWorld, new object[]
                {
                    player,
                    base.Properties.MuzzleEffectTextureID
                });
            }
        }

        public MK20_SSR(RWeaponProperties rwp, RWeaponVisuals rwv)
        {
            base.SetPropertiesAndVisuals(rwp, rwv);
        }

        public override RWeapon Copy()
        {
            WpnSniperRifle wpnSniperRifle = new WpnSniperRifle(base.Properties, base.Visuals);
            wpnSniperRifle.CopyStatsFrom(this);
            return wpnSniperRifle;
        }

        public override void OnReloadAnimationEvent(Player player, AnimationEvent animEvent, SubAnimationPlayer subAnim)
        {
            if (player.GameOwner != GameOwnerEnum.Server)
            {
                if (animEvent == AnimationEvent.EnterFrame && subAnim.GetCurrentFrameIndex() == 1)
                {
                    base.SpawnMagazine(player, "MagAssaultRifle", new Vector2(-11f, -3f));
                    SoundHandler.PlaySound("MagnumReloadEnd", player.Position, player.GameWorld);
                }
                if (animEvent == AnimationEvent.EnterFrame && subAnim.GetCurrentFrameIndex() == 4)
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
                    SoundHandler.PlaySound("SniperDraw", player.GameWorld);
                }
            }
        }

        public override void OnSetPostFireAction(Player player)
        {
            if (player.CurrentAction == PlayerAction.ManualAim)
            {
                player.AnimationUpperOverride = new PlayerPostFireAnimation_BoltActionManualAim(player, this);
                return;
            }
            player.AnimationUpperOverride = new PlayerPostFireAnimation_BoltActionHipFire(player, this);
        }

        public override void OnPostFireAnimationEvent(Player player, AnimationEvent animEvent, SubAnimationPlayer subAnim)
        {
            if (player.GameOwner != GameOwnerEnum.Server && animEvent == AnimationEvent.EnterFrame)
            {
                if (subAnim.GetCurrentFrameIndex() == 1)
                {
                    if (base.Properties.ShellID != "")
                    {
                        RWeapon currentRifleWeapon = player.CurrentRifleWeapon;
                        if (currentRifleWeapon != null)
                        {
                            if (currentRifleWeapon.CurrentRoundsInWeapon > 0)
                            {
                                this.lastRoundPumped = false;
                                base.SpawnUnsyncedShell(player, base.Properties.ShellID);
                            }
                            else if (!this.lastRoundPumped)
                            {
                                this.lastRoundPumped = true;
                                base.SpawnUnsyncedShell(player, base.Properties.ShellID);
                            }
                        }
                    }
                    SoundHandler.PlaySound("SniperBoltAction1", player.GameWorld);
                }
                if (subAnim.GetCurrentFrameIndex() == 3)
                {
                    SoundHandler.PlaySound("SniperBoltAction2", player.GameWorld);
                }
            }
        }

        public override void OnReloadAnimationFinished(Player player)
        {
            player.AnimationUpperOverride = new PlayerEmptyBoltActionAnimation();
            this.lastRoundPumped = false;
        }

        public override void OnThrowWeaponItem(Player player, ObjectWeaponItem thrownWeaponItem)
        {
            thrownWeaponItem.Body.SetAngularVelocity(thrownWeaponItem.Body.GetAngularVelocity() * 0.7f);
            Vector2 linearVelocity = thrownWeaponItem.Body.GetLinearVelocity();
            linearVelocity.X *= 0.8f;
            linearVelocity.Y *= 0.8f;
            thrownWeaponItem.Body.SetLinearVelocity(linearVelocity);
            base.OnThrowWeaponItem(player, thrownWeaponItem);
        }

        public bool lastRoundPumped;
    }
}
