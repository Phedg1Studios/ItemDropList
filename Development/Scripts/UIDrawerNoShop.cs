using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RoR2;

namespace Phedg1Studios {
    namespace ItemDropList {
        public class UIDrawerNoShop : MonoBehaviour {
            static public void DrawUI() {
                if (Data.mode == DataNoShop.mode) {

                }
            }


            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


            static public void Refresh() {
                if (Data.mode == DataNoShop.mode) {
                    foreach (int itemID in UIDrawer.itemImages.Keys) {
                        if (DataNoShop.itemsToDrop.Contains(itemID)) {
                            for (int imageIndex = 0; imageIndex < 2; imageIndex++) {
                                UIDrawer.itemImages[itemID][imageIndex].color = UIConfig.enabledColor;
                            }
                            for (int imageIndex = 0; imageIndex < 2; imageIndex++) {
                                UIDrawer.itemImages[itemID][imageIndex + 2].gameObject.SetActive(true);
                            }
                        } else {
                            for (int imageIndex = 0; imageIndex < 2; imageIndex++) {
                                UIDrawer.itemImages[itemID][imageIndex].color = UIConfig.disabledColor;
                            }
                            for (int imageIndex = 0; imageIndex < 2; imageIndex++) {
                                UIDrawer.itemImages[itemID][imageIndex + 2].gameObject.SetActive(false);
                            }
                        }
                    }
                }
            }
        }
    }
}
