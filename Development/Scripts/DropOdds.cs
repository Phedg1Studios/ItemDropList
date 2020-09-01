using System;
using System.Collections.Generic;
using UnityEngine;
using RoR2;

namespace Phedg1Studios {
    namespace ItemDropList {
        public class DropOdds : MonoBehaviour {
            static public void UpdateChestTierOdds(SpawnCard spawnCard, string interactableName) {
                if (InteractableCalculator.chestInteractables.Contains(interactableName)) {
                    ChestBehavior chestBehavior = spawnCard.prefab.GetComponent<ChestBehavior>();
                    if (Data.modEnabled && !InteractableCalculator.chestTierOdds.ContainsKey(interactableName)) {
                        InteractableCalculator.chestTierOdds.Add(interactableName, new List<float>());
                        InteractableCalculator.chestTierOdds[interactableName].Add(chestBehavior.tier1Chance);
                        InteractableCalculator.chestTierOdds[interactableName].Add(chestBehavior.tier2Chance);
                        InteractableCalculator.chestTierOdds[interactableName].Add(chestBehavior.tier3Chance);
                    }

                    if (InteractableCalculator.chestTierOdds.ContainsKey(interactableName)) {
                        chestBehavior.tier1Chance = InteractableCalculator.chestTierOdds[interactableName][0];
                        chestBehavior.tier2Chance = InteractableCalculator.chestTierOdds[interactableName][1];
                        chestBehavior.tier3Chance = InteractableCalculator.chestTierOdds[interactableName][2];
                    }
                    if (Data.modEnabled) {
                        if (InteractableCalculator.subsetTiersPresent.ContainsKey(interactableName)) {
                            if (!InteractableCalculator.subsetTiersPresent[interactableName]["tier1"]) {
                                chestBehavior.tier1Chance = 0;
                            }
                            if (!InteractableCalculator.subsetTiersPresent[interactableName]["tier2"]) {
                                chestBehavior.tier2Chance = 0;
                            }
                            if (!InteractableCalculator.subsetTiersPresent[interactableName]["tier3"]) {
                                chestBehavior.tier3Chance = 0;
                            }
                        } else {
                            if (!InteractableCalculator.tiersPresent["tier1"]) {
                                chestBehavior.tier1Chance = 0;
                            }
                            if (!InteractableCalculator.tiersPresent["tier2"]) {
                                chestBehavior.tier2Chance = 0;
                            }
                            if (!InteractableCalculator.tiersPresent["tier3"]) {
                                chestBehavior.tier3Chance = 0;
                            }
                        }
                    }
                }
            }

            static public void UpdateShrineTierOdds(DirectorCard directorCard, string interactableName) {
                if (InteractableCalculator.shrineInteractables.Contains(interactableName)) {
                    ShrineChanceBehavior shrineBehavior = directorCard.spawnCard.prefab.GetComponent<ShrineChanceBehavior>();
                    if (Data.modEnabled && !InteractableCalculator.shrineTierOdds.ContainsKey(interactableName)) {
                        InteractableCalculator.shrineTierOdds.Add(interactableName, new List<float>());
                        InteractableCalculator.shrineTierOdds[interactableName].Add(shrineBehavior.tier1Weight);
                        InteractableCalculator.shrineTierOdds[interactableName].Add(shrineBehavior.tier2Weight);
                        InteractableCalculator.shrineTierOdds[interactableName].Add(shrineBehavior.tier3Weight);
                        InteractableCalculator.shrineTierOdds[interactableName].Add(shrineBehavior.equipmentWeight);
                    }

                    if (InteractableCalculator.shrineTierOdds.ContainsKey(interactableName)) {
                        shrineBehavior.tier1Weight = InteractableCalculator.shrineTierOdds[interactableName][0];
                        shrineBehavior.tier2Weight = InteractableCalculator.shrineTierOdds[interactableName][1];
                        shrineBehavior.tier3Weight = InteractableCalculator.shrineTierOdds[interactableName][2];
                        shrineBehavior.equipmentWeight = InteractableCalculator.shrineTierOdds[interactableName][3];
                    }
                    if (Data.modEnabled) {
                        if (!InteractableCalculator.tiersPresent["tier1"]) {
                            shrineBehavior.tier1Weight = 0;
                        }
                        if (!InteractableCalculator.tiersPresent["tier2"]) {
                            shrineBehavior.tier2Weight = 0;
                        }
                        if (!InteractableCalculator.tiersPresent["tier3"]) {
                            shrineBehavior.tier3Weight = 0;
                        }
                        if (!InteractableCalculator.tiersPresent["equipment"]) {
                            shrineBehavior.equipmentWeight = 0;
                        }
                    }
                }
            }

            static public void UpdateDropTableTierOdds(SpawnCard spawnCard, string interactableName) {
                if (InteractableCalculator.dropTableInteractables.Contains(interactableName)) {
                    BasicPickupDropTable dropTable = spawnCard.prefab.GetComponent<RouletteChestController>().dropTable as BasicPickupDropTable;
                    if (Data.modEnabled && !InteractableCalculator.dropTableTierOdds.ContainsKey(interactableName)) {
                        InteractableCalculator.dropTableTierOdds.Add(interactableName, new List<float>());
                        InteractableCalculator.dropTableTierOdds[interactableName].Add(dropTable.tier1Weight);
                        InteractableCalculator.dropTableTierOdds[interactableName].Add(dropTable.tier2Weight);
                        InteractableCalculator.dropTableTierOdds[interactableName].Add(dropTable.tier3Weight);
                        InteractableCalculator.dropTableTierOdds[interactableName].Add(dropTable.equipmentWeight);
                    }
                    if (InteractableCalculator.dropTableTierOdds.ContainsKey(interactableName)) {
                        dropTable.tier1Weight = InteractableCalculator.dropTableTierOdds[interactableName][0];
                        dropTable.tier2Weight = InteractableCalculator.dropTableTierOdds[interactableName][1];
                        dropTable.tier3Weight = InteractableCalculator.dropTableTierOdds[interactableName][2];
                        dropTable.equipmentWeight = InteractableCalculator.dropTableTierOdds[interactableName][3];
                    }
                    if (Data.modEnabled) {
                        if (!InteractableCalculator.tiersPresent["tier1"]) {
                            dropTable.tier1Weight = 0;
                        }
                        if (!InteractableCalculator.tiersPresent["tier2"]) {
                            dropTable.tier2Weight = 0;
                        }
                        if (!InteractableCalculator.tiersPresent["tier3"]) {
                            dropTable.tier3Weight = 0;
                        }
                        if (!InteractableCalculator.tiersPresent["equipment"]) {
                            dropTable.equipmentWeight = 0;
                        }
                    }
                    System.Type type = typeof(RoR2.BasicPickupDropTable);
                    System.Reflection.MethodInfo method = type.GetMethod("GenerateWeightedSelection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    method.Invoke(dropTable, new object[] { Run.instance });
                }
            }

            static public void UpdateDropTableItemOdds(ExplicitPickupDropTable dropTable, string interactableName) {
                if (InteractableCalculator.dropTableItemInteractables.Contains(interactableName)) {
                    if (Data.modEnabled && !InteractableCalculator.dropTableItemOdds.ContainsKey(interactableName)) {
                        InteractableCalculator.dropTableItemOdds.Add(interactableName, new List<float>());
                        foreach (ExplicitPickupDropTable.Entry entry in dropTable.entries) {
                            InteractableCalculator.dropTableItemOdds[interactableName].Add(entry.pickupWeight);
                        }
                    }

                    if (InteractableCalculator.dropTableItemOdds.ContainsKey(interactableName)) {
                        for (int entryIndex = 0; entryIndex < dropTable.entries.Length; entryIndex++) {

                            dropTable.entries[entryIndex].pickupWeight = InteractableCalculator.dropTableItemOdds[interactableName][entryIndex];
                        }
                    }
                    if (Data.modEnabled) {
                        for (int entryIndex = 0; entryIndex < dropTable.entries.Length; entryIndex++) {
                            if (!Data.itemsToDrop.Contains(Data.allItemsIndexes[PickupCatalog.FindPickupIndex(dropTable.entries[entryIndex].pickupName).itemIndex])) {
                                dropTable.entries[entryIndex].pickupWeight = 0;
                            }
                        }
                    }
                    System.Type type = typeof(RoR2.ExplicitPickupDropTable);
                    System.Reflection.MethodInfo method = type.GetMethod("GenerateWeightedSelection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    method.Invoke(dropTable, new object[0]);
                }
            }
        }
	}
}
