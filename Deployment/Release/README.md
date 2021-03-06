# Phedg1 Studios - Item Drop List

You can contact me in the modding discord as *@Phedg1*
If you are a fan of my work and would like to support me, I now have a [DONATION LINK](http://paypal.me/Phedg1Studios).
The [SOURCE CODE](https://github.com/Phedg1Studios/ItemDropList) for this project is now available.

My mods would not be possible without the amazing core developers who made and maintian *BepInEx* and *R2API*. The tools they have created, as well as their forthcoming support in the modding discord, made all the difference in the world. If you have chosen to donate to me, I would ask that you also consider donating to them. None of this would be possible without them.

### TLDR ###
This mod will allow players to choose which items can be found in chests and from boss drops.

## DESCRIPTION ##
The main menu will have a new button labelled *"Item Drops"*. Clicking it will open up the drop management interface. The interface displayed and how it functions will depend on which mode the mod is running in. 

Three buttons, labelled "Profile: 1", "Profile: 2" and "Profile: 3", are displayed in the top right of the interface. Clicking these will allow the player to change the current item loadout. Enabling an item in one profile does not affect the others, this is so a player could configure each profile differently to easily switch between them between games. Each of the 3 profiles is unique to each mod mode.

This mod does not change item rarity spawn chance, other than where certain tiers are completely disabled. Making an item available or unavailable does not change whether it has been unlocked or discovered on the player's profile. This mod can be enabled and disabled from this menu, should the player wish to play normally again. All drones are also listed in this menu and can enabled or disabled as well.

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
Ebkr, Harb, iDeathHD, KingEnderBrine

** Support, Suggestions, Beta Testing and Bug Reports **
blazingdrummer
aschente, Borst, breadguy69, Cookiefox, Mc Fow1er, Pears, Undead, VoidInsanity

## Changelog ##
** v1.2.6 **

* ACTUALLY added missing dependancy for ItemDropAPIFixes.

** v1.2.5 **

* Added missing dependancy for ItemDropAPIFixes.

** v1.2.4 **

* Added config for whether items have to be discovered to show up in the menu.
* Added *BiggerBazaar* compatibility.
* Refactored every hook into its own function.
* Refactored spawn and tier limiting code into a standalone mod so other users and devs can take advantage of it.

** v1.2.3 **

* Fixed Scrapper and Artifact of Command interface not using the item what was clicked.

** v1.2.2 **

* Fixed spawning only Shrines of Chance.
* Fixed Scrapper and Artifact of Command interface not being adjusted in the normal mode.

** v1.2.1 **

* Added flavor text.
* Fixed Shrines of Chance giving disabled items.

** v1.2.0 **

* Updated mod icon.
* Added profiles, so multiple loadouts can easily be configured and activated.
* Added ability to toggle entire tiers of items and all items.
* Added config to include Defensive Microbots in the item list.
* Added config to limit monsters to the same item list (Void Fields, Artifact of Evolution, Scavengers).
* Fixed errors and improved functionality in the Bazaar.
* Fixed aptitude score when using Cleansing Pools.
* Migrated configs to mirror other mods.
* Added credits to readme.
* Updated readme formatting.
* Published source code.
* Increased required version of R2API.

** v1.1.15 **

* Now compatible with the 1.0.1 update.
* Updated readme formatting.

** v1.1.14 **

* Dud release

** v1.1.13 **

* Fixed microbots dropping from chests.
* Removed microbots from gui.

** v1.1.12 **

* Fixed incorrect number of boss drops appearing when using ShareSuite.

** v1.1.11 **

* Fixed artifact of command interface when opening equipment.

** v1.1.10 **

* Fixed artifact of command interface.
* Fixed back button position.

** v1.1.9 **

* Fixed scrap dropping from chests.
* Fixed chests spawning when only scrap is selected.
* Fixed missing R2API submodule dependencies.

** v1.1.8 **

* Fixed purchases not reducing available currency.
* Renamed menu button.
* Improved controller support.
* Improved profile specific config.

** v1.1.7 **

* Now compatible with the 1.0 update.
* Added controller support.
* Renamed scrap to aptitude score.
* Updated training mode menu names and instruction text.
* Added new flavor text panel detailing the training menu's functions.
* Added new welcome screen displaying aptitude score earned since last entering the menu.
* Streamlined the config which is not meant to be edited.
* Updated readme formatting.
* Increased required version of BepInEx and R2API.

** v1.1.6 **

* Fixed mod disabled text not being greyed out when reentering the menu.
* Fixed corrupted font glitch.

** v1.1.5 **

* Increased the default prices for common and uncommon items.
* Added new setting to config to control the maximum amount of scrap that can be held at once.

** v1.1.4 **

* Created an event that other mods could use to use this mods base functionality to set drop tables and correct chest spawns. Use * "Phedg1Studios.ItemDropList.ItemDropList.itemDropList.setDropList" and update "Phedg1Studios.ItemDropList.Data.itemsToDrop". This is a list of all my internal ids that will be dropped. Use "Phedg1Studios.ItemDropList.Data.allItemsIndexes" and "Phedg1Studios.ItemDropList.Data.allEquipmentIndexes" to get an id.
* Internal broken items should no be listed in the Item Drop List menu any longer.
* Damage, Healing and Utility chests should no longer be able to give out nothing when opened.
* Added the new Shop mode.
* Added the scrap collection system.
* Changed how equipment and drone id's are stored to try and mirror the source code in an effort to somewhat future proof.
* Updated manifest to reflect the required version of BepInEx that was changed in 1.1.3

** v1.1.3 **

* Increased required version of R2API so that custom items are now supported.
* R2API ItemDropAPI is now unhooked when this mod is enabled. Without this the mod would not function when any other mod was present that initialized this api (such as ShareSuite).
* Added version parameter to the config so changes to config files can be applied to existing config files.
* Added support so ShareSuite RandomizeSharePickups should now use the drop list from this mod.
* Modified config descriptions so they show up cleaner in the mod manager.

** v1.1.2 **

* Changed configuration files and extensions to better support r2modman.

** v1.1.1 **

* Added ability to toggle whether spawning for each drone type.

** v1.1.0 **

* Now compatible with the Artifacts update.
* Updated the visual style to match the new UI.
* Fixed UI scaling at different resolutions and aspect ratios.
* Increased the scrolling speed of the starting items menu.
* Fixed a bug where unfinished items would show when show all items was enabled.
* Decoupled the mod UI from the base game to a much greater extent.
* Boss item drops, adaptive chests, scavenger backpacks, cleansing pools and many other interactables now working correctly.
* Fixed chests given no items.
* Added functionality to prevent chests from spawning if they have no items to give.
* Added ability to scale the amount of interactables spawned per stage.

** v1.0.0 **

* Initial Release