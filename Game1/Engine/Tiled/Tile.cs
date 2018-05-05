using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Physics;

namespace Engine.Tiled
{
    public class Tile : Sprite
    {
        
        // Reference to the layer this Tile belongs to;        
        public Layer Layer;

        // display texture
        //public Texture2D Texture;
        //public Rectangle TextureBoundingRect;
        //public Color Tint;

        // the tile id
        public int ID { get; }

        //physics
        //public Body Body;
        
        // constructor
        public Tile(Layer layer, Texture2D pTexture, Vector2 positionInTexture, Vector2 pPosition, int ID)
            : base(pTexture, pPosition, layer.Level.TileWidth, layer.Level.TileWidth, false)
        {
            this.Layer = layer;

            this.Texture = pTexture;

            this.TextureBoundingRect = new Rectangle(
                (int)positionInTexture.X * this.Layer.Level.TileWidth,
                (int)positionInTexture.Y * this.Layer.Level.TileHeight,
                this.Layer.Level.TileWidth,
                this.Layer.Level.TileHeight
            );


            /*
             * we do not multiply the world rect size for a scale
             * because we will do that in the Camera2D class
             */

            this.ID = ID;

            this.Body = new Body(pPosition.X, pPosition.Y, this.Layer.Level.TileWidth, this.Layer.Level.TileHeight);

            this.Tint = Color.White;

        }

        // logic update
        public void Update(GameTime pGameTime)
        {

        }

        // render
        public void Draw(SpriteBatch pSpriteBatch)
        {
            pSpriteBatch.Draw(this.Texture, this.Body.Position, this.TextureBoundingRect, this.Tint);
        }

    }
}
