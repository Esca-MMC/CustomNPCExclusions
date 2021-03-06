﻿using System;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace CustomNPCExclusions
{
    public partial class ModEntry
    {
        /// <summary>The set of characters that may separate each "entry" in an NPC's exclusion data. Any number or combination of them is allowed between each asset.</summary>
        private static readonly char[] delimiters = new[] { ' ', ',', '/', '\\' };

        private static int cacheTime = 0;
        private static int cacheDays = 0;
        /// <summary>The current cache of NPC exclusion data. <see cref="ExclusionData"/> should be referenced instead.</summary>
        private static Dictionary<string, string> exclusionData = null;
        /// <summary>The current set of NPC exclusion data. Returns cached data if in-game time has not changed since the most recent load.</summary>
        public static Dictionary<string, string> ExclusionData
        {
            get
            {
                if (exclusionData == null || cacheTime != Game1.timeOfDay || cacheDays != Game1.Date.TotalDays) //if the cached exclusions have not been updated during the current in-game time
                {
                    exclusionData = Instance.Helper.Content.Load<Dictionary<string, string>>(AssetName, ContentSource.GameContent); //load all NPC exclusion data
                    cacheTime = Game1.timeOfDay; //update cache time
                    cacheDays = Game1.Date.TotalDays; //update cache days
                    Instance.Monitor.VerboseLog($"Updated local cache of Data/CustomNPCExclusions.");
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
                    dataForThisNPC = ExclusionData[name]; //use this NPC's data
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
