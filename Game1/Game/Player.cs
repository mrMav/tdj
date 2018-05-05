using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine;

namespace TDJGame
{
    public class Player : Sprite
    {
        public Player(Texture2D texture, Vector2 position, int width, int height, bool isControllable = true)
            : base(texture, position, width, height, true)
        {
            
        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState)
        {

            this.Body.ResetMovingDirections();

            // controlling 
            if (this.IsControllable && keyboardState != null)
            {

                float ellapsedTimeMultiplier = 100f;

                // move left
                if (keyboardState.IsKeyDown(Keys.A))
                {                
                    this.Body.Velocity.X -= this.Body.Acceleration.X * (float)gameTime.ElapsedGameTime.TotalSeconds * ellapsedTimeMultiplier;
                }
                // move right
                if (keyboardState.IsKeyDown(Keys.D))
                {
                    this.Body.Velocity.X += this.Body.Acceleration.X * (float)gameTime.ElapsedGameTime.TotalSeconds * ellapsedTimeMultiplier;
                }
                // move up
                if (keyboardState.IsKeyDown(Keys.W))
                {
                    this.Body.Velocity.Y -= this.Body.Acceleration.X * (float)gameTime.ElapsedGameTime.TotalSeconds * ellapsedTimeMultiplier;
                }
                // move down
                if (keyboardState.IsKeyDown(Keys.S))
                {
                    this.Body.Velocity.Y += this.Body.Acceleration.X * (float)gameTime.ElapsedGameTime.TotalSeconds * ellapsedTimeMultiplier;
                }

                // apply velocity
                this.Body.X += this.Body.Velocity.X;
                this.Body.Y += this.Body.Velocity.Y;

                // apply drag
                this.Body.Velocity *= this.Body.Drag;

            }

            this.Body.Update(gameTime);

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }

    }
}
