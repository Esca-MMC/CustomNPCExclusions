﻿using HarmonyLib;
using StardewModdingAPI;
using System;
using System.Collections.Generic;

namespace CustomNPCExclusions
{
    public partial class ModEntry : Mod
    {
        /// <summary>A reference to this mod's current instance, allowing easier access to SMAPI utilites.</summary>
        internal static Mod Instance { get; set; } = null;

        /// <summary>The name/address of the asset used to store NPC exclusion settings.</summary>
        public static string AssetName { get; set; } = "Data/CustomNPCExclusions";

        /// <summary>Runs when SMAPI loads this mod.</summary>
        /// <param name="helper">This mod's API for most SMAPI features.</param>
        public override void Entry(IModHelper helper)
        {
            //initialize static helpers
            Instance = this;
            InitializeDataHelper(helper);

            //initialize Harmony and apply all patches
            Harmony harmony = new Harmony(this.ModManifest.UniqueID);
            HarmonyPatch_ItemDeliveryQuest.ApplyPatch(harmony);
            HarmonyPatch_SocializeQuest.ApplyPatch(harmony);
            HarmonyPatch_WinterStarGifts.ApplyPatch(harmony);
            HarmonyPatch_ShopDialog.ApplyPatch(harmony);
            HarmonyPatch_IslandVisit.ApplyPatch(harmony, helper);
            HarmonyPatch_PerfectionFriendship.ApplyPatch(harmony);
            HarmonyPatch_MovieInvitation.ApplyPatch(harmony);
            HarmonyPatch_Greetings.ApplyPatch(harmony);
            HarmonyPatch_BirthdayCalendar.ApplyPatch(harmony);
        }
    }
}
