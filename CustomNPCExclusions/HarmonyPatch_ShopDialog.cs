﻿using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace CustomNPCExclusions
{
    /// <summary>A Harmony patch that excludes a list of NPCs from random town-related shop dialog.</summary>
    public static class HarmonyPatch_ShopDialog
    {
        public static void ApplyPatch(Harmony harmony)
        {
            ModEntry.Instance.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_ShopDialog)}\": transpiling SDV method \"Game1.UpdateShopPlayerItemInventory(string, HashSet<NPC>)\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.UpdateShopPlayerItemInventory), new[] { typeof(string), typeof(HashSet<NPC>) }),
                prefix: new HarmonyMethod(typeof(HarmonyPatch_ShopDialog), nameof(Game1_UpdateShopPlayerItemInventory))
            );
        }

        /// <summary>Excludes a list of NPCs from randomly discussing items that players have sold to certain shops.</summary>
        /// <param name="purchased_item_npcs">A list of NPCs to exclude from receiving shop dialog (normally used to avoid an NPC receiving multiple sets of dialog simultaneously).</param>
        private static void Game1_UpdateShopPlayerItemInventory(ref HashSet<NPC> purchased_item_npcs)
        {
            try
            {
                List<string> excluded = new List<string>(); //a list of NPC names to exclude from giving or receiving gifts

                foreach (KeyValuePair<string, List<string>> data in ModEntry.GetAllNPCExclusions()) //for each NPC's set of exclusion data
                {
                    if (data.Value.Exists(entry =>
                        entry.StartsWith("All", StringComparison.OrdinalIgnoreCase) //if this NPC is excluded from everything
                     || entry.StartsWith("TownEvent", StringComparison.OrdinalIgnoreCase) //OR if this NPC is excluded from town events
                     || entry.StartsWith("ShopDialog", StringComparison.OrdinalIgnoreCase) //OR this NPC is excluded from dialog about the town shop
                    ))
                    {
                        if (Game1.getCharacterFromName(data.Key) is NPC npc) //if the excluded NPC exists
                        {
                            excluded.Add(data.Key); //add their name to the exclusion list
                            purchased_item_npcs.Add(npc); //add them to the set of NPCs who already received dialog this time
                        }
                    }
                }

                if (excluded.Count > 0 && ModEntry.Instance.Monitor.IsVerbose) //if any NPCs were excluded
                {
                    string logMessage = String.Join(", ", excluded);
                    ModEntry.Instance.Monitor.Log($"Excluded NPCs from random shop dialog: {logMessage}", LogLevel.Trace);
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.LogOnce($"Harmony patch \"{nameof(Game1_UpdateShopPlayerItemInventory)}\" has encountered an error. The \"ShopDialog\" setting may not work correctly. Full error message: \n{ex.ToString()}", LogLevel.Error);
                return; //run the original method
            }
        }
    }
}