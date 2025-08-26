using SFD;
using SFD.Sounds;
using SFD.Tiles;
using SFD.Weapons;
using SFR.Fighter.Jetpacks;
using SFR.Helper;
using SFR.Sync.Generic;
using Player = SFD.Player;

namespace SFR.Weapons.Others;

internal sealed class JetpackEditor : HItem
{
    internal JetpackEditor()
    {
        HItemProperties itemProperties = new(105, LanguageHelper.GetText("weapon.Others.Jetpack_Editor"), "ItemJetpackEditor", false, WeaponCategory.Supply)
        {
            GrabSoundID = "GetHealthSmall",
            VisualText = LanguageHelper.GetText("weapon.Others.Jetpack_Editor")
        };

        HItemVisuals visuals = new(Textures.GetTexture("Pills"));

        Properties = itemProperties;
        Visuals = visuals;
    }

    private JetpackEditor(HItemProperties itemProperties, HItemVisuals itemVisuals) : base(itemProperties, itemVisuals) { }

    public override void OnPickup(Player player, HItem instantPickupItem)
    {
        if (player.GameOwner != GameOwnerEnum.Client)
        {
            SoundHandler.PlaySound(instantPickupItem.Properties.GrabSoundID, player.Position, player.GameWorld);

            var extendedPlayer = player.GetExtension();
            extendedPlayer.JetpackType = JetpackType.JetpackEditor;
            extendedPlayer.GenericJetpack = new Fighter.Jetpacks.JetpackEditor();
            if (player.GameOwner == GameOwnerEnum.Server)
            {
                GenericData.SendGenericDataToClients(new(DataType.ExtraClientStates, [], player.ObjectID, extendedPlayer.GetStates()));
            }
        }
    }

    public override bool CheckDoPickup(Player player, HItem instantPickupItem) => true;

    public override HItem Copy() => new JetpackEditor(Properties, Visuals);
}