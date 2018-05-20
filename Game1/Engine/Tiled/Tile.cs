using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Animations;

namespace Engine.Tiled
{
    public class Tile : Sprite
    {
        
        // Reference to the layer this Tile belongs to;        
        public Layer Layer;
        
        // the tile id
        public int ID { get; }
        
        // constructor
        public Tile(GameState state, Layer layer, Texture2D pTexture, Vector2 positionInTexture, Vector2 pPosition, int ID)
            : base(state, pTexture, pPosition, layer.Level.TileWidth, layer.Level.TileWidth, false)
        {
            this.Layer = layer;

            this.Texture = pTexture;

            this.Animations.CurrentFrame = new Frame(
                (int)positionInTexture.X * this.Layer.Level.TileWidth,
                (int)positionInTexture.Y * this.Layer.Level.TileHeight,
                this.Layer.Level.TileWidth,
                this.Layer.Level.TileHeight,
                0
            );
            
            this.ID = ID;
            
            this.Tint = Color.White;

            this.FacingDirection = 1;

        }
    }
}
