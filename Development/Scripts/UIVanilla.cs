using UnityEngine;
using UnityEngine.UI;
using RoR2;
using R2API;
using System.Collections.Generic;

namespace Phedg1Studios {
    namespace ItemDropList {
        public class UIVanilla : MonoBehaviour {
            static public GameObject logbookButton;
            static public RoR2.UI.MainMenu.MainMenuController menuController;
            static public RoR2.UI.MainMenu.BaseMainMenuScreen mainMenu;


            static public void GetObjectsFromScene() {
                menuController = GameObject.FindObjectOfType(typeof(RoR2.UI.MainMenu.MainMenuController)) as RoR2.UI.MainMenu.MainMenuController;

                Transform newObject = null;
                List<string> objectHierarchyA = new List<string>() { "MainMenu", "MENU: Title", "TitleMenu"};
                if (Util.GetObjectFromScene(ref newObject, objectHierarchyA, 0, null)) {
                    mainMenu = newObject.GetComponent<RoR2.UI.MainMenu.BaseMainMenuScreen>();
                }
                List<string> objectHierarchyB = new List<string>() { "MainMenu", "MENU: Title", "TitleMenu", "SafeZone", "GenericMenuButtonPanel", "JuicePanel", "GenericMenuButton (Logbook)" };
                if (Util.GetObjectFromScene(ref newObject, objectHierarchyB, 0, null)) {
                    logbookButton = newObject.gameObject;
                }
            }

            static public void CreateMenuButton() {
                if (logbookButton != null) {
                    GameObject button = ButtonCreator.SpawnBlueButton(logbookButton.transform.parent.gameObject, new Vector2(0, 1), new Vector2(320, 48), "Item Drops", TMPro.TextAlignmentOptions.Left, new List<Image>());
                    button.transform.SetSiblingIndex(logbookButton.transform.GetSiblingIndex());
                    float localScale = logbookButton.GetComponent<RectTransform>().rect.width / 320;
                    button.GetComponent<RectTransform>().localScale = new Vector3(localScale, localScale, 1);
                    button.GetComponent<RoR2.UI.HGButton>().onClick.AddListener(() => {
                        UIDrawer.SetMenuStartingItems();
                    });
                }
            }

            static public void PopulateDroneCatalogue() {
                int index = 2000;
                RoR2.InteractableSpawnCard[] allInteractables = UnityEngine.Resources.LoadAll<RoR2.InteractableSpawnCard>("SpawnCards/InteractableSpawnCard");
                foreach (RoR2.InteractableSpawnCard spawnCard in allInteractables) {
                    if (spawnCard.name.Contains("Broken")) {
                        string interactableName = InteractableCalculator.GetSpawnCardName(spawnCard);
                        Texture texture = spawnCard.prefab.GetComponent<RoR2.SummonMasterBehavior>().masterPrefab.GetComponent<RoR2.CharacterMaster>().bodyPrefab.GetComponent<RoR2.CharacterBody>().portraitIcon;
                        Sprite sprite = Sprite.Create(texture as Texture2D, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                        if (!Data.allDroneNames.ContainsKey(interactableName)) {
                            Data.allDroneIDs.Add(index, interactableName);
                            Data.allDroneNames.Add(interactableName, index);
                            Data.allDroneIcons.Add(index, sprite);
                            index += 1;
                        }
                    }
                }
            }
        }
    }
}
