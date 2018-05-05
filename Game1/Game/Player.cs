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
        public bool Floating;
        public bool Press;

        public float Energy;
        public float FloatingUpSpeed = 2f;
        public float MaxEnergy = 200f;

        public Vector2 Size;

        public Player(GraphicsDeviceManager graphics, Texture2D texture, Vector2 position, int width, int height, bool isControllable = true)
            : base(graphics, texture, position, width, height, true)
        {

            Energy = MaxEnergy;
            Size = new Vector2(16, 32);

            Body.Velocity.X = 0;
            Body.Velocity.Y = -2f;

            Body.Acceleration.X = 0.1f;
            Body.MaxVelocity = 3f;
            Body.Drag = 0.9f;

        }

        public void UpdateMotion(GameTime gameTime, KeyboardState keyboardState, Level level)
        {
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

                if (keyboardState.IsKeyDown(Keys.Space)) // Basicly trigger
                {
                    Press = true;
                }

                if (Press && keyboardState.IsKeyUp(Keys.Space)) //Switch entre estados
                {
                    Floating = !Floating;
                    Press = false;
                }
                
                /* Floating */

                if (Floating)
                {
                    if (Body.Position.Y >= Size.Y / 2)
                    {
                        Body.Velocity.Y = -FloatingUpSpeed; //Floating Up
                    }
                    if (Energy < MaxEnergy)
                        Energy += 1;
                }

                if (!Floating && Body.Position.Y <= Graphics.PreferredBackBufferHeight - Size.Y)
                {
                    if (Energy > 0f)
                    {
                        Body.Velocity.Y = 4f; //Floating Down
                        Energy -= 1f;
                    }
                    else
                        Floating = true;
                }

                /* ---- */

                // cap velocity
                if (Body.Velocity.Length() > Body.MaxVelocity)
                {
                    Body.Velocity.Normalize();
                    Body.Velocity *= Body.MaxVelocity;
                }

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
                //this.Body.Velocity *= this.Body.Drag;

                Body.Velocity.X *= Body.Drag;
                Body.Velocity.Y = 0;

                this.Body.Update(gameTime);


            }


            #region [Testing]

            /*
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
                 *//*

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
            */

            #endregion

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }

    }
}
