﻿using Harmony;
using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Collections.Generic;

namespace CustomNPCExclusions
{
    public partial class ModEntry : Mod, IAssetLoader
    {
        /// <summary>A reference to this mod's current instance, allowing easier access to SMAPI utilites.</summary>
        internal static Mod Instance { get; set; } = null;

        /// <summary>The name/address of the asset used to store NPC exclusion settings.</summary>
        public static string AssetName { get; set; } = "Data/CustomNPCExclusions";

        /// <summary>Runs when SMAPI loads this mod.</summary>
        /// <param name="helper">This mod's API for most SMAPI features.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this; //set the reference to this mod's current instance

            //apply all Harmony patches
            HarmonyInstance harmony = HarmonyInstance.Create(this.ModManifest.UniqueID); //create a Harmony instance for this mod
            HarmonyPatch_ItemDeliveryQuest.ApplyPatch(harmony);
            HarmonyPatch_SocializeQuest.ApplyPatch(harmony);
            HarmonyPatch_WinterStarGifts.ApplyPatch(harmony);
            HarmonyPatch_ShopDialog.ApplyPatch(harmony);
        }

        /// <summary>Get whether this mod can load the initial version of the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanLoad<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals(AssetName)) //if this asset's name matches
            {
                return true; //this mod can load this asset
            }

            return false; //this mod CANNOT load this asset
        }

        /// <summary>Load a matched asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public T Load<T>(IAssetInfo asset)
        {

            if (asset.AssetNameEquals(AssetName)) //if this asset's name matches
            {
                return (T)(object)new Dictionary<string, string>(); //return an empty string dictionary (i.e. create a new data file)
            }

            throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
        }
    }
}
