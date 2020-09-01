using System;
using System.Collections.Generic;
using UnityEngine;
using RoR2;

namespace Phedg1Studios {
    namespace ItemDropList {
        public class DropList : MonoBehaviour {
            static private float smallChestTier1 = 0.8f;
            static private float smallChestTier2 = 0.2f;
            static private float smallChestTier3 = 0.01f;
            static private float mediumChestTier2 = 0.8f;
            static private float mediumChestTier3 = 0.2f;

            static public List<PickupIndex> tier1DropList = new List<PickupIndex>();
            static public List<PickupIndex> tier2DropList = new List<PickupIndex>();
            static public List<PickupIndex> tier3DropList = new List<PickupIndex>();
            static public List<PickupIndex> equipmentDropList = new List<PickupIndex>();

            static private List<PickupIndex> tier1DropListBackup = new List<PickupIndex>();
            static private List<PickupIndex> tier2DropListBackup = new List<PickupIndex>();
            static private List<PickupIndex> tier3DropListBackup = new List<PickupIndex>();
            static private List<PickupIndex> equipmentDropListBackup = new List<PickupIndex>();

            static private void DuplicateDropList(List<PickupIndex> original, List<PickupIndex> backup) {
                backup.Clear();
                foreach (PickupIndex pickupIndex in original) {
                    backup.Add(pickupIndex);
                }
            }

            static public void SetDropLists(List<PickupIndex> givenTier1, List<PickupIndex> givenTier2, List<PickupIndex> givenTier3, List<PickupIndex> givenEquipment) {
                if (Data.modEnabled) {
                    List<PickupIndex> none = new List<PickupIndex>() { PickupIndex.none };
                    List<List<PickupIndex>> availableItems = new List<List<PickupIndex>>() { new List<PickupIndex>(), new List<PickupIndex>(), new List<PickupIndex>(), new List<PickupIndex>() };
                    DuplicateDropList(givenTier1, availableItems[0]);
                    DuplicateDropList(givenTier2, availableItems[1]);
                    DuplicateDropList(givenTier3, availableItems[2]);
                    DuplicateDropList(givenEquipment, availableItems[3]);
                    for (int availableIndex = 0; availableIndex < 4; availableIndex++) {
                        if (availableItems[availableIndex].Count == 0) {
                            availableItems[availableIndex] = none;
                        }
                    }

                    DuplicateDropList(Run.instance.availableTier1DropList, tier1DropListBackup);
                    DuplicateDropList(Run.instance.availableTier2DropList, tier2DropListBackup);
                    DuplicateDropList(Run.instance.availableTier3DropList, tier3DropListBackup);
                    DuplicateDropList(Run.instance.availableEquipmentDropList, equipmentDropListBackup);
                    System.Type type = typeof(RoR2.Run);
                    System.Reflection.FieldInfo tier1 = type.GetField("availableTier1DropList", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    System.Reflection.FieldInfo tier2 = type.GetField("availableTier2DropList", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    System.Reflection.FieldInfo tier3 = type.GetField("availableTier3DropList", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    System.Reflection.FieldInfo equipment = type.GetField("availableEquipmentDropList", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    tier1.SetValue(RoR2.Run.instance, availableItems[0]);
                    tier2.SetValue(RoR2.Run.instance, availableItems[1]);
                    tier3.SetValue(RoR2.Run.instance, availableItems[2]);
                    equipment.SetValue(RoR2.Run.instance, availableItems[3]);
                }
            }

            static public void RevertDropLists() {
                if (Data.modEnabled) {
                    System.Type type = typeof(RoR2.Run);
                    System.Reflection.FieldInfo tier1 = type.GetField("availableTier1DropList", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    System.Reflection.FieldInfo tier2 = type.GetField("availableTier2DropList", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    System.Reflection.FieldInfo tier3 = type.GetField("availableTier3DropList", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    System.Reflection.FieldInfo equipment = type.GetField("availableEquipmentDropList", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                    List<List<PickupIndex>> oldItems = new List<List<PickupIndex>>() { new List<PickupIndex>(), new List<PickupIndex>(), new List<PickupIndex>(), new List<PickupIndex>() };
                    DuplicateDropList(tier1DropListBackup, oldItems[0]);
                    DuplicateDropList(tier2DropListBackup, oldItems[1]);
                    DuplicateDropList(tier3DropListBackup, oldItems[2]);
                    DuplicateDropList(equipmentDropListBackup, oldItems[3]);

                    tier1.SetValue(RoR2.Run.instance, oldItems[0]);
                    tier2.SetValue(RoR2.Run.instance, oldItems[1]);
                    tier3.SetValue(RoR2.Run.instance, oldItems[2]);
                    equipment.SetValue(RoR2.Run.instance, oldItems[3]);
                }
            }

            static public  void ClearAllLists(RoR2.Run self) {
                for (int choiceIndex = 0; choiceIndex < self.smallChestDropTierSelector.Count; choiceIndex++) {
                    if (self.smallChestDropTierSelector.GetChoice(choiceIndex).value.Count > 0) {
                        if (self.smallChestDropTierSelector.GetChoice(choiceIndex).value[0].itemIndex != null) {
                            ItemTier itemTier = RoR2.ItemCatalog.GetItemDef(self.smallChestDropTierSelector.GetChoice(choiceIndex).value[0].itemIndex).tier;
                            if (itemTier == ItemTier.Tier1) {
                                smallChestTier1 = self.smallChestDropTierSelector.GetChoice(choiceIndex).weight;
                            } else if (itemTier == ItemTier.Tier2) {
                                smallChestTier2 = self.smallChestDropTierSelector.GetChoice(choiceIndex).weight;
                            } else if (itemTier == ItemTier.Tier3) {
                                smallChestTier3 = self.smallChestDropTierSelector.GetChoice(choiceIndex).weight;
                            }
                        }
                    }
                }
                for (int choiceIndex = 0; choiceIndex < self.mediumChestDropTierSelector.Count; choiceIndex++) {
                    if (self.mediumChestDropTierSelector.GetChoice(choiceIndex).value.Count > 0) {
                        if (self.mediumChestDropTierSelector.GetChoice(choiceIndex).value[0].itemIndex != null) {
                            ItemTier itemTier = RoR2.ItemCatalog.GetItemDef(self.mediumChestDropTierSelector.GetChoice(choiceIndex).value[0].itemIndex).tier;
                            if (itemTier == ItemTier.Tier2) {
                                mediumChestTier2 = self.mediumChestDropTierSelector.GetChoice(choiceIndex).weight;
                            } else if (itemTier == ItemTier.Tier3) {
                                mediumChestTier3 = self.mediumChestDropTierSelector.GetChoice(choiceIndex).weight;
                            }
                        }
                    }
                }

                DuplicateDropList(self.availableTier1DropList, tier1DropList);
                DuplicateDropList(self.availableTier2DropList, tier2DropList);
                DuplicateDropList(self.availableTier3DropList, tier3DropList);
                DuplicateDropList(self.availableEquipmentDropList, equipmentDropList);

                self.availableTier1DropList.Clear();
                self.availableTier2DropList.Clear();
                self.availableTier3DropList.Clear();
                self.availableBossDropList.Clear();
                self.availableLunarDropList.Clear();
                self.availableEquipmentDropList.Clear();

                self.smallChestDropTierSelector.Clear();
                self.mediumChestDropTierSelector.Clear();
                self.largeChestDropTierSelector.Clear();
            }

            static public void SetItems(RoR2.Run self) {
                foreach (ItemIndex itemIndex in RoR2.ItemCatalog.tier1ItemList) {
                    if (!Data.scrapItems.Contains(itemIndex)) {
                        if (Data.itemsToDrop.Contains(Data.allItemsIndexes[itemIndex])) {
                            self.availableTier1DropList.Add(new PickupIndex(itemIndex));
                        }
                    }
                }
                foreach (ItemIndex itemIndex in RoR2.ItemCatalog.tier2ItemList) {
                    if (!Data.scrapItems.Contains(itemIndex)) {
                        if (Data.itemsToDrop.Contains(Data.allItemsIndexes[itemIndex])) {
                            self.availableTier2DropList.Add(new PickupIndex(itemIndex));
                        }
                    }
                }
                foreach (ItemIndex itemIndex in RoR2.ItemCatalog.tier3ItemList) {
                    if (!Data.scrapItems.Contains(itemIndex)) {
                        if (Data.itemsToDrop.Contains(Data.allItemsIndexes[itemIndex])) {
                            self.availableTier3DropList.Add(new PickupIndex(itemIndex));
                        }
                    }
                }
                foreach (ItemIndex itemIndex in Data.bossItems) {
                    if (!Data.scrapItems.Contains(itemIndex)) {
                        if (Data.itemsToDrop.Contains(Data.allItemsIndexes[itemIndex])) {
                            self.availableBossDropList.Add(new PickupIndex(itemIndex));
                        }
                    }
                }
                foreach (ItemIndex itemIndex in RoR2.ItemCatalog.lunarItemList) {
                    if (Data.itemsToDrop.Contains(Data.allItemsIndexes[itemIndex])) {
                        self.availableLunarDropList.Add(new PickupIndex(itemIndex));
                    }
                }
                foreach (EquipmentIndex equipmentIndex in RoR2.EquipmentCatalog.equipmentList) {
                    if (!Data.lunarEquipment.Contains(equipmentIndex) && Data.itemsToDrop.Contains(Data.allEquipmentIndexes[equipmentIndex])) {
                        self.availableEquipmentDropList.Add(new PickupIndex(equipmentIndex));
                    }
                }
                foreach (EquipmentIndex equipmentIndex in Data.lunarEquipment) {
                    if (Data.itemsToDrop.Contains(Data.allEquipmentIndexes[equipmentIndex])) {
                        self.availableLunarDropList.Add(new PickupIndex(equipmentIndex));
                    }
                }
                foreach (EquipmentIndex equipmentIndex in Data.eliteEquipment) {
                    if (Data.itemsToDrop.Contains(Data.allEquipmentIndexes[equipmentIndex])) {
                        //self.availableEquipmentDropList.Add(new PickupIndex(equipmentIndex));
                    }
                }

                foreach (PickupIndex pickupIndex in self.availableTier1DropList) {
                    self.availableItems.AddItem(pickupIndex.itemIndex);
                }
                foreach (PickupIndex pickupIndex in self.availableTier2DropList) {
                    self.availableItems.AddItem(pickupIndex.itemIndex);
                }
                foreach (PickupIndex pickupIndex in self.availableTier3DropList) {
                    self.availableItems.AddItem(pickupIndex.itemIndex);
                }
                foreach (PickupIndex pickupIndex in self.availableBossDropList) {
                    self.availableItems.AddItem(pickupIndex.itemIndex);
                }
                foreach (PickupIndex pickupIndex in self.availableLunarDropList) {
                    self.availableItems.AddItem(pickupIndex.itemIndex);
                }
                foreach (PickupIndex pickupIndex in self.availableEquipmentDropList) {
                    self.availableEquipment.AddEquipment(pickupIndex.equipmentIndex);
                }

                R2API.DefaultItemDrops.AddDefaults();
                AddChestChoices(self);

                if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.funkfrog_sipondo.sharesuite")) {
                    AddressShareSuite(self);
                }
            }

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining | System.Runtime.CompilerServices.MethodImplOptions.NoOptimization)]
            static public void AddressShareSuite(Run self) {
                System.Type type = typeof(ShareSuite.Blacklist);

                System.Reflection.FieldInfo tier1 = type.GetField("_availableTier1DropList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                System.Reflection.FieldInfo tier2 = type.GetField("_availableTier2DropList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                System.Reflection.FieldInfo tier3 = type.GetField("_availableTier3DropList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                System.Reflection.FieldInfo lunar = type.GetField("_availableLunarDropList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                System.Reflection.FieldInfo boss = type.GetField("_availableBossDropList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                System.Reflection.FieldInfo available = type.GetField("_cachedAvailableItems", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                tier1.SetValue(RoR2.Run.instance, self.availableTier1DropList);
                tier2.SetValue(RoR2.Run.instance, self.availableTier2DropList);
                tier3.SetValue(RoR2.Run.instance, self.availableTier3DropList);
                lunar.SetValue(RoR2.Run.instance, self.availableLunarDropList);
                boss.SetValue(RoR2.Run.instance, self.availableBossDropList);
                available.SetValue(RoR2.Run.instance, self.availableItems);
            }

            static public void AddChestChoices(RoR2.Run self) {
                float smallChestTotal = 0;
                float mediumChestTotal = 0;
                if (self.availableTier1DropList.Count > 0) {
                    smallChestTotal += smallChestTier1;
                }
                if (self.availableTier2DropList.Count > 0) {
                    smallChestTotal += smallChestTier2;
                    mediumChestTotal += mediumChestTier2;
                }
                if (self.availableTier3DropList.Count > 0) {
                    smallChestTotal += smallChestTier3;
                    mediumChestTotal += mediumChestTier3;
                }

                if (self.availableTier1DropList.Count > 0) {
                    self.smallChestDropTierSelector.AddChoice(self.availableTier1DropList, smallChestTier1 / smallChestTotal);
                }
                if (self.availableTier2DropList.Count > 0) {
                    self.smallChestDropTierSelector.AddChoice(self.availableTier2DropList, smallChestTier2 / smallChestTotal);
                    self.mediumChestDropTierSelector.AddChoice(self.availableTier2DropList, mediumChestTier2 / mediumChestTotal);
                }
                if (self.availableTier3DropList.Count > 0) {
                    self.smallChestDropTierSelector.AddChoice(self.availableTier3DropList, smallChestTier3 / smallChestTotal);
                    self.mediumChestDropTierSelector.AddChoice(self.availableTier3DropList, mediumChestTier3 / mediumChestTotal);
                }
            }
        }
    }
}
