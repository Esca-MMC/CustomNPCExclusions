using StardewModdingAPI;
using StardewValley.GameData.Characters;
using System;
using System.Collections.Generic;

namespace CustomNPCExclusions
{
    /// <summary>A content editor that excludes designated NPCs from the perfection system's friendship percentage.</summary>
    /// <remarks>
    /// As of Stardew v1.6, this exclusion rule is provided by the base game, and can be used without this mod.
    /// The exclusion rule is still available here for compatibility purposes, but now uses the base game's implementation.
    /// </remarks>
    public static class PerfectFriend
    {
        /// <summary>Initializes and enables this feature.</summary>
        public static void Enable()
        {
            ModEntry.Instance.Helper.Events.Content.AssetRequested += Content_AssetRequested;
        }

        /// <summary>Implements the "PerfectFriend" exclusion by editing the relevant fields in Data/Characters.</summary>
        private static void Content_AssetRequested(object sender, StardewModdingAPI.Events.AssetRequestedEventArgs e)
        {
            try
            {
                if (e.NameWithoutLocale.IsEquivalentTo("Data/Characters"))
                {
                    e.Edit(asset =>
                    {
                        var exclusionData = ModEntry.GetAllNPCExclusions();
                        var characterData = asset.AsDictionary<string, CharacterData>().Data;

                        List<string> excluded = new List<string>(); //a list of NPCs excluded here

                        foreach (var data in exclusionData) //for each NPC with any exclusion rules
                        {
                            if (data.Value.Exists(entry =>
                                entry.StartsWith("All", StringComparison.OrdinalIgnoreCase) //if this NPC is excluded from everything
                                || entry.StartsWith("OtherEvent", StringComparison.OrdinalIgnoreCase) //OR if this NPC is excluded from other events
                                || entry.StartsWith("PerfectFriend", StringComparison.OrdinalIgnoreCase) //OR if this NPC is excluded from the perfection system's friendship percentage
                            ))
                            {
                                if (characterData.ContainsKey(data.Key)) //if the NPC exists in Data/Characters
                                {
                                    characterData[data.Key].ExcludeFromPerfectionScore = true; //exclude them from the perfection check
                                    excluded.Add(data.Key); //add their name to the excluded list
                                }
                            }
                        }

                        if (excluded.Count > 0 && ModEntry.Instance.Monitor.IsVerbose) //if any NPCs were excluded
                        {
                            string logMessage = string.Join(", ", excluded);
                            ModEntry.Instance.Monitor.Log($"Edited \"Data/Characters\" to exclude NPCs from perfect friendship checks: {logMessage}", LogLevel.Trace);
                        }

                    }, StardewModdingAPI.Events.AssetEditPriority.Late); //apply after other mods' edits if possible
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.LogOnce($"Exclusion rule \"PerfectFriend\" encountered an error. Custom NPCs might not be properly excluded from perfect friendship checks. Full error message:\n{ex.ToString()}", LogLevel.Error);
            }
        }
    }
}