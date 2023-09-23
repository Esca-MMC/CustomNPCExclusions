﻿using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomNPCExclusions
{
    public partial class ModEntry
    {
        /// <summary>The set of characters that may separate each "entry" in an NPC's exclusion data. Any number or combination of them is allowed between each asset.</summary>
        private static readonly char[] delimiters = new[] { ' ', ',', '/', '\\' };

        /// <summary>Initializes methods used to retrieve exclusion data.</summary>
        public static void InitializeDataHelper(IModHelper helper)
        {
            helper.Events.Content.AssetRequested += Content_AssetRequested;
            helper.Events.Player.Warped += Player_Warped_InvalidateCache;
        }

        /// <summary>Loads the initial version of this mod's data asset.</summary>
        /// <remarks>This is a replacement for the IAssetLoader system, which was deprecated in SMAPI v3.14.</remarks>
        private static void Content_AssetRequested(object sender, StardewModdingAPI.Events.AssetRequestedEventArgs e)
        {
            if (e.Name.IsEquivalentTo(AssetName, false)) //if this is the exclusion data asset
            {
                e.LoadFrom(() => new Dictionary<string, string>(), StardewModdingAPI.Events.AssetLoadPriority.Medium, null); //load an empty asset
            }
        }

        private static void Player_Warped_InvalidateCache(object sender, StardewModdingAPI.Events.WarpedEventArgs e)
        {
            exclusionData = null; //invalidate cache (e.g. to account for Content Patcher's OnLocationChange)
        }

        private static int cacheTime = 0;
        private static int cacheDays = 0;
        /// <summary>The current cache of NPC exclusion data. <see cref="ExclusionData"/> should be referenced instead.</summary>
        private static Dictionary<string, string> exclusionData = null;
        /// <summary>The current set of NPC exclusion data. Returns cached data if in-game time has not changed since the most recent load.</summary>
        public static Dictionary<string, string> ExclusionData
        {
            get
            {
                if (exclusionData == null || !Context.IsWorldReady || cacheTime != Game1.timeOfDay || cacheDays != Game1.Date.TotalDays) //if no cache exists OR it hasn't been updated for the current in-game day and time
                {
                    exclusionData = Instance.Helper.GameContent.Load<Dictionary<string, string>>(AssetName); //load all NPC exclusion data
                    
                    if (Context.IsWorldReady)
                    {
                        //update cache timestamp
                        cacheTime = Game1.timeOfDay;
                        cacheDays = Game1.Date.TotalDays; //note: Game1.Date causes a null error when checked in certain circumstances (e.g. while the game is still launching), so the context check is required
                    }
                    else
                    {
                        //reset cache timestamp
                        cacheTime = 0;
                        cacheDays = 0;
                    }

                    if (ModEntry.Instance.Monitor.IsVerbose)
                        Instance.Monitor.Log($"Updated local cache of Data/CustomNPCExclusions.", LogLevel.Trace);
                }

                return exclusionData;
            }
        }

        /// <summary>Loads and parses the current exclusion data for a specific NPC name.</summary>
        /// <param name="name">The name of the NPC.</param>
        /// <returns>A list of each entry in this NPC's exclusion data.</returns>
        public static List<string> GetNPCExclusions(string name, bool forceCacheUpdate = false)
        {
            if (forceCacheUpdate)
                exclusionData = null; //clear cached exclusion data

            string dataForThisNPC = null;
            foreach (string key in ExclusionData.Keys) //for each NPC name in the exclusion data
            {
                if (key.Equals(name, StringComparison.OrdinalIgnoreCase)) //if this name matches the provided name
                {
                    dataForThisNPC = ExclusionData[key]; //use this NPC's data
                    break;
                }
            }

            if (dataForThisNPC == null) //if no data was found for this NPC
                return new List<string>(); //return an empty list

            return dataForThisNPC.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).ToList(); //split this NPC's data into a list of entries and return it
        }

        /// <summary>Loads and parses the current exclusion data for all NPC names.</summary>
        /// <param name="forceCacheUpdate">If true, exclusion data will be cleared and reloaded before use.</param>
        /// <returns>A case-insensitive dictionary of NPC names (keys) and lists of exclusion data entries (values).</returns>
        public static Dictionary<string, List<string>> GetAllNPCExclusions(bool forceCacheUpdate = false)
        {
            if (forceCacheUpdate)
                exclusionData = null; //clear cached exclusion data

            Dictionary<string, List<string>> parsedExclusions = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase); //create a case-insensitive exclusion dictionary

            foreach (KeyValuePair<string, string> entry in ExclusionData) //for each entry in the exclusion data
            {
                List<string> parsedValue = entry.Value.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).ToList(); //get a parsed list of this NPC's data entries
                parsedExclusions.Add(entry.Key, parsedValue); //add this NPC's name (key) and parsed value to the parsed dictionary
            }

            return parsedExclusions; //return the parsed dictionary
        }
    }
}
