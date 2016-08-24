using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// 主要对游戏的文本进行常规处理
/// 里边新增了颜色 字体大小 的处理
/// 采用 C#原始类型扩展方法—this参数修饰符
/// </summary>
public static class RichTextUtils {
    public static Dictionary<SpaceColor, string> ColorStyleDict = new Dictionary<SpaceColor, string>();
    public static Action<string, string> HrefCallback = null;
    public static Action<string, string> NotHrefCallback = null;
    public static Func<string, string, string> FunHrefColor = null;
    public static Func<string> FunShieldRegex = null;
    public static Func<string> FunHrefRegex = null;

    private static Regex shieldRegex = null;
    private static Regex hrefRegex = null;

    public static Regex ShieldRegex {
        get {
            if (RichTextUtils.shieldRegex == null) {
                RichTextUtils.shieldRegex = new Regex(RichTextUtils.FunShieldRegex(), RegexOptions.Singleline);
            }
            return RichTextUtils.shieldRegex;
        }
    }

    public static Regex HrefRegex {
        get {
            if (RichTextUtils.hrefRegex == null) {
                RichTextUtils.hrefRegex = new Regex(RichTextUtils.FunHrefRegex(), RegexOptions.Singleline);
            }
            return RichTextUtils.hrefRegex;
        }
    }

    public static string ToColor(this string text, string colorCode) {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Length = 0;
        stringBuilder.Append("<color=");
        stringBuilder.Append(colorCode + ">");
        stringBuilder.Append(text);
        stringBuilder.Append("</color>");
        return stringBuilder.ToString();
    }

    public static string ToColor(this string text, SpaceColor style) {
        string colorCode = RichTextUtils.GetColorCode(style);
        return text.ToColor(colorCode);
    }

    public static string ToColor(this int text, SpaceColor style) {
        return text.ToString().ToColor(style);
    }

    public static string GetColorCode(SpaceColor style) {
        string result;
        if (!RichTextUtils.ColorStyleDict.TryGetValue(style, out result)) {
            return string.Empty;
        }
        return result;
    }

    public static string SetSize(this string text, int size) {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Length = 0;
        stringBuilder.Append("<size=");
        stringBuilder.Append(size.ToString());
        stringBuilder.Append(">");
        stringBuilder.Append(text);
        stringBuilder.Append("</size>");
        return stringBuilder.ToString();
    }

    public static string AnalyzeHref(string text, List<HrefInfo> hrefList) {
        StringBuilder stringBuilder = new StringBuilder();
        int num = 0;
        IEnumerator enumerator = RichTextUtils.HrefRegex.Matches(text).GetEnumerator();
        try {
            while (enumerator.MoveNext()) {
                Match match = (Match)enumerator.Current;
                string tab = match.Groups[1].Value;
                string key = match.Groups[2].Value;
                string value = match.Groups[3].Value;
                stringBuilder.Append(text.Substring(num, match.Index - num));
                stringBuilder.Append(value.ToColor(RichTextUtils.FunHrefColor(tab, key)));
                num = match.Index + match.Length;
                hrefList.Add(new HrefInfo(key, value,
                    delegate {
                        RichTextUtils.HrefCallback(tab, key);
                    },
                    delegate {
                        RichTextUtils.NotHrefCallback(tab, key);
                    }));
            }
        }
        finally {
            IDisposable disposable = enumerator as IDisposable;
            if (disposable != null) {
                disposable.Dispose();
                disposable = null;
            }
        }
        stringBuilder.Append(text.Substring(num, text.Length - num));
        text = stringBuilder.ToString();
        return text;
    }
}

public class HrefInfo {
    public readonly List<Rect> bounds = new List<Rect>();

    public string UID {
        get;
        set;
    }

    public string Name {
        get;
        set;
    }

    public SpaceColor ColorStyle {
        get;
        set;
    }

    public int StartIndex {
        get;
        set;
    }

    public int EndIndex {
        get;
        set;
    }

    public Action ClickEvent {
        get;
        set;
    }

    public Action NotAreaClickEvent {
        get;
        set;
    }

    /// <summary>
    /// 超链接信息
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_event"></param>
    public HrefInfo(string _name, Action _event) {
        this.UID = "0";
        this.Name = _name;
        this.ClickEvent = _event;
        this.NotAreaClickEvent = null;
    }

    /// <summary>
    /// 超链接信息
    /// </summary>
    /// <param name="_uid">key值 自己定义 例如 UI NPC 或者ITEM</param>
    /// <param name="_name">key对应的值</param>
    /// <param name="_event">点击该字体的回调函数</param>
    /// <param name="_notArea">点击非key值以外字体的回调函数</param>
    public HrefInfo(string _uid, string _name, Action _event, Action _notArea = null) {
        this.UID = _uid;
        this.Name = _name;
        this.ClickEvent = _event;
        this.NotAreaClickEvent = _notArea;
    }

    public void SetPos(int _start, int _end) {
        this.StartIndex = _start;
        this.EndIndex = _end;
    }
}