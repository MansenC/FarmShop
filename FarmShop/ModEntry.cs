using System;
using System.Collections.Generic;
using HarmonyLib;
using Madia.FarmShop.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;

/*
 * 
 * ToDos:
 * - register the OutdoorStand in Robin's shop. This is annoying.
 * - register the OutdoorStand as a buildable building in BuildableGameLocation.
 *   This means patching the function since it isn't dynamically implemented... Bruh
 *   
 * Adding the blueprint to robin: Class is StardewValley.Menus.CarpenterMenu
 * Why is CarpenterMenu::setNewActiveBlueprint there?
 * 
 */

namespace Madia.FarmShop
{
    public class ModEntry : Mod
    {
        public static ModEntry Instance { get; private set; }

        public override void Entry(IModHelper helper)
        {
            Instance = this;

            var harmony = new Harmony(ModManifest.UniqueID);
            CarpenterMenu_SetNewActiveBlueprint.Apply(harmony, Monitor);
            BuildableGameLocation_BuildStructure.Apply(harmony, Monitor);

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs args)
        {
            // TODO add to robins menu
        }
    }
}
