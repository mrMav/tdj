using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDJGame
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
        public Sprite(Texture2D pTexture, Vector2 pPosition, bool pIsControllable = true)
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
        public void Update(GameTime pGameTime, KeyboardState kState)
        {
            // rotate over time
            this.angle += 0.1f;

            // controlling 
            if (this.isControllable && kState != null)
            {

                float ellapsedTimeMultiplier = 100f;

                // move left
                if (kState.IsKeyDown(Keys.A))
                {
                    this.velocity.X -= this.acceleration.X * (float)pGameTime.ElapsedGameTime.TotalSeconds * ellapsedTimeMultiplier;
                }
                // move right
                if (kState.IsKeyDown(Keys.D))
                {
                    this.velocity.X += this.acceleration.X * (float)pGameTime.ElapsedGameTime.TotalSeconds * ellapsedTimeMultiplier;
                }
                // move up
                if (kState.IsKeyDown(Keys.W))
                {
                    this.velocity.Y -= this.acceleration.Y * (float)pGameTime.ElapsedGameTime.TotalSeconds * ellapsedTimeMultiplier;
                }
                // move down
                if (kState.IsKeyDown(Keys.S))
                {
                    this.velocity.Y += this.acceleration.Y * (float)pGameTime.ElapsedGameTime.TotalSeconds * ellapsedTimeMultiplier;
                }

                // apply velocity
                this.position = this.position + this.velocity;
                // apply drag
                this.velocity *= this.drag;

            }

        }

        // render
        public void Draw(GameTime pGameTime, SpriteBatch pSpriteBatch)
        {

            pSpriteBatch.Draw(this.texture, this.position, this.textureBoundingRect, Color.White, this.angle, this.origin, 1.0f, SpriteEffects.None, 1);

        }
    }
}
