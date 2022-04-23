# Information:

<p>Lets you customize vanilla and modded altars to spawn whatever boss you want and choose what sacrifice item & amount. Through json configuration.
This would be useful for players that designed their own bosses through RRR or in unity and want to spawn them through already established altars.</p>

Feel free to make a post with information about other altars mods bring in, that you'd like to be added to the default configuration.

# What's in this mod:

* The ability to customize all vanilla altars. E.g (Name & BossPrefab & Bossitem & BossItems)

* The ability to customize all altars brought into world generation through other mods.


**<h3>Mods that are included in the default configuration:</h3>**

* EVA﻿


# Configuration:

**<h3>You can configure the (Name & BossPrefab & Bossitem & BossItems) of all altars in the game, Including altars brought into world generation through other mods</h3>**



* Json configuration available in a new json file called "Huntard.CustomizeAltars.json" in your "BepInEx/config" folder.

* This json file can be edited in-game and will update when you save & load in an altar to your in-game scene.


Example Config for Bonemass's altar.

	{
		"AltarPrefabName" : "Bonemass",
		"Name": "Boiling death",
		"BossPrefab": "Bonemass",
		"SacrificeItem": "WitheredBone",
		"SacrificeAmount" : 10
	}

**<h3>You can input your own custom configuration in the json to configure altars not covered in the default json.</h3>**

Example Config for another mods altar.

	{
		"AltarPrefabName" : "PrefabID of altar",
		"Name": "Name",
		"BossPrefab": "Boss that gets summoned",
		"SacrificeItem": "Sacrifice Item",
		"SacrificeAmount" : 10
	}


# Known Issues:

* Can't configure the sacrifice item/amount for the Moder/Yagluth altars.

## Credit:

 @thedefside

 @Belasias

## Installation:

* Make sure you have installed bepinex.

* Drag the .dll into your \Valheim\BepInEx\plugins folder.

## Support:

If you like what I do and want to support me.

<a href="https://www.buymeacoffee.com/Huntard"><img src="https://img.buymeacoffee.com/button-api/?text=Buy me a coffee&emoji=&slug=Huntard&button_colour=FFDD00&font_colour=000000&font_family=Cookie&outline_colour=000000&coffee_colour=ffffff" /></a>

## Github:

Feel free to submit bug reports or pull requests here.

https://github.com/swolewizard/CustomizeAltars


## Showcase Video:

[![Watch the video](https://i.imgur.com/q8lw6QQ.png)](https://www.youtube.com/watch?v=32iO2J5tVg0)
