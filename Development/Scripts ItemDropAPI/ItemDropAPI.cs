using BepInEx;
using RoR2;
using RoR2.UI;
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
    namespace ItemDropAPI {
        [BepInDependency("com.bepis.r2api")]
        [BepInDependency("com.funkfrog_sipondo.sharesuite", BepInDependency.DependencyFlags.SoftDependency)]
        [R2API.Utils.R2APISubmoduleDependency("ItemDropAPI")]
        [BepInPlugin(PluginGUID, "ItemDropAPI", "1.0.0")]

        public class ItemDropAPI : BaseUnityPlugin {
            ItemDropAPI itemDropAPI;
            public const string PluginGUID = "com.Phedg1Studios.ItemDropAPI";

            private string latestInteractionName = "";
            static public UnityEngine.Events.UnityAction setDropLists = new UnityEngine.Events.UnityAction(EmptyMethod);
            private Dictionary<ItemTier, bool> tierValidMonsterTeam = new Dictionary<ItemTier, bool>();
            private Dictionary<ItemTier, bool> tierValidScav = new Dictionary<ItemTier, bool>();
            private ItemTier[] patternAdjusted = new ItemTier[0];
            static public List<PickupIndex> playerItems = new List<PickupIndex>();
            static public List<PickupIndex> monsterItems = new List<PickupIndex>();
            static public DropList playerDropList = new DropList();
            static public DropList monsterDropList = new DropList();
            static public InteractableCalculator playerInteractables = new InteractableCalculator();
            static public InteractableCalculator monsterInteractables = new InteractableCalculator();

            static public void EmptyMethod() {
            }

            void Start() {
                itemDropAPI = this;
                Catalogue.PopulateItemCatalogues();
                SetHooks();
            }

            public void SetHooks() {
                UnhookR2API();
                On.RoR2.Run.BuildDropTable += BuildDropTable;
                On.RoR2.SceneDirector.PopulateScene += PopulateScene;
                On.RoR2.ShopTerminalBehavior.GenerateNewPickupServer += GenerateNewPickupServer;
                On.RoR2.DirectorCore.TrySpawnObject += TrySpawnObject;
                On.RoR2.ShrineChanceBehavior.AddShrineStack += AddShrineStack;
                On.RoR2.BossGroup.DropRewards += DropRewards;
                On.RoR2.Interactor.AttemptInteraction += AttemptInteraction;
                On.RoR2.UI.PickupPickerPanel.SetPickupOptions += SetPickupOptions;

                On.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.GenerateAvailableItemsSet += GenerateAvailableItemsSet;
                On.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.EnsureMonsterTeamItemCount += EnsureMonsterTeamItemCount;
                IL.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.GrantMonsterTeamItem += GrantMonsterTeamItem;
                On.EntityStates.ScavMonster.FindItem.OnEnter += OnEnter;
                On.RoR2.ScavengerItemGranter.Start += ScavengerItemGranterStart;
                On.RoR2.Inventory.GiveRandomEquipment += GiveRandomEquipment;
            }

            public void UnetHooks() {
                HookR2API();
                On.RoR2.Run.BuildDropTable -= BuildDropTable;
                On.RoR2.SceneDirector.PopulateScene -= PopulateScene;
                On.RoR2.ShopTerminalBehavior.GenerateNewPickupServer -= GenerateNewPickupServer;
                On.RoR2.DirectorCore.TrySpawnObject -= TrySpawnObject;
                On.RoR2.ShrineChanceBehavior.AddShrineStack -= AddShrineStack;
                On.RoR2.BossGroup.DropRewards -= DropRewards;
                On.RoR2.Interactor.AttemptInteraction -= AttemptInteraction;
                On.RoR2.UI.PickupPickerPanel.SetPickupOptions -= SetPickupOptions;

                On.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.GenerateAvailableItemsSet -= GenerateAvailableItemsSet;
                On.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.EnsureMonsterTeamItemCount -= EnsureMonsterTeamItemCount;
                IL.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.GrantMonsterTeamItem -= GrantMonsterTeamItem;
                On.EntityStates.ScavMonster.FindItem.OnEnter -= OnEnter;
                On.RoR2.ScavengerItemGranter.Start -= ScavengerItemGranterStart;
                On.RoR2.Inventory.GiveRandomEquipment -= GiveRandomEquipment;
            }

            void SetDefaultDropLists(Run run) {
                List<PickupIndex> masterList = new List<PickupIndex>();
                foreach (PickupIndex pickupIndex in run.availableTier1DropList) {
                    masterList.Add(pickupIndex);
                }
                foreach (PickupIndex pickupIndex in run.availableTier2DropList) {
                    masterList.Add(pickupIndex);
                }
                foreach (PickupIndex pickupIndex in run.availableTier3DropList) {
                    masterList.Add(pickupIndex);
                }
                foreach (PickupIndex pickupIndex in run.availableBossDropList) {
                    masterList.Add(pickupIndex);
                }
                foreach (PickupIndex pickupIndex in run.availableLunarDropList) {
                    masterList.Add(pickupIndex);
                }
                foreach (PickupIndex pickupIndex in run.availableEquipmentDropList) {
                    masterList.Add(pickupIndex);
                }
                foreach (EquipmentIndex equipmentIndex in Catalogue.eliteEquipment) {
                    masterList.Add(PickupCatalog.FindPickupIndex(equipmentIndex));
                }
                DropList.DuplicateDropList(masterList, playerItems);
                DropList.DuplicateDropList(masterList, monsterItems);
            }


            //-------------------------
            //-------------------------


            void BuildDropTable(On.RoR2.Run.orig_BuildDropTable orig, Run run) {
                orig(run);
                SetDefaultDropLists(run);
                setDropLists();
                monsterDropList.GenerateItems(run);
                playerDropList.ClearAllLists(run);
                playerDropList.GenerateItems(run);
                playerDropList.SetItems(run);
                DropList.AddChestChoices(run);
                playerInteractables.CalculateInvalidInteractables(playerItems);
                monsterInteractables.CalculateInvalidInteractables(monsterItems);
            }

            void PopulateScene(On.RoR2.SceneDirector.orig_PopulateScene orig, SceneDirector sceneDirector) {
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
                }

                if (ClassicStageInfo.instance != null) {
                    int categoriesLength = ClassicStageInfo.instance.interactableCategories.categories.Length;
                    for (int categoryIndex = 0; categoryIndex < categoriesLength; categoryIndex++) {
                        List<DirectorCard> directorCards = new List<DirectorCard>();
                        foreach (DirectorCard directorCard in ClassicStageInfo.instance.interactableCategories.categories[categoryIndex].cards) {
                            string interactableName = InteractableCalculator.GetSpawnCardName(directorCard.spawnCard);
                            if (new List<string>() { }.Contains(interactableName)) {
                            }
                            if (playerInteractables.interactablesInvalid.Contains(interactableName)) {
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
            }

            void GenerateNewPickupServer(On.RoR2.ShopTerminalBehavior.orig_GenerateNewPickupServer orig, ShopTerminalBehavior shopTerminalBehavior) {
                List<PickupIndex> shopList = new List<PickupIndex>();
                if (shopTerminalBehavior.itemTier == ItemTier.Tier1) {
                    shopList = Run.instance.availableTier1DropList;
                } else if (shopTerminalBehavior.itemTier == ItemTier.Tier2) {
                    shopList = Run.instance.availableTier2DropList;
                } else if (shopTerminalBehavior.itemTier == ItemTier.Tier3) {
                    shopList = Run.instance.availableTier3DropList;
                } else if (shopTerminalBehavior.itemTier == ItemTier.Boss) {
                    shopList = Run.instance.availableBossDropList;
                } else if (shopTerminalBehavior.itemTier == ItemTier.Lunar) {
                    shopList = Run.instance.availableLunarDropList;
                }
                if (shopList.Count > 0) {
                    orig(shopTerminalBehavior);
                } else {
                    shopTerminalBehavior.SetNoPickup();
                    RoR2.PurchaseInteraction purchaseInteraction = shopTerminalBehavior.GetComponent<RoR2.PurchaseInteraction>();
                    if (purchaseInteraction != null) {
                        purchaseInteraction.SetAvailable(false);
                    }
                }
            }

            GameObject TrySpawnObject(On.RoR2.DirectorCore.orig_TrySpawnObject orig, DirectorCore directorCore, DirectorSpawnRequest directorSpawnRequest) {
                if (directorSpawnRequest.spawnCard.name == "iscScavBackpack") {
                    if (playerInteractables.interactablesInvalid.Contains("ScavBackpack")) {
                        return null;
                    }
                }
                return orig(directorCore, directorSpawnRequest);
            }

            void AddShrineStack(On.RoR2.ShrineChanceBehavior.orig_AddShrineStack orig, ShrineChanceBehavior shrineChangeBehavior, Interactor interactor) {
                DropList.SetDropLists(RoR2.Run.instance.availableTier1DropList, RoR2.Run.instance.availableTier2DropList, RoR2.Run.instance.availableTier3DropList, RoR2.Run.instance.availableEquipmentDropList);
                orig(shrineChangeBehavior, interactor);
                DropList.RevertDropLists();
            }

            void DropRewards(On.RoR2.BossGroup.orig_DropRewards orig, BossGroup bossGroup) {
                System.Reflection.FieldInfo info = typeof(BossGroup).GetField("bossDrops", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                ICollection collection = info.GetValue(bossGroup) as ICollection;
                List<PickupIndex> bossDrops = new List<PickupIndex>();
                List<PickupIndex> bossDropsAdjusted = new List<PickupIndex>();
                foreach (object bossDrop in collection) {
                    PickupIndex pickupIndex = (PickupIndex)bossDrop;
                    bossDrops.Add(pickupIndex);
                    if (PickupCatalog.GetPickupDef(pickupIndex).itemIndex != ItemIndex.None && playerItems.Contains(pickupIndex)) {
                        bossDropsAdjusted.Add(pickupIndex);
                    }
                }
                int normalCount = Run.instance.availableTier2DropList.Count;
                if (bossGroup.forceTier3Reward) {
                    normalCount = Run.instance.availableTier3DropList.Count;
                }
                if (normalCount != 0 || bossDropsAdjusted.Count != 0) {
                    float bossDropChanceOld = bossGroup.bossDropChance;
                    if (normalCount == 0) {
                        DropList.SetDropLists(new List<PickupIndex>(), new List<PickupIndex>(), new List<PickupIndex>(), new List<PickupIndex>());
                        bossGroup.bossDropChance = 1;
                    } else if (bossDropsAdjusted.Count == 0) {
                        bossGroup.bossDropChance = 0;
                    }
                    info.SetValue(bossGroup, bossDropsAdjusted);
                    orig(bossGroup);
                    info.SetValue(bossGroup, bossDrops);
                    bossGroup.bossDropChance = bossDropChanceOld;
                    if (normalCount == 0) {
                        DropList.RevertDropLists();
                    }
                }
            }

            void AttemptInteraction(On.RoR2.Interactor.orig_AttemptInteraction orig, Interactor interactor, GameObject gameObject) {
                if (interactor.GetComponent<NetworkBehaviour>().hasAuthority) {
                    latestInteractionName = gameObject.name;
                }
                orig(interactor, gameObject);
            }

            void SetPickupOptions(On.RoR2.UI.PickupPickerPanel.orig_SetPickupOptions orig, PickupPickerPanel pickupPickerPanel, PickupPickerController.Option[] options) {
                List<RoR2.PickupPickerController.Option> optionsAdjusted = new List<PickupPickerController.Option>();
                foreach (RoR2.PickupPickerController.Option option in options) {
                    if (latestInteractionName.Contains("Scrapper")) {
                        if (playerItems.Contains(PickupCatalog.FindPickupIndex(Catalogue.GetScrapIndex(ItemCatalog.GetItemDef(PickupCatalog.GetPickupDef(option.pickupIndex).itemIndex).tier)))) {
                            optionsAdjusted.Add(option);
                        }
                    } else if (latestInteractionName.Contains("CommandCube")) {
                        if (playerItems.Contains(option.pickupIndex)) {
                            optionsAdjusted.Add(option);
                        }
                    } else {
                        optionsAdjusted.Add(option);
                    }
                }
                options = new RoR2.PickupPickerController.Option[optionsAdjusted.Count];
                for (int optionIndex = 0; optionIndex < optionsAdjusted.Count; optionIndex++) {
                    options[optionIndex] = optionsAdjusted[optionIndex];
                }
                orig(pickupPickerPanel, options);
            }


            //-------------------------
            //-------------------------


            void GenerateAvailableItemsSet(On.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.orig_GenerateAvailableItemsSet orig) {
                List<ItemTag> forbiddenTags = new List<ItemTag>();
                System.Type type = typeof(RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager);
                System.Reflection.FieldInfo info = type.GetField("forbiddenTags", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                ICollection collection = info.GetValue(null) as ICollection;
                foreach (object itemTag in collection) {
                    forbiddenTags.Add((ItemTag)itemTag);
                }

                tierValidScav.Clear();
                tierValidMonsterTeam.Clear();
                List<PickupIndex> tier1Adjusted = monsterDropList.availableTier1DropList;
                tierValidMonsterTeam.Add(ItemTier.Tier1, ListContainsValidItems(forbiddenTags, tier1Adjusted));
                tierValidScav.Add(ItemTier.Tier1, ListContainsValidItems(new List<ItemTag>() { ItemTag.AIBlacklist }, tier1Adjusted));
                if (!tierValidMonsterTeam[ItemTier.Tier1]) {
                    tier1Adjusted = DropList.tier1DropListOriginal;
                }
                List<PickupIndex> tier2Adjusted = monsterDropList.availableTier2DropList;
                tierValidMonsterTeam.Add(ItemTier.Tier2, ListContainsValidItems(forbiddenTags, tier2Adjusted));
                tierValidScav.Add(ItemTier.Tier2, ListContainsValidItems(new List<ItemTag>() { ItemTag.AIBlacklist }, tier2Adjusted));
                if (!tierValidMonsterTeam[ItemTier.Tier2]) {
                    tier2Adjusted = DropList.tier2DropListOriginal;
                }
                List<PickupIndex> tier3Adjusted = monsterDropList.availableTier3DropList;
                tierValidMonsterTeam.Add(ItemTier.Tier3, ListContainsValidItems(forbiddenTags, tier3Adjusted));
                tierValidScav.Add(ItemTier.Tier3, ListContainsValidItems(new List<ItemTag>() { ItemTag.AIBlacklist }, tier3Adjusted));
                if (!tierValidMonsterTeam[ItemTier.Tier3]) {
                    tier3Adjusted = DropList.tier3DropListOriginal;
                }

                DropList.SetDropLists(tier1Adjusted, tier2Adjusted, tier3Adjusted, DropList.equipmentDropListOriginal);
                orig();
                DropList.RevertDropLists();


                info = type.GetField("pattern", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                collection = info.GetValue(null) as ICollection;
                List<ItemTier> patternAdjustedList = new List<ItemTier>();
                int patternIndex = 0;
                foreach (object itemTier in collection) {
                    patternAdjustedList.Add((ItemTier)itemTier);
                    patternIndex += 1;
                }
                if (!tierValidMonsterTeam[ItemTier.Tier1]) {
                    while (patternAdjustedList.Contains(ItemTier.Tier1)) {
                        patternAdjustedList.Remove(ItemTier.Tier1);
                    }
                }
                if (!tierValidMonsterTeam[ItemTier.Tier2]) {
                    while (patternAdjustedList.Contains(ItemTier.Tier2)) {
                        patternAdjustedList.Remove(ItemTier.Tier2);
                    }
                }
                if (!tierValidMonsterTeam[ItemTier.Tier3]) {
                    while (patternAdjustedList.Contains(ItemTier.Tier3)) {
                        patternAdjustedList.Remove(ItemTier.Tier3);
                    }
                }
                patternAdjusted = new ItemTier[patternAdjustedList.Count];
                patternIndex = 0;
                foreach (ItemTier itemTier in patternAdjustedList) {
                    patternAdjusted[patternIndex] = itemTier;
                    patternIndex += 1;
                }
            }

            void EnsureMonsterTeamItemCount(On.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.orig_EnsureMonsterTeamItemCount orig, int integer) {
                if (patternAdjusted.Length > 0) {
                    orig(integer);
                }
            }

            void GrantMonsterTeamItem(ILContext ilContext) {
                //https://github.com/risk-of-thunder/R2Wiki/wiki/Working-with-IL
                System.Type type = typeof(RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager);
                System.Reflection.FieldInfo pattern = type.GetField("pattern", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                System.Reflection.FieldInfo currentItemIterator = type.GetField("currentItemIterator", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);


                ILCursor cursor = new ILCursor(ilContext);
                cursor.GotoNext(
                    x => x.MatchLdsfld(pattern),
                    x => x.MatchLdsfld(currentItemIterator),
                    x => x.MatchDup(),
                    x => x.MatchLdcI4(1),
                    x => x.MatchAdd(),
                    x => x.MatchStsfld(currentItemIterator),
                    x => x.MatchLdsfld(pattern),
                    x => x.MatchLdlen(),
                    x => x.MatchConvI4(),
                    x => x.MatchRem(),
                    x => x.MatchLdelemI4(),
                    x => x.MatchStloc(0)
                );
                cursor.RemoveRange(11);

                cursor.EmitDelegate<System.Func<ItemTier>>(() => {
                    int currentItemIteratorValue = int.Parse(currentItemIterator.GetValue(null).ToString());
                    ItemTier itemTier = patternAdjusted[currentItemIteratorValue % patternAdjusted.Length];
                    currentItemIterator.SetValue(null, currentItemIteratorValue + 1);
                    return itemTier;
                });
            }

            void OnEnter(On.EntityStates.ScavMonster.FindItem.orig_OnEnter orig, EntityStates.ScavMonster.FindItem findItem) {
                bool valid = false;
                foreach (bool tierValid in tierValidScav.Values) {
                    if (tierValid) {
                        valid = true;
                        break;
                    }
                }
                if (valid) {
                    List<PickupIndex> tier1Adjusted = monsterDropList.availableTier1DropList;
                    if (!tierValidMonsterTeam[ItemTier.Tier1]) {
                        tier1Adjusted = DropList.tier1DropListOriginal;
                    }
                    List<PickupIndex> tier2Adjusted = monsterDropList.availableTier2DropList;
                    if (!tierValidMonsterTeam[ItemTier.Tier2]) {
                        tier2Adjusted = DropList.tier2DropListOriginal;
                    }
                    List<PickupIndex> tier3Adjusted = monsterDropList.availableTier3DropList;
                    if (!tierValidScav[ItemTier.Tier3]) {
                        tier3Adjusted = DropList.tier3DropListOriginal;
                    }
                    DropList.SetDropLists(tier1Adjusted, tier2Adjusted, tier3Adjusted, DropList.equipmentDropListOriginal);

                    List<float> scavTierChanceBackup = new List<float>();
                    scavTierChanceBackup.Add(EntityStates.ScavMonster.FindItem.tier1Chance);
                    scavTierChanceBackup.Add(EntityStates.ScavMonster.FindItem.tier2Chance);
                    scavTierChanceBackup.Add(EntityStates.ScavMonster.FindItem.tier3Chance);

                    if (!tierValidScav[ItemTier.Tier1]) {
                        EntityStates.ScavMonster.FindItem.tier1Chance = 0;
                    }
                    if (!tierValidScav[ItemTier.Tier2]) {
                        EntityStates.ScavMonster.FindItem.tier2Chance = 0;
                    }
                    if (!tierValidScav[ItemTier.Tier3]) {
                        EntityStates.ScavMonster.FindItem.tier3Chance = 0;
                    }

                    orig(findItem);

                    DropList.RevertDropLists();
                    EntityStates.ScavMonster.FindItem.tier1Chance = scavTierChanceBackup[0];
                    EntityStates.ScavMonster.FindItem.tier2Chance = scavTierChanceBackup[1];
                    EntityStates.ScavMonster.FindItem.tier3Chance = scavTierChanceBackup[2];
                }
            }

            void ScavengerItemGranterStart(On.RoR2.ScavengerItemGranter.orig_Start orig, ScavengerItemGranter scavengerItemGranter) {
                List<int> scavTierTypesBackup = new List<int>();
                scavTierTypesBackup.Add(scavengerItemGranter.tier1Types);
                scavTierTypesBackup.Add(scavengerItemGranter.tier2Types);
                scavTierTypesBackup.Add(scavengerItemGranter.tier3Types);

                if (!tierValidScav[ItemTier.Tier1]) {
                    scavengerItemGranter.tier1Types = 0;
                }
                if (!tierValidScav[ItemTier.Tier2]) {
                    scavengerItemGranter.tier2Types = 0;
                }
                if (!tierValidScav[ItemTier.Tier3]) {
                    scavengerItemGranter.tier3Types = 0;
                }

                orig(scavengerItemGranter);

                scavengerItemGranter.tier1Types = scavTierTypesBackup[0];
                scavengerItemGranter.tier2Types = scavTierTypesBackup[1];
                scavengerItemGranter.tier3Types = scavTierTypesBackup[2];
            }

            void GiveRandomEquipment(On.RoR2.Inventory.orig_GiveRandomEquipment orig, Inventory inventory) {
                if (monsterDropList.availableEquipmentDropList.Count > 0) {
                    orig(inventory);
                }
            }


            //-------------------------
            //-------------------------


            static public void UnhookR2API() {
                System.Type type = typeof(R2API.ItemDropAPI);
                System.Reflection.MethodInfo runOnBuildDropTable = type.GetMethod("RunOnBuildDropTable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                On.RoR2.Run.BuildDropTable -= (On.RoR2.Run.hook_BuildDropTable)runOnBuildDropTable.CreateDelegate(typeof(On.RoR2.Run.hook_BuildDropTable));

                System.Reflection.MethodInfo addShrineStack = type.GetMethod("ShrineChanceBehaviorOnAddShrineStack", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                IL.RoR2.ShrineChanceBehavior.AddShrineStack -= addShrineStack.CreateDelegate(typeof(ILContext.Manipulator)) as ILContext.Manipulator;
            }

            static public void HookR2API() {
                System.Type type = typeof(R2API.ItemDropAPI);
                System.Reflection.MethodInfo runOnBuildDropTable = type.GetMethod("RunOnBuildDropTable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                On.RoR2.Run.BuildDropTable += (On.RoR2.Run.hook_BuildDropTable)runOnBuildDropTable.CreateDelegate(typeof(On.RoR2.Run.hook_BuildDropTable));

                System.Reflection.MethodInfo addShrineStack = type.GetMethod("ShrineChanceBehaviorOnAddShrineStack", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                IL.RoR2.ShrineChanceBehavior.AddShrineStack += addShrineStack.CreateDelegate(typeof(ILContext.Manipulator)) as ILContext.Manipulator;
            }


            //-------------------------
            //-------------------------


            static public bool ListContainsValidItems(List<ItemTag> forbiddenTags, List<PickupIndex> givenList) {
                foreach (PickupIndex pickupIndex in givenList) {
                    bool validItem = true;
                    ItemDef itemDef = ItemCatalog.GetItemDef(PickupCatalog.GetPickupDef(pickupIndex).itemIndex);
                    foreach (ItemTag itemTag in forbiddenTags) {
                        if (itemDef.ContainsTag(itemTag)) {
                            validItem = false;
                            break;
                        }
                    }
                    if (validItem) {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
 
 