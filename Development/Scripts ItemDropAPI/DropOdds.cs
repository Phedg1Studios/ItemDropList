using System;
using System.Collections.Generic;
using UnityEngine;
using RoR2;

namespace Phedg1Studios {
    namespace ItemDropAPI {
        public class DropOdds : MonoBehaviour {
            static public void UpdateChestTierOdds(SpawnCard spawnCard, string interactableName) {
                if (InteractableCalculator.chestInteractables.Contains(interactableName)) {
                    ChestBehavior chestBehavior = spawnCard.prefab.GetComponent<ChestBehavior>();
                    if (!ItemDropAPI.playerInteractables.chestTierOdds.ContainsKey(interactableName)) {
                        ItemDropAPI.playerInteractables.chestTierOdds.Add(interactableName, new List<float>());
                        ItemDropAPI.playerInteractables.chestTierOdds[interactableName].Add(chestBehavior.tier1Chance);
                        ItemDropAPI.playerInteractables.chestTierOdds[interactableName].Add(chestBehavior.tier2Chance);
                        ItemDropAPI.playerInteractables.chestTierOdds[interactableName].Add(chestBehavior.tier3Chance);
                    }

                    if (ItemDropAPI.playerInteractables.chestTierOdds.ContainsKey(interactableName)) {
                        chestBehavior.tier1Chance = ItemDropAPI.playerInteractables.chestTierOdds[interactableName][0];
                        chestBehavior.tier2Chance = ItemDropAPI.playerInteractables.chestTierOdds[interactableName][1];
                        chestBehavior.tier3Chance = ItemDropAPI.playerInteractables.chestTierOdds[interactableName][2];
                    }
                    if (ItemDropAPI.playerInteractables.subsetTiersPresent.ContainsKey(interactableName)) {
                        if (!ItemDropAPI.playerInteractables.subsetTiersPresent[interactableName]["tier1"]) {
                            chestBehavior.tier1Chance = 0;
                        }
                        if (!ItemDropAPI.playerInteractables.subsetTiersPresent[interactableName]["tier2"]) {
                            chestBehavior.tier2Chance = 0;
                        }
                        if (!ItemDropAPI.playerInteractables.subsetTiersPresent[interactableName]["tier3"]) {
                            chestBehavior.tier3Chance = 0;
                        }
                    } else {
                        if (!ItemDropAPI.playerInteractables.tiersPresent["tier1"]) {
                            chestBehavior.tier1Chance = 0;
                        }
                        if (!ItemDropAPI.playerInteractables.tiersPresent["tier2"]) {
                            chestBehavior.tier2Chance = 0;
                        }
                        if (!ItemDropAPI.playerInteractables.tiersPresent["tier3"]) {
                            chestBehavior.tier3Chance = 0;
                        }
                    }
                }
            }

            static public void UpdateShrineTierOdds(DirectorCard directorCard, string interactableName) {
                if (InteractableCalculator.shrineInteractables.Contains(interactableName)) {
                    ShrineChanceBehavior shrineBehavior = directorCard.spawnCard.prefab.GetComponent<ShrineChanceBehavior>();
                    if (!ItemDropAPI.playerInteractables.shrineTierOdds.ContainsKey(interactableName)) {
                        ItemDropAPI.playerInteractables.shrineTierOdds.Add(interactableName, new List<float>());
                        ItemDropAPI.playerInteractables.shrineTierOdds[interactableName].Add(shrineBehavior.tier1Weight);
                        ItemDropAPI.playerInteractables.shrineTierOdds[interactableName].Add(shrineBehavior.tier2Weight);
                        ItemDropAPI.playerInteractables.shrineTierOdds[interactableName].Add(shrineBehavior.tier3Weight);
                        ItemDropAPI.playerInteractables.shrineTierOdds[interactableName].Add(shrineBehavior.equipmentWeight);
                    }

                    if (ItemDropAPI.playerInteractables.shrineTierOdds.ContainsKey(interactableName)) {
                        shrineBehavior.tier1Weight = ItemDropAPI.playerInteractables.shrineTierOdds[interactableName][0];
                        shrineBehavior.tier2Weight = ItemDropAPI.playerInteractables.shrineTierOdds[interactableName][1];
                        shrineBehavior.tier3Weight = ItemDropAPI.playerInteractables.shrineTierOdds[interactableName][2];
                        shrineBehavior.equipmentWeight = ItemDropAPI.playerInteractables.shrineTierOdds[interactableName][3];
                    }

                    if (!ItemDropAPI.playerInteractables.tiersPresent["tier1"]) {
                        shrineBehavior.tier1Weight = 0;
                    }
                    if (!ItemDropAPI.playerInteractables.tiersPresent["tier2"]) {
                        shrineBehavior.tier2Weight = 0;
                    }
                    if (!ItemDropAPI.playerInteractables.tiersPresent["tier3"]) {
                        shrineBehavior.tier3Weight = 0;
                    }
                    if (!ItemDropAPI.playerInteractables.tiersPresent["equipment"]) {
                        shrineBehavior.equipmentWeight = 0;
                    }
                }
            }

            static public void UpdateDropTableTierOdds(SpawnCard spawnCard, string interactableName) {
                if (InteractableCalculator.dropTableInteractables.Contains(interactableName)) {
                    BasicPickupDropTable dropTable = spawnCard.prefab.GetComponent<RouletteChestController>().dropTable as BasicPickupDropTable;
                    if (!ItemDropAPI.playerInteractables.dropTableTierOdds.ContainsKey(interactableName)) {
                        ItemDropAPI.playerInteractables.dropTableTierOdds.Add(interactableName, new List<float>());
                        ItemDropAPI.playerInteractables.dropTableTierOdds[interactableName].Add(dropTable.tier1Weight);
                        ItemDropAPI.playerInteractables.dropTableTierOdds[interactableName].Add(dropTable.tier2Weight);
                        ItemDropAPI.playerInteractables.dropTableTierOdds[interactableName].Add(dropTable.tier3Weight);
                        ItemDropAPI.playerInteractables.dropTableTierOdds[interactableName].Add(dropTable.equipmentWeight);
                    }
                    if (ItemDropAPI.playerInteractables.dropTableTierOdds.ContainsKey(interactableName)) {
                        dropTable.tier1Weight = ItemDropAPI.playerInteractables.dropTableTierOdds[interactableName][0];
                        dropTable.tier2Weight = ItemDropAPI.playerInteractables.dropTableTierOdds[interactableName][1];
                        dropTable.tier3Weight = ItemDropAPI.playerInteractables.dropTableTierOdds[interactableName][2];
                        dropTable.equipmentWeight = ItemDropAPI.playerInteractables.dropTableTierOdds[interactableName][3];
                    }
                    if (!ItemDropAPI.playerInteractables.tiersPresent["tier1"]) {
                        dropTable.tier1Weight = 0;
                    }
                    if (!ItemDropAPI.playerInteractables.tiersPresent["tier2"]) {
                        dropTable.tier2Weight = 0;
                    }
                    if (!ItemDropAPI.playerInteractables.tiersPresent["tier3"]) {
                        dropTable.tier3Weight = 0;
                    }
                    if (!ItemDropAPI.playerInteractables.tiersPresent["equipment"]) {
                        dropTable.equipmentWeight = 0;
                    }
                    System.Type type = typeof(RoR2.BasicPickupDropTable);
                    System.Reflection.MethodInfo method = type.GetMethod("GenerateWeightedSelection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    method.Invoke(dropTable, new object[] { Run.instance });
                }
            }

            static public void UpdateDropTableItemOdds(ExplicitPickupDropTable dropTable, string interactableName) {
                if (InteractableCalculator.dropTableItemInteractables.Contains(interactableName)) {
                    if (!ItemDropAPI.playerInteractables.dropTableItemOdds.ContainsKey(interactableName)) {
                        ItemDropAPI.playerInteractables.dropTableItemOdds.Add(interactableName, new List<float>());
                        foreach (ExplicitPickupDropTable.Entry entry in dropTable.entries) {
                            ItemDropAPI.playerInteractables.dropTableItemOdds[interactableName].Add(entry.pickupWeight);
                        }
                    }

                    if (ItemDropAPI.playerInteractables.dropTableItemOdds.ContainsKey(interactableName)) {
                        for (int entryIndex = 0; entryIndex < dropTable.entries.Length; entryIndex++) {

                            dropTable.entries[entryIndex].pickupWeight = ItemDropAPI.playerInteractables.dropTableItemOdds[interactableName][entryIndex];
                        }
                    }
                    for (int entryIndex = 0; entryIndex < dropTable.entries.Length; entryIndex++) {
                        if (!ItemDropAPI.playerItems.Contains(PickupCatalog.FindPickupIndex(dropTable.entries[entryIndex].pickupName))) {
                            dropTable.entries[entryIndex].pickupWeight = 0;
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
