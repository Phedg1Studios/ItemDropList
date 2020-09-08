using System;
using System.IO;
using System.Collections.Generic;
using RoR2;
using UnityEngine;
using R2API;

namespace Phedg1Studios {
    namespace ItemDropList {
        public class DataShop : MonoBehaviour {
            static public readonly int mode = 1;
            static public int shopMode = 0;
            static public readonly int shopModeCount = 2;
            static public bool canDisablePurchasedBlueprints;
            //static public float scrapDropChance = 1;

            static public bool canDisablePurchasedBlueprintsDefault = false;
            static public float maxScrapMultiplierDefault = 3;
            static public List<int> pricesDefault = new List<int>() { 90, 55, 8, 2, 6, 8, 40 };

            static public float maxScrapMultiplier;
            static public List<int> scrap = new List<int>() { 0, 0, 0, 0, 0, 0, 0 };
            static public List<int> scrapRecent = new List<int>() { 0, 0, 0, 0, 0, 0, 0 };
            static public List<int> scrapStarting = new List<int>() { 0, 0, 0, 0, 0, 0, 0 };
            static public List<int> prices = new List<int>() { 0, 0, 0, 0, 0, 0, 0 };
            static public List<int> blueprintsPurchased = new List<int>();
            static public List<List<int>> itemsToDrop = new List<List<int>>();

            static private string scrapFile = "Scrap.txt";
            static private string blueprintsPurchasedFile = "BlueprintsPurchased.txt";
            static public string itemsToDropFile = "ItemsToDropShop.txt";

            static public List<string> toggleItemsName = new List<string>() { "canOptOutOfTraining" };
            static public List<string> maxScoreMultiplierName = new List<string>() { "maxAptitudeScoreMultiplier" };
            static public List<string> tier1PriceName = new List<string>() { "tier1Price" };
            static public List<string> tier2PriceName = new List<string>() { "tier2Price" };
            static public List<string> tier3PriceName = new List<string>() { "tier3Price" };
            static public List<string> tierBossPriceName = new List<string>() { "bossPrice" };
            static public List<string> tierLunarPriceName = new List<string>() { "lunarPrice" };
            static public List<string> tierEquipmentPriceName = new List<string>() { "equipmentPrice" };
            static public List<string> tierDronePriceName = new List<string>() { "dronePrice" };

            static public List<string> itemsPurchasedName = new List<string>() { "itemsPurchased", "1" };
            static public List<string> itemsToDropName = new List<string>() { "itemsToDropShop", "2" };
            static public List<string> scrapName = new List<string>() { "score", "3" };
            static public List<string> scrapRecentName = new List<string>() { "scoreRecent", "4" };
            static public List<string> scrapStartingName = new List<string>() { "scoreStarting", "5" };

            /*
            static public int blueprintsPurchasedLine = 1;
            static public int itemsToDropLine = 2;
            static public int scrapLine = 3;
            static public int scrapRecentLine = 4;
            static public int scrapStartingLine = 5;
            */

            static public int purchaseTier = 0;
            static public int purchaseCost = 0;
            static public Dictionary<CostTypeIndex, int> costTypeTier = new Dictionary<CostTypeIndex, int>() {
                { CostTypeIndex.WhiteItem, 0 },
                { CostTypeIndex.GreenItem, 1 },
                { CostTypeIndex.RedItem, 2 },
                { CostTypeIndex.BossItem, 3 },
                { CostTypeIndex.LunarItemOrEquipment, 4},
                { CostTypeIndex.Equipment, 5},
            };

            static public List<string> duplicatorNames = new List<string>() {
                "Duplicator(Clone)",
                "DuplicatorLarge(Clone)",
                "DuplicatorMilitary(Clone)",
                "DuplicatorWild(Clone)",
            };
            static public List<string> cauldronNames = new List<string>() {
                "LunarCauldron, GreenToRed",
                "LunarCauldron, WhiteToGreen",
            };


            static private List<List<float>> itemsClicked = new List<List<float>>();
            static private float doubleClickWindow = 0.25f;

            static private List<string> scrapTiers = new List<string>() {
                "tier1",
                "tier2",
                "tier3",
                "boss",
                "lunar",
                "equipment",
                "drone",
            };

            static public void RefreshInfo(Dictionary<string, string> configGlobal, Dictionary<string, string> configProfile) {
                GetConfig(configGlobal);
                List<List<int>> nestedList = new List<List<int>>();
                Data.GetItemList(configProfile, itemsPurchasedName, nestedList, blueprintsPurchasedFile, mode);
                blueprintsPurchased = nestedList[0];
                GetScrap(configProfile, scrapName, scrap);
                GetScrap(configProfile, scrapRecentName, scrapRecent);
                GetScrap(configProfile, scrapStartingName, scrapStarting);
                AdjustScrap();
                NormalizeScrap();
                Data.GetItemList(configProfile, itemsToDropName, itemsToDrop, itemsToDropFile, mode);
                AdjustItemDrops();
            }

            static void GetConfig(Dictionary<string, string> config) {
                canDisablePurchasedBlueprints = Data.ParseBool(canDisablePurchasedBlueprintsDefault, Util.GetConfig(config, toggleItemsName));
                maxScrapMultiplier = Data.ParseFloat(maxScrapMultiplierDefault, Util.GetConfig(config, maxScoreMultiplierName));
                prices[0] = Data.ParseInt(pricesDefault[0], Util.GetConfig(config, tier1PriceName));
                prices[1] = Data.ParseInt(pricesDefault[1], Util.GetConfig(config, tier2PriceName));
                prices[2] = Data.ParseInt(pricesDefault[2], Util.GetConfig(config, tier3PriceName));
                prices[3] = Data.ParseInt(pricesDefault[3], Util.GetConfig(config, tierBossPriceName));
                prices[4] = Data.ParseInt(pricesDefault[4], Util.GetConfig(config, tierLunarPriceName));
                prices[5] = Data.ParseInt(pricesDefault[5], Util.GetConfig(config, tierEquipmentPriceName));
                prices[6] = Data.ParseInt(pricesDefault[6], Util.GetConfig(config, tierDronePriceName));
            }

            static void GetScrap(Dictionary<string, string> config, List<string> configName, List<int> scrapList) {
                string line = Util.GetConfig(config, configName);
                string scrapPath = BepInEx.Paths.BepInExRootPath + "/" + "config" + "/" + Data.modFolder + "/" + Data.userProfile + "/" + scrapFile;
                line = Util.MultilineToSingleLine(line, scrapPath);
                string[] scrapNew = line.Split(Data.splitChar);
                int tierIndex = 0;
                foreach (string scrapCountString in scrapNew) {
                    if (!string.IsNullOrEmpty(scrapCountString)) {
                        int scrapCount = 0;
                        bool scrapCountParsed = int.TryParse(scrapCountString, out scrapCount);
                        if (scrapCountParsed) {
                            if (scrapList.Count > tierIndex) {
                                scrapList[tierIndex] = scrapCount;
                            }
                        }
                    }
                    tierIndex += 1;
                }
            }

            static void AdjustScrap() {
                ItemDropAPI.InteractableCalculator interactableCalculator = new ItemDropAPI.InteractableCalculator();
                interactableCalculator.CalculateInvalidInteractables(Data.ConvertDropList(blueprintsPurchased));
                for (int tierIndex = 0; tierIndex < scrapTiers.Count; tierIndex++) {
                    if (!interactableCalculator.tiersPresent[scrapTiers[tierIndex]]) {
                        if (scrap[tierIndex] < prices[tierIndex]) {
                            scrap[tierIndex] = prices[tierIndex];
                        }
                    }
                }
            }

            static void AdjustItemDrops() {
                List<int> removeIndexes = new List<int>();
                for (int profileIndex = 0; profileIndex < Data.profileCount; profileIndex++) {
                    for (int dropIndex = 0; dropIndex < itemsToDrop[profileIndex].Count; dropIndex++) {
                        if (!blueprintsPurchased.Contains(itemsToDrop[profileIndex][dropIndex])) {
                            removeIndexes.Add(dropIndex);
                        }
                    }
                    removeIndexes.Reverse();
                    foreach (int dropIndex in removeIndexes) {
                        itemsToDrop[profileIndex].RemoveAt(dropIndex);
                    }
                }
            }


            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            static public void SetShopMode(int givenMode) {
                if (shopMode != givenMode) {
                    shopMode = givenMode;
                    UIDrawer.DrawUI();
                }
            }

            static public void ToggleItem(int givenID, bool shouldRefresh = true) {
                if (Data.mode == mode) {
                    if (shopMode == 0) {
                        if (!blueprintsPurchased.Contains(givenID)) {
                            itemsClicked.Add(new List<float>() { givenID, Time.time });
                            if (itemsClicked.Count == 2) {
                                if (itemsClicked[0][0] == itemsClicked[1][0] && itemsClicked[1][1] - itemsClicked[0][1] <= doubleClickWindow) {
                                    int itemTier = Data.GetItemTier(givenID);
                                    if (scrap[itemTier] >= prices[itemTier]) {
                                        blueprintsPurchased.Add(givenID);
                                        scrap[itemTier] -= prices[itemTier];
                                        SetScrapStarting();
                                        UIDrawerShop.PurchaseItem(givenID);
                                        if (shouldRefresh) {
                                            Data.SaveConfigProfile();
                                            UIDrawer.Refresh();
                                        }
                                    }
                                }
                                itemsClicked.RemoveAt(0);
                            }
                        }
                    } else if (shopMode == 1) {
                        if (canDisablePurchasedBlueprints) {
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
                }
            }

            static public bool AddScrap(CharacterBody characterBody, int itemTier) {
                //bool result = Data.CoinToss(scrapDropChance);
                //if (true) {//result) {
                scrap[itemTier] += 1;
                scrapRecent[itemTier] += 1;
                Data.SaveConfigProfile();
                //Chat.AddPickupMessage(characterBody, "Scrap", (Color32)UIConfig.tierColours[itemTier], (uint)scrap[itemTier]);
                //}
                return true;
            }

            static public void RemoveScrap(int itemTier, int amount) {
                scrap[itemTier] -= amount;
                scrapRecent[itemTier] = scrapRecent[itemTier] - amount;
                Data.SaveConfigProfile();
            }

            static public bool RecentScrap() {
                foreach (int scrapCount in scrapRecent) {
                    if (scrapCount > 0) {
                        return true;
                    }
                }
                return false;
            }

            static public void ClearRecentScrap() {
                for (int scrapIndex = 0; scrapIndex < scrapRecent.Count; scrapIndex++) {
                    scrapRecent[scrapIndex] = 0;
                }
                Data.SaveConfigProfile();
            }

            static public void NormalizeScrap() {
                for (int tierIndex = 0; tierIndex < scrap.Count; tierIndex++) {
                    int maximumScrap = Mathf.RoundToInt(maxScrapMultiplier * prices[tierIndex]);
                    if (maxScrapMultiplier < 1) {
                        maximumScrap = 1000000;
                    }
                    int scrapOld = scrap[tierIndex];
                    scrap[tierIndex] = Mathf.Max(scrapStarting[tierIndex], Mathf.Min(maximumScrap, scrap[tierIndex]));
                    int scrapDifference = scrap[tierIndex] - scrapOld;
                    scrapRecent[tierIndex] = Mathf.Max(0, Mathf.Min(maximumScrap, scrapRecent[tierIndex] + scrapDifference));
                }
            }

            static public void SetScrapStarting() {
                scrapStarting = Data.DuplicateItemList(scrap);
            }

            static public void SetDropList() {
                if (Data.mode == mode) {
                    if (!canDisablePurchasedBlueprints) {
                        Data.itemsToDrop = Data.DuplicateItemList(blueprintsPurchased);
                    } else {
                        List<int> itemsToDropAdjusted = Data.DuplicateItemList(blueprintsPurchased);
                        foreach (int itemID in itemsToDrop[Data.profile[mode]]) {
                            if (itemsToDropAdjusted.Contains(itemID)) {
                                itemsToDropAdjusted.Remove(itemID);
                            }
                        }
                        Data.itemsToDrop = itemsToDropAdjusted;
                    }
                }
            }
        }
    }
}
