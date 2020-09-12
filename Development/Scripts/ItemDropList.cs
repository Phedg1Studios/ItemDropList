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
        [BepInDependency("com.Phedg1Studios.ItemDropAPIFixes")]
        [BepInPlugin(PluginGUID, "ItemDropList", "1.2.6")]

        public class ItemDropList : BaseUnityPlugin {
            public const string PluginGUID = "com.Phedg1Studios.ItemDropList";

            static public ItemDropList itemDropList;
            List<Coroutine> characterMasterCoroutines = new List<Coroutine>();
            private int stageClearCountOld = -1;
            private string latestInteractionName = "";
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
                Phedg1Studios.ItemDropAPIFixes.ItemDropAPIFixes.setDropLists += Data.SetDropList;
                UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneLoaded;
                SetHooks();
            }

            void SceneLoadSetup() {
                UIDrawer.CreateCanvas();
                UIVanilla.GetObjectsFromScene();
                UIVanilla.CreateMenuButton();
            }

            public void SetHooks() {
                On.RoR2.Run.BuildDropTable += BuildDropTable;
                On.RoR2.SceneDirector.PopulateScene += PopulateScene;
                On.RoR2.Stage.Start += StageStart;
                On.RoR2.Inventory.RpcItemAdded += RpcItemAdded;
                On.RoR2.Inventory.SetEquipmentInternal += SetEquipmentInternal;
                On.RoR2.Interactor.AttemptInteraction += AttemptInteraction;
                On.RoR2.Interactor.RpcInteractionResult += InteractionResult;
                On.RoR2.PickupPickerController.SubmitChoice += SubmitChoice;
                On.RoR2.UI.ScrollToSelection.ScrollToRect += ScrollToRect;
            }

            public void UnsetHooks() {
                On.RoR2.Run.BuildDropTable -= BuildDropTable;
                On.RoR2.SceneDirector.PopulateScene -= PopulateScene;
                On.RoR2.Stage.Start -= StageStart;
                On.RoR2.Inventory.RpcItemAdded -= RpcItemAdded;
                On.RoR2.Inventory.SetEquipmentInternal -= SetEquipmentInternal;
                On.RoR2.Interactor.AttemptInteraction -= AttemptInteraction;
                On.RoR2.Interactor.RpcInteractionResult -= InteractionResult;
                On.RoR2.PickupPickerController.SubmitChoice -= SubmitChoice;
                On.RoR2.UI.ScrollToSelection.ScrollToRect -= ScrollToRect;
            }

            void Start() {
                itemDropList = this;
                OnceSetup();
                //util.LogAllInteractables();
            }

            static public void ToggleDropAPIHooks() {
                if (Data.modEnabled) {
                    //ItemDropAPI.ItemDropAPI.itemDropAPI.UnsetHooks();
                } else {
                    //ItemDropAPI.ItemDropAPI.itemDropAPI.SetHooks();
                }
            }


            //-------------------------
            //-------------------------


            void SceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode) {
                if (scene.name == "title") {
                    SceneLoadSetup();
                }
            }

            void BuildDropTable(On.RoR2.Run.orig_BuildDropTable orig, RoR2.Run run) {
                Data.RefreshInfo();
                if (Data.modEnabled) {
                    DataShop.SetScrapStarting();
                    Data.SaveConfigProfile();
                }
                orig(run);
            }

            void PopulateScene(On.RoR2.SceneDirector.orig_PopulateScene orig, RoR2.SceneDirector sceneDirector) {
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
                        string interactableName = ItemDropAPIFixes.InteractableCalculator.GetSpawnCardName(spawnCard);
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
                        if (!Data.itemsToDrop.Contains(key) && !ItemDropAPIFixes.ItemDropAPIFixes.playerInteractables.interactablesInvalid.Contains(Data.allDroneIDs[key])) {
                            ItemDropAPIFixes.ItemDropAPIFixes.playerInteractables.interactablesInvalid.Add(Data.allDroneIDs[key]);
                        }
                    }
                }
                orig(sceneDirector);
            }

            void StageStart(On.RoR2.Stage.orig_Start orig, RoR2.Stage stage) {
                orig(stage);
                if (Data.modEnabled) {
                    if (Data.mode == DataShop.mode) {
                        foreach (Coroutine coroutine in characterMasterCoroutines) {
                            if (coroutine != null) {
                                StopCoroutine(coroutine);
                            }
                        }
                        characterMasterCoroutines.Clear();
                        characterBody = null;
                        inventoryLocal = null;
                        latestInteractionName = "";
                        stageClearCountOld = -1;
                        if (NetworkClient.active) {
                            foreach (NetworkUser networkUser in RoR2.NetworkUser.readOnlyInstancesList) {
                                if (networkUser.isLocalPlayer) {
                                    characterMasterCoroutines.Add(StartCoroutine(GetMasterController(networkUser)));
                                }
                            }
                        }
                    }
                }
            }

            void RpcItemAdded(On.RoR2.Inventory.orig_RpcItemAdded orig, RoR2.Inventory inventory, ItemIndex itemIndex) {
                if (Data.modEnabled) {
                    if (Data.mode == DataShop.mode) {
                        if (Data.modEnabled) {
                            if (Data.mode == DataShop.mode) {
                                if (inventoryLocal != null) {
                                    if (inventoryLocal == inventory) {
                                        //if (!Data.scrapItems.Contains(itemIndex)) {
                                        DataShop.AddScrap(characterBody, Data.GetItemTier(Data.allItemsIndexes[itemIndex]));
                                        //}
                                    }
                                }
                            }
                        }
                    }
                }
                orig(inventory, itemIndex);
            }

            bool SetEquipmentInternal(On.RoR2.Inventory.orig_SetEquipmentInternal orig, RoR2.Inventory inventory, EquipmentState equipmentState, uint slot) {
                bool equipmentChanged = orig(inventory, equipmentState, slot);
                if (Data.modEnabled) {
                    if (Data.mode == DataShop.mode) {
                        if (inventoryLocal != null) {
                            if (inventoryLocal == inventory) {
                                int stageClearCountNew = Run.instance.stageClearCount;
                                if (stageClearCountOld != stageClearCountNew) {
                                    stageClearCountOld = stageClearCountNew;
                                    equipmentFoundThisStage.Clear();
                                    equipmentFoundThisStageTwice.Clear();
                                    foreach (EquipmentState equipmentStateOlder in latestEquipment) {
                                        if (equipmentStateOlder.equipmentIndex != EquipmentIndex.None && !equipmentFoundThisStage.Contains(equipmentStateOlder.equipmentIndex)) {
                                            equipmentFoundThisStage.Add(equipmentStateOlder.equipmentIndex);
                                        }
                                    }
                                }
                                int slotAdjusted = (int)slot;
                                if (slotAdjusted + 1 > latestEquipment.Count) {
                                    for (int appendIndex = 0; appendIndex < slotAdjusted + 1 - latestEquipment.Count; appendIndex++) {
                                        latestEquipment.Add(new EquipmentState(EquipmentIndex.None, new Run.FixedTimeStamp(), 0));
                                    }
                                }
                                EquipmentState equipmentStateOld = latestEquipment[slotAdjusted];
                                if (!EquipmentState.Equals(equipmentStateOld, equipmentState)) {
                                    latestEquipment[slotAdjusted] = equipmentState;
                                    if (!equipmentFoundThisStage.Contains(latestEquipment[slotAdjusted].equipmentIndex)) {
                                        if (Data.allEquipmentIndexes.ContainsKey(equipmentState.equipmentIndex)) {
                                            if (DataShop.AddScrap(characterBody, Data.GetItemTier(Data.allEquipmentIndexes[equipmentState.equipmentIndex]))) {
                                                equipmentFoundThisStage.Add(equipmentState.equipmentIndex);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return equipmentChanged;
            }

            void AttemptInteraction(On.RoR2.Interactor.orig_AttemptInteraction orig, RoR2.Interactor interactor, GameObject gameObject) {
                if (Data.modEnabled && interactor.GetComponent<NetworkBehaviour>().hasAuthority) {
                    if (Data.mode == DataShop.mode) {
                        latestInteractionName = gameObject.name;
                        DataShop.purchaseCost = 0;
                        DataShop.purchaseTier = -1;
                        if (gameObject.name.ToLower().Contains("duplicator")) {
                            Util.LogComponentsOfObject(gameObject);
                        }
                        if (gameObject.name == "Scrapper(Clone)") {
                            DataShop.purchaseCost = gameObject.GetComponent<ScrapperController>().maxItemsToScrapAtATime;
                        } else {
                            RoR2.PurchaseInteraction purchaseInteraction = purchaseInteraction = gameObject.GetComponent<RoR2.PurchaseInteraction>();
                            if (purchaseInteraction != null) {
                                if (DataShop.costTypeTier.ContainsKey(purchaseInteraction.costType)) {
                                    DataShop.purchaseTier = DataShop.costTypeTier[purchaseInteraction.costType];
                                    DataShop.purchaseCost = purchaseInteraction.cost;
                                }
                            }
                        }
                    }
                }
                orig(interactor, gameObject);
            }

            void InteractionResult(On.RoR2.Interactor.orig_RpcInteractionResult orig, RoR2.Interactor interactor, bool boolean) {
                if (Data.modEnabled) {
                    if (Data.mode == DataShop.mode) {
                        if (interactor.GetComponent<NetworkBehaviour>().hasAuthority) {
                            if (boolean) {
                                if (DataShop.purchaseTier != -1 && DataShop.purchaseCost > 0) {
                                    DataShop.RemoveScrap(DataShop.purchaseTier, DataShop.purchaseCost);
                                }

                                Dictionary<string, string> specialCases = new Dictionary<string, string>() {
                                    { "BrokenDroneMissile", "BrokenMissileDrone" },
                                };
                                string prefix = "Broken";
                                string suffix = "Master(Clone)";
                                string adjustedName = latestInteractionName;
                                if (adjustedName.Length > suffix.Length) {
                                    adjustedName = adjustedName.Substring(0, adjustedName.Length - suffix.Length);
                                }
                                adjustedName = prefix + adjustedName;
                                if (specialCases.ContainsKey(adjustedName)) {
                                    adjustedName = specialCases[adjustedName];
                                }
                                if (Data.allDroneNames.ContainsKey(adjustedName)) {
                                    DataShop.AddScrap(characterBody, 6);
                                }
                            }
                        }
                    }
                }
                orig(interactor, boolean);
            }

            void SubmitChoice(On.RoR2.PickupPickerController.orig_SubmitChoice orig, RoR2.PickupPickerController pickupPickerController, int integer) {
                if (Data.modEnabled) {
                    if (Data.mode == DataShop.mode) {
                        System.Type type = typeof(RoR2.PickupPickerController);
                        System.Reflection.FieldInfo info = type.GetField("options", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        ICollection collection = info.GetValue(pickupPickerController) as ICollection;
                        int index = 0;
                        foreach (object item in collection) {
                            if (index == integer) {
                                RoR2.PickupPickerController.Option option = (RoR2.PickupPickerController.Option)item;
                                ItemIndex itemIndex = RoR2.PickupCatalog.GetPickupDef(option.pickupIndex).itemIndex;

                                RoR2.NetworkUIPromptController promptController = pickupPickerController.GetComponent<RoR2.NetworkUIPromptController>();
                                type = typeof(RoR2.NetworkUIPromptController);
                                info = type.GetField("_currentLocalParticipant", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                                RoR2.LocalUser localUser = (RoR2.LocalUser)info.GetValue(promptController);
                                foreach (NetworkUser networkUser in RoR2.NetworkUser.readOnlyInstancesList) {
                                    if (localUser.currentNetworkUser.netId == networkUser.netId) {
                                        if (networkUser.isLocalPlayer) {
                                            DataShop.RemoveScrap(Data.GetItemTier(Data.allItemsIndexes[itemIndex]), Mathf.Min(DataShop.purchaseCost, networkUser.GetCurrentBody().inventory.GetItemCount(itemIndex)));
                                        }
                                        break;
                                    }
                                }
                                break;
                            }
                            index += 1;
                        }
                    }
                    orig(pickupPickerController, integer);
                };
            }


            //-------------------------
            //-------------------------


            void ScrollToRect(On.RoR2.UI.ScrollToSelection.orig_ScrollToRect orig, RoR2.UI.ScrollToSelection scrollToSelection, RectTransform rectTransform) {
                orig(scrollToSelection, rectTransform);

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
 
 