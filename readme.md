# Custom NPC Exclusions
 A mod for the game Stardew Valley that allows mods to exclude specific NPCs from certain quests and events, particularly those that randomly select NPCs.

## Contents
* [Installation](#installation)
* [Exclusion Rules](#adding-exclusion-rules)
* [Mod Examples](#mod-examples)
     * [SMAPI Mods](#smapi-mods)
     * [Content Patcher Mods](#content-patcher-mods)

## Installation
1. **Install the latest version of [SMAPI](https://smapi.io/).**
2. **Download Custom NPC Exclusions** from [the Releases page on GitHub](https://github.com/Esca-MMC/CustomNPCExclusions/releases), Nexus Mods, or ModDrop.
3. **Unzip Custom NPC Exclusions** into the `Stardew Valley\Mods` folder.

Mods that use Custom NPC Exclusions should now work correctly. For information about creating mods, see the sections below.

Multiplayer note:
* It is recommended that **all players** install this mod for multiplayer sessions. While there are no known issues directly related to this mod, NPC mods can cause unexpected errors if mismatched.

## Exclusion Rules
This mod can apply different exclusion rules to each individual NPC, preventing them being involved in the related quests and events.

The available exclusion rules are:

Rule | Category | Description
-----|----------|------------
All | | Excludes the NPC from all content affected by this mod.
TownEvent | | Excludes the NPC from all content in the "TownEvent" category (see below).
TownQuest | | Excludes the NPC from all content in the "TownQuest" category (see below).
ShopDialog | TownEvent | Excludes the NPC from randomly discussing items that players have sold to certain shops.
WinterStar | TownEvent | Excludes the NPC from giving or receiving secret gifts at the Feast of the Winter Star festival.
ItemDelivery | TownQuest | Excludes the NPC from providing item delivery quests, e.g. from the Help Wanted board.
Socialize | TownQuest | Excludes the NPC from "socialize" quests, which currently includes the "Introductions" quest at the start of the game.

## Mod Examples
This mod loads a new data asset into Stardew: `Data/CustomNPCExclusions`.

The asset is similar to other "Data" types and is a `Dictionary<string, string>` internally. Each entry should have a NPC's name as the *key*, and a set of exclusion rules for that NPC as the *value*. Exclusion rules can be separated by spaces, commas, or forward slashes.

The asset can be edited in multiple ways; see the sections below for example of specific methods.

### SMAPI Mods
SMAPI mods can edit NPC exclusion data by using the `IAssetEditor` interface. See this wiki page for an overview: [https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Content#Edit_a_game_asset](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Content#Edit_a_game_asset)

Below is a more specific example, which adds the "WinterStar" and "ItemDelivery" rules to a custom NPC named "MyCustomNpcName":

```
public bool CanEdit<T>(IAssetInfo asset)
{
	if (asset.AssetNameEquals("Data/CustomNPCExclusions"))
		return true;
		
	return false;
}

public void Edit<T>(IAssetData asset)
{
	if (asset.AssetNameEquals("Data/CustomNPCExclusions"))
	{
		IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
		data.Add("MyCustomNpcName", "WinterStar ItemDelivery"); 
	}
}
```

### Content Patcher Mods
Content Patcher's content packs can edit NPC exclusion data by using `"Action": "EditData"` with `"Target": "Data/CustomNPCExclusions"`. See the Content Patcher author guide for an overview: [https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide.md#editdata](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide.md#editdata)

Below is a more specific example, which adds the "WinterStar" and "ItemDelivery" rules to a custom NPC named "MyCustomNpcName":

```
{
   "Format": "1.18.0",
   "Changes": [
      {
         "Action": "EditData",
         "Target": "Data/CustomNPCExclusions",
         "Entries": {
            "MyCustomNpcName": "WinterStar ItemDelivery"
         }
      }
   ]
}
```