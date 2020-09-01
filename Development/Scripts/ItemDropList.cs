using BepInEx;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MonoMod.Cil;


namespace Phedg1Studios {
    namespace ItemDropList {
        [BepInDependency("com.bepis.r2api")]
        [BepInDependency("com.funkfrog_sipondo.sharesuite", BepInDependency.DependencyFlags.SoftDependency)]
        [R2API.Utils.R2APISubmoduleDependency("ItemDropAPI")]
        [R2API.Utils.R2APISubmoduleDependency("PrefabAPI")]
        [R2API.Utils.R2APISubmoduleDependency("ResourcesAPI")]
        [BepInPlugin("com.Phedg1Studios.ItemDropList", "ItemDropList", "1.1.13")]

        public class ItemDropList : BaseUnityPlugin {
            static public ItemDropList itemDropList;
            public UnityEngine.Events.UnityAction setDropList = new UnityEngine.Events.UnityAction(EmptyMethod);
            List<Coroutine> characterMasterCoroutines = new List<Coroutine>();
            private int stageClearCountOld = -1;
            private int scrapperMax = 10000;
            private List<EquipmentState> latestEquipment = new List<EquipmentState>();
            private List<EquipmentIndex> equipmentFoundThisStage = new List<EquipmentIndex>();
            private List<EquipmentIndex> equipmentFoundThisStageTwice = new List<EquipmentIndex>();
            Inventory inventoryLocal = null;
            CharacterBody characterBody = null;
            private string latestInteractionName = "";
            private bool scrapRemaining = false;

            static public void EmptyMethod() {
            }

            void OnceSetup() {
                UnhookR2API();
                gameObject.AddComponent<Util>();
                Resources.LoadResources();
                SceneLoadSetup();
                Data.PopulateItemCatalogues();
                setDropList += Data.SetDropList;
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
                    orig(self);
                    Data.RefreshInfo();
                    if (Data.modEnabled) {
                        setDropList();
                        DropList.ClearAllLists(self);
                        DropList.SetItems(self);
                        DropList.AddChestChoices(self);
                        DataShop.SetScrapStarting();
                        Data.SaveProfileConfig();
                    }
                });
                On.RoR2.SceneDirector.PopulateScene += ((orig, sceneDirector) => {
                    if (Data.modEnabled) {
                        sceneDirector.interactableCredit = Mathf.FloorToInt(sceneDirector.interactableCredit * Data.interactableMultiplier);
                        InteractableCalculator.CalculateInvalidInteractables(Data.itemsToDrop);

                        List<SpawnCard> spawnCards = new List<SpawnCard>();
                        List<string> spawnCardNames = new List<string>();
                        //spawnCardNames = new List<string>() { "BrokenDrone1", "BrokenDrone2", "BrokenEmergencyDrone", "BrokenEquipmentDrone", "BrokenFlameDrone", "BrokenMegaDrone", "BrokenMissileDrone", "BrokenTurret1" };
                        //spawnCardNames = new List<string>() { "Duplicator", "DuplicatorLarge", "DuplicatorMilitary", "DuplicatorWild" };
                        //spawnCardNames = new List<string>() { "EquipmentBarrel", "TripleShopEquipment" };

                        RoR2.InteractableSpawnCard[] allInteractables = UnityEngine.Resources.LoadAll<RoR2.InteractableSpawnCard>("SpawnCards/InteractableSpawnCard");
                        foreach (RoR2.InteractableSpawnCard spawnCard in allInteractables) {
                            string interactableName = InteractableCalculator.GetSpawnCardName(spawnCard);
                            if (interactableName == "Lockbox" || interactableName == "ScavBackpack") {
                                DropOdds.UpdateChestTierOdds(spawnCard, interactableName);
                            } else if (interactableName == "CasinoChest") {
                                DropOdds.UpdateDropTableTierOdds(spawnCard, interactableName);
                            } else if (interactableName == "ShrineCleanse") {
                                ExplicitPickupDropTable dropTable = spawnCard.prefab.GetComponent<RoR2.ShopTerminalBehavior>().dropTable as ExplicitPickupDropTable;
                                DropOdds.UpdateDropTableItemOdds(dropTable, interactableName);
                            }
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

                        int categoriesLength = ClassicStageInfo.instance.interactableCategories.categories.Length;
                        for (int categoryIndex = 0; categoryIndex < categoriesLength; categoryIndex++) {
                            List<DirectorCard> directorCards = new List<DirectorCard>();
                            foreach (DirectorCard directorCard in ClassicStageInfo.instance.interactableCategories.categories[categoryIndex].cards) {
                                string interactableName = InteractableCalculator.GetSpawnCardName(directorCard.spawnCard);
                                if (new List<string>() { }.Contains(interactableName)) {
                                }
                                if (Data.modEnabled && InteractableCalculator.interactablesInvalid.Contains(interactableName)) {
                                } else {
                                    DropOdds.UpdateChestTierOdds(directorCard.spawnCard, interactableName);
                                    DropOdds.UpdateShrineTierOdds(directorCard, interactableName);
                                    directorCards.Add(directorCard);
                                }
                            }
                            DirectorCard[] directorCardArray = new DirectorCard[directorCards.Count];
                            for (int cardIndex = 0; cardIndex < directorCards.Count; cardIndex++) {
                                directorCardArray[cardIndex] = directorCards[cardIndex];
                            }
                            if (directorCardArray.Length == 0) {
                                ClassicStageInfo.instance.interactableCategories.categories[categoryIndex].selectionWeight = 0;
                            }
                            ClassicStageInfo.instance.interactableCategories.categories[categoryIndex].cards = directorCardArray;
                        }
                    }
                    orig(sceneDirector);
                });
                On.RoR2.DirectorCore.TrySpawnObject += ((orig, directorCore, directorSpawnRequest) => {
                    if (Data.modEnabled) {
                        if (directorSpawnRequest.spawnCard.name == "iscScavBackpack") {
                            if (InteractableCalculator.interactablesInvalid.Contains("ScavBackpack")) {
                                return null;
                            }
                        }
                    }
                    return orig(directorCore, directorSpawnRequest);
                });
                On.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.GenerateAvailableItemsSet += ((orig) => {
                    if (Data.modEnabled) {
                        //DropList.SetDropLists(DropList.tier1DropList, DropList.tier2DropList, DropList.tier3DropList, DropList.equipmentDropList);
                        DropList.SetDropLists(RoR2.Run.instance.availableTier1DropList, RoR2.Run.instance.availableTier2DropList, RoR2.Run.instance.availableTier3DropList, RoR2.Run.instance.availableEquipmentDropList);
                        orig();
                        DropList.RevertDropLists();
                    } else {
                        orig();
                    }
                });
                On.RoR2.ShrineChanceBehavior.AddShrineStack += ((orig, self, interactor) => {
                    if (Data.modEnabled) {
                        DropList.SetDropLists(RoR2.Run.instance.availableTier1DropList, RoR2.Run.instance.availableTier2DropList, RoR2.Run.instance.availableTier3DropList, RoR2.Run.instance.availableEquipmentDropList);
                        orig(self, interactor);
                        DropList.RevertDropLists();
                    } else {
                        orig(self, interactor);
                    }
                });
                On.RoR2.BossGroup.DropRewards += ((orig, self) => {
                    if (Data.modEnabled) {
                        System.Reflection.FieldInfo info = typeof(BossGroup).GetField("bossDrops", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        ICollection collection = info.GetValue(self) as ICollection;
                        List<PickupIndex> bossDrops = new List<PickupIndex>();
                        List<PickupIndex> bossDropsAdjusted = new List<PickupIndex>();
                        foreach (object bossDrop in collection) {
                            PickupIndex pickupIndex = (PickupIndex)bossDrop;
                            bossDrops.Add(pickupIndex);
                            if (Data.allItemsIndexes.ContainsKey(pickupIndex.itemIndex) && Data.itemsToDrop.Contains(Data.allItemsIndexes[pickupIndex.itemIndex])) {
                                bossDropsAdjusted.Add(pickupIndex);
                            }
                        }
                        int normalCount = Run.instance.availableTier2DropList.Count;
                        if (self.forceTier3Reward) {
                            normalCount = Run.instance.availableTier3DropList.Count;
                        }
                        if (normalCount != 0 || bossDropsAdjusted.Count != 0) {
                            float bossDropChanceOld = self.bossDropChance;
                            if (normalCount == 0) {
                                DropList.SetDropLists(new List<PickupIndex>(), new List<PickupIndex>(), new List<PickupIndex>(), new List<PickupIndex>());
                                self.bossDropChance = 1;
                            } else if (bossDropsAdjusted.Count == 0) {
                                self.bossDropChance = 0;
                            }
                            info.SetValue(self, bossDropsAdjusted);
                            orig(self);
                            info.SetValue(self, bossDrops);
                            self.bossDropChance = bossDropChanceOld;
                            if (normalCount == 0) {
                                DropList.RevertDropLists();
                            }
                        }
                    } else {
                        orig(self);
                    }
                });
                On.RoR2.Stage.Start += (orig, stage) => {
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
                };
                On.RoR2.Inventory.RpcItemAdded += (orig, inventory, itemIndex) => {
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
                };
                On.RoR2.Inventory.SetEquipmentInternal += ((orig, inventory, equipmentState, slot) => {
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
                });
                On.RoR2.Interactor.AttemptInteraction += (orig, interactor, gameObject) => {
                    if (Data.modEnabled) {
                        if (interactor.GetComponent<NetworkBehaviour>().hasAuthority) {
                            latestInteractionName = gameObject.name;
                            if (Data.mode == DataShop.mode) {
                                if (gameObject.name == "Scrapper(Clone)") {
                                    scrapperMax = gameObject.GetComponent<ScrapperController>().maxItemsToScrapAtATime;
                                }
                                if (DataShop.duplicatorNames.Contains(latestInteractionName)) {
                                    int duplicatorTier = DataShop.duplicatorNames.IndexOf(latestInteractionName);
                                    if (Data.GetItemTier(Data.allItemsIndexes[Data.GetScrapIndex(duplicatorTier)]) == duplicatorTier) {
                                        scrapRemaining = interactor.GetComponent<CharacterBody>().inventory.GetItemCount(Data.GetScrapIndex(duplicatorTier)) > 0;
                                    }
                                }
                            }
                        }
                    }
                    orig(interactor, gameObject);
                };
                On.RoR2.Interactor.RpcInteractionResult += (orig, interactor, result) => {
                    if (Data.modEnabled) {
                        if (Data.mode == DataShop.mode) {
                            if (interactor.GetComponent<NetworkBehaviour>().hasAuthority) {
                                if (result) {
                                    if (DataShop.duplicatorNames.Contains(latestInteractionName)) {
                                        //if (!scrapRemaining) {
                                        int duplicatorTier = DataShop.duplicatorNames.IndexOf(latestInteractionName);
                                        DataShop.RemoveScrap(duplicatorTier, 1);
                                        //}
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

                                    latestInteractionName = "";
                                }
                            }
                        }
                    }
                    orig(interactor, result);
                };
                On.RoR2.PickupPickerController.SubmitChoice += (submitChoice, pickupPickerController, givenIndex) => {
                    if (Data.modEnabled) {
                        if (Data.mode == DataShop.mode) {
                            System.Type type = typeof(RoR2.PickupPickerController);
                            System.Reflection.FieldInfo info = type.GetField("options", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            ICollection collection = info.GetValue(pickupPickerController) as ICollection;
                            int index = 0;
                            foreach (object item in collection) {
                                if (index == givenIndex) {
                                    RoR2.PickupPickerController.Option option = (RoR2.PickupPickerController.Option)item;
                                    ItemIndex itemIndex = RoR2.PickupCatalog.GetPickupDef(option.pickupIndex).itemIndex;
                                    NetworkIdentity networkIdentity = pickupPickerController.GetComponent<NetworkIdentity>();

                                    RoR2.NetworkUIPromptController promptController = pickupPickerController.GetComponent<RoR2.NetworkUIPromptController>();
                                    type = typeof(RoR2.NetworkUIPromptController);
                                    info = type.GetField("_currentLocalParticipant", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                                    RoR2.LocalUser localUser = (RoR2.LocalUser)info.GetValue(promptController);
                                    foreach (NetworkUser networkUser in RoR2.NetworkUser.readOnlyInstancesList) {
                                        if (localUser.currentNetworkUser.netId == networkUser.netId) {
                                            if (networkUser.isLocalPlayer) {
                                                DataShop.RemoveScrap(Data.GetItemTier(Data.allItemsIndexes[itemIndex]), Mathf.Min(scrapperMax, networkUser.GetCurrentBody().inventory.GetItemCount(itemIndex)));
                                            }
                                            break;
                                        }
                                    }
                                    break;
                                }
                                index += 1;
                            }
                        }
                    }
                    submitChoice(pickupPickerController, givenIndex);
                };
                On.RoR2.UI.PickupPickerPanel.SetPickupOptions += (setPickupOptions, pickupPickerPanel, options) => {
                    if (Data.modEnabled) {
                        List<RoR2.PickupPickerController.Option> optionsAdjusted = new List<PickupPickerController.Option>();
                        foreach (RoR2.PickupPickerController.Option option in options) {
                            if (latestInteractionName.Contains("Scrapper")) {
                                if (Data.itemsToDrop.Contains(Data.allItemsIndexes[Data.GetScrapIndex(Data.GetItemTier(Data.allItemsIndexes[PickupCatalog.GetPickupDef(option.pickupIndex).itemIndex]))])) {
                                    optionsAdjusted.Add(option);
                                }
                            } else if (latestInteractionName.Contains("CommandCube")) {
                                if (Data.allItemsIndexes.ContainsKey(PickupCatalog.GetPickupDef(option.pickupIndex).itemIndex)) {
                                    if (Data.itemsToDrop.Contains(Data.allItemsIndexes[PickupCatalog.GetPickupDef(option.pickupIndex).itemIndex])) {
                                        optionsAdjusted.Add(option);
                                    }
                                } else if (Data.allEquipmentIndexes.ContainsKey(PickupCatalog.GetPickupDef(option.pickupIndex).equipmentIndex)) {
                                    if (Data.itemsToDrop.Contains(Data.allEquipmentIndexes[PickupCatalog.GetPickupDef(option.pickupIndex).equipmentIndex])) {
                                        optionsAdjusted.Add(option);
                                    }
                                }
                            } else {
                                optionsAdjusted.Add(option);
                            }
                        }
                        options = new RoR2.PickupPickerController.Option[optionsAdjusted.Count];
                        for (int optionIndex = 0; optionIndex < optionsAdjusted.Count; optionIndex++) {
                            options[optionIndex] = optionsAdjusted[optionIndex];
                        }
                    }
                    setPickupOptions(pickupPickerPanel, options);
                };
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

            static public void ToggleR2APIHooks() {
                if (Data.modEnabled) {
                    UnhookR2API();
                } else {
                    HookR2API();
                }
            }

            static public void HookR2API() {
                System.Type type = typeof(R2API.ItemDropAPI);
                System.Reflection.MethodInfo runOnBuildDropTable = type.GetMethod("RunOnBuildDropTable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                On.RoR2.Run.BuildDropTable -= (On.RoR2.Run.hook_BuildDropTable)runOnBuildDropTable.CreateDelegate(typeof(On.RoR2.Run.hook_BuildDropTable));

                //System.Reflection.MethodInfo dropRewards = type.GetMethod("DropRewards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                //IL.RoR2.BossGroup.DropRewards -= dropRewards.CreateDelegate(typeof(ILContext.Manipulator)) as ILContext.Manipulator;
            }

            static public void UnhookR2API() {
                System.Type type = typeof(R2API.ItemDropAPI);
                System.Reflection.MethodInfo runOnBuildDropTable = type.GetMethod("RunOnBuildDropTable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                On.RoR2.Run.BuildDropTable -= (On.RoR2.Run.hook_BuildDropTable)runOnBuildDropTable.CreateDelegate(typeof(On.RoR2.Run.hook_BuildDropTable));

                //System.Reflection.MethodInfo dropRewards = type.GetMethod("DropRewards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                //IL.RoR2.BossGroup.DropRewards -= dropRewards.CreateDelegate(typeof(ILContext.Manipulator)) as ILContext.Manipulator;
            }
        }
    }
}
 
 