using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDJGame.Engine
{
    public class Sprite
    {
        // display texture
        public Texture2D texture;
        public Rectangle textureBoundingRect;

        // physics properties
        public Vector2 position;
        public Vector2 acceleration;
        public Vector2 velocity;
        public Vector2 origin;
        public float angle = 0f;
        public float drag = 0.95f;

        public bool isControllable;

        // constructor
        public Sprite(Texture2D pTexture, Vector2 pPosition, bool pIsControllable = false)
        {
            this.texture = pTexture;
            this.position = pPosition;
            this.isControllable = pIsControllable;

            this.textureBoundingRect = new Rectangle(0, 0, this.texture.Width, this.texture.Height);

            this.acceleration = new Vector2(1f);
            this.velocity = new Vector2();
            this.origin = new Vector2(this.texture.Width / 2, this.texture.Height / 2);

        }

        // logic update
        public virtual void Update(GameTime pGameTime, KeyboardState kState)
        {
            
        }

        // render
        public virtual void Draw(GameTime pGameTime, SpriteBatch pSpriteBatch)
        {

            pSpriteBatch.Draw(this.texture, this.position, this.textureBoundingRect, Color.White, this.angle, this.origin, 1.0f, SpriteEffects.None, 1);

        }
    }
}
