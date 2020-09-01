using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RoR2;


namespace Phedg1Studios {
    namespace ItemDropList {
        public class UIDrawer : MonoBehaviour {
            static public RectTransform rootTransform;
            static public List<GameObject> shopInterfaces = new List<GameObject>();
            static public RoR2.UI.MainMenu.BaseMainMenuScreen itemDropListMenu;
            static public List<TMPro.TextMeshProUGUI> statusTexts = new List<TMPro.TextMeshProUGUI>();
            static public Dictionary<int, List<Image>> itemImages = new Dictionary<int, List<Image>>();
            static public Dictionary<int, List<TMPro.TextMeshProUGUI>> itemTexts = new Dictionary<int, List<TMPro.TextMeshProUGUI>>();
            static public float storeHeight = 0;
            static private List<string> instructionTexts = new List<string>() {
                "LEFT CLICK to toggle",
                "DOUBLE CLICK to enroll in a course\nPICK UP items of each rarity in game\nto collect aptitude score of the same rarity",
                "PICK UP items of each rarity in game\nto collect aptitude score of the same rarity",
                "LEFT CLICK to toggle\nPICK UP items of each rarity in game\nto collect aptitude score of the same rarity",
            };
            static public GameObject backButton;


            static public void DrawUI() {
                ResetRootPivot();
                ClearShopInterfaces();
                DrawShading();
                CalculateVerticalOffset();
                DrawShop();
                DrawBlackButtons();
                DrawInstructions();
                DrawModName();
                UIDrawerNoShop.DrawUI();
                UIDrawerShop.DrawUI();
                Refresh();
            }

            static void CalculateVerticalOffset() {
                storeHeight = (UIConfig.itemButtonWidth + UIConfig.itemPaddingOuter * 2 + UIConfig.itemTextHeight * UIConfig.textCount[UIConfig.GetMode()]) * UIConfig.displayRows[UIConfig.GetMode()] + UIConfig.scrollPadding * 2 + UIConfig.panelPadding * 2;
                float totalHeight = UIConfig.blueButtonCount[Data.mode] * (UIConfig.blueButtonHeight + UIConfig.spacingVertical) + storeHeight + UIConfig.spacingVertical + UIConfig.blackButtonHeight;
                UIConfig.offsetVertical = (rootTransform.rect.height - totalHeight) / 2f;
            }

            static public void ResetRootPivot() {
                rootTransform.pivot = new Vector2(0, 1);
            }

            static public void CreateCanvas() {
                if (rootTransform != null) {
                    DestroyImmediate(rootTransform.transform.parent.gameObject);
                }
                GameObject root = new GameObject();
                root.name = "Canvas";
                Canvas canvas = root.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = -1;
                CanvasScaler canvasScaler = root.AddComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.matchWidthOrHeight = 1;
                canvasScaler.referenceResolution = new Vector2(1920, 1080);

                CanvasGroup canvasGroup = root.AddComponent<CanvasGroup>();
                canvasGroup.blocksRaycasts = false;

                root.AddComponent<GraphicRaycaster>();
                root.AddComponent<RoR2.UI.MPEventSystemProvider>();
                Image background = ElementCreator.SpawnImageOffset(new List<Image>(), root, null, new Color(0, 0, 0, 0), new Vector2(0, 1), new Vector2(0, 0), new Vector2(0, 0));
                background.raycastTarget = true;

                GameObject rootTransformObject = new GameObject("Base");
                rootTransformObject.transform.parent = root.transform;
                rootTransform = rootTransformObject.AddComponent<RectTransform>();
                rootTransform.pivot = new Vector2(0, 0);
                rootTransform.anchorMin = Vector2.zero;
                rootTransform.anchorMax = Vector2.one;
                rootTransform.offsetMin = Vector2.zero;
                rootTransform.offsetMax = Vector2.zero;
                rootTransform.localScale = Vector3.one;

                CanvasGroup canvasGroupChild = rootTransformObject.AddComponent<CanvasGroup>();
                canvasGroupChild.blocksRaycasts = true;

                itemDropListMenu = rootTransform.gameObject.AddComponent<RoR2.UI.MainMenu.BaseMainMenuScreen>();
                itemDropListMenu.desiredCameraTransform = new GameObject().transform;
                itemDropListMenu.desiredCameraTransform.position = new Vector3(20, 40, 0);
                itemDropListMenu.desiredCameraTransform.eulerAngles = new Vector3(90, 20, 0);
                //itemDropListMenu.desiredCameraTransform.position = new Vector3(60, 10, 35);
                //itemDropListMenu.desiredCameraTransform.eulerAngles = new Vector3(40, 165, 0);
                itemDropListMenu.desiredCameraTransform.localScale = new Vector3(1, 1, 1);
                itemDropListMenu.onEnter = new UnityEngine.Events.UnityEvent();
                itemDropListMenu.onEnter.AddListener(OpenStartingItems);
                itemDropListMenu.onExit = new UnityEngine.Events.UnityEvent();
                itemDropListMenu.onExit.AddListener(CloseStartingItems);
            }

            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            static void DrawShading() {
                float shadingHeight = 270;
                Image shadingA = ElementCreator.SpawnImageOffset(new List<Image>(), rootTransform.gameObject, Resources.panelTextures[8], new Color(0, 0, 0, 1), new Vector2(0, 1), new Vector2(0, 0), new Vector2(0, -(rootTransform.rect.height - shadingHeight)));
                Image shadingB = ElementCreator.SpawnImageOffset(new List<Image>(), rootTransform.gameObject, Resources.panelTextures[9], new Color(0, 0, 0, 1), new Vector2(0, 1), new Vector2(0, rootTransform.rect.height - shadingHeight), new Vector2(0, 0));
                shopInterfaces.Add(shadingA.gameObject);
                shopInterfaces.Add(shadingB.gameObject);
            }

            static void DrawBlackButtons() {
                backButton = ButtonCreator.SpawnBlackButton(rootTransform.gameObject, new Vector2(UIConfig.blackButtonWidth, UIConfig.blackButtonHeight), "Back", new List<TMPro.TextMeshProUGUI>(), true);
                backButton.transform.parent.GetComponent<RectTransform>().localPosition = new Vector3(UIConfig.offsetHorizontal, -UIConfig.offsetVertical - UIConfig.blueButtonCount[Data.mode] * (UIConfig.blueButtonHeight + UIConfig.spacingVertical) - storeHeight - UIConfig.spacingVertical, 0);
                backButton.GetComponent<RoR2.UI.HGButton>().onClick.AddListener(() => {
                    SetMenuTitle();
                });
                shopInterfaces.Add(backButton.transform.parent.gameObject);

                GameObject statusButton = ButtonCreator.SpawnBlackButton(rootTransform.gameObject, new Vector2(UIConfig.blackButtonWidth, UIConfig.blackButtonHeight), "", statusTexts);
                statusButton.transform.parent.GetComponent<RectTransform>().localPosition = new Vector3(UIConfig.offsetHorizontal + UIConfig.blackButtonWidth + UIConfig.spacingHorizontal, -UIConfig.offsetVertical - UIConfig.blueButtonCount[Data.mode] * (UIConfig.blueButtonHeight + UIConfig.spacingVertical) - storeHeight - UIConfig.spacingVertical, 0);
                statusButton.GetComponent<RoR2.UI.HGButton>().onClick.AddListener(() => {
                    Data.ToggleEnabled();
                });
                shopInterfaces.Add(statusButton.transform.parent.gameObject);
            }

            static void DrawInstructions() {
                List<TMPro.TextMeshProUGUI> instructionsText = new List<TMPro.TextMeshProUGUI>();

                Vector3 position = new Vector3();
                position.x = rootTransform.rect.width / 2f;
                position.y = -UIConfig.offsetVertical - UIConfig.blueButtonCount[Data.mode] * (UIConfig.blueButtonHeight + UIConfig.spacingVertical) - storeHeight - UIConfig.spacingVertical;
                ElementCreator.SpawnTextSize(instructionsText, rootTransform.gameObject, new Color(1, 1, 1, 1), 24, 0, new Vector2(0.5f, 1), new Vector2(500, UIConfig.blackButtonHeight), position);
                if (UIConfig.GetMode() == 0) {
                    instructionsText[0].text = instructionTexts[0];
                } else if (UIConfig.GetMode() == 1) {
                    instructionsText[0].text = instructionTexts[1];
                } else if (UIConfig.GetMode() == 2) {
                    if (!DataShop.canDisablePurchasedBlueprints) {
                        instructionsText[0].text = instructionTexts[2];
                    } else {
                        instructionsText[0].text = instructionTexts[3];
                    }
                }
                shopInterfaces.Add(instructionsText[0].gameObject);
            }

            static void DrawModName() {
                List<TMPro.TextMeshProUGUI> modText = new List<TMPro.TextMeshProUGUI>();
                Vector3 position = new Vector3();
                position.x = rootTransform.rect.width - UIConfig.offsetHorizontal - UIConfig.blackButtons[Data.mode] * (UIConfig.blackButtonWidth / 2f + UIConfig.spacingHorizontal);
                position.y = -UIConfig.offsetVertical - UIConfig.blueButtonCount[Data.mode] * (UIConfig.blueButtonHeight + UIConfig.spacingVertical) - storeHeight - UIConfig.spacingVertical;
                ElementCreator.SpawnTextSize(modText, rootTransform.gameObject, new Color(1, 1, 1, 0.025f), 24, 0, new Vector2(1, 1), new Vector2(300, UIConfig.blackButtonHeight), position);
                modText[0].text = Data.developerName + ":\n" + Data.modName;
                modText[0].alignment = TMPro.TextAlignmentOptions.Right;
                shopInterfaces.Add(modText[0].gameObject);
            }

            static void DrawShop() {
                List<int> storeItems = new List<int>();
                if (UIConfig.GetMode() == 0) {
                    storeItems = GetStoreItems(false, false);
                } else if (UIConfig.GetMode() == 1) {
                    storeItems = GetStoreItems(true, false);
                } else if (UIConfig.GetMode() == 2) {
                    storeItems = GetStoreItems(true, true);
                }
                shopInterfaces.Add(ScrollCreator.CreateScroll(rootTransform, UIConfig.displayRows[UIConfig.GetMode()], UIConfig.textCount[UIConfig.GetMode()], storeItems, rootTransform.rect.width - UIConfig.offsetHorizontal * 2, new Vector3(UIConfig.offsetHorizontal, -UIConfig.offsetVertical - UIConfig.blueButtonCount[Data.mode] *  (UIConfig.blueButtonHeight + UIConfig.spacingVertical), 0), itemImages, itemTexts));
            }

            static List<int> GetStoreItems(bool blueprintsMatter, bool ownBlueprint) {
                List<int> storeItems = new List<int>();
                foreach (ItemIndex itemIndex in RoR2.ItemCatalog.tier1ItemList) {
                    if (Data.UnlockedItem(Data.allItemsIndexes[itemIndex], Data.mode)) {
                        if (Data.mode == DataNoShop.mode || (Data.mode == DataShop.mode && (!blueprintsMatter || ownBlueprint == DataShop.blueprintsPurchased.Contains(Data.allItemsIndexes[itemIndex])))) {
                            storeItems.Add(Data.allItemsIndexes[itemIndex]);
                        }
                    }
                }
                foreach (ItemIndex itemIndex in RoR2.ItemCatalog.tier2ItemList) {
                    if (Data.UnlockedItem(Data.allItemsIndexes[itemIndex], Data.mode)) {
                        if (Data.mode == DataNoShop.mode || (Data.mode == DataShop.mode && (!blueprintsMatter || ownBlueprint == DataShop.blueprintsPurchased.Contains(Data.allItemsIndexes[itemIndex])))) {
                            storeItems.Add(Data.allItemsIndexes[itemIndex]);
                        }
                    }
                }
                foreach (ItemIndex itemIndex in RoR2.ItemCatalog.tier3ItemList) {
                    if (Data.UnlockedItem(Data.allItemsIndexes[itemIndex], Data.mode)) {
                        if (Data.mode == DataNoShop.mode || (Data.mode == DataShop.mode && (!blueprintsMatter || ownBlueprint == DataShop.blueprintsPurchased.Contains(Data.allItemsIndexes[itemIndex])))) {
                            storeItems.Add(Data.allItemsIndexes[itemIndex]);
                        }
                    }
                }
                foreach (ItemIndex itemIndex in Data.bossItems) {
                    if (Data.UnlockedItem(Data.allItemsIndexes[itemIndex], Data.mode)) {
                        if (Data.mode == DataNoShop.mode || (Data.mode == DataShop.mode && (!blueprintsMatter || ownBlueprint == DataShop.blueprintsPurchased.Contains(Data.allItemsIndexes[itemIndex])))) {
                            storeItems.Add(Data.allItemsIndexes[itemIndex]);
                        }
                    }
                }
                foreach (ItemIndex itemIndex in RoR2.ItemCatalog.lunarItemList) {
                    if (Data.UnlockedItem(Data.allItemsIndexes[itemIndex], Data.mode)) {
                        if (Data.mode == DataNoShop.mode || (Data.mode == DataShop.mode && (!blueprintsMatter || ownBlueprint == DataShop.blueprintsPurchased.Contains(Data.allItemsIndexes[itemIndex])))) {
                            storeItems.Add(Data.allItemsIndexes[itemIndex]);
                        }
                    }
                }
                foreach (EquipmentIndex equipmentIndex in RoR2.EquipmentCatalog.equipmentList) {
                    if (Data.UnlockedItem(Data.allEquipmentIndexes[equipmentIndex], Data.mode) && !Data.lunarEquipment.Contains(equipmentIndex)) {
                        if (Data.mode == DataNoShop.mode || (Data.mode == DataShop.mode && (!blueprintsMatter || ownBlueprint == DataShop.blueprintsPurchased.Contains(Data.allEquipmentIndexes[equipmentIndex])))) {
                            storeItems.Add(Data.allEquipmentIndexes[equipmentIndex]);
                        }
                    }
                }
                foreach (EquipmentIndex equipmentIndex in Data.lunarEquipment) {
                    if (Data.UnlockedItem(Data.allEquipmentIndexes[equipmentIndex], Data.mode)) {
                        if (Data.mode == DataNoShop.mode || (Data.mode == DataShop.mode && (!blueprintsMatter || ownBlueprint == DataShop.blueprintsPurchased.Contains(Data.allEquipmentIndexes[equipmentIndex])))) {
                            storeItems.Add(Data.allEquipmentIndexes[equipmentIndex]);
                        }
                    }
                }
                foreach (EquipmentIndex equipmentIndex in Data.eliteEquipment) {
                    if (Data.UnlockedItem(Data.allEquipmentIndexes[equipmentIndex], Data.mode)) {
                        if (Data.mode == DataNoShop.mode || (Data.mode == DataShop.mode && (!blueprintsMatter || ownBlueprint == DataShop.blueprintsPurchased.Contains(Data.allEquipmentIndexes[equipmentIndex])))) {
                            storeItems.Add(Data.allEquipmentIndexes[equipmentIndex]);
                        }
                    }
                }
                foreach (int droneID in Data.allDroneIDs.Keys) {
                    if (Data.mode == DataNoShop.mode || (Data.mode == DataShop.mode && (!blueprintsMatter || ownBlueprint == DataShop.blueprintsPurchased.Contains(droneID)))) {
                        storeItems.Add(droneID);
                    }
                }
                return storeItems;
            }

            


            static void ClearShopInterfaces() {
                foreach (GameObject gameObject in shopInterfaces) {
                    Destroy(gameObject);
                }
                shopInterfaces.Clear();
                statusTexts.Clear();
                itemImages.Clear();
                itemTexts.Clear();
            }

            static public void Refresh() {
                Color enabledColor = new Color(1, 1, 1, 1);
                Color disabledColor = new Color(0.25f, 0.25f, 0.25f, 1);

                if (Data.modEnabled) {
                    statusTexts[0].color = enabledColor;
                    statusTexts[0].text = "Enabled";
                } else {
                    statusTexts[0].color = disabledColor;
                    statusTexts[0].text = "Disabled";
                }

                UIDrawerNoShop.Refresh();
                UIDrawerShop.Refresh();
            }


            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


            static public void SetMenuStartingItems() {
                UIVanilla.menuController.SetDesiredMenuScreen(UIDrawer.itemDropListMenu);
            }

            static public void SetMenuTitle() {
                UIVanilla.menuController.SetDesiredMenuScreen(UIVanilla.mainMenu);
            }

            static public void OpenStartingItems() {
                DataShop.shopMode = 0;
                Data.RefreshInfo();
                UIDrawer.DrawUI();
                UIDrawer.rootTransform.transform.parent.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }

            static public void CloseStartingItems() {
                UIDrawer.rootTransform.transform.parent.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
        }
    }
}
