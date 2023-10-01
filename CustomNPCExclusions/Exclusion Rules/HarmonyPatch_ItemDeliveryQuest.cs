using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Quests;
using System;
using System.Collections.Generic;

namespace CustomNPCExclusions
{
    /// <summary>A Harmony patch that excludes a list of NPCs from <see cref="ItemDeliveryQuest"/>.</summary>
    public static class HarmonyPatch_ItemDeliveryQuest
    {
        public static void ApplyPatch(Harmony harmony)
        {
            ModEntry.Instance.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_ItemDeliveryQuest)}\": postfixing SDV method \"ItemDeliveryQuest.GetValidTargetList()\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(ItemDeliveryQuest), nameof(ItemDeliveryQuest.GetValidTargetList)),
                postfix: new HarmonyMethod(typeof(HarmonyPatch_ItemDeliveryQuest), nameof(ItemDeliveryQuest_GetValidTargetList))
            );
        }

        /// <summary>A Harmony postfix patch that excludes a list of NPCs from <see cref="ItemDeliveryQuest"/>.</summary>
        /// <param name="__result">A list of NPCs this quest type could target.</param>
        public static void ItemDeliveryQuest_GetValidTargetList(ref List<NPC> __result)
        {
            try
            {
                var excluded = DataHelper.GetNPCsWithExclusions("All", "TownQuest", "ItemDelivery"); //get all NPCs with the Socialize exclusion (or applicable categories)
                var excludedNPCsWhoExist = new HashSet<string>(); //a record of NPCs excluded below

                for (int x = __result.Count - 1; x >= 0; x--) //for each valid NPC returned by the original method (looping backward to allow removal)
                {
                    if (excluded.Contains(__result[x].Name)) //if this NPC has the relevant exclusion rule
                    {
                        excludedNPCsWhoExist.Add(__result[x].Name); //add this NPC to the record
                        __result.RemoveAt(x); //remove this NPC from the original results
                    }
                }

                if (excludedNPCsWhoExist.Count > 0 && ModEntry.Instance.Monitor.IsVerbose) //if any NPCs were excluded
                {
                    ModEntry.Instance.Monitor.Log($"Excluded NPCs from item delivery quest: {String.Join(", ", excludedNPCsWhoExist)}", LogLevel.Trace);
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.LogOnce($"Harmony patch \"{nameof(ItemDeliveryQuest_GetValidTargetList)}\" has encountered an error and may revert to default behavior. Full error message:\n{ex.ToString()}", LogLevel.Error);
            }
        }


    }
}