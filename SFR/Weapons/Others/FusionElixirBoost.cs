using SFD;
using SFD.Objects;
using SFD.Sounds;
using SFD.Tiles;
using SFD.Weapons;
using SFR.Helper;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SFDGameScriptInterface;

namespace SFR.Weapons.Others;

internal class FusionElixirBoost : PItem
{
    internal FusionElixirBoost()
    {
        var itemProperties = new PItemProperties(116, LanguageHelper.GetText("weapon.Others.FusionElixirBoost"), "ItemFusionElixirBoost", false, WeaponCategory.Supply)
        {
            PickupSoundID = "GetSlomo",
            ActivateSoundID = ""
        };

        var itemVisuals = new PItemVisuals(Textures.GetTexture("FusionElixirBoost"), Textures.GetTexture("FusionElixirBoostD"));
        itemProperties.VisualText = LanguageHelper.GetText("weapon.Others.FusionElixirBoost");

        SetPropertiesAndVisuals(itemProperties, itemVisuals);
    }

    private FusionElixirBoost(PItemProperties properties, PItemVisuals visuals)
    {
        SetPropertiesAndVisuals(properties, visuals);
    }

    public override void OnActivation(Player player, PItem powerupItem)
    {
        if (player.StrengthBoostPrepare())
        {
            SoundHandler.PlaySound(powerupItem.Properties.ActivateSoundID, player.Position, player.GameWorld);
        }
    }

    internal void OnEffectStart(Player player)
    {
        if (player.GameOwner != GameOwnerEnum.Client)
        {
            SoundHandler.PlaySound("Syringe", player.Position, player.GameWorld);
            SoundHandler.PlaySound("StrengthBoostStart", player.Position, player.GameWorld);
            player.StrengthBoostApply(15000f);
            player.SpeedBoostApply(15000f);
            var extendedPlayer = player.GetExtension();
            extendedPlayer.ApplyAdrenalineBoost();
            if (!player.InfiniteAmmo)
            {
                player.RemovePowerup();
            }
        }
        
    }

    public override PItem Copy() => new FusionElixirBoost(Properties, Visuals);
}