using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Madia.FarmShop.Buildings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace Madia.FarmShop.Patches
{
    internal static class CarpenterMenu_SetNewActiveBlueprint
    {
        private static bool _applied;
        private static IMonitor _monitor;
        private static Texture2D _outdoorStandBlueprint;

        public static void Apply(Harmony harmony, IMonitor monitor)
        {
            if (_applied || monitor == null)
            {
                return;
            }

            _outdoorStandBlueprint = ModEntry.Instance.Helper.ModContent
                .Load<Texture2D>("assets/OutdoorStand.png");
            _monitor = monitor;
            monitor.Log(
                $"Patching CarpenterMenu::setNewActiveBlueprint to accord for new blueprints",
                LogLevel.Debug);

            harmony.Patch(
                original: AccessTools.Method(
                    typeof(CarpenterMenu),
                    nameof(CarpenterMenu.setNewActiveBlueprint)),
                prefix: new HarmonyMethod(
                    typeof(CarpenterMenu_SetNewActiveBlueprint),
                    nameof(NewSetNewActiveBlueprint)));

            _applied = true;
        }

        /// <summary>
        ///     This patch adds new Buildings to robin's construction menu,
        ///     in a somewhat hacky way without patching the constructor
        /// </summary>
        /// <param name="__instance"></param>
        /// <returns></returns>
        private static bool NewSetNewActiveBlueprint(CarpenterMenu __instance)
        {
            try
            {
                FieldInfo blueprintsField = typeof(CarpenterMenu)
                    .GetField("blueprints", BindingFlags.NonPublic | BindingFlags.Instance);

                List<BluePrint> blueprints = blueprintsField.GetValue(__instance) as List<BluePrint>;
                if (!blueprints.Any(x => x.name == BlueprintFactory.OutdoorStandName))
                {
                    blueprints.Add(BlueprintFactory.NewOutdoorStandBlueprint(null));
                }

                blueprintsField.SetValue(__instance, blueprints);

                int currentIndex = PatchUtils.GetPrivateValue<int>(__instance, "currentBlueprintIndex");
                if (!blueprints[currentIndex].name.Contains(BlueprintFactory.OutdoorStandName))
                {
                    return true;
                }

                var ingredients = new List<Item>();
                foreach (var kvp in blueprints[currentIndex].itemsRequired)
                {
                    ingredients.Add(new StardewValley.Object(kvp.Key, kvp.Value));
                }

                PatchUtils.SetPrivateValue(__instance, "price", blueprints[currentIndex].moneyRequired);
                PatchUtils.SetPrivateValue(__instance, "ingredients", ingredients);
                PatchUtils.SetPrivateValue(__instance, "buildingDescription", blueprints[currentIndex].description);
                PatchUtils.SetPrivateValue(__instance, "buildingName", blueprints[currentIndex].displayName);
                PatchUtils.SetPrivateValue(
                    __instance,
                    "currentBuilding",
                    new OutdoorStand(blueprints[currentIndex], Vector2.Zero));

                return false;
            }
            catch (Exception ex)
            {
                _monitor.Log(ex.Message, LogLevel.Error);
                return true;
            }
        }
    }
}
