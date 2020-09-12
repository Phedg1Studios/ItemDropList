using System;
using System.IO;
using System.Collections.Generic;
using RoR2;
using UnityEngine;
using R2API;

namespace Phedg1Studios {
    namespace ItemDropList {
        public class Data : MonoBehaviour {
            static public Dictionary<int, ItemIndex> allItemIDs = new Dictionary<int, ItemIndex>();
            static public Dictionary<ItemIndex, int> allItemsIndexes = new Dictionary<ItemIndex, int>();
            static public List<ItemIndex> scrapItems = new List<ItemIndex>();
            static public List<ItemIndex> bossItems = new List<ItemIndex>();
            static public Dictionary<int, EquipmentIndex> allEquipmentIDs = new Dictionary<int, EquipmentIndex>();
            static public Dictionary<EquipmentIndex, int> allEquipmentIndexes = new Dictionary<EquipmentIndex, int>();
            static public List<EquipmentIndex> lunarEquipment = new List<EquipmentIndex>();
            static public List<EquipmentIndex> eliteEquipment = new List<EquipmentIndex>();
            
            static public List<ItemIndex> pearls = new List<ItemIndex>() {
                ItemIndex.Pearl,
                ItemIndex.ShinyPearl,
            };
            static public Dictionary<int, string> allDroneIDs = new Dictionary<int, string>();
            static public Dictionary<string, int> allDroneNames = new Dictionary<string, int>();
            static public Dictionary<int, Sprite> allDroneIcons = new Dictionary<int, Sprite>();

            static public List<RoR2.ItemIndex> badItems = new List<ItemIndex>() {
                ItemIndex.CaptainDefenseMatrix
                /*
                ItemIndex.TempestOnKill,
                ItemIndex.WarCryOnCombat,
                ItemIndex.BurnNearby,
                ItemIndex.CritHeal,
                ItemIndex.ExtraLifeConsumed,
                ItemIndex.TonicAffliction,
                */
            };
            static public bool modEnabledDefault = true;
            static private bool showAllItemsDefault = false;
            static public float interactableMultiplierDefault = 1;
            static public int modeDefault = 0;
            static public bool effectMonsterItems;
            static public bool effectMonsterItemsDefault = false;

            static private int configVersion = 6;
            static private int configFileVersion = -1;
            static public bool modEnabled;
            static private bool showAllItems;
            static public float interactableMultiplier;
            static public List<int> profile = new List<int>() { 0, 0};
            static public int profileCount = 3;
            static public string userProfile = "";
            static public int mode;
            static public int modeCount = 2;
            static public List<int> itemsToDrop = new List<int>();
            static private bool defensiveMicrobots;
            static private bool defensiveMicrobotsDefault = false;

            static public string developerName = "Phedg1 Studios";
            static public string modName = "Item Drop List";
            static public string modFolder;
            static public string configFile = ".cfg";
            static public string profileConfigFile = ".txt";
            static public char profileChar = '/';
            static public readonly char splitChar = ',';
            static public char variableChar = '=';

            static public List<string> configVersionName = new List<string>() { "configVersion" };
            static public List<string> enabledName = new List<string>() { "enabled" };
            static public List<string> showAllName = new List<string>() { "showAllItems" };
            static public List<string> modeName = new List<string>() { "mode" };
            static public List<string> interactableMultiplierName = new List<string>() { "interactableMultiplier" };
            static public List<string> profileName = new List<string>() { "profile" };
            static public List<string> effectMonsterItemsName = new List<string>() { "effectMonsterItems" };
            static public List<string> defensiveMicrobotsName = new List<string>() { "defensiveMicrobots" };

            static public List<string> configName = new List<string>();
            static public List<string> configProfileName = new List<string>();

            static private List<string> oldConfigFiles = new List<string>();

            static public Dictionary<int, bool> discoveredRequired = new Dictionary<int, bool>() {
                { 0, true },
                { 1, false },
            };


            static public void PopulateItemCatalogues() {
                int index = 0;
                foreach (ItemIndex itemIndex in RoR2.ItemCatalog.allItems) {
                    allItemIDs.Add(index, itemIndex);
                    allItemsIndexes.Add(itemIndex, index);

                    if (itemIndex.ToString().ToLower().Contains("scrap")) {
                        scrapItems.Add(itemIndex);
                    }
                    if (!RoR2.ItemCatalog.tier1ItemList.Contains(itemIndex) &&
                        !RoR2.ItemCatalog.tier2ItemList.Contains(itemIndex) &&
                        !RoR2.ItemCatalog.tier3ItemList.Contains(itemIndex) &&
                        !RoR2.ItemCatalog.lunarItemList.Contains(itemIndex)) {
                        bossItems.Add(itemIndex);
                    }
                    index += 1;
                }
                index = 1000;
                foreach (EquipmentIndex equipmentIndex in RoR2.EquipmentCatalog.allEquipment) {
                    allEquipmentIDs.Add(index, equipmentIndex);
                    allEquipmentIndexes.Add(equipmentIndex, index);
                    
                    if (!RoR2.EquipmentCatalog.equipmentList.Contains(equipmentIndex)) {
                        eliteEquipment.Add(equipmentIndex);
                    } else if (RoR2.EquipmentCatalog.GetEquipmentDef(equipmentIndex).isLunar) {
                        lunarEquipment.Add(equipmentIndex);
                    }
                    index += 1;
                }
                UIVanilla.PopulateDroneCatalogue();
            }

            static public void MakeDirectoryExist() {
                if (!Directory.Exists(BepInEx.Paths.BepInExRootPath + "/" + "config" + "/" + modName)) {
                    Directory.CreateDirectory(BepInEx.Paths.BepInExRootPath + "/" + "config" + "/" + modName);
                }
            }

            static public void UpdateConfigLocations() {
                modFolder = developerName + "/" + modName;
                configName.Add(BepInEx.Paths.BepInExRootPath + "/" + "config" + "/" + ItemDropList.PluginGUID + configFile);
                configName.Add(BepInEx.Paths.BepInExRootPath + "/" + "config" + "/" + developerName + "/" + modName + "/Config" + configFile);
                configProfileName.Add(BepInEx.Paths.BepInExRootPath + "/" + "config" + "/" + modName + "/");
                configProfileName.Add(BepInEx.Paths.BepInExRootPath + "/" + "config" + "/" + developerName + "/" + modName + "/");
            }


            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


            static public void RefreshInfo(string givenProfileID = "") {
                oldConfigFiles.Clear();
                MakeDirectoryExist();
                GetUserProfileID(givenProfileID);
                Dictionary<string, string> configGlobal = ReadConfig(configName);
                GetConfig(configGlobal);
                Dictionary<string, string> configProfile = ReadConfig(configProfileName, userProfile + profileConfigFile);
                GetConfigProfile(configProfile);
                DataNoShop.RefreshInfo(configGlobal, configProfile);
                DataShop.RefreshInfo(configGlobal, configProfile);
                //CorrectConfig();
                SaveConfig();
                Data.SaveConfigProfile();
                DeleteOldConfig();
                DeleteOldConfigLocations();
            }

            static private void DeleteOldConfigLocations() {
                foreach (string file in oldConfigFiles) {
                    string directory = System.IO.Path.GetDirectoryName(file);
                    File.Delete(file);
                    while (Directory.GetFiles(directory).Length + Directory.GetDirectories(directory).Length == 0) {
                        Directory.Delete(directory);
                        directory = Directory.GetParent(directory).FullName;
                    }
                }
            }

            static void DeleteOldConfig() {
                string oldPath = BepInEx.Paths.BepInExRootPath + "/" + "config" + "/" + Data.modFolder + "/" + Data.userProfile;
                if (Directory.Exists(oldPath)) {
                    Directory.Delete(BepInEx.Paths.BepInExRootPath + "/" + "config" + "/" + Data.modFolder + "/" + Data.userProfile, true);
                }
            }

            static void GetUserProfileID(string givenProfileID = "") {
                if (givenProfileID == "") {
                    List<string> profileIDs = RoR2.UserProfile.GetAvailableProfileNames();
                    foreach (string userID in profileIDs) {
                        if (RoR2.UserProfile.GetProfile(userID) != null) {
                            if (RoR2.UserProfile.GetProfile(userID).loggedIn) {
                                userProfile = userID;
                            }
                        }
                    }
                } else {
                    userProfile = givenProfileID;
                }
            }

            static Dictionary<string, string> ReadConfig(List<string> givenPaths, string givenSuffix = "") {
                Dictionary<string, string> config = new Dictionary<string, string>();
                foreach (string givenPath in givenPaths) {
                    if (File.Exists(givenPath + givenSuffix)) {
                        if (givenPath != givenPaths[0]) {
                            oldConfigFiles.Add(givenPath + givenSuffix);
                        }

                        List<string> lines = new List<string>();
                        StreamReader reader = new StreamReader(givenPath + givenSuffix);
                        while (reader.Peek() >= 0) {
                            lines.Add(reader.ReadLine());
                        }
                        reader.Close();
                        int lineIndex = 0;
                        foreach (string lineRaw in lines) {
                            string line = lineRaw;
                            if ((line.Length >= 4 && line.Substring(0, 4) == "### ")) {
                                line = line.Substring(4, line.Length - 4);
                            }
                            if (!string.IsNullOrEmpty(line) && !new List<string>() { "#", " " }.Contains(line.Substring(0, 1))) {
                                string[] splitLine = line.Split(variableChar);
                                if (splitLine.Length == 2) {
                                    for (int splitIndex = 0; splitIndex < splitLine.Length; splitIndex++) {
                                        splitLine[splitIndex] = splitLine[splitIndex].Replace(" ", "");
                                    }
                                    if (!config.ContainsKey(splitLine[0])) {
                                        config.Add(splitLine[0], splitLine[1]);
                                    }
                                } else if (splitLine.Length == 1) {
                                    config.Add(lineIndex.ToString(), splitLine[0]);
                                }
                                lineIndex += 1;
                            }
                        }
                        break;
                    }
                }
                return config;
            }

            static void GetConfig(Dictionary<string, string> config) {
                configFileVersion = ParseInt(-1, Util.GetConfig(config, configVersionName));
                bool modEnabledOld = modEnabled;
                modEnabled = ParseBool(modEnabledDefault, Util.GetConfig(config, enabledName));
                if (modEnabled != modEnabledOld) {
                    ItemDropList.ToggleDropAPIHooks();
                    modEnabledOld = modEnabled;
                }
                showAllItems = ParseBool(showAllItemsDefault, Util.GetConfig(config, showAllName));
                mode = ParseInt(modeDefault, Util.GetConfig(config, modeName));
                interactableMultiplier = ParseFloat(interactableMultiplierDefault, Util.GetConfig(config, interactableMultiplierName));
                effectMonsterItems = ParseBool(effectMonsterItemsDefault, Util.GetConfig(config, effectMonsterItemsName));
                defensiveMicrobots = ParseBool(defensiveMicrobotsDefault, Util.GetConfig(config, defensiveMicrobotsName));
            }

            static void GetConfigProfile(Dictionary<string, string> config) {
                string line = Util.GetConfig(config, profileName);
                profile.Clear();
                for (int profileIndex = 0; profileIndex < modeCount; profileIndex++) {
                    profile.Add(0);
                }
                string[] splitString = line.Split(splitChar);
                for (int profileIndex = 0; profileIndex < splitString.Length; profileIndex++) {
                    if (profileIndex < modeCount) {
                        int newProfile = 0;
                        if (int.TryParse(splitString[profileIndex], out newProfile)) {
                            if (newProfile >= 0 && newProfile < profileCount) {
                                profile[profileIndex] = newProfile;
                            }
                        }
                    }
                }
            }

            static void CorrectConfig() {
                if (configFileVersion != configVersion) {
                    SaveConfig();
                }
            }

            static public bool ParseBool(bool givenDefault, string givenString) {
                bool result = false;
                if (bool.TryParse(givenString, out result)) {
                    return result;
                }
                return givenDefault;
            }

            static public int ParseInt(int givenDefault, string givenString) {
                int result = 0;
                if (int.TryParse(givenString, out result)) {
                    return result;
                }
                return givenDefault;
            }

            static public float ParseFloat(float givenDefault, string givenString) {
                float result = 0;
                if (float.TryParse(givenString, out result)) {
                    return result;
                }
                return givenDefault;
            }


            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


            static public bool ItemExists(int itemID) {
                if (allItemIDs.ContainsKey(itemID)) {
                    if (RoR2.ItemCatalog.GetItemDef(allItemIDs[itemID]).pickupIconSprite != null && RoR2.ItemCatalog.GetItemDef(allItemIDs[itemID]).pickupIconSprite.name != "texNullIcon") {
                        if (RoR2.ItemCatalog.GetItemDef(allItemIDs[itemID]).tier != ItemTier.NoTier) {
                            if (!badItems.Contains(allItemIDs[itemID]) || (allItemIDs[itemID] == ItemIndex.CaptainDefenseMatrix && defensiveMicrobots)) {
                                return true;
                            }
                        }
                    }
                } else if (allEquipmentIDs.ContainsKey(itemID)) {
                    if (RoR2.EquipmentCatalog.GetEquipmentDef(allEquipmentIDs[itemID]).pickupIconSprite != null && RoR2.EquipmentCatalog.GetEquipmentDef(allEquipmentIDs[itemID]).pickupIconSprite.name != "texNullIcon") {
                        return true;
                    }
                } else if (allDroneIDs.ContainsKey(itemID)) {
                    return true;
                }
                return false;
            }

            static public bool UnlockedItem(int itemID, int givenMode) {
                if (ItemExists(itemID)) {
                    if (showAllItems) {
                        return true;
                    }
                    if (allItemIDs.ContainsKey(itemID)) {
                        if (RoR2.UserProfile.GetProfile(userProfile).HasUnlockable(RoR2.ItemCatalog.GetItemDef(allItemIDs[itemID]).unlockableName)) {
                            if (discoveredRequired[givenMode] == false || RoR2.UserProfile.GetProfile(userProfile).HasDiscoveredPickup(PickupCatalog.FindPickupIndex(allItemIDs[itemID]))) {
                                return true;
                            }
                        }
                    } else if (allEquipmentIDs.ContainsKey(itemID)) {
                        if (RoR2.UserProfile.GetProfile(userProfile).HasUnlockable(RoR2.EquipmentCatalog.GetEquipmentDef(allEquipmentIDs[itemID]).unlockableName)) {
                            if (discoveredRequired[givenMode] == false || RoR2.UserProfile.GetProfile(userProfile).HasDiscoveredPickup(PickupCatalog.FindPickupIndex(allEquipmentIDs[itemID]))) {
                                return true;
                            }
                        }
                    } else if (allDroneIDs.ContainsKey(itemID)) {
                        return true;
                    }
                }
                return false;
            }

            static public int GetItemTier(int givenID) {
                if (allItemIDs.ContainsKey(givenID)) {
                    if (RoR2.ItemCatalog.tier1ItemList.Contains(allItemIDs[givenID])) {
                        return 0;
                    } else if (RoR2.ItemCatalog.tier2ItemList.Contains(allItemIDs[givenID])) {
                        return 1;
                    } else if (RoR2.ItemCatalog.tier3ItemList.Contains(allItemIDs[givenID])) {
                        return 2;
                    } else if (bossItems.Contains(allItemIDs[givenID])) {
                        return 3;
                    } else if (RoR2.ItemCatalog.lunarItemList.Contains(allItemIDs[givenID])) {
                        return 4;
                    }
                } else if (allEquipmentIDs.ContainsKey(givenID)) {
                    if (lunarEquipment.Contains(allEquipmentIDs[givenID])) {
                        return 4;
                    }
                    return 5;
                } else if (allDroneIDs.ContainsKey(givenID)) {
                    return 6;
                }
                return 5;
            }

            static public ItemIndex GetScrapIndex(int givenTeir) {
                foreach (ItemIndex scrapIndex in Data.scrapItems) {
                    if (Data.GetItemTier(Data.allItemsIndexes[scrapIndex]) == givenTeir) {
                        return scrapIndex;
                    }
                }
                return ItemIndex.None;
            }

            static public void GetItemList(Dictionary<string, string> config, List<string> configName, List<List<int>> givenList, string givenFile, int givenMode) {
                string line = Util.GetConfig(config, configName);
                string itemsPurchasedPath = BepInEx.Paths.BepInExRootPath + "/" + "config" + "/" + Data.modFolder + "/" + Data.userProfile + "/" + givenFile;
                line = Util.MultilineToSingleLine(line, itemsPurchasedPath);
                givenList.Clear();
                for (int profile = 0; profile < profileCount; profile++) {
                    givenList.Add(new List<int>());
                }

                int profileIndex = 0;
                string[] profiles = line.Split(Data.profileChar);
                foreach (string profile in profiles) {
                    if (profileIndex < givenList.Count) {
                        string[] itemIDs = profile.Split(Data.splitChar);
                        foreach (string itemIDString in itemIDs) {
                            if (!string.IsNullOrEmpty(itemIDString)) {
                                int itemID = 0;
                                bool itemIDParsed = int.TryParse(itemIDString, out itemID);
                                if (itemIDParsed) {
                                    if (Data.UnlockedItem(itemID, givenMode)) {
                                        if (!givenList[profileIndex].Contains(itemID)) {
                                            givenList[profileIndex].Add(itemID);
                                        }
                                    }
                                }
                            }
                        }
                        profileIndex += 1;
                    }
                }
            }


            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


            static public void SetProfile(int givenProfile) {
                profile[mode] = givenProfile;
                SaveConfigProfile();
                UIDrawer.Refresh();
            }

            static public void ToggleItem(int givenID, bool shouldRefresh = true) {
                DataNoShop.ToggleItem(givenID, shouldRefresh);
                DataShop.ToggleItem(givenID, shouldRefresh);
            }

            static public void ToggleTier(int givenID) {
                List<int> toggleList = GetLargeToggleList();
                List<int> storeItems = UIDrawer.GetStoreItemsA();
                bool desiredToggle = toggleList.Contains(givenID);
                int desiredTier = GetItemTier(givenID);
                foreach (int item in storeItems) {
                    if (GetItemTier(item) == desiredTier) {
                        if (toggleList.Contains(item) == desiredToggle) {
                            ToggleItem(item, false);
                        }
                    }
                }
                SaveConfigProfile();
                UIDrawer.Refresh();
            }

            static public void ToggleAll(int givenID) {

                List<int> toggleList = GetLargeToggleList();
                List<int> storeItems = UIDrawer.GetStoreItemsA();
                bool desiredToggle = toggleList.Contains(givenID);
                foreach (int item in storeItems) {
                    if (toggleList.Contains(item) == desiredToggle) {
                        ToggleItem(item, false);
                    }
                }
                SaveConfigProfile();
                UIDrawer.Refresh();
            }

            static List<int> GetLargeToggleList() {
                if (Data.mode == DataNoShop.mode) {
                    return DataNoShop.itemsToDrop[profile[mode]];
                } else if (Data.mode == DataShop.mode) {
                    return DataShop.itemsToDrop[profile[mode]];
                }
                return new List<int>();
            }

            static public void ToggleEnabled() {
                modEnabled = !modEnabled;
                SaveConfig();
                UIDrawer.Refresh();
                ItemDropList.ToggleDropAPIHooks();
            }

            static void SaveConfig() {
                string spacing = " ";
                string variableCharUpdate = spacing + variableChar + spacing;
                string configPath = configName[0];
                StreamWriter writer = new StreamWriter(configPath, false);
                string configString = "";
                configString += "### configVersion = " + configVersion.ToString();
                configString += "\n\n[General]";
                configString += "\n\n# Enable/disable the ItemDropList mod\n#\n# Setting type: Boolean\n# Default value: " + modEnabledDefault.ToString() + "\n" + enabledName[0] + variableCharUpdate + modEnabled.ToString().ToLower();
                configString += "\n\n# Enable/disable showing all items mod\n# When enabled all items and equipment will be listed, even those which the player has not unlocked and discovered\n#\n# Setting type: Boolean\n# Default value: " + showAllItemsDefault.ToString() + "\n" + showAllName[0] + variableCharUpdate + showAllItems.ToString();
                configString += "\n\n# The mode currently in use\n# 0 is Standard, 1 is Training\n#\n# Setting type: Integer\n# Default value: " + modeDefault.ToString() + "\n" + modeName[0] + variableCharUpdate + mode.ToString();
                configString += "\n\n# Whether monsters should be limited to the same item list\n# When active this effects the Artifact of Evolution, Void Fields and Scavengers\n#\n# Setting type: Boolean\n# Default value: " + effectMonsterItemsDefault.ToString() + "\n" + effectMonsterItemsName[0] + variableCharUpdate + effectMonsterItems.ToString();
                configString += "\n\n# Whether Defensive Microbots should be added to the item list\n# When active it can be found in chests and be given to monsters\n#\n# Setting type: Boolean\n# Default value: " + defensiveMicrobotsDefault.ToString() + "\n" + defensiveMicrobotsName[0].ToString() + variableCharUpdate + defensiveMicrobots.ToString();
                configString += "\n\n# Multiply amount of intereactables spawned per stage\n#\n# Setting type: Float\n# Default value: " + interactableMultiplierDefault.ToString() + "\n" + interactableMultiplierName[0] + variableCharUpdate + interactableMultiplier.ToString();
                configString += "\n\n[Standard]";
                configString += "\n\n# Whether items must be discovered to show up in the menu\n#\n# Setting type: Boolean\n# Default value: " + DataNoShop.discoveredRequiredDefault.ToString() + "\n" + DataNoShop.discoveredRequiredName[0] + variableCharUpdate + discoveredRequired[DataNoShop.mode].ToString();
                configString += "\n\n[Training]";
                //configString += "\n\n# Chance to find scrap when picking up an item\n#\n# Setting type: Float\n# Default value: 1.0\nscrapDropChance = " + DataShop.scrapDropChance.ToString();
                configString += "\n\n# Whether purchased items can be removed from the drop list\n#\n# Setting type: Boolean\n# Default value: " + DataShop.canDisablePurchasedBlueprintsDefault.ToString() + "\n" + DataShop.toggleItemsName[0] + variableCharUpdate + DataShop.canDisablePurchasedBlueprints.ToString();
                configString += "\n\n# The multiplier for how much aptitude score of each tier can be held at once relative to that tier's price\n# The formula is maxAptitudeScoreMultiplier x price\n# A value smaller than 1 will allow unlimited aptitude score\n#\n# Setting type: Float\n# Default value: " + DataShop.maxScrapMultiplierDefault.ToString() + "\n" + DataShop.maxScoreMultiplierName[0] + variableCharUpdate + DataShop.maxScrapMultiplier.ToString();
                configString += "\n\n# How much common aptitude score a common item costs\n#\n# Setting type: Integer\n# Default value: " + DataShop.pricesDefault[0].ToString() + "\n" + DataShop.tier1PriceName[0] + variableCharUpdate + DataShop.prices[0].ToString();
                configString += "\n\n# How much uncommon aptitude score an uncommon item costs\n#\n# Setting type: Integer\n# Default value: " + DataShop.pricesDefault[1].ToString() + "\n" + DataShop.tier2PriceName[0] + variableCharUpdate + DataShop.prices[1].ToString();
                configString += "\n\n# How much rare aptitude score a rare item costs\n#\n# Setting type: Integer\n# Default value: " + DataShop.pricesDefault[2].ToString() + "\n" + DataShop.tier3PriceName[0] + variableCharUpdate + DataShop.prices[2].ToString();
                configString += "\n\n# How much boss aptitude score a boss item costs\n#\n# Setting type: Integer\n# Default value: " + DataShop.pricesDefault[3].ToString() + "\n" + DataShop.tierBossPriceName[0] + variableCharUpdate + DataShop.prices[3].ToString();
                configString += "\n\n# How much lunar scaptitude scorerap a lunar item or equipment costs\n#\n# Setting type: Integer\n# Default value: " + DataShop.pricesDefault[4].ToString() + "\n" + DataShop.tierLunarPriceName[0] + variableCharUpdate + DataShop.prices[4].ToString();
                configString += "\n\n# How much equipment aptitude score equipment costs\n#\n# Setting type: Integer\n# Default value: " + DataShop.pricesDefault[5].ToString() + "\n" + DataShop.tierEquipmentPriceName[0] + variableCharUpdate + DataShop.prices[5].ToString();
                configString += "\n\n# How much drone aptitude score a drone costs\n#\n# Setting type: Integer\n# Default value: " + DataShop.pricesDefault[6].ToString() + "\n" + DataShop.tierDronePriceName[0] + variableCharUpdate + DataShop.prices[6].ToString();
                writer.Write(configString);
                writer.Close();
            }

            static public void SaveConfigProfile() {
                string spacing = " ";
                string variableCharUpdate = spacing + variableChar + spacing;
                string configPath = configProfileName[0] + userProfile + profileConfigFile;
                StreamWriter writer = new StreamWriter(configPath, false);
                string configString = "";
                configString += Data.profileName[0] + variableCharUpdate + Util.ListToString(Data.profile);
                configString += "\n" + DataNoShop.itemsToDropName[0] + variableCharUpdate + Util.ListListToString(DataNoShop.itemsToDrop);
                configString += "\n" + DataShop.itemsPurchasedName[0] + variableCharUpdate + Util.ListToString(DataShop.blueprintsPurchased);
                configString += "\n" + DataShop.itemsToDropName[0] + variableCharUpdate + Util.ListListToString(DataShop.itemsToDrop);
                configString += "\n" + DataShop.scrapName[0] + variableCharUpdate + Util.ListToString(DataShop.scrap);
                configString += "\n" + DataShop.scrapRecentName[0] + variableCharUpdate + Util.ListToString(DataShop.scrapRecent);
                configString += "\n" + DataShop.scrapStartingName[0] + variableCharUpdate + Util.ListToString(DataShop.scrapStarting);
                writer.Write(configString);
                writer.Close();
            }


            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


            static public List<int> DuplicateItemList(List<int> givenList) {
                List<int> newList = new List<int>();
                foreach(int integer in givenList) {
                    newList.Add(integer);
                }
                return newList;
            }

            static public void SetDropList() {
                if (Data.modEnabled) {
                    DataNoShop.SetDropList();
                    DataShop.SetDropList();
                    ItemDropAPIFixes.ItemDropAPIFixes.playerItems = ConvertDropList(itemsToDrop);
                    if (effectMonsterItems) {
                        ItemDropAPIFixes.ItemDropAPIFixes.monsterItems = ConvertDropList(itemsToDrop);
                    }
                }
            }

            static public List<PickupIndex> ConvertDropList(List<int> givenDropList) {
                List<PickupIndex> pickupList = new List<PickupIndex>();
                foreach (int itemID in givenDropList) {
                    if (allItemIDs.ContainsKey(itemID)) {
                        pickupList.Add(PickupCatalog.FindPickupIndex(allItemIDs[itemID]));
                    } else if (allEquipmentIDs.ContainsKey(itemID)) {
                        pickupList.Add(PickupCatalog.FindPickupIndex(allEquipmentIDs[itemID]));
                    }
                }
                return pickupList;
            }
        }
    }
}
