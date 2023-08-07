using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace CustomNPCExclusions.Fixes
{
    /// <summary>A Harmony patch that attempts to fix a possible null error in SDV 1.5.6 and earlier.</summary>
    /// <<remarks>Fix designed by atravita. This class should be removed in Stardew v1.6 and later versions, due to internal fixes.</remarks>
    public static class HarmonyPatch_ShopDialog_ErrorFix
    {
        public static void ApplyPatch(Harmony harmony)
        {
            ModEntry.Instance.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_ShopDialog_ErrorFix)}\": prefixing SDV method \"Game1.UpdateShopPlayerItemInventory(string, HashSet<NPC>)\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.UpdateShopPlayerItemInventory), new[] { typeof(string), typeof(HashSet<NPC>) }),
                prefix: new HarmonyMethod(typeof(HarmonyPatch_ShopDialog_ErrorFix), nameof(Game1_UpdateShopPlayerItemInventory))
            );
        }

        /// <summary>Attempts to fix a possible null error in SDV 1.5.6 and earlier.</summary>
        /// <param name="location_name">The name of the shop location being checked.</param>
        private static void Game1_UpdateShopPlayerItemInventory(string location_name)
        {
            try
            {
                if (Game1.getLocationFromName(location_name) is not ShopLocation shop || shop.itemsFromPlayerToSell.Count == 0) //if the named shop doesn't exist OR the shop isn't selling any of the players' items
                    return; //do nothing

                int badItems = 0;
                for (int x = shop.itemsFromPlayerToSell.Count - 1; x >= 0; x--)
                {
                    Item item = shop.itemsFromPlayerToSell[x];

                    if (item is null || item.Stack <= 0) //if the item is null or would otherwise call null reference errors
                    {
                        shop.itemsFromPlayerToSell.RemoveAt(x); //remove it from the list
                        badItems++;
                    }
                }

                if (badItems > 0)
                    ModEntry.Instance.Monitor.Log($"Removed {badItems} null/hazardous items from {location_name}.itemsFromPlayerToSell", LogLevel.Trace);
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_ShopDialog_ErrorFix)}\" has encountered an error. Full error message: \n{ex.ToString()}", LogLevel.Error);
                return; //run the original method
            }
        }
    }
}