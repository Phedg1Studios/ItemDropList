# Phedg1 Studios - Item Drop List

### License ###
This repository is licensed under the include license.

### Distribution ###
A compiled version of this mod can  be located on the [Thunderstore](https://thunderstore.io/package/Phedg1Studios/ItemDropList/).

### TLDR ###
This mod will allow players to choose which items can be found in chests and from boss drops.

## DESCRIPTION ##
The main menu will have a new button labelled *"Item Drops"*. Clicking it will open up the drop management interface. The interface displayed and how it functions will depend on which mode the mod is running in. 

It is possible to play games with every single item disabled. Chests that are spawned will always give out an item. If every item a particular chest is able to spawn has been disabled that chest will no longer be spawned. This also applies to unique interactables like Shrines of Order (if you have every item disabled), Scavenger Backpacks, Cleansing Pools and 3D Printers to name a few. All drones are also listed in this menu and can enabled or disabled as well.

This mod does not change item rarity spawn chance, other than where certain tiers are completely disabled. Making an item available or unavailable does not change whether it has been unlocked or discovered on the player's profile. This mod can be enabled and disabled from this menu, should the player wish to play normally again.

### MODES ###

** STANDARD MODE **
Every item and piece of equipment that has been unlocked (completed the requisite achievement) and discovered (been picked up at least once) will be listed here. Every item or piece of equipment that will be available when playing the game will be shown in full colour. Every item or piece of equipment that will not be available will be shown darkened. Left click and item or piece of equipment to toggle its availability, hold left click to toggle an entire tier of item or every item.

** TRAINING MODE **
This mode has two menus that can be switched by hitting the buttons in the upper left hand corner. The first menu is the *"Courses"*. Every item that has been unlocked (completed the requisite achievement) will be listed here, alonside its price. Items and pieces of equipment that can be afforded will be shown in full colour, items that are unaffordable will be shown darkened. Double click an item to purchase it, the item will now have a tick over it and will not be shown on this menu again in the future. The second menu is labled *"Purchased"*, it will list every item that has been purchased.

Items and equipment are purchased for *"Aptitude Score"*. *Aptitude score* will increase every time an item is picked up while this mod is active. The *aptitude score* will be of the same rarity as the item that was picked up. Your *aptitude score* totals are shown at the top of the both menus in this mode.

Training mode was inspired by features in other roguelite games such as *Undermine*, *Dungreed* and *Dead Cells*. The goal was to give a returning player a sense of progression, to give them a boost to make them more formidable the more they play, as well as give greater variation to play.

## CONFIGURATION ##
After a player has opened the item shop interface for the first time a number of configuration files will be created. These can be found at *"<RISK OF RAIN 2 INSTALL LOCATION>/Risk of Rain 2/BepInEx/config/"* and *"<RISK OF RAIN 2 INSTALL LOCATION>/Risk of Rain 2/BepInEx/config/Item Drop List/"*. If you desire to alter the configuration of the mod it must be done by editing these files. The intention is that this would only be necessary once, with the rest of the player's interactions being done through the shop interface inside the game itself.

** com.Phedg1Studios.ItemDropList.cfg **
This file contains all of the settings which persist between profiles. It stores the whether the mod is currently enabled or disabled, whether all items (including items which have not been unlocked) should be listed, what mode the mod should use and the multiplier for how many interactables should be spawned per stage. It also contains whether items that have been purchased may be disabled afterwards at the players leisure, how much *aptitude score* can be held at one time and how much each tier of item costs to buy.

** <PROFILE ID>.txt **
This is the record of what items and equipment the player has chosen to make available in the different modes and profiles, as well as any currency that has been earned and a few internal varialbes to improve the user experience, so that they can persist after the game has been closed. 

## CREDITS ##
** Icon Design **
Ben C.

** Education **
Ebkr, Harb, iDeathHD

** Support, Suggestions, Beta Testing and Bug Reports **
blazingdrummer
Borst, breadguy69, Cookiefox, FunkFrog, itλy, Mc Fow1er, Pears, Siponodo, Undead, VoidInsanity