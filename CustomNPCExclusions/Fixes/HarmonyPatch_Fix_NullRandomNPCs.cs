using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace CustomNPCExclusions.Fixes
{
    /// <summary>A Harmony patch that attempts to prevent null results when getting random town NPCs.</summary>
    /// <remarks>
    /// Some custom NPCs and mods handle NPC data and instances in ways that cause null errors when calling "getRandomTownNPC".
    /// While the original method avoids returning null when possible, Harmony patches and NPC subclasses can subvert that behavior.
    /// NOTE: This method should be updated to target "Utility.getRandomNpcFromHomeRegion(string, Random)" in Stardew v1.6. Cross-compatibility is possible but less efficient.
    /// </remarks>
    public static class HarmonyPatch_Fix_NullRandomNPCs
    {
        public static void ApplyPatch(Harmony harmony)
        {
            //Note: In Stardew v1.5.6, the () overload calls the (Random) overload. In v1.6, they both call a new method instead.

            ModEntry.Instance.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_Fix_NullRandomNPCs)}\": postfixing SDV method \"Utility.getRandomTownNPC(Random)\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.getRandomTownNPC), new[] { typeof(Random) }),
                postfix: new HarmonyMethod(typeof(HarmonyPatch_Fix_NullRandomNPCs), nameof(Utility_getRandomTownNPC_Postfix))
            );
        }

        /// <summary>True while this patch is attempting to fix a null result. Intended as a basic lock to prevent unnecessary recursion.</summary>
        private static bool currentlyFixing = false;

        /// <summary>Forces the method to re-call itself repeatedly until a non-null result is found.</summary>
        [HarmonyPriority(Priority.Low)] //use low priority to run after most other postfixes
        private static void Utility_getRandomTownNPC_Postfix(Random r, ref NPC __result)
        {
            try
            {
                if (!currentlyFixing && __result == null) //if this the "first" call to this postfix AND the return NPC is null
                {
                    currentlyFixing = true; //disable any further calls to this postfix until this call is complete

                    ModEntry.Instance.Monitor.Log($"The method \"Utility.getRandomTownNPC\" returned null (nothing) instead of an NPC. This is sometimes caused by custom NPC mods with unusual behavior. Retrying until a valid NPC is found...", LogLevel.Trace);

                    NPC newResult = Utility.getRandomTownNPC(r); //try again with the same Random
                    int count = 1; //the number of retry attempts
                    while (newResult == null) //while the result is still null, keep trying
                    {
                        newResult = Utility.getRandomTownNPC(r);
                        count++;
                    }

                    __result = newResult; //override the return value

                    ModEntry.Instance.Monitor.Log($"Process complete. The method was called again {count} time(s) to find a valid NPC and prevent errors.", LogLevel.Trace);

                    currentlyFixing = false; //task complete, so re-enable other calls to this postfix
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_Fix_NullRandomNPCs)}\" has encountered an error. Full error message: \n{ex.ToString()}", LogLevel.Error);
                return; //run the original method
            }
        }
    }
}