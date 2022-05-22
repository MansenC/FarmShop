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
using StardewValley.Locations;

namespace Madia.FarmShop.Patches
{
    internal class BuildableGameLocation_BuildStructure
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
                $"Patching BuildableGameLocation::buildStructure to accord for new blueprints",
                LogLevel.Debug);

            harmony.Patch(
                original: AccessTools.Method(
                    typeof(BuildableGameLocation),
                    nameof(BuildableGameLocation.buildStructure),
                    new[]
                    {
                        typeof(BluePrint),
                        typeof(Vector2),
                        typeof(Farmer),
                        typeof(bool),
                        typeof(bool)
                    }),
                prefix: new HarmonyMethod(
                    typeof(BuildableGameLocation_BuildStructure),
                    nameof(NewBuildStructure)));

            _applied = true;
        }

        /// <summary>
        ///     This patch is required due to a new Building() call for unknown construction things.
        ///     This also means I had to copy the entire logic or else fucked up things would happen.
        ///     I won't re-implement the other buildings though
        /// </summary>
        private static bool NewBuildStructure(
            BuildableGameLocation __instance,
            BluePrint structureForPlacement,
            Vector2 tileLocation,
            Farmer who,
            bool magicalConstruction,
            bool skipSafetyChecks,
            ref bool __result)
        {
            try
            {
                if (!structureForPlacement.name.Contains(BlueprintFactory.OutdoorStandName))
                {
                    return true;
                }

                if (!skipSafetyChecks)
                {
                    for (int y = 0; y < structureForPlacement.tilesHeight; y++)
                    {
                        for (int x = 0; x < structureForPlacement.tilesWidth; x++)
                        {
                            __instance.pokeTileForConstruction(new Vector2(tileLocation.X + x, tileLocation.Y + y));
                        }
                    }

                    foreach (var point in structureForPlacement.additionalPlacementTiles)
                    {
                        int x2 = point.X;
                        int y2 = point.Y;
                        __instance.pokeTileForConstruction(new Vector2(tileLocation.X + x2, tileLocation.Y + y2));
                    }

                    for (int y3 = 0; y3 < structureForPlacement.tilesHeight; y3++)
                    {
                        for (int x3 = 0; x3 < structureForPlacement.tilesWidth; x3++)
                        {
                            Vector2 currentGlobalTilePosition = new Vector2(tileLocation.X + x3, tileLocation.Y + y3);
                            if (CheckTile(__instance, currentGlobalTilePosition, x3, y3))
                            {
                                continue;
                            }

                            __result = false;
                            return false;
                        }
                    }

                    foreach (var point in structureForPlacement.additionalPlacementTiles)
                    {
                        Vector2 currentGlobalTilePosition = new Vector2(tileLocation.X + point.X, tileLocation.Y + point.Y);
                        if (CheckTile(__instance, currentGlobalTilePosition, point.X, point.Y))
                        {
                            continue;
                        }

                        __result = false;
                        return false;
                    }

                    if (structureForPlacement.humanDoor != new Point(-1, -1))
                    {
                        Vector2 doorPos = tileLocation + new Vector2(structureForPlacement.humanDoor.X, structureForPlacement.humanDoor.Y + 1);
                        if (!__instance.isBuildable(doorPos) && !__instance.isPath(doorPos))
                        {
                            __result = false;
                            return false;
                        }
                    }
                }

                OutdoorStand stand = new OutdoorStand(structureForPlacement, tileLocation);
                stand.owner.Value = who.UniqueMultiplayerID;
                if (!skipSafetyChecks)
                {
                    string checkResult = stand.isThereAnythingtoPreventConstruction(__instance, tileLocation);
                    if (checkResult != null)
                    {
                        Game1.addHUDMessage(new HUDMessage(checkResult, Color.Red, 3500f));
                        __result = false;
                        return false;
                    }
                }

                for (int y = 0; y < structureForPlacement.tilesHeight; y++)
                {
                    for (int x = 0; x < structureForPlacement.tilesWidth; x++)
                    {
                        Vector2 globalPos = new Vector2(tileLocation.X + x, tileLocation.Y + y);
                        __instance.terrainFeatures.Remove(globalPos);
                    }
                }

                __instance.buildings.Add(stand);
                stand.performActionOnConstruction(__instance);
                if (magicalConstruction)
                {
                    PatchUtils.GetPrivateStaticValue<Game1, Multiplayer>("multiplayer")
                        .globalChatInfoMessage("BuildingMagicBuild", new string[]
                        {
                            Game1.player.Name,
                            Utility.AOrAn(structureForPlacement.displayName),
                            structureForPlacement.displayName,
                            Game1.player.farmName.ToString()
                        });
                }
                else
                {
                    PatchUtils.GetPrivateStaticValue<Game1, Multiplayer>("multiplayer")
                        .globalChatInfoMessage("BuildingBuild", new string[]
                        {
                            Game1.player.Name,
                            Utility.AOrAn(structureForPlacement.displayName),
                            structureForPlacement.displayName,
                            Game1.player.farmName.ToString()
                        });
                }

                __result = true;
                return false;
            }
            catch (Exception ex)
            {
                _monitor.Log(ex.Message, LogLevel.Error);
            }

            return true;
        }

        private static bool CheckTile(BuildableGameLocation location, Vector2 globalPosition, int x, int y)
        {
            if (!location.isBuildable(globalPosition))
            {
                return false;
            }

            using (var enumerator = location.farmers.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.GetBoundingBox().Intersects(new Rectangle(x * 64, y * 64, 64, 64)))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
