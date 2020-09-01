using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Phedg1Studios {
    namespace ItemDropList {
        public class InteractableCalculator : MonoBehaviour {
            static public int prefixLength = 3;
            static public Dictionary<string, List<float>> chestTierOdds = new Dictionary<string, List<float>>();
            static public Dictionary<string, List<float>> shrineTierOdds = new Dictionary<string, List<float>>();
            static public Dictionary<string, List<float>> dropTableTierOdds = new Dictionary<string, List<float>>();
            static public Dictionary<string, List<float>> dropTableItemOdds = new Dictionary<string, List<float>>();
            static public List<string> chestInteractables = new List<string>() {
                "Chest1",
                "Chest2",
                "CategoryChestDamage",
                "CategoryChestHealing",
                "CategoryChestUtility",
                "Chest1Stealthed",
                "Lockbox",
                "ScavBackpack",
            };

            static public List<string> shrineInteractables = new List<string>() {
                "ShrineChance",
            };
            static public List<string> dropTableInteractables = new List<string>() {
                "CasinoChest",
            };
            static public List<string> dropTableItemInteractables = new List<string>() {
                "ShrineCleanse",
            };
            static private List<string> subsetChests = new List<string>() {
                "CategoryChestDamage",
                "CategoryChestHealing",
                "CategoryChestUtility",
            };
            static public Dictionary<string, Dictionary<string, bool>> subsetTiersPresent = new Dictionary<string, Dictionary<string, bool>>() {
               
            };
            static public Dictionary<string, bool> tiersPresent = new Dictionary<string, bool>() {
                { "tier1", false },
                { "tier2", false },
                { "tier3", false },
                { "boss", false },
                { "lunar", false },
                { "equipment", false },
                { "lunarEquipment", false },
                { "damage", false },
                { "healing", false },
                { "utility", false },
                { "pearl", false },
                { "drone", false },
            };
            static public Dictionary<string, int> tierConversion = new Dictionary<string, int>() {
                { "tier1", 0 },
                { "tier2", 1 },
                { "tier3", 2 },
                { "boss", 3 },
            };
            static public Dictionary<string, Dictionary<string, bool>> interactablesTiers = new Dictionary<string, Dictionary<string, bool>>() {
                { "Chest1", new Dictionary<string, bool>() {
                    { "tier1", false },
                    //{ "tier2", false },
                    //{ "tier3", false },
                }},
                { "Chest2", new Dictionary<string, bool>() {
                    { "tier2", false },
                    //{ "tier3", false },
                }},
                { "EquipmentBarrel", new Dictionary<string, bool>() {
                    { "equipment", false },
                }},
                { "TripleShop", new Dictionary<string, bool>() {
                    { "tier1", false },
                }},
                { "LunarChest", new Dictionary<string, bool>() {
                    { "lunar", false },
                }},
                { "TripleShopLarge", new Dictionary<string, bool>() {
                    { "tier2", false },
                }},
                { "CategoryChestDamage", new Dictionary<string, bool>() {
                    { "damage", false },
                }},
                { "CategoryChestHealing", new Dictionary<string, bool>() {
                    { "healing", false },
                }},
                { "CategoryChestUtility", new Dictionary<string, bool>() {
                    { "utility", false },
                }},
                { "ShrineChance", new Dictionary<string, bool>() {
                    { "tier1", false },
                    { "tier2", false },
                    { "tier3", false },
                    { "equipment", false},
                }},
                { "ShrineCleanse", new Dictionary<string, bool>() {
                    { "lunar", false },
                    { "pearl", false}
                }},
                { "ShrineRestack", new Dictionary<string, bool>() {
                    { "tier1", false },
                    { "tier2", false },
                    { "tier3", false },
                    { "boss", false },
                    { "lunar", false },
                }},
                { "TripleShopEquipment", new Dictionary<string, bool>() {
                    { "equipment", false },
                }},
                { "BrokenEquipmentDrone", new Dictionary<string, bool>() {
                    { "equipment", false },
                }},
                { "Chest1Stealthed", new Dictionary<string, bool>() {
                    { "tier1", false },
                    { "tier2", false },
                    { "tier3", false },
                }},
                { "GoldChest", new Dictionary<string, bool>() {
                    { "tier3", false },
                }},
                { "Scrapper", new Dictionary<string, bool>() {
                    { "tier1", false },
                    { "tier2", false },
                    { "tier3", false },
                    { "boss", false },
                }},
                { "Duplicator", new Dictionary<string, bool>() {
                    { "tier1", false },
                }},
                { "DuplicatorLarge", new Dictionary<string, bool>() {
                    { "tier2", false },
                }},
                { "DuplicatorMilitary", new Dictionary<string, bool>() {
                    { "tier3", false },
                }},
                { "DuplicatorWild", new Dictionary<string, bool>() {
                    { "boss", false },
                }},
                { "ScavBackpack", new Dictionary<string, bool>() {
                    { "tier1", false },
                    { "tier2", false },
                    { "tier3", false },
                }},
                { "CasinoChest", new Dictionary<string, bool>() {
                    { "tier1", false },
                    { "tier2", false },
                    { "tier3", false },
                    { "equipment", false },
                }},
            };
            static public List<string> invalidIfNotFirst = new List<string>() {
                "Chest1",
                "Chest2",
            };
            static public List<string> allTiersMustBePresent = new List<string>() {
                "ShrineCleanse",
            };
            static public List<string> interactablesInvalid = new List<string>();


            static public string GetSpawnCardName(RoR2.SpawnCard givenSpawncard) {
                return givenSpawncard.name.Substring(InteractableCalculator.prefixLength, givenSpawncard.name.Length - prefixLength);
            }

            static public void CalculateInvalidInteractables(List<int> itemsToDrop) {
                List<string> tiersPresentKeys = tiersPresent.Keys.ToList();
                foreach (string tier in tiersPresentKeys) {
                    tiersPresent[tier] = false;
                }
                subsetTiersPresent.Clear();
                foreach (string subsetChest in subsetChests) {
                    subsetTiersPresent.Add(subsetChest, new Dictionary<string, bool>());
                    foreach (string tier in tiersPresentKeys) {
                        subsetTiersPresent[subsetChest].Add(tier, false);
                    }
                }
                foreach (int itemID in itemsToDrop) {
                    if (Data.allItemIDs.ContainsKey(itemID)) {
                        if (!Data.scrapItems.Contains(Data.allItemIDs[itemID])) {
                            if (RoR2.ItemCatalog.tier1ItemList.Contains(Data.allItemIDs[itemID])) {
                                tiersPresent["tier1"] = true;
                            } else if (RoR2.ItemCatalog.tier2ItemList.Contains(Data.allItemIDs[itemID])) {
                                tiersPresent["tier2"] = true;
                            } else if (RoR2.ItemCatalog.tier3ItemList.Contains(Data.allItemIDs[itemID])) {
                                tiersPresent["tier3"] = true;
                            } else if (Data.pearls.Contains(Data.allItemIDs[itemID])) {
                                tiersPresent["pearl"] = true;
                            } else if (Data.bossItems.Contains(Data.allItemIDs[itemID])) {
                                tiersPresent["boss"] = true;
                            } else if (RoR2.ItemCatalog.lunarItemList.Contains(Data.allItemIDs[itemID])) {
                                tiersPresent["lunar"] = true;
                            }

                            if (!RoR2.ItemCatalog.lunarItemList.Contains(Data.allItemIDs[itemID]) && !Data.bossItems.Contains(Data.allItemIDs[itemID]) && !Data.pearls.Contains(Data.allItemIDs[itemID])) {
                                RoR2.ItemDef itemDef = RoR2.ItemCatalog.GetItemDef(Data.allItemIDs[itemID]);
                                foreach (RoR2.ItemTag itemTag in itemDef.tags) {
                                    string interactableName = "";
                                    if (itemTag == RoR2.ItemTag.Damage) {
                                        tiersPresent["damage"] = true;
                                        interactableName = "CategoryChestDamage";
                                    } else if (itemTag == RoR2.ItemTag.Healing) {
                                        tiersPresent["healing"] = true;
                                        interactableName = "CategoryChestHealing";
                                    } else if (itemTag == RoR2.ItemTag.Utility) {
                                        tiersPresent["utility"] = true;
                                        interactableName = "CategoryChestUtility";
                                    }
                                    if (subsetChests.Contains(interactableName)) {
                                        if (RoR2.ItemCatalog.tier1ItemList.Contains(Data.allItemIDs[itemID])) {
                                            subsetTiersPresent[interactableName]["tier1"] = true;
                                        } else if (RoR2.ItemCatalog.tier2ItemList.Contains(Data.allItemIDs[itemID])) {
                                            subsetTiersPresent[interactableName]["tier2"] = true;
                                        } else if (RoR2.ItemCatalog.tier3ItemList.Contains(Data.allItemIDs[itemID])) {
                                            subsetTiersPresent[interactableName]["tier3"] = true;
                                        }
                                    }
                                }
                            }
                        }
                    } else if (Data.allEquipmentIDs.ContainsKey(itemID)) {
                        if (!Data.eliteEquipment.Contains(Data.allEquipmentIDs[itemID])) {
                            RoR2.EquipmentDef equipmentDef = RoR2.EquipmentCatalog.GetEquipmentDef(Data.allEquipmentIDs[itemID]);
                            if (equipmentDef.isLunar) {
                                tiersPresent["lunar"] = true;
                            } else if (equipmentDef.isBoss) {

                            } else {
                                tiersPresent["equipment"] = true;
                            }
                            
                        }
                    } else if (Data.allDroneIDs.ContainsKey(itemID)) {
                        tiersPresent["drone"] = true;
                    }
                }
                List<string> interactableTypeKeys = interactablesTiers.Keys.ToList();
                foreach (string interactableType in interactableTypeKeys) {
                    List<string> interactableTypeTierKeys = interactablesTiers[interactableType].Keys.ToList();
                    foreach (string tier in interactableTypeTierKeys) {
                        interactablesTiers[interactableType][tier] = false;
                    }
                }
                foreach (string tier in tiersPresent.Keys) {
                    if (tiersPresent[tier]) {
                        foreach (string interactableType in interactableTypeKeys) {
                            if (interactablesTiers[interactableType].ContainsKey(tier)) {
                                interactablesTiers[interactableType][tier] = true;
                            }
                        }
                    }
                }
                List<string> scrapTierKeys = interactablesTiers["Scrapper"].Keys.ToList();
                foreach (string tier in scrapTierKeys) {
                    if (interactablesTiers["Scrapper"][tier]) {
                        if (!itemsToDrop.Contains(Data.allItemsIndexes[Data.GetScrapIndex(tierConversion[tier])])) {
                            interactablesTiers["Scrapper"][tier] = false;
                        }
                    }
                }

                interactablesInvalid.Clear();
                foreach (string interactableType in interactablesTiers.Keys) {
                    bool interactableValid = false;
                    bool allTrue = true;
                    foreach (string tier in interactablesTiers[interactableType].Keys) {
                        if (interactablesTiers[interactableType][tier]) {
                            interactableValid = true;
                        } else {
                            allTrue = false;
                        }
                    }
                    if (!interactableValid || (allTiersMustBePresent.Contains(interactableType) && !allTrue)) {
                        interactablesInvalid.Add(interactableType);
                    }
                }
                foreach (int droneID in Data.allDroneIDs.Keys) {
                    if (!itemsToDrop.Contains(droneID)) {
                        if (!interactablesInvalid.Contains(Data.allDroneIDs[droneID])) {
                            interactablesInvalid.Add(Data.allDroneIDs[droneID]);
                        }
                    }
                }
            }
        }
    }
}
