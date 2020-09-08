using System;
using System.IO;
using System.Collections.Generic;
using RoR2;
using UnityEngine;
using R2API;

namespace Phedg1Studios {
    namespace ItemDropAPI {
        public class Catalogue : MonoBehaviour {
            static public List<ItemIndex> allItemsIndexes = new List<ItemIndex>();
            static public Dictionary<ItemTier, ItemIndex> scrapItems = new Dictionary<ItemTier, ItemIndex>();
            static public List<ItemIndex> bossItems = new List<ItemIndex>();
            static public List<EquipmentIndex> allEquipmentIndexes = new List<EquipmentIndex>();
            static public List<EquipmentIndex> lunarEquipment = new List<EquipmentIndex>();
            static public List<EquipmentIndex> eliteEquipment = new List<EquipmentIndex>();
            
            static public List<ItemIndex> pearls = new List<ItemIndex>() {
                ItemIndex.Pearl,
                ItemIndex.ShinyPearl,
            };

            static public void PopulateItemCatalogues() {
                foreach (ItemIndex itemIndex in RoR2.ItemCatalog.allItems) {
                    allItemsIndexes.Add(itemIndex);

                    if (itemIndex.ToString().ToLower().Contains("scrap")) {
                        scrapItems.Add(RoR2.ItemCatalog.GetItemDef(itemIndex).tier, itemIndex);
                    }
                    if (!RoR2.ItemCatalog.tier1ItemList.Contains(itemIndex) &&
                        !RoR2.ItemCatalog.tier2ItemList.Contains(itemIndex) &&
                        !RoR2.ItemCatalog.tier3ItemList.Contains(itemIndex) &&
                        !RoR2.ItemCatalog.lunarItemList.Contains(itemIndex)) {
                        bossItems.Add(itemIndex);
                    }
                }
                foreach (EquipmentIndex equipmentIndex in RoR2.EquipmentCatalog.allEquipment) {
                    allEquipmentIndexes.Add(equipmentIndex);
                    
                    if (!RoR2.EquipmentCatalog.equipmentList.Contains(equipmentIndex)) {
                        eliteEquipment.Add(equipmentIndex);
                    } else if (RoR2.EquipmentCatalog.GetEquipmentDef(equipmentIndex).isLunar) {
                        lunarEquipment.Add(equipmentIndex);
                    }
                }
            }

            static public ItemIndex GetScrapIndex(ItemTier itemTier) {
                if (scrapItems.ContainsKey(itemTier)) {
                    return scrapItems[itemTier];
                }
                return ItemIndex.None;
            }
        }
    }
}
