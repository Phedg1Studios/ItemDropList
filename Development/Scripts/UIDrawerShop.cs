using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RoR2;

namespace Phedg1Studios {
    namespace ItemDropList {
        public class UIDrawerShop : MonoBehaviour {
            static private List<TMPro.TextMeshProUGUI> scrapTexts = new List<TMPro.TextMeshProUGUI>();
            static private List<List<Image>> modeImages = new List<List<Image>>();
            static private GameObject infoPanel;
            static public readonly List<string> shopModeNames = new List<string>() {
                "Courses",
                "Purchased",
            };
            static Dictionary<int, RoR2.UI.HGButton> modeButtons = new Dictionary<int, RoR2.UI.HGButton>();

            static public void DrawUI() {
                if (Data.mode == DataShop.mode) {
                    scrapTexts.Clear();
                    modeImages.Clear();
                    DrawBlueButtons();
                    DrawScrapTotals();
                    DisableHighlightImages();
                    SetPrices();
                    DrawInfoButton();
                    DrawRecentPanel();
                }
            }

            static void DrawBlueButtons() {
                modeButtons.Clear();
                for (int modeIndex = 0; modeIndex < DataShop.shopModeCount; modeIndex++) {
                    modeImages.Add(new List<Image>());
                    GameObject button = ButtonCreator.SpawnBlueButton(UIDrawer.rootTransform.gameObject, new Vector2(0, 1), new Vector2(UIConfig.blueButtonWidth, UIConfig.blueButtonHeight), shopModeNames[modeIndex], TMPro.TextAlignmentOptions.Center, modeImages[modeIndex]);
                    button.GetComponent<RectTransform>().localPosition = new Vector3(UIConfig.offsetHorizontal + (UIConfig.blueButtonWidth + UIConfig.spacingHorizontal) * modeIndex, -UIConfig.offsetVertical, 0);
                    modeButtons.Add(modeIndex, button.GetComponent<RoR2.UI.HGButton>());
                    int mode = modeIndex;
                    GameObject buttonObject = button;
                    button.GetComponent<RoR2.UI.HGButton>().onClick.AddListener(() => {
                        buttonObject.GetComponent<RoR2.UI.MPEventSystemLocator>().eventSystemProvider.resolvedEventSystem.SetSelectedGameObject(null);
                        DataShop.SetShopMode(mode);
                        SelectModeButton();
                    });
                    UIDrawer.shopInterfaces.Add(button);
                }
            }

            static void DrawScrapTotals() {
                if (DataShop.shopMode == 0 || (DataShop.shopMode == 1 && !DataShop.canDisablePurchasedBlueprints)) {
                    float scrapSpacing = 75;
                    float titlePreferredWidth = 195.76f;
                    GameObject panelOutline = PanelCreator.CreatePanelSize(UIDrawer.rootTransform);
                    RectTransform panelTransform = panelOutline.GetComponent<RectTransform>();
                    float panelWidth = titlePreferredWidth + (DataShop.scrap.Count + 0.5f) * scrapSpacing + UIConfig.panelPadding * 2 + 10;
                    panelTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelWidth);
                    panelTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, UIConfig.blueButtonHeight);
                    panelTransform.localPosition = new Vector3(UIDrawer.rootTransform.rect.width - UIConfig.offsetHorizontal - panelWidth, -UIConfig.offsetVertical, 0);
                    UIDrawer.shopInterfaces.Add(panelOutline);

                    for (int tierIndex = 0; tierIndex < DataShop.scrap.Count; tierIndex++) {
                        ElementCreator.SpawnTextSize(scrapTexts, panelOutline.transform.GetChild(0).gameObject, new Color(1, 1, 1, 1), 30, 0, new Vector2(0.5f, 1), new Vector2(300, UIConfig.blueButtonHeight - UIConfig.panelPadding * 2), new Vector3((tierIndex + 0.5f) * scrapSpacing, 0, 0));
                    }
                    List<TMPro.TextMeshProUGUI> titleText = new List<TMPro.TextMeshProUGUI>();
                    ElementCreator.SpawnTextSize(titleText, panelOutline.transform.GetChild(0).gameObject, new Color(1, 1, 1, 1), 30, 0, new Vector2(0, 1), new Vector2(400, UIConfig.blueButtonHeight - UIConfig.panelPadding * 2), new Vector3((DataShop.scrap.Count + 0.5f) * scrapSpacing, 0, 0));
                    titleText[0].text = "APTITUDE SCORES";
                    titleText[0].alignment = TMPro.TextAlignmentOptions.Left;
                }
            }

            static void DisableHighlightImages() {
                foreach (int itemID in UIDrawer.itemImages.Keys) {
                    for (int imageIndex = 0; imageIndex < 2; imageIndex++) {
                        UIDrawer.itemImages[itemID][imageIndex + 2].gameObject.SetActive(false);
                    }
                }
            }

            static void SetPrices() {
                if (DataShop.shopMode == 0) {
                    foreach (int itemID in UIDrawer.itemTexts.Keys) {
                        TMPro.TextMeshProUGUI itemText = UIDrawer.itemTexts[itemID][0];
                        int itemTier = Data.GetItemTier(itemID);
                        itemText.text = DataShop.prices[itemTier].ToString();
                    }
                }
            }

            static void DrawInfoButton() {
                GameObject infoButton = ButtonCreator.SpawnBlackButton(UIDrawer.rootTransform.gameObject, new Vector2(UIConfig.blackButtonWidth / 2f, UIConfig.blackButtonHeight), "?", new List<TMPro.TextMeshProUGUI>());
                infoButton.transform.parent.GetComponent<RectTransform>().localPosition = new Vector3(UIDrawer.rootTransform.rect.width - UIConfig.offsetHorizontal - UIConfig.blackButtonWidth / 2f, -UIConfig.offsetVertical - UIConfig.blueButtonCount[Data.mode] * (UIConfig.blueButtonHeight + UIConfig.spacingVertical) - UIDrawer.storeHeight - UIConfig.spacingVertical, 0);
                infoButton.GetComponent<RoR2.UI.HGButton>().onClick.AddListener(() => {
                    DrawInfoPanel();
                });
                UIDrawer.shopInterfaces.Add(infoButton.transform.parent.gameObject);
            }

            static void DrawRecentPanel() {
                if (DataShop.RecentScrap()) {
                    Transform background = ElementCreator.SpawnImageOffset(new List<Image>(), UIDrawer.rootTransform.transform.parent.gameObject, null, new Color(0, 0, 0, 0.95f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero).transform;
                    background.GetComponent<Image>().raycastTarget = true;

                    GameObject panelOutline = PanelCreator.CreatePanelSize(background);
                    RectTransform panelTransform = panelOutline.GetComponent<RectTransform>();
                    float panelWidth = 600 + UIConfig.panelPadding * 2 + 10;
                    float panelHeight = 400 + UIConfig.panelPadding * 2 + 10;
                    panelTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelWidth);
                    panelTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, panelHeight);
                    panelTransform.localPosition = new Vector3(-panelWidth / 2f, panelHeight / 2f, 0);
                    RectTransform panelChildTransform = panelTransform.GetChild(0).GetComponent<RectTransform>();

                    List<TMPro.TextMeshProUGUI> text = new List<TMPro.TextMeshProUGUI>();
                    ElementCreator.SpawnTextOffset(text, panelTransform.GetChild(0).gameObject, new Color(1, 1, 1), 24, 0, new Vector2(UIConfig.spacingHorizontal, UIConfig.spacingVertical + UIConfig.blackButtonHeight + UIConfig.spacingVertical * 2 + UIConfig.blueButtonHeight + UIConfig.spacingVertical), new Vector2(-UIConfig.spacingHorizontal, -UIConfig.spacingVertical));
                    text[0].text = "THANK YOU";
                    text[0].text += "\nFor your (or your kinsman's) continued service. Working to the benefit of UES is working to the benefit of us all.";
                    text[0].text += "\n";
                    text[0].text += "\nYour UES Aptitude Scores have been increased by the following amounts:";

                    float scrapSpacing = 75;
                    List<TMPro.TextMeshProUGUI> scrapTexts = new List<TMPro.TextMeshProUGUI>();
                    for (int tierIndex = 0; tierIndex < DataShop.scrap.Count; tierIndex++) {
                        ElementCreator.SpawnTextSize(scrapTexts, panelChildTransform.gameObject, new Color(1, 1, 1, 1), 30, 0, new Vector2(0.5f, 1), new Vector2(300, UIConfig.blueButtonHeight - UIConfig.panelPadding * 2), new Vector3(panelChildTransform.rect.width / 2f + (-DataShop.scrap.Count / 2f + tierIndex + 0.5f) * scrapSpacing, -panelChildTransform.rect.height + UIConfig.spacingVertical + UIConfig.blackButtonHeight + UIConfig.spacingVertical * 2 + UIConfig.blueButtonHeight, 0));
                        scrapTexts[tierIndex].text = DataShop.scrapRecent[tierIndex].ToString();
                        if (DataShop.scrapRecent[tierIndex] > 0) {
                            scrapTexts[tierIndex].color = UIConfig.tierColours[tierIndex];
                        } else {
                            scrapTexts[tierIndex].color = UIConfig.tierColours[tierIndex] * 0.5f;
                        }
                    }

                    GameObject backButton = ButtonCreator.SpawnBlackButton(panelChildTransform.gameObject, new Vector2(UIConfig.blackButtonWidth, UIConfig.blackButtonHeight), "Back", new List<TMPro.TextMeshProUGUI>());
                    RectTransform backButtonTransform = backButton.transform.parent.GetComponent<RectTransform>();
                    backButtonTransform.localPosition = new Vector3(panelChildTransform.rect.width / 2f - UIConfig.blackButtonWidth / 2f, -panelHeight + UIConfig.spacingVertical + UIConfig.blackButtonHeight, backButtonTransform.localPosition.z);

                    RoR2.UI.HGButton previousSelectable = backButton.GetComponent<RoR2.UI.MPEventSystemLocator>().eventSystem.currentSelectedGameObject.GetComponent<RoR2.UI.HGButton>();
                    Button backButtonButton = backButton.GetComponent<RoR2.UI.HGButton>();
                    backButtonButton.onClick.AddListener(() => {
                        UIDrawer.rootTransform.GetComponent<CanvasGroup>().interactable = true;
                        if (backButtonButton.GetComponent<RoR2.UI.MPEventSystemLocator>().eventSystem.currentInputSource == RoR2.UI.MPEventSystem.InputSource.Gamepad) {
                            previousSelectable.Select();
                        } else {
                            previousSelectable.enabled = false;
                            previousSelectable.enabled = true;
                        }
                        DataShop.ClearRecentScrap();
                        Data.SaveConfigProfile();
                        Destroy(background.gameObject);
                    });
                    backButtonButton.Select();

                    UIDrawer.rootTransform.GetComponent<CanvasGroup>().interactable = false;
                }
            }

            static void DrawInfoPanel() {
                Transform background = ElementCreator.SpawnImageOffset(new List<Image>(), UIDrawer.rootTransform.transform.parent.gameObject, null, new Color(0, 0, 0, 0.95f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero).transform;
                background.GetComponent<Image>().raycastTarget = true;

                GameObject panelOutline = PanelCreator.CreatePanelSize(background);
                RectTransform panelTransform = panelOutline.GetComponent<RectTransform>();
                float panelWidth = 700 + UIConfig.panelPadding * 2 + 10;
                float panelHeight = 600 + UIConfig.panelPadding * 2 + 10;
                panelTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelWidth);
                panelTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, panelHeight);
                panelTransform.localPosition = new Vector3(- panelWidth / 2f, panelHeight / 2f, 0);
                RectTransform panelChildTransform = panelTransform.GetChild(0).GetComponent<RectTransform>();

                List<TMPro.TextMeshProUGUI> text = new List<TMPro.TextMeshProUGUI>();
                ElementCreator.SpawnTextOffset(text, panelChildTransform.gameObject, new Color(1, 1, 1), 24, 0, new Vector2(UIConfig.spacingHorizontal, UIConfig.spacingVertical + UIConfig.blackButtonHeight + UIConfig.spacingVertical), new Vector2(-UIConfig.spacingHorizontal, -UIConfig.spacingVertical));
                text[0].text = "UES EMPLOYMENT CONTRACT";
                text[0].text += "\nSUBSECTION 17b: SALVAGE";
                text[0].text += "\n";
                text[0].text += "\nShould the employee be deployed to the site of any crashed UES vessels they are to prioritize the recovery of any intact cargo. As part of our vertical integration policies (38d), the employee will be reimbursed for any cargo salvaged with increased UES Aptitude Scores(11a). The category of aptitude increased is dependant on the value of the cargo salvaged, as this demonstrates ability of the employee work to UES’s interests. Employees with sufficiently high scores may redeem them to enrol in approved UES corporate training courses, to be upskilled in the use of various UES products. Should the employee perish during deployment (41a), a drone will be dispatched to recover any cargo that was in their possession. Any Aptitude Score owing will be accredited to the employee’s next of kin, should they also be an employee of UES, as well as the right to enrol in any corporate training courses the employee had previously redeemed.";

                GameObject backButton = ButtonCreator.SpawnBlackButton(panelChildTransform.gameObject, new Vector2(UIConfig.blackButtonWidth, UIConfig.blackButtonHeight), "Back", new List<TMPro.TextMeshProUGUI>());
                RectTransform backButtonTransform = backButton.transform.parent.GetComponent<RectTransform>();
                backButtonTransform.localPosition = new Vector3(panelWidth / 2f - UIConfig.blackButtonWidth / 2f, -panelHeight + UIConfig.spacingVertical + UIConfig.blackButtonHeight, backButtonTransform.localPosition.z);

                RoR2.UI.HGButton previousSelectable = backButton.GetComponent<RoR2.UI.MPEventSystemLocator>().eventSystem.currentSelectedGameObject.GetComponent<RoR2.UI.HGButton>();
                Button backButtonButton = backButton.GetComponent<RoR2.UI.HGButton>();
                backButtonButton.onClick.AddListener(() => {
                    UIDrawer.rootTransform.GetComponent<CanvasGroup>().interactable = true;
                    if (backButtonButton.GetComponent<RoR2.UI.MPEventSystemLocator>().eventSystem.currentInputSource == RoR2.UI.MPEventSystem.InputSource.Gamepad) {
                        previousSelectable.Select();
                    } else {
                        previousSelectable.enabled = false;
                        previousSelectable.enabled = true;
                    }
                    Destroy(background.gameObject);
                });
                backButtonButton.Select();

                UIDrawer.rootTransform.GetComponent<CanvasGroup>().interactable = false;
            }

            static void SelectModeButton() {
                modeButtons[DataShop.shopMode].Select();
            }


            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


            static public void PurchaseItem(int givenID) {
                UIDrawer.itemImages[givenID][UIDrawer.itemImages[givenID].Count - 1].gameObject.SetActive(true);
                UIDrawer.itemImages[givenID][UIDrawer.itemImages[givenID].Count - 2].gameObject.SetActive(true);
            }

            static public void Refresh() {
                if (Data.mode == DataShop.mode) {
                    foreach (int itemID in UIDrawer.itemImages.Keys) {
                        if (DataShop.shopMode == 0) {
                            if (DataShop.scrap[Data.GetItemTier(itemID)] >= DataShop.prices[Data.GetItemTier(itemID)]) {
                                for (int imageIndex = 0; imageIndex < 2; imageIndex++) {
                                    UIDrawer.itemImages[itemID][imageIndex].color = UIConfig.enabledColor;
                                }
                                UIDrawer.itemTexts[itemID][0].color = UIConfig.tierColours[Data.GetItemTier(itemID)];
                            } else {
                                for (int imageIndex = 0; imageIndex < 2; imageIndex++) {
                                    UIDrawer.itemImages[itemID][imageIndex].color = UIConfig.disabledColor;
                                }
                                UIDrawer.itemTexts[itemID][0].color = UIConfig.tierColours[Data.GetItemTier(itemID)] * 0.5f;
                            }
                        } else if (DataShop.shopMode == 1) {
                            if (DataShop.canDisablePurchasedBlueprints) {
                                if (!DataShop.itemsToDrop[Data.profile[DataShop.mode]].Contains(itemID)) {
                                    for (int imageIndex = 0; imageIndex < 2; imageIndex++) {
                                        UIDrawer.itemImages[itemID][imageIndex].color = UIConfig.enabledColor;
                                    }
                                    for (int imageIndex = 0; imageIndex < 2; imageIndex++) {
                                        UIDrawer.itemImages[itemID][imageIndex + 2].gameObject.SetActive(true);
                                    }
                                } else {
                                    for (int imageIndex = 0; imageIndex < 2; imageIndex++) {
                                        UIDrawer.itemImages[itemID][imageIndex].color = UIConfig.disabledColor;
                                    }
                                    for (int imageIndex = 0; imageIndex < 2; imageIndex++) {
                                        UIDrawer.itemImages[itemID][imageIndex + 2].gameObject.SetActive(false);
                                    }
                                }
                            }
                        }
                    }
                    for (int shopModeIndex = 0; shopModeIndex < DataShop.shopModeCount; shopModeIndex++) {
                        foreach (Image image in modeImages[shopModeIndex]) {
                            image.gameObject.SetActive(shopModeIndex == DataShop.shopMode);
                        }
                    }
                    if (DataShop.shopMode == 0) {
                        for (int tierIndex = 0; tierIndex < DataShop.scrap.Count; tierIndex++) {
                            scrapTexts[tierIndex].text = DataShop.scrap[tierIndex].ToString();
                            if (DataShop.scrap[tierIndex] >= DataShop.prices[tierIndex]) {
                                scrapTexts[tierIndex].color = UIConfig.tierColours[tierIndex];
                            } else {
                                scrapTexts[tierIndex].color = UIConfig.tierColours[tierIndex] * 0.5f;
                            }
                        }
                    }
                }
            }
        }
    }
}
