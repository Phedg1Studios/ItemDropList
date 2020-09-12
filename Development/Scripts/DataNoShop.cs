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

            static public bool discoveredRequiredDefault = true;

            static public List<List<int>> itemsToDrop = new List<List<int>>();
            static private string itemsToDropFile = "ItemsToDrop.txt";

            static public List<string> itemsToDropName = new List<string>() { "itemsToDrop", "0" };
            static public List<string> discoveredRequiredName = new List<string>() { "discoveredRequired" };

            static public void RefreshInfo(Dictionary<string, string> configGlobal, Dictionary<string, string> configProfile) {
                GetConfig(configGlobal);
                Data.GetItemList(configProfile, itemsToDropName, itemsToDrop, itemsToDropFile, mode);
            }

            static void GetConfig(Dictionary<string, string> config) {
                Data.discoveredRequired[mode] = Data.ParseBool(discoveredRequiredDefault, Util.GetConfig(config, discoveredRequiredName));
            }


            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


            static public void ToggleItem(int givenID, bool shouldRefresh = true) {
                if (Data.mode == mode) {
                    if (itemsToDrop[Data.profile[mode]].Contains(givenID)) {
                        itemsToDrop[Data.profile[mode]].Remove(givenID);
                        if (shouldRefresh) {
                            Data.SaveConfigProfile();
                            UIDrawer.Refresh();
                        }
                    } else {
                        itemsToDrop[Data.profile[mode]].Add(givenID);
                        if (shouldRefresh) {
                            Data.SaveConfigProfile();
                            UIDrawer.Refresh();
                        }
                    }
                }
            }

            static public void SetDropList() {
                if (Data.mode == mode) {
                    Data.itemsToDrop = Data.DuplicateItemList(itemsToDrop[Data.profile[mode]]);
                }
            }
        }
    }
}
