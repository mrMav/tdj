using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDJGame.Engine.Tiled
{
    public class Tile
    {
        
        // Reference to the layer this Tile belongs to;        
        public Layer Layer;

        // display texture
        public Texture2D texture;
        public Rectangle textureBoundingRect;
        public Rectangle worldBoundingRect;        
        public Vector2 position;
        
        // constructor
        public Tile(Layer layer, Texture2D pTexture, Vector2 positionInTexture, Vector2 pPosition)
        {
            this.Layer = layer;

            this.texture = pTexture;
            this.position = pPosition;

            this.textureBoundingRect = new Rectangle(
                (int)positionInTexture.X * this.Layer.Level.TileWidth,
                (int)positionInTexture.Y * this.Layer.Level.TileHeight,
                this.Layer.Level.TileWidth,
                this.Layer.Level.TileHeight
            );

            this.worldBoundingRect = new Rectangle(
                (int)pPosition.X,
                (int)pPosition.Y,
                this.Layer.Level.TileWidth,  
                this.Layer.Level.TileHeight  
            );

            /*
             * we do not multiply the world rect size for a scale
             * because we will do that in the Camera2D class
             */


        }

        // logic update
        public void Update(GameTime pGameTime)
        {

        }

        // render
        public void Draw(SpriteBatch pSpriteBatch)
        {

            pSpriteBatch.Draw(this.texture, this.worldBoundingRect, this.textureBoundingRect, Color.White);

        }

    }
}
