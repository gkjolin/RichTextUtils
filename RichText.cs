using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RichText : Text, IEventSystemHandler, IPointerClickHandler {
    public Action OnClickEmpty;
    private List<HrefInfo> HrefList = new List<HrefInfo>();

    public override string text {
        get {
            return base.text;
        }
        set {
            base.text = value;
        }
    }

    public virtual void ShowText(string str) {
        this.HrefList.Clear();
        str = RichTextUtils.AnalyzeHref(str, this.HrefList);
        int startIndex = 0;

        for (int i = 0; i < this.HrefList.Count; i++) {
            int num = str.IndexOf(this.HrefList[i].Name, startIndex);
            if (num == -1) {
                this.HrefList[i].SetPos(0, 0);
            }
            else {
                startIndex = num + this.HrefList[i].Name.Length;
                int start = num * 4;
                int end = (num + this.HrefList[i].Name.Length - 1) * 4 + 3;
                this.HrefList[i].SetPos(start, end);
            }
        }
        this.text = str;
    }

    public virtual void OnPointerClick(PointerEventData eventData) {
        Vector2 point;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(base.rectTransform, eventData.position, eventData.pressEventCamera, out point);
        foreach (HrefInfo current in this.HrefList) {
            List<Rect> boxes = current.bounds;
            for (int i = 0; i < boxes.Count; i++) {
                if (boxes[i].Contains(point)) {
                    if (current.ClickEvent != null) {
                        current.ClickEvent();
                    }
                    return;
                }
            }
            current.NotAreaClickEvent();
        }
        if (this.OnClickEmpty != null) {
            this.OnClickEmpty();
        }
    }

    public void HrefClickEvent() {
        foreach (HrefInfo current in this.HrefList) {
            if (current.ClickEvent != null) {
                current.ClickEvent();
                break;
            }
        }
    }

    protected override void OnPopulateMesh(VertexHelper vh) {
        base.OnPopulateMesh(vh);

        Vector2 extents = rectTransform.rect.size;
        var settings = GetGenerationSettings(extents);
        float unitsPerPixel = 1 / pixelsPerUnit;
        float fontRealSize = fontSize * pixelsPerUnit;
        cachedTextGenerator.Populate(this.text, settings);
        var vbo = cachedTextGenerator.verts;

        HrefInfo value = null;
        for (int k = 0; k < this.HrefList.Count; k++) {
            value = this.HrefList[k];
            value.bounds.Clear();

            if (value.StartIndex < vbo.Count) {
                Vector3 position = vbo[value.StartIndex].position;
                Bounds bounds = new Bounds(position, Vector3.zero);
                int l = value.StartIndex;
                int endIndex = value.EndIndex;
                while (l < endIndex) {
                    if (l >= vbo.Count) {
                        break;
                    }
                    position = vbo[l].position;
                    if (position.x < bounds.min.x) {
                        this.HrefList[k].bounds.Add(new Rect(bounds.min.x, bounds.min.y, bounds.size.x, bounds.size.y));
                        bounds = new Bounds(position, Vector3.zero);
                    }
                    else {
                        bounds.Encapsulate(position);
                    }
                    l++;
                }
                this.HrefList[k].bounds.Add(new Rect(bounds.min.x, bounds.min.y, bounds.size.x, bounds.size.y));
            }
        }
    }
}