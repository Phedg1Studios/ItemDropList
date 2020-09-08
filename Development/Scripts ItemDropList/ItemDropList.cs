using BepInEx;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MonoMod.Cil;
using Mono.Cecil.Cil;


namespace Phedg1Studios {
    namespace ItemDropList {
        [BepInDependency("com.bepis.r2api")]
        [R2API.Utils.R2APISubmoduleDependency("PrefabAPI")]
        [R2API.Utils.R2APISubmoduleDependency("ResourcesAPI")]
        [BepInDependency("com.Phedg1Studios.ItemDropAPI")]
        [BepInPlugin(PluginGUID, "ItemDropList", "1.2.2")]

        public class ItemDropList : BaseUnityPlugin {
            public const string PluginGUID = "com.Phedg1Studios.ItemDropList";

            static public ItemDropList itemDropList;
            List<Coroutine> characterMasterCoroutines = new List<Coroutine>();
            private int stageClearCountOld = -1;
            private List<EquipmentState> latestEquipment = new List<EquipmentState>();
            private List<EquipmentIndex> equipmentFoundThisStage = new List<EquipmentIndex>();
            private List<EquipmentIndex> equipmentFoundThisStageTwice = new List<EquipmentIndex>();
            Inventory inventoryLocal = null;
            CharacterBody characterBody = null;

            void OnceSetup() {
                Data.UpdateConfigLocations();
                gameObject.AddComponent<Util>();
                Resources.LoadResources();
                SceneLoadSetup();
                Data.PopulateItemCatalogues();
                ItemDropAPI.ItemDropAPI.setDropLists += Data.SetDropList;
            }

            void SceneLoadSetup() {
                UIDrawer.CreateCanvas();
                UIVanilla.GetObjectsFromScene();
                UIVanilla.CreateMenuButton();
            }

            void Start() {
                itemDropList = this;
                OnceSetup();
                //util.LogAllInteractables();

                UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, mode) => {
                    if (scene.name == "title") {
                        SceneLoadSetup();
                    }
                };
                On.RoR2.Run.BuildDropTable += ((orig, self) => {
                    Data.RefreshInfo();
                    if (Data.modEnabled) {
                        DataShop.SetScrapStarting();
                        Data.SaveConfigProfile();
                    }
                    orig(self);
                });
                On.RoR2.SceneDirector.PopulateScene += ((orig, sceneDirector) => {
                    if (Data.modEnabled) {
                        sceneDirector.interactableCredit = Mathf.FloorToInt(sceneDirector.interactableCredit * Data.interactableMultiplier);

                        List<SpawnCard> spawnCards = new List<SpawnCard>();
                        List<string> spawnCardNames = new List<string>();
                        //spawnCardNames = new List<string>() { "ShrineChance" };
                        //spawnCardNames = new List<string>() { "BrokenDrone1", "BrokenDrone2", "BrokenEmergencyDrone", "BrokenEquipmentDrone", "BrokenFlameDrone", "BrokenMegaDrone", "BrokenMissileDrone", "BrokenTurret1" };
                        //spawnCardNames = new List<string>() { "Duplicator", "DuplicatorLarge", "DuplicatorMilitary", "DuplicatorWild" };
                        //spawnCardNames = new List<string>() { "EquipmentBarrel", "TripleShopEquipment" };

                        RoR2.InteractableSpawnCard[] allInteractables = UnityEngine.Resources.LoadAll<RoR2.InteractableSpawnCard>("SpawnCards/InteractableSpawnCard");
                        foreach (RoR2.InteractableSpawnCard spawnCard in allInteractables) {
                            string interactableName = ItemDropAPI.InteractableCalculator.GetSpawnCardName(spawnCard);
                            if (spawnCardNames.Contains(interactableName)) {
                                spawnCards.Add(spawnCard);
                            }
                        }

                        if (spawnCardNames.Count > 0) {
                            DirectorCard[] directorCards = new DirectorCard[spawnCards.Count];
                            for (int cardIndex = 0; cardIndex < spawnCards.Count; cardIndex++) {
                                DirectorCard directorCard = new DirectorCard();
                                directorCard.spawnCard = spawnCards[cardIndex];
                                directorCard.selectionWeight = 1;
                                directorCards[cardIndex] = directorCard;
                            }

                            DirectorCardCategorySelection.Category category = new DirectorCardCategorySelection.Category();
                            category.name = "FORCED";
                            category.cards = directorCards;
                            category.selectionWeight = 1000;

                            DirectorCardCategorySelection.Category[] categoriesAdjusted = new DirectorCardCategorySelection.Category[ClassicStageInfo.instance.interactableCategories.categories.Length + 1];
                            for (int categoryIndex = 0; categoryIndex < ClassicStageInfo.instance.interactableCategories.categories.Length; categoryIndex++) {
                                categoriesAdjusted[categoryIndex] = ClassicStageInfo.instance.interactableCategories.categories[categoryIndex];
                            }
                            categoriesAdjusted[categoriesAdjusted.Length - 1] = category;
                            ClassicStageInfo.instance.interactableCategories.categories = categoriesAdjusted;
                        };

                        foreach (int key in Data.allDroneIDs.Keys) {
                            if (!Data.itemsToDrop.Contains(key) && !ItemDropAPI.ItemDropAPI.playerInteractables.interactablesInvalid.Contains(Data.allDroneIDs[key])) {
                                ItemDropAPI.ItemDropAPI.playerInteractables.interactablesInvalid.Add(Data.allDroneIDs[key]);
                            }
                        }
                    }
                    orig(sceneDirector);
                });
                On.RoR2.UI.ScrollToSelection.ScrollToRect += (scrollToRect, scrollToSelection, transform) => {
                    scrollToRect(scrollToSelection, transform);

                    ScrollRect scrollRect = scrollToSelection.GetComponent<ScrollRect>();
                    if (!scrollRect.horizontal || !(bool)(UnityEngine.Object)scrollRect.horizontalScrollbar) {
                        return;
                    }
                    Vector3[] targetWorldCorners = new Vector3[4];
                    Vector3[] viewPortWorldCorners = new Vector3[4];
                    Vector3[] contentWorldCorners = new Vector3[4];
                    scrollToSelection.GetComponent<RoR2.UI.MPEventSystemLocator>().eventSystem.currentSelectedGameObject.GetComponent<RectTransform>().GetWorldCorners(targetWorldCorners);
                    scrollRect.viewport.GetWorldCorners(viewPortWorldCorners);
                    scrollRect.content.GetWorldCorners(contentWorldCorners);
                    float x5 = targetWorldCorners[2].x;
                    double x6 = (double)targetWorldCorners[0].x;
                    float x7 = viewPortWorldCorners[2].x;
                    float x8 = viewPortWorldCorners[0].x;
                    float x9 = contentWorldCorners[2].x;
                    float x10 = contentWorldCorners[0].x;
                    float num5 = x5 - x7;
                    double num6 = (double)x8;
                    float num7 = (float)(x6 - num6);
                    float num8 = (x9 - x10) - (x7 - x8);
                    if ((double)num5 > 0.0)
                        scrollRect.horizontalScrollbar.value += num5 / num8;
                    if ((double)num7 >= 0.0)
                        return;
                    scrollRect.horizontalScrollbar.value += num7 / num8;
                };
            }

            IEnumerator<float> GetMasterController(NetworkUser networkUser) {
                PlayerCharacterMasterController masterController = networkUser.masterController;
                while (masterController == null) {
                    masterController = networkUser.masterController;
                    yield return 0;
                }
                characterBody = masterController.master.GetBody();
                while (characterBody == null) {
                    characterBody = masterController.master.GetBody();
                    yield return 0;
                }
                if (stageClearCountOld == -1 || stageClearCountOld == 0) {
                    float startingTime = Time.time;
                    while (Time.time - startingTime < 2) {
                        yield return 0;
                    }
                }
                inventoryLocal = characterBody.inventory;
            }
        }
    }
}
 
 