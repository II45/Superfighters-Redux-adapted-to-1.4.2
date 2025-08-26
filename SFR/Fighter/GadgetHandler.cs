using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SFD;
using SFD.Tiles;
using SFR.Helper;
using SFR.Misc;
using System;
using Player = SFD.Player;
using SFD.Weapons;
using SFD.Projectiles;

namespace SFR.Fighter;

/// <summary>
/// Here we handle all the HUD or visual effects regarding players, such as dev icons.
/// </summary>
[HarmonyPatch]
internal static class GadgetHandler
{
    private static readonly Texture2D RAmmoIcon = Textures.GetTexture("RAmmoIcon");
    private enum PowerupAmmoType {random};
    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerHUD), nameof(PlayerHUD.DrawTeamIcon))]
    private static bool DrawHudTeamIcon(Player player, GameUser user, int x, int y, SpriteBatch spriteBatch, float elapsed)
    {
        var teamIcon = Constants.GetTeamIcon(user.GameSlotTeam);
        if (teamIcon != null)
        {
            if (player is not null && !player.IsRemoved && !player.IsDead && !player.IsBot && user is not null)
            {
                if (DevHandler.GetDeveloperIcon(user.Account) is { } devIcon)
                {
                    teamIcon = devIcon;
                }
            }

            spriteBatch.Draw(teamIcon, new Rectangle(x - 8, y - 6, teamIcon.Width * 2, teamIcon.Height * 2), Color.White);
        }

        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.DrawPlates))]
    private static bool DrawExtraInfo(float ms, Player __instance)
    {
        var vector = Camera.ConvertWorldToScreen(__instance.Position + new Vector2(0f, 24f));
        float num = MathHelper.Max(Camera.Zoom * 0.4f, 1f);

        NameIconHandler.Draw(__instance, vector, num);

        // Handle message icons.
        if (__instance is { IsDead: false, IsRemoved: false, ChatActive: true })
        {
            if (__instance.m_chatIconTimer > 250f)
            {
                __instance.m_chatIconFrame = (__instance.m_chatIconFrame + 1) % 4;
                __instance.m_chatIconTimer -= 250f;
            }
            else
            {
                __instance.m_chatIconTimer += ms;
            }

            __instance.m_spriteBatch.Draw(Constants.ChatIcon,
                new Vector2(vector.X + __instance.m_nameTextSize.X * 0.25f * num, vector.Y - __instance.m_nameTextSize.Y * num),
                new Rectangle(1 + __instance.m_chatIconFrame * 13, 1, 12, 12), ColorCorrection.FromXNAToCustom(Constants.COLORS.CHAT_ICON), 0f, Vector2.Zero,
                num, SpriteEffects.None, 1f);
        }

        StatusBarHandler.Draw(__instance, vector, num);

        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.DrawColor), MethodType.Getter)]
    private static bool GetPlayerDrawColor(Player __instance, ref Color __result)
    {
        var extendedPlayer = __instance.GetExtension();

        if (extendedPlayer.AdrenalineBoost)
        {
            __result = ColorCorrection.CreateCustom(Globals.RageBoost);
            return false;
        }
        if (extendedPlayer.Ice)
        {
            __result = ColorCorrection.CreateCustom(Globals.Freeze);
            return false;
        }
        return true;
    }



    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerHUD), nameof(PlayerHUD.DrawWeaponPanel))]
    private static bool DrawWeaponPanelex(PlayerHUD __instance, int x, int y, SpriteBatch spriteBatch, float elapsed)
    {
        __instance.DrawBackground(x, y, 240, 68, spriteBatch);
        __instance.DrawWeaponSlots(x, y, spriteBatch);
        y += 54;
        __instance.DrawWeaponDurability(x, y - 2, spriteBatch);
        if (PlayerHUD.m_currentPlayerGUIInfo.LabelWeaponName != __instance.m_weaponName)
        {
            __instance.SetWeaponName(PlayerHUD.m_currentPlayerGUIInfo.LabelWeaponName);
        }
        __instance.m_weaponLabel.Draw(spriteBatch, elapsed, new Vector2(x, (y - 6)));
        __instance.DrawText(__instance.m_displayWeaponName, new Vector2(x, (y - 6)), spriteBatch, Color.White);
        if (PlayerHUD.m_currentPlayerGUIInfo.InThrowingMode)
        {
            Rectangle rectangle = new Rectangle(x + 240 - 32, y + 4, 32, 16);
            spriteBatch.Draw(PlayerHUD.ThrowingModeIcon, rectangle, Color.White);
        }
        else
        {
            if (PlayerHUD.m_currentPlayerGUIInfo.LabelAmmo != "")
            {
                string text = PlayerHUD.m_currentPlayerGUIInfo.LabelAmmo + " | " + PlayerHUD.m_currentPlayerGUIInfo.LabelAmmoMags;
                Vector2 vector = Constants.Font1.MeasureString(text);
                __instance.DrawText(text, new Vector2((x + 240) - vector.X, (y - 6)), spriteBatch, Color.White);
                switch (PlayerHUD.m_currentPlayerGUIInfo.SpecialAmmoType)
                {
                    case PlayerGUIInformation.PowerupAmmoType.Bouncing:
                        {
                            var color = new Color(64, 64, 255);
                            vector = Constants.Font1.MeasureString(PlayerHUD.m_currentPlayerGUIInfo.LabelSpecialAmmo);
                            spriteBatch.Draw(__instance.BouncingAmmoIcon, new Vector2(x + 240 - 40, y + 14), null, color, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
                            __instance.DrawText(PlayerHUD.m_currentPlayerGUIInfo.LabelSpecialAmmo, new Vector2(x + 240 - vector.X, y + 14), spriteBatch, color);
                            break;
                        }
                    case PlayerGUIInformation.PowerupAmmoType.Fire:
                        {
                            Color color2 = new Color(255, 64, 32);
                            vector = Constants.Font1.MeasureString(PlayerHUD.m_currentPlayerGUIInfo.LabelSpecialAmmo);
                            spriteBatch.Draw(__instance.FireAmmoIcon, new Vector2(x + 240 - 40, y + 14), null, color2, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
                            __instance.DrawText(PlayerHUD.m_currentPlayerGUIInfo.LabelSpecialAmmo, new Vector2(x + 240 - vector.X, y + 14), spriteBatch, color2);
                            break;
                        }
                    case PlayerGUIInformation.PowerupAmmoType.random:
                        {
                            Color color3 = new Color(0, 255, 0);
                            vector = Constants.Font1.MeasureString(PlayerHUD.m_currentPlayerGUIInfo.LabelSpecialAmmo);
                            spriteBatch.Draw(RAmmoIcon, new Vector2((float)(x + 240 - 40), (float)(y + 13)), null, color3, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
                            __instance.DrawText(PlayerHUD.m_currentPlayerGUIInfo.LabelSpecialAmmo, new Vector2((float)(x + 240) - vector.X, (float)(y + 13)), spriteBatch, color3);
                            break;
                        }
                }
            }
        }
        return false;
    }

}