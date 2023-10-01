using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace CustomNPCExclusions
{
    /// <summary>A Harmony patch that excludes a list of NPCs from random town-related shop dialog.</summary>
    public static class HarmonyPatch_ShopDialog
    {
        public static void ApplyPatch(Harmony harmony)
        {
            ModEntry.Instance.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_ShopDialog)}\": prefixing SDV method \"Game1.UpdateShopPlayerItemInventory(string, HashSet<NPC>)\".", LogLevel.Trace);
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
                var excluded = DataHelper.GetNPCsWithExclusions("All", "TownEvent", "WinterStar"); //get all NPCs with the WinterStar exclusion (or applicable categories)
                var excludedNPCsWhoExist = new HashSet<string>(); //a record of NPCs excluded below

                foreach (string name in excluded) //for each excluded NPC name
                {
                    if (Game1.getCharacterFromName(name) is NPC npc) //if the excluded NPC currently exists
                    {
                        excludedNPCsWhoExist.Add(name); //add their name to the exclusion list
                        purchased_item_npcs.Add(npc); //add them to the set of NPCs who already received dialog this time
                    }
                }

                if (excludedNPCsWhoExist.Count > 0 && ModEntry.Instance.Monitor.IsVerbose) //if any NPCs were excluded
                {
                    string logMessage = String.Join(", ", excludedNPCsWhoExist);
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