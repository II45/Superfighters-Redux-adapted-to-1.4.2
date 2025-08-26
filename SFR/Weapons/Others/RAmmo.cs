using SFD;
using SFD.Sounds;
using SFD.Tiles;
using SFD.Weapons;
using SFR.Fighter.Jetpacks;
using SFR.Helper;
using SFR.Sync.Generic;


namespace SFR.Weapons
{
    public class RAmmo : HItem
    {
        public RAmmo()
        {
            HItemProperties hitemProperties = new(113, LanguageHelper.GetText("weapon.Others.RAmmo"), "ItemRAmmo", false, WeaponCategory.Supply)
            {
                GrabSoundID = "GetHealthLarge"
            };
            HItemVisuals hitemVisuals = new(Textures.GetTexture("ItemHealthAmmoM"));
            Properties = hitemProperties;
            Visuals = hitemVisuals;
            hitemProperties.VisualText = LanguageHelper.GetText("weapon.Others.RAmmo");
        }

        public RAmmo(HItemProperties properties, HItemVisuals visuals)
            : base(properties, visuals)
        {
        }

        public override void OnPickup(Player player, HItem instantPickupItem)
        {
            SoundHandler.PlaySound(instantPickupItem.Properties.GrabSoundID, player.Position, player.GameWorld);
            RWeapon rweapon = player.GetCurrentRangedWeaponInUse();
            if (rweapon != null && !rweapon.Properties.CanUseFireBulletsPowerup)
            {
                rweapon = null;
            }
            if (rweapon == null)
            {
                rweapon = player.CurrentRifleWeapon;
                if (rweapon != null && !rweapon.Properties.CanUseFireBulletsPowerup)
                {
                    rweapon = null;
                }
                if (rweapon == null)
                {
                    rweapon = player.CurrentHandgunWeapon;
                }
                if (rweapon != null && !rweapon.Properties.CanUseFireBulletsPowerup)
                {
                    rweapon = null;
                }
            }
            if (rweapon != null)
            {
                rweapon.PowerupFireRounds = (ushort)WpnBouncingAmmo.FillSpecialAmmoToGun(rweapon);
                rweapon.PowerupBouncingRounds = (ushort)WpnBouncingAmmo.FillSpecialAmmoToGun(rweapon);
                player.SyncRWeaponItem(rweapon);
                return;
            }
            player.WeaponFireUpgradeQueued = true;
            player.WeaponBouncingUpgradeQueued = false;
        }

        public override bool CheckDoPickup(Player player, HItem instantPickupItem)
        {
            RWeapon rweapon = player.GetCurrentRangedWeaponInUse();
            if (rweapon != null && !rweapon.Properties.CanUseFireBulletsPowerup)
            {
                rweapon = null;
            }
            if (rweapon == null)
            {
                rweapon = player.CurrentRifleWeapon;
                if (rweapon != null && !rweapon.Properties.CanUseFireBulletsPowerup)
                {
                    rweapon = null;
                }
                if (rweapon == null)
                {
                    rweapon = player.CurrentHandgunWeapon;
                }
                if (rweapon != null && !rweapon.Properties.CanUseFireBulletsPowerup)
                {
                    rweapon = null;
                }
            }
            return rweapon != null;
        }

        public override HItem Copy()
        {
            return new RAmmo(Properties, Visuals);
        }
    }
}
