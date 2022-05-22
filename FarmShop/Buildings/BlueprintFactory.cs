using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Madia.FarmShop.Buildings
{
    internal static class BlueprintFactory
    {
        public const string OutdoorStandName = "OutdoorStand";

        /// <summary>
        ///     The base blueprint name
        /// </summary>
        private const string BaseBlueprint = "Info Tool";

        public const int ClothId = 428;
        public const int BatteryPackId = 787;

        /// <summary>
        ///     Creates a new outdoor stand blueprint
        /// </summary>
        /// <param name="texture">The already loaded textrue for the Outdoor Stand</param>
        /// <returns></returns>
        public static BluePrint NewOutdoorStandBlueprint(Texture2D texture)
        {
            BluePrint blueprint = CreateBaseBlueprint();
            blueprint.name = OutdoorStandName;
            blueprint.displayName = "Outdoor Stand";
            blueprint.description = "A small stand where you can sell goods.";
            blueprint.moneyRequired = 69;
            blueprint.tilesWidth = 5;
            blueprint.tilesHeight = 3;
            blueprint.sourceRectForMenuView = new Rectangle(0, 0, 80, 112);
            blueprint.daysToConstruct = 3;
            blueprint.itemsRequired = new Dictionary<int, int>
            {
                [Object.wood] = 350,
                [Object.ironBar] = 5,
                [ClothId] = 10
            };

            SetTexture(blueprint, texture);
            return blueprint;
        }

        /// <summary>
        ///     Updates the public readonly Texture2D in the blueprint to the correct value
        /// </summary>
        /// <param name="blueprint">The blueprint to update</param>
        /// <param name="texture">The new texture</param>
        private static void SetTexture(BluePrint blueprint, Texture2D texture)
        {
            typeof(BluePrint).GetField("texture", BindingFlags.Public | BindingFlags.Instance)
                .SetValue(blueprint, texture);
        }

        private static BluePrint CreateBaseBlueprint() =>
            new BluePrint(BaseBlueprint)
            {
                humanDoor = new Point(-1, -1),
                animalDoor = new Point(-1, -1),
                blueprintType = "Buildings",
                maxOccupants = -1,
                namesOfOkayBuildingLocations = { "Farm" }
            };
    }
}
