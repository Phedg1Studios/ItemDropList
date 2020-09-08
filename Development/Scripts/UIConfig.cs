using System;
using System.Collections.Generic;
using UnityEngine;

namespace Phedg1Studios {
    namespace ItemDropList {
        public class UIConfig : MonoBehaviour {
            static public float offsetVertical = 25;
            static public float offsetHorizontal = 75f;
            static public float spacingVertical = 75f / 4f;
            static public float spacingHorizontal = 75f / 4f;
            static public float panelPadding = 4;
            static public float scrollPadding = 10;
            static public float itemButtonWidth = 100;
            static public float itemButtonHeight = 100;
            static public float itemPaddingOuter = 4;
            static public float itemPaddingInner = 1;
            static public float itemSelectionPadding = 3;
            static public float itemTextHeight = 25;
            static public List<int> displayRows = new List<int>() { 8, 6, 8 };
            static public List<int> textCount = new List<int>() { 0, 1, 0 };
            static public List<int> blueButtonCount = new List<int>() { 1, 1 };
            static public float blueButtonWidth = 200;
            static public float blueButtonHeight = 48f;
            static public float blackButtonWidth = 200;
            static public float blackButtonHeight = 48f;
            static public Color enabledColor = new Color(1, 1, 1, 1);
            static public Color disabledColor = new Color(0.4f, 0.4f, 0.4f, 1);
            static public List<Color> tierColours = new List<Color>() {
                new Color(255f / 255f, 255f / 255f, 255f / 255f, 1),
                new Color(102f / 255f, 238f / 255f, 2f / 255f, 1),
                new Color(231f / 255f, 85f / 255f, 58f / 255f, 1),
                new Color(255f / 255f, 235f / 255f, 0f / 255f, 1),
                new Color(50f / 255f, 127f / 255f, 255f / 255f, 1),
                new Color(255f / 255f, 128f / 255f, 0f / 255f, 1),
                new Color(145f / 255f, 7f / 255f, 226f / 255f, 1),
            };
            static public int GetMode() {
                if (Data.mode == DataNoShop.mode) {
                    return Data.mode;
                } else {//if (Data.mode == DataShop.mode) {
                    return DataShop.mode + DataShop.shopMode;
                }
            }
            static public List<int> blackButtons = new List<int>() { 1, 1 };
        }
    }
}
