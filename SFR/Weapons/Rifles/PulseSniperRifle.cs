using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SFD;
using SFD.Effects;
using SFD.Objects;
using SFD.Sounds;
using SFD.Tiles;
using SFD.Weapons;

using SFR.Helper;
using static SFD.GUIConstants;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;


namespace SFR.Weapons.Rifles
{
    public class PulseSniperRifle : RWeapon, IExtendedWeapon
    {
        public float time = 0f;
        public PulseSniperRifle()
        {
            RWeaponProperties rweaponProperties = new(119, LanguageHelper.GetText("weapon.Rifles.PulseSniperRifle"), "WpnPulseSniperRifle", false, WeaponCategory.Primary)
            {
                MaxMagsInWeapon = 1,
                MaxRoundsInMag = 1,
                MaxCarriedSpareMags = 2,
                StartMags = 2,
                CooldownBeforePostAction = 600,
                CooldownAfterPostAction = 900,
                ExtraAutomaticCooldown = 300,
                ProjectilesEachBlast = 1,
                ShellID = "ShellBig",
                AccuracyDeflection = 0.005f,
                ProjectileID = 109,
                MuzzlePosition = new Microsoft.Xna.Framework.Vector2(14f, -1.5f),
                CursorAimOffset = new Microsoft.Xna.Framework.Vector2(0f, 2.5f),
                LazerPosition = new Microsoft.Xna.Framework.Vector2(6f, -4.5f),
                MuzzleEffectTextureID = "MuzzleFlashS",
                BlastSoundID = "PulseSniperRifle",
                DrawSoundID = "SniperDraw",
                GrabAmmoSoundID = "SniperReload",
                OutOfAmmoSoundID = "OutOfAmmoHeavy",
                AimStartSoundID = "PistolAim",
                AI_DamageOutput = DamageOutputType.High,
                CanRefilAtAmmoStashes = false,
                ReloadPostCooldown = 750f,
                BreakDebris = new string[] { "MetalDebris00C", "ItemDebrisStockDark00", "ItemDebrisShiny01" },
                SpecialAmmoBulletsRefill = 3,
                VisualText = LanguageHelper.GetText("weapon.Rifles.PulseSniperRifle")
            };
            RWeaponVisuals rweaponVisuals = new ();
            rweaponVisuals.SetModelTexture("PulseSniperRifleM");
            rweaponVisuals.SetDrawnTexture("PulseSniperRifleD");
            rweaponVisuals.SetSheathedTexture("PulseSniperRifleS");
            rweaponVisuals.SetThrowingTexture("PulseSniperRifleThrowing");
            rweaponVisuals.AnimIdleUpper = "UpperIdleRifle";
            rweaponVisuals.AnimCrouchUpper = "UpperCrouchRifle";
            rweaponVisuals.AnimJumpKickUpper = "UpperJumpKickRifle";
            rweaponVisuals.AnimJumpUpper = "UpperJumpRifle";
            rweaponVisuals.AnimJumpUpperFalling = "UpperJumpFallingRifle";
            rweaponVisuals.AnimKickUpper = "UpperKickRifle";
            rweaponVisuals.AnimStaggerUpper = "UpperStaggerHandgun";
            rweaponVisuals.AnimRunUpper = "UpperRunRifle";
            rweaponVisuals.AnimWalkUpper = "UpperWalkRifle";
            rweaponVisuals.AnimUpperHipfire = "UpperHipfireRifle";
            rweaponVisuals.AnimFireArmLength = 2f;
            rweaponVisuals.AnimDraw = "UpperDrawRifle";
            rweaponVisuals.AnimManualAim = "ManualAimRifle";
            rweaponVisuals.AnimManualAimStart = "ManualAimRifleStart";
            rweaponVisuals.AnimReloadUpper = "UpperReload";
            rweaponVisuals.AnimFullLand = "FullLandHandgun";
            rweaponVisuals.AnimToggleThrowingMode = "UpperToggleThrowing";
            base.SetPropertiesAndVisuals(rweaponProperties, rweaponVisuals);
            base.LazerUpgrade = 1;
            this.CacheDrawnTextures(new string[] { "Reload" });
        }

        public override void OnRecoilEvent(Player player)
        {
            if (player.GameOwner != GameOwnerEnum.Server)
            {
                SoundHandler.PlaySound(base.Properties.BlastSoundID, player.Position, player.GameWorld);
                EffectHandler.PlayEffect("MZLED", Microsoft.Xna.Framework.Vector2.Zero, player.GameWorld, new object[]
                {
                    player,
                    Properties.MuzzleEffectTextureID
                });
            }
        }

        public PulseSniperRifle(RWeaponProperties rwp, RWeaponVisuals rwv)
        {
            base.SetPropertiesAndVisuals(rwp, rwv);
        }

        public override RWeapon Copy()
        {
            PulseSniperRifle PulseSniperRifle = new(Properties, Visuals);
            PulseSniperRifle.CopyStatsFrom(this);
            return PulseSniperRifle;
        }

        public override void OnReloadAnimationEvent(Player player, AnimationEvent animEvent, SubAnimationPlayer subAnim)
        {
            if (player.GameOwner != GameOwnerEnum.Server)
            {
                if (animEvent == AnimationEvent.EnterFrame && subAnim.GetCurrentFrameIndex() == 1)
                {
                    SpawnMagazine(player, "MagAssaultRifle", new Microsoft.Xna.Framework.Vector2(-11f, -3f));
                    SoundHandler.PlaySound("MagnumReloadEnd", player.Position, player.GameWorld);
                }
                if (animEvent == AnimationEvent.EnterFrame && subAnim.GetCurrentFrameIndex() == 4)
                {
                    SoundHandler.PlaySound("PistolReload", player.Position, player.GameWorld);
                }
            }
        }
        public void Update(Player player, float ms, float realMs)
        {
                time -= ms;
                RWeapon rweapon = player.GetCurrentRangedWeaponInUse();
                if (rweapon != null && time <= -12000f)
                {
                    rweapon.FillAmmoMax();
                    SoundHandler.PlaySound("TommyGunDraw", player.Position, player.GameWorld);
                    time = 0f;
                }


        }

        public void GetDealtDamage(Player player, float damage) { }
        public void OnHit(Player player, Player target) 
        {


        }
        public void OnHitObject(Player player, PlayerHitEventArgs args, ObjectData obj)
        {
        }
        public void DrawExtra(SpriteBatch spritebatch, Player player, float ms)
        {
      
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
                               SpawnUnsyncedShell(player, Properties.ShellID);
                            }
                            else if (!lastRoundPumped)
                            {
                                lastRoundPumped = true;
                                SpawnUnsyncedShell(player, Properties.ShellID);
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
            Microsoft.Xna.Framework.Vector2 linearVelocity = thrownWeaponItem.Body.GetLinearVelocity();
            linearVelocity.X *= 0.8f;
            linearVelocity.Y *= 0.8f;
            thrownWeaponItem.Body.SetLinearVelocity(linearVelocity);
            base.OnThrowWeaponItem(player, thrownWeaponItem);
        }

        public bool lastRoundPumped;
    }
}
