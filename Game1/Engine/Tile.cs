using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDJGame
{
    public class Tile
    {
        // display texture
        public Texture2D texture;
        public Rectangle textureBoundingRect;
        public Rectangle worldBoundingRect;        
        public Vector2 position;
        public float size;
        
        // constructor
        public Tile(Texture2D pTexture, Vector2 positionInTexture, Vector2 pPosition, float size)
        {
            this.texture = pTexture;
            this.position = pPosition;
            this.size = size;

            this.textureBoundingRect = new Rectangle((int)positionInTexture.X * 16, (int)positionInTexture.Y * 16, 16, 16);
            this.worldBoundingRect = new Rectangle((int)pPosition.X, (int)pPosition.Y, (int)size, (int)size);

        }

        // logic update
        public void Update(GameTime pGameTime)
        {

        }

        // render
        public void Draw(SpriteBatch pSpriteBatch, float scale)
        {

            pSpriteBatch.Draw(this.texture, this.worldBoundingRect, this.textureBoundingRect, Color.White);

        }

    }
}
