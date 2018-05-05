using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine;
using Engine.Tiled;
using Engine.Physics;

namespace TDJGame
{
    public class Player : Sprite
    {
        public Player(Texture2D texture, Vector2 position, int width, int height, bool isControllable = true)
            : base(texture, position, width, height, true)
        {
            
        }

        public void UpdateMotion(GameTime gameTime, KeyboardState keyboardState, Level level)
        {

            this.Body.ResetMovingDirections();

            // controlling 
            if (this.IsControllable && keyboardState != null)
            {

                float ellapsedTimeMultiplier = 1000f;

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

                // cap velocity
                if (Body.Velocity.Length() > Body.MaxVelocity)
                {
                    Body.Velocity.Normalize();
                    Body.Velocity *= Body.MaxVelocity;
                }

                /*
                 * Collisions
                 * 
                 * In order to solve the 'sticking' issue with tilemaps,
                 * first move in x axis and solve possible collisions
                 * second move in y and solve possible collisions.
                 * 
                 * A broadphase should be implemented for performance.
                 * (in case of a tilemap, a broadphase is plain simple,
                 * implement when performance drops only. Prototype phase
                 * should not really need it)
                 * 
                 * Maybe use a layer for collisions only?
                 * 
                 */

                this.Body.ResetCollisions();

                // apply x velocity
                this.Body.X += this.Body.Velocity.X;

                // solve x collisions
                for (int i = 0; i < level.CollidableTiles.Count; i++)
                {
                    Physics.Collide(this, level.CollidableTiles[i], 0); // collide in x
                }

                // apply y velocity
                this.Body.Y += this.Body.Velocity.Y;

                // solve y collisions
                for (int i = 0; i < level.CollidableTiles.Count; i++)
                {
                    Physics.Collide(this, level.CollidableTiles[i], 1); // collide in y
                }

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
