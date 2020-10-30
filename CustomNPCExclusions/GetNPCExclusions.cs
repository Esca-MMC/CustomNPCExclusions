using System;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace CustomNPCExclusions
{
    public partial class ModEntry
    {
        /// <summary>Loads and parses the current exclusion data for a specific NPC name.</summary>
        /// <param name="name">The name of the NPC.</param>
        /// <returns>A list of each entry in this NPC's exclusion data.</returns>
        public static List<string> GetNPCExclusions(string name)
        {
            char[] delimiters = new[] { ' ', ',', '/', '\\' }; //allowed characters between each "entry" in an NPC's exclusion data
            Dictionary<string, string> allExclusions = Instance.Helper.Content.Load<Dictionary<string, string>>(AssetName, ContentSource.GameContent); //load all NPC exclusion data

            string dataForThisNPC = null;
            foreach (string key in allExclusions.Keys) //for each NPC name in the exclusion data
            {
                if (key.Equals(name, StringComparison.OrdinalIgnoreCase)) //if this name matches the provided name
                {
                    dataForThisNPC = allExclusions[name]; //use this NPC's data
                    break;
                }
            }

            if (dataForThisNPC == null) //if no data was found for this NPC
                return new List<string>(); //return an empty list

            return dataForThisNPC.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).ToList(); //split this NPC's data into a list of entries and return it
        }

        /// <summary>Loads and parses the current exclusion data for all NPC names.</summary>
        /// <returns>A case-insensitive dictionary of NPC names (keys) and lists of exclusion data entries (values).</returns>
        public static Dictionary<string, List<string>> GetAllNPCExclusions()
        {
            char[] delimiters = new[] { ' ', ',', '/', '\\' }; //allowed characters between each "entry" in an NPC's exclusion data
            Dictionary<string, string> allExclusions = Instance.Helper.Content.Load<Dictionary<string, string>>(AssetName, ContentSource.GameContent); //load all NPC exclusion data

            Dictionary<string, List<string>> parsedExclusions = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase); //create a case-insensitive exclusion dictionary

            foreach (KeyValuePair<string, string> entry in allExclusions) //for each entry in the exclusion data
            {
                List<string> parsedValue = entry.Value.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).ToList(); //get a parsed list of this NPC's data entries
                parsedExclusions.Add(entry.Key, parsedValue); //add this NPC's name (key) and parsed value to the parsed dictionary
            }

            return parsedExclusions; //return the parsed dictionary
        }
    }
}
