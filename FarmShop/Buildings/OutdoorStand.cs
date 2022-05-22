using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;

namespace Madia.FarmShop.Buildings
{
    /// <summary>
    ///     Represents the instance of an outdoor stand, where up to 5 goods can be sold
    ///     using basic selling chests
    /// </summary>
    public class OutdoorStand : Building
    {
        public OutdoorStand()
        {
        }

        public OutdoorStand(BluePrint blueprint, Vector2 location)
            : base(blueprint, location)
        {
        }

        /// <summary>
        ///     Lazily loads the texture from a custom point rather than from the
        ///     internal files. Also may apply paintable textures
        /// </summary>
        public override void resetTexture()
        {
            texture = new Lazy<Texture2D>(() =>
            {
                Texture2D texture = ModEntry.Instance.Helper.ModContent
                    .Load<Texture2D>("assets/OutdoorStand.png");
                if (paintedTexture != null)
                {
                    paintedTexture.Dispose();
                    paintedTexture = null;
                }

                // TODO apply paintedTexture
                if (paintedTexture != null)
                {
                    texture = paintedTexture;
                }

                return texture;
            });
        }

        /// <summary>
        ///     Returns the drawable rect for a menu
        /// </summary>
        public override Rectangle getSourceRectForMenu()
        {
            return new Rectangle(0, 0, texture.Value.Bounds.Width, texture.Value.Bounds.Height);
        }

        /// <summary>
        ///     Checks whether the player can interact with the object
        /// </summary>
        /// <param name="boundingBox">The bounding box of the object</param>
        /// <returns>False</returns>
        public override bool intersects(Rectangle boundingBox)
        {
            return false;
        }

        /// <summary>
        ///     Performs an action when the building is placed. Adds to a list of
        ///     selling options
        /// </summary>
        public override void performActionOnBuildingPlacement()
        {
            base.performActionOnBuildingPlacement();
        }

        /// <summary>
        ///     Performs an action when the building is deleted. removes from list
        ///     of selling options
        /// </summary>
        /// <param name="location">The location from where the building is deleted</param>
        public override void performActionOnDemolition(GameLocation location)
        {
            // TODO destroy goods stored and remove from location list
            base.performActionOnDemolition(location);
        }
    }
}
