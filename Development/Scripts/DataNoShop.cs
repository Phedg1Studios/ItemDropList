using System;
using System.IO;
using System.Collections.Generic;
using RoR2;
using UnityEngine;
using R2API;

namespace Phedg1Studios {
    namespace ItemDropList {
        public class DataNoShop : MonoBehaviour {
            static public readonly int mode = 0;

            static public List<int> itemsToDrop = new List<int>();
            static private string itemsToDropFile = "ItemsToDrop.txt";
            //static public int itemsToDropLine = 0;

            static public List<string> itemsToDropName = new List<string>() { "itemsToDrop", "0" };

            static public void RefreshInfo(Dictionary<string, string> configGlobal, Dictionary<string, string> configProfile) {
                GetConfig(configGlobal);
                Data.GetItemList(configProfile, itemsToDropName, itemsToDrop, itemsToDropFile, mode);
            }

            static void GetConfig(Dictionary<string, string> config) {

            }


            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


            static public void ToggleItem(int givenID) {
                if (Data.mode == mode) {
                    if (itemsToDrop.Contains(givenID)) {
                        itemsToDrop.Remove(givenID);
                        Data.SaveProfileConfig();
                        UIDrawer.Refresh();
                    } else {
                        itemsToDrop.Add(givenID);
                        Data.SaveProfileConfig();
                        UIDrawer.Refresh();
                    }
                }
            }

            static public void SetDropList() {
                if (Data.mode == mode) {
                    Data.itemsToDrop = Data.DuplicateItemList(itemsToDrop);
                }
            }
        }
    }
}
