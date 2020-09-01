using System;
using System.IO;
using System.Collections.Generic;
using R2API;
using UnityEngine;
using UnityEngine.UI;

namespace Phedg1Studios {
    namespace ItemDropList {
        public class Util : MonoBehaviour {

            void Update() {
                if (Input.GetKeyDown(KeyCode.F2)) {
                    /*
                    Transform newObject = null;
                    List<string> objectHierarchyB = new List<string>() { "MainMenu", "MENU: Title", "TitleMenu", "SafeZone", "GenericMenuButtonPanel", "JuicePanel", "GenericMenuButton (Logbook)" };
                    if (Util.util.GetObjectFromHierarchy(ref newObject, objectHierarchyB, 0, null)) {
                        //newObject.gameObject;
                    }
                    */
                    //SaveSceneHierarchy();
                    //LogComponentsOfObject(GameObject.Find("ItemEntryIcon(Clone)"));
                    //LogComponentsOfType(typeof(RoR2.UI.TooltipController));
                }
            }

            static public string TrimString(string givenString) {
                if (givenString.Length > 0) {
                    return givenString.Substring(0, givenString.Length - 1);
                }
                return "";
            }

            static public string MultilineToSingleLine(string givenFallback, string givenPath) {
                if (File.Exists(givenPath)) {
                    StreamReader reader = new StreamReader(givenPath);
                    string line = "";
                    while (reader.Peek() >= 0) {
                        line += reader.ReadLine() + Data.splitChar;
                    }
                    reader.Close();
                    line = TrimString(line);
                    return line;
                }
                return givenFallback;
            }

            static public string ListToString(List<int> givenList) {
                string newString = "";
                foreach (int item in givenList) {
                    newString += item.ToString() + Data.splitChar;
                }
                newString = TrimString(newString);
                return newString;
            }

            static public string GetConfig(Dictionary<string, string> config, List<string> keys) {
                foreach (string key in keys) {
                    if (config.ContainsKey(key)) {
                        return config[key];
                    }
                }
                return "";
            }

            static public void LogComponentsOfObject(GameObject givenObject) {
                if (givenObject != null) {
                    Component[] components = givenObject.GetComponents(typeof(Component));
                    foreach (Component component in components) {
                        print(component);
                    };
                }
            }

            static public void LogComponentsOfType(Type givenType) {
                UnityEngine.Object[] sceneObjects = GameObject.FindObjectsOfType(givenType);
                print(sceneObjects.Length);
                foreach (UnityEngine.Object sceneObject in sceneObjects) {
                    RoR2.UI.TooltipController controller = sceneObject as RoR2.UI.TooltipController;
                    print(controller.GetComponent<Canvas>());
                    print(controller.GetComponent<Canvas>().sortingOrder);
                    print(sceneObject.name);
                }
            }

            static public void LogComponentsOfChildren(Transform givenTransform) {
                LogComponentsOfObject(givenTransform.gameObject);
                for (int childIndex = 0; childIndex < givenTransform.childCount; childIndex++) {
                    LogComponentsOfChildren(givenTransform.GetChild(childIndex));
                }
            }

            static public void SaveSceneHierarchy() {
                GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
                string sceneTree = "";
                foreach (GameObject rootObject in rootObjects) {
                    sceneTree = MapHierarchy(rootObject.transform, 0, sceneTree);
                }
                string sceneTreePath = BepInEx.Paths.BepInExRootPath + "/" + "config" + "/" + Data.modFolder + "/" + "SceneTree.txt";
                StreamWriter writer = new StreamWriter(sceneTreePath, false);
                writer.Write(sceneTree);
                writer.Close();
            }

            static string MapHierarchy(Transform givenTransform, int givenDepth, string givenTree) {
                string workingString = "";
                for (int spaceNumber = 0; spaceNumber < givenDepth * 4; spaceNumber++) {
                    workingString += " ";
                }
                workingString += givenTransform.gameObject.name + "\n";
                givenTree += workingString;
                for (int childIndex = 0; childIndex < givenTransform.childCount; childIndex++) {
                    givenTree = MapHierarchy(givenTransform.GetChild(childIndex), givenDepth + 1, givenTree);
                }
                return givenTree;
            }

            static public bool GetObjectFromScene(ref Transform desiredObject, List<string> hierarchy, int hierarchyIndex, Transform parent) {
                GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
                for (int rootIndex = 0; rootIndex < rootObjects.Length; rootIndex++) {
                    if (rootObjects[rootIndex].name == hierarchy[hierarchyIndex]) {
                        parent = rootObjects[rootIndex].transform;
                        if (hierarchyIndex == hierarchy.Count - 1) {
                            desiredObject = parent;
                            return true;
                        } else {
                            return GetObjectFromHierarchy(ref desiredObject, hierarchy, hierarchyIndex + 1, parent);
                        }
                    }
                }
                return false;
            }

            static public bool GetObjectFromHierarchy(ref Transform desiredObject, List<string> hierarchy, int hierarchyIndex, Transform parent) {
                bool childFound = false;
                for (int childIndex = 0; childIndex < parent.childCount; childIndex++) {
                    if (parent.GetChild(childIndex).name == hierarchy[hierarchyIndex]) {
                        parent = parent.GetChild(childIndex);
                        childFound = true;
                    }
                }
                if (childFound) {
                    if (hierarchyIndex == hierarchy.Count - 1) {
                        desiredObject = parent;
                        return true;
                    } else {
                        return GetObjectFromHierarchy(ref desiredObject, hierarchy, hierarchyIndex + 1, parent);
                    }
                }
                return false;
            }

            static public void LogAllInteractables() {
                RoR2.InteractableSpawnCard[] allInteractables = UnityEngine.Resources.LoadAll<RoR2.InteractableSpawnCard>("SpawnCards/InteractableSpawnCard");
                foreach (RoR2.InteractableSpawnCard spawnCard in allInteractables) {
                    if(false){//spawnCard.name == "iscBrokenDrone1") {
                        LogComponentsOfObject(spawnCard.prefab);
                        print("-");
                        print(spawnCard.prefab.GetComponent<RoR2.SummonMasterBehavior>().masterPrefab.GetComponent<RoR2.CharacterMaster>().bodyPrefab.GetComponent<RoR2.CharacterBody>().portraitIcon.name);
                    }
                    print(spawnCard.name);
                    if (false) {//spawnCard.name.Contains("Shrine")) {
                        print("-");
                        //print(MapHierarchy(spawnCard.prefab.transform, 0, "\n"));
                        //util.LogComponentsOfObject(spawnCard.prefab);
                        //print("-");
                        Transform desiredTransform = null;
                        print(spawnCard.prefab.name);
                        if (GetObjectFromHierarchy(ref desiredTransform, new List<string>() { "Symbol" }, 0, spawnCard.prefab.transform)) {
                            //print(desiredTransform.GetComponent<MeshFilter>().mesh.name);
                            //print(desiredTransform.GetComponent<MeshRenderer>().material.mainTexture.name);
                            //print(desiredTransform.GetComponent<MeshRenderer>().material.shader.name);

                            
                            Color imageColor = desiredTransform.GetComponent<MeshRenderer>().material.GetColor("_TintColor");
                            print(Mathf.FloorToInt(imageColor.r * 255f).ToString() + ", " + Mathf.FloorToInt(imageColor.g * 255f).ToString() + ", " + Mathf.FloorToInt(imageColor.b * 255f).ToString());
                            //desiredTransform.GetComponent<MeshRenderer>().material.SetColor("_TintColor", new Color(1, 1, 1, 1));
                            //DestroyImmediate(desiredTransform.gameObject);
                            //util.LogComponentsOfObject(desiredTransform.gameObject);
                            //print(desiredTransform.GetComponent<Image>().sprite.name);
                            //print(desiredTransform.GetComponent<Image>().color);

                            /*
                            [Info   : Unity Log] Symbol (UnityEngine.Transform)
                            [Info   : Unity Log] Symbol (UnityEngine.MeshRenderer)
                            [Info   : Unity Log] Symbol (UnityEngine.MeshFilter)
                            [Info   : Unity Log] Symbol (RoR2.Billboard)
                            */
                        }


                        //util.LogComponentsOfObject(spawnCard.prefab.GetComponent<RoR2.IHologramContentProvider>().GetHologramContentPrefab());
                        //print(spawnCard.prefab.GetComponent<RoR2.SummonMasterBehavior>().masterPrefab.GetComponent<RoR2.CharacterMaster>().bodyPrefab.GetComponent<RoR2.CharacterBody>().portraitIcon.name);
                    }
                }
            }
        }
    }
}
