using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SFD.MenuControls;
using SFD;

namespace SFR.UI;

internal sealed class UL : Panel
{
    internal UL() : base("版本说明", 1000, 600)
    {
        List<MenuItem> items = new()
        {
            new MenuItemSeparator("作者"),
            new MenuItemLabel("此版本由1145在SFR最新版的基础上更改,对部分内容进行了平衡和调整", Align.Center, Color.OrangeRed),
            new MenuItemSeparator("SFR&&SFD的一些小改动"),
            new MenuItemLabel("1.燃烧瓶范围增大", Align.Center, Color.White),
            new MenuItemLabel("2.RCM在离线模式下炮弹是跟随鼠标控制而不是键盘控制", Align.Center, Color.White),
            new MenuItemLabel("3.RCM导弹速度略微提高,AA12射速减慢", Align.Center, Color.White),
            new MenuItemLabel("4.碎片手榴弹爆炸产生的子弹增多", Align.Center, Color.White),
            new MenuItemLabel("5.防爆盾目前代码不完整具体还要等SFR更新", Align.Center, Color.White),
            new MenuItemSeparator("汉化"),
            new MenuItemLabel("对SFR武器和SFD指令内容进行了汉化\n汉化由Pakd和Boki提供", Align.Center, Color.White),
            new MenuItemLabel("", Align.Center, Color.White),
            new MenuItemSeparator("UI界面"),
            new MenuItemLabel("UI界面颜色可在Config.ini中更改,详细请点击下面的'UI介绍'", Align.Center, Color.White),
            new MenuItemLabel("打开config.ini", Align.Center, Color.Orange, _ => Process.Start("SFR\\config.ini")),
            new MenuItemLabel("UI介绍", Align.Center, Color.BlueViolet, _ => Process.Start("SFR\\UI介绍.docx")),
            new MenuItemSeparator("部分自制武器介绍"),
            new MenuItemLabel("1.爆炸弩,射中后过一段时间会爆炸", Align.Center, Color.White),
            new MenuItemLabel("2.代码武器&鬼妖村正,不会刷新,只能通过指令获取", Align.Center, Color.White),
            new MenuItemLabel("3.充能狙击步枪,每隔10s会获得弹匣,能无限穿透可摧毁的物体,击中人能使其丧失无限体力并且掉落武器", Align.Center, Color.White),
            new MenuItemLabel("4.冰冻枪,击中时冰冻玩家0.95s", Align.Center, Color.White),
            new MenuItemLabel("5.原子能枪,主要靠击飞打伤害", Align.Center, Color.White),
            new MenuItemLabel("6.死神镰刀,攻击会使自己扣3滴血,每次攻击造成20%的伤害,能摧毁动态物体", Align.Center, Color.White),
            new MenuItemLabel("7.吸血刀,HP>80时攻击会使自己扣血(不可恢复),HP<80时攻击恢复2HP,可发射子弹,用投掷模式来控制子弹的发射方向", Align.Center, Color.White),
            new MenuItemLabel("最后,贴图大部分都是乱画的,因为不会画,平衡问题可找我(1145)反馈,有bug可以进群反馈", Align.Center, Color.Gold),
            new MenuItemSeparator("QQ群聊"),
            new MenuItemLabel("点击加入QQ群,欢迎各位加群", Align.Center, Color.Yellow, _ => Process.Start("https://qm.qq.com/q/MagL8VLEQK"))

    };

        Menu menu = new(new Vector2(0f, 40f), Width, Height - 40, this, items.ToArray());
        members.Add(menu);
    }

    private void Close(object sender)
    {
        ParentPanel.CloseSubPanel();
    }

    public override void KeyPress(Keys key)
    {
        if (subPanel == null && key == Keys.Escape)
        {
            Close(null);
            return;
        }

        base.KeyPress(key);
    }
}