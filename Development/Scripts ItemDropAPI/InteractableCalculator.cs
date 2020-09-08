using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;

namespace Phedg1Studios {
    namespace ItemDropAPI {
        public class InteractableCalculator : MonoBehaviour {
            static public int prefixLength = 3;
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
            static public List<string> allTiersMustBePresent = new List<string>() {
                "ShrineCleanse",
            };
            static public Dictionary<string, ItemTier> tierConversion = new Dictionary<string, ItemTier>() {
                { "tier1", ItemTier.Tier1 },
                { "tier2", ItemTier.Tier2 },
                { "tier3", ItemTier.Tier3 },
                { "boss", ItemTier.Boss },
            };

            public List<string> interactablesInvalid = new List<string>();
            public Dictionary<string, List<float>> chestTierOdds = new Dictionary<string, List<float>>();
            public Dictionary<string, List<float>> shrineTierOdds = new Dictionary<string, List<float>>();
            public Dictionary<string, List<float>> dropTableTierOdds = new Dictionary<string, List<float>>();
            public Dictionary<string, List<float>> dropTableItemOdds = new Dictionary<string, List<float>>();
            private List<string> subsetChests = new List<string>() {
                "CategoryChestDamage",
                "CategoryChestHealing",
                "CategoryChestUtility",
            };
            public Dictionary<string, Dictionary<string, bool>> subsetTiersPresent = new Dictionary<string, Dictionary<string, bool>>() {
               
            };
            public Dictionary<string, bool> tiersPresent = new Dictionary<string, bool>() {
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
            public Dictionary<string, Dictionary<string, bool>> interactablesTiers = new Dictionary<string, Dictionary<string, bool>>() {
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

            static public string GetSpawnCardName(RoR2.SpawnCard givenSpawncard) {
                return givenSpawncard.name.Substring(prefixLength, givenSpawncard.name.Length - prefixLength);
            }

            public void CalculateInvalidInteractables(List<PickupIndex> itemsToDrop) {
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
                foreach (PickupIndex pickupIndex in itemsToDrop) {
                    PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
                    if (pickupDef.itemIndex != ItemIndex.None) {
                        if (!Catalogue.scrapItems.ContainsValue(pickupDef.itemIndex)) {
                            if (RoR2.ItemCatalog.tier1ItemList.Contains(pickupDef.itemIndex)) {
                                tiersPresent["tier1"] = true;
                            } else if (RoR2.ItemCatalog.tier2ItemList.Contains(pickupDef.itemIndex)) {
                                tiersPresent["tier2"] = true;
                            } else if (RoR2.ItemCatalog.tier3ItemList.Contains(pickupDef.itemIndex)) {
                                tiersPresent["tier3"] = true;
                            } else if (Catalogue.pearls.Contains(pickupDef.itemIndex)) {
                                tiersPresent["pearl"] = true;
                            } else if (Catalogue.bossItems.Contains(pickupDef.itemIndex)) {
                                tiersPresent["boss"] = true;
                            } else if (RoR2.ItemCatalog.lunarItemList.Contains(pickupDef.itemIndex)) {
                                tiersPresent["lunar"] = true;
                            }

                            if (!RoR2.ItemCatalog.lunarItemList.Contains(pickupDef.itemIndex) && !Catalogue.bossItems.Contains(pickupDef.itemIndex) && !Catalogue.pearls.Contains(pickupDef.itemIndex)) {
                                RoR2.ItemDef itemDef = RoR2.ItemCatalog.GetItemDef(pickupDef.itemIndex);
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
                                        if (RoR2.ItemCatalog.tier1ItemList.Contains(pickupDef.itemIndex)) {
                                            subsetTiersPresent[interactableName]["tier1"] = true;
                                        } else if (RoR2.ItemCatalog.tier2ItemList.Contains(pickupDef.itemIndex)) {
                                            subsetTiersPresent[interactableName]["tier2"] = true;
                                        } else if (RoR2.ItemCatalog.tier3ItemList.Contains(pickupDef.itemIndex)) {
                                            subsetTiersPresent[interactableName]["tier3"] = true;
                                        }
                                    }
                                }
                            }
                        }
                    } else if (pickupDef.equipmentIndex != EquipmentIndex.None) {
                        if (!Catalogue.eliteEquipment.Contains(pickupDef.equipmentIndex)) {
                            RoR2.EquipmentDef equipmentDef = RoR2.EquipmentCatalog.GetEquipmentDef(pickupDef.equipmentIndex);
                            if (equipmentDef.isLunar) {
                                tiersPresent["lunar"] = true;
                            } else if (equipmentDef.isBoss) {

                            } else {
                                tiersPresent["equipment"] = true;
                            }
                            
                        }
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
                        if (!itemsToDrop.Contains(PickupCatalog.FindPickupIndex(Catalogue.scrapItems[tierConversion[tier]]))) {
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
            }
        }
    }
}
