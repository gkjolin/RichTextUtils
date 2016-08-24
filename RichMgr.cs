using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RichTextHelper : Component {
    private static readonly string HrefRegexCode = "<(npc|item|ui|)=(.*?),(.*?)>";
    private static readonly string ShieldRegexCode = "<(npc|item|hero|ui|)=(.*?),(.*?)>|<c|<C|</c|</C|<s|<S|</s|</S|<b|</b|<B|</B|<i|</i|<I|</I|<q|<Q|{|}";
    private static readonly SpaceColor CommonHrefColor = SpaceColor.Green;
    private static readonly SpaceColor NpcHrefColor = SpaceColor.Org;
    private static readonly SpaceColor ItemHrefColor = SpaceColor.Yellow;
    private static readonly SpaceColor UiHrefColor = SpaceColor.Org;
    private static readonly SpaceColor HttpHrefColor = SpaceColor.Blue;
    private static readonly SpaceColor OtherHrefColor = SpaceColor.Blue;
    private static readonly SpaceColor MyNameHrefColor = SpaceColor.Green;
    private static readonly SpaceColor QuestionHrefColor = SpaceColor.Yellow;
    private static string emojiRegex = null;
    private static string sysIconRegex = null;
    private static string allIocnRegex = null;

    public static readonly string[] ColorStyles = new string[]
	{
		"#5faafd",
		"#693e15",
		"#cd6eff",
		"#93ec5a",
		"#b3753e",
		"#f75a4a",
		"#fcdb72",
		"#fdeabd",
		"#ffa200",
		"#ffe5d7",
		"#ffffff",
		"#AEAEAE"
	};

    /// <summary>
    /// 监听区域回调函数
    /// </summary>
    /// <param name="tab"></param>
    /// <param name="key"></param>
    public static void HrefEvent(string tab, string key) {
        switch (tab) {
            case "item":
                RichTextHelper.HrefItemEvent(key);
                break;

            case "ui":
                RichTextHelper.HrefUIEvent(key);
                break;

            default:
                break;
        }

        Debug.Log(string.Format("HrefEvent ==> event key = {0}, event code = {1}", tab, key));
    }

    /// <summary>
    /// 非点击区域回
    /// </summary>
    /// <param name="tab"></param>
    /// <param name="key"></param>
    public static void NotHrefEvent(string tab, string key) {
        if (tab != null) {
            switch (tab) {
                case "item":
                    break;

                case "conquer":
                    break;

                default:
                    break;
            }
            Debug.Log(string.Format("NotHrefEvent ==> event key = {0}, event code = {1}", tab, key));
        }
    }

    private static void HrefUIEvent(string key) {
        int num = int.Parse(key);
        if (num <= 0) {
            var errmsg = string.Format("can't find the UI widget by event code {0}", num);
            new Exception(errmsg);
        }
        else {
            GameEventCenter.getEventCenter().DispatchEvent(new GameEvent(num));
        }
    }

    /// <summary>
    /// 根据key 获得ItemEntity 然后做对应操作
    /// </summary>
    /// <param name="key"></param>
    public static void HrefItemEvent(string key) {
        ///根据key值获得itemEntity
    }

    public static string AddItemHrefCode(string content, HerfMsg[] itemUIDLst) {
        return RichTextHelper.AddHrefCode(content, itemUIDLst, "item");
    }

    private static string AddHrefCode(string content, HerfMsg[] newUIDLst, string tab) {
        int num = 0;
        for (int i = 0; i < newUIDLst.Length; i++) {
            string name = newUIDLst[i].name;
            num = content.IndexOf(name, num);
            if (num >= 0) {
                string str = string.Empty;
                string str2 = string.Empty;
                if (num > 0) {
                    str = content.Substring(0, num);
                }
                string text = content.Substring(num, name.Length);
                text = text.Replace(name, string.Concat(new object[]
				{
					"<",
					tab,
					"=",
					newUIDLst[i].uid,
					",",
					name,
					">"
				}));
                if (num + name.Length + 1 < content.Length) {
                    str2 = content.Substring(num + name.Length);
                }
                content = str + text + str2;
                num = content.IndexOf(name, num) + 1;
            }
            else {
                num = 0;
                Debug.Log(string.Format("添加链接,没有找到{0}", name));
            }
        }
        return content;
    }

    public static Color GetColor(string color) {
        string text = color.TrimStart(new char[]
		{
			'#'
		});
        int num = (int)Convert.ToInt16("0x" + text.Substring(0, 2).ToString(), 16);
        int num2 = (int)Convert.ToInt16("0x" + text.Substring(2, 2).ToString(), 16);
        int num3 = (int)Convert.ToInt16("0x" + text.Substring(4, 2).ToString(), 16);
        Color result = new Color((float)num / 255f, (float)num2 / 255f, (float)num3 / 255f, 1f);
        return result;
    }

    /// <summary>
    /// 初始化颜色值
    /// </summary>
    private static void InitColorStyle() {
        RichTextUtils.ColorStyleDict[SpaceColor.Blue] = RichTextHelper.ColorStyles[0];
        RichTextUtils.ColorStyleDict[SpaceColor.Puce] = RichTextHelper.ColorStyles[1];
        RichTextUtils.ColorStyleDict[SpaceColor.Purple] = RichTextHelper.ColorStyles[2];
        RichTextUtils.ColorStyleDict[SpaceColor.Green] = RichTextHelper.ColorStyles[3];
        RichTextUtils.ColorStyleDict[SpaceColor.Brown] = RichTextHelper.ColorStyles[4];
        RichTextUtils.ColorStyleDict[SpaceColor.Red] = RichTextHelper.ColorStyles[5];
        RichTextUtils.ColorStyleDict[SpaceColor.Yellow] = RichTextHelper.ColorStyles[6];
        RichTextUtils.ColorStyleDict[SpaceColor.Button] = RichTextHelper.ColorStyles[7];
        RichTextUtils.ColorStyleDict[SpaceColor.Org] = RichTextHelper.ColorStyles[8];
        RichTextUtils.ColorStyleDict[SpaceColor.Text] = RichTextHelper.ColorStyles[9];
        RichTextUtils.ColorStyleDict[SpaceColor.White] = RichTextHelper.ColorStyles[10];
        RichTextUtils.ColorStyleDict[SpaceColor.Gray] = RichTextHelper.ColorStyles[11];
    }

    public static string GetHrefColor(string tab, string key) {
        switch (tab) {
            case "npc":
                return RichTextUtils.GetColorCode(RichTextHelper.NpcHrefColor);

            case "item":
                return RichTextUtils.GetColorCode(RichTextHelper.ItemHrefColor);
        }
        return RichTextUtils.GetColorCode(RichTextHelper.CommonHrefColor);
    }

    /// <summary>
    /// 初始化超链接回调
    /// </summary>
    private static void InitCallBackFun() {
        RichTextUtils.HrefCallback = new Action<string, string>(RichTextHelper.HrefEvent);
        RichTextUtils.FunHrefRegex = new Func<string>(RichTextHelper.LoadHrefRegex);
        RichTextUtils.NotHrefCallback = new Action<string, string>(RichTextHelper.NotHrefEvent);
        RichTextUtils.FunHrefColor = new Func<string, string, string>(RichTextHelper.GetHrefColor);
    }

    /// <summary>
    /// 使用方法 打开<ui=9,摄政王>只要按照 <关键字=id>即可
    /// </summary>
    public static void OnAdded() {
        InitColorStyle();
        InitCallBackFun();
    }

    public static string LoadHrefRegex() {
        return RichTextHelper.HrefRegexCode;
    }
}

public class HerfMsg {
    public ulong uid;
    public string name;
}

public enum SpaceColor {
    White,
    Green,
    Blue,
    Purple,
    Org,
    Red,
    Puce,
    Brown,
    Yellow,
    Button,
    Text,
    Gray
}