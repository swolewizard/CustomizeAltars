**Information:**
This mod allows you to customize both vanilla and modded altars, enabling you to spawn any boss you desire and choose the sacrifice item and amount. Utilizing JSON configuration, this feature is particularly useful for players who have designed their own bosses through mods like RRR or Unity and wish to spawn them using established altars.

**What's in this mod:**
- Customization options for all vanilla altars, including Name, BossPrefab, BossItem, and BossItems.
- Ability to customize altars introduced by other mods into world generation.

**Configuration:**
You can configure the Name, BossPrefab, BossItem, and BossItems of all altars in the game, including those introduced by other mods.
- JSON configuration is available in a new file named "Huntard.CustomizeAltars.json" within your "BepInEx/config" folder.
- This JSON file can be edited in-game and will update when you save and load an altar to your in-game scene.

*Example Config for Bonemass's altar:*

	{
	"AltarPrefabName" : "Bonemass",
	"Name" : "$piece_offerbowl_bonemass",
	"BossPrefab" : "Bonemass",
	"SacrificeItem" : "WitheredBone",
	"SacrificeAmount" : 10,
	"ItemStandEnabled": false,
	"ItemStandName" : ""
	}

Deleting the .json configuration will prompt the generation of a new .json file upon game startup and entering a world, automatically incorporating all altars in the game, both vanilla and modded ones.

**Known Issues:**
- None

**Credits:**
- @thedefside
- @Belasias
- @wackymole

**Installation:**
1. Ensure you have installed BepInEx.
2. Drag the .dll file into your \Valheim\BepInEx\plugins folder.

**Support:**
If you like what I do and want to support me, 

[![Buy Me A Coffee](https://i.imgur.com/d5IpNXJ.png)](https://www.buymeacoffee.com/Huntard)

**Github:**
Feel free to submit bug reports or pull requests [here](https://github.com/swolewizard/CustomizeAltars).

## Showcase Video:

[![Watch the video](https://i.imgur.com/q8lw6QQ.png)](https://www.youtube.com/watch?v=32iO2J5tVg0)