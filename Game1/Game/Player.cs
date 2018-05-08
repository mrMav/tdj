using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine;
using Engine.Tiled;
using Engine.Physics;
using System.Collections.Generic;
using System;

namespace TDJGame
{
    public class Player : Sprite
    {
        public bool Floating;
        public bool Press;

        public float Energy;
        public float FloatingUpSpeed = 2f;
        public float AnchorDownSpeed = 4f;
        public float MaxEnergy = 200f;
        public float EnergyGainRate = 1f;
        public float EnergyDrainRate = 3f;
        public bool EnergyRecharging = false;

        public float LastShot = 0f;
        public float ShootingVelocity = 3f;
        public float ShootRate = 80f;
        public float BulletCost = 10f;

        public List<Bullet> Bullets;
        public Vector2 Size;

        // 1 right -1 left
        public int FacingDirection = 1;

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

            /* Create a few bullets */
            Bullets = new List<Bullet>();
            for(int i = 0; i < 50; i++)
            {
                Bullet b = new Bullet(graphics, texture, Vector2.Zero, this);
                b.TextureBoundingRect = new Rectangle(0 * 16, 2 * 16, 16, 16);

                Bullets.Add(b);
            }

            Floating = true;

        }

        public void UpdateMotion(GameTime gameTime, KeyboardState keyboardState, Level level)
        {

            if (Alive)
            {

                base.Update(gameTime, keyboardState);

                if (this.IsControllable && keyboardState != null)
                {

                    float ellapsedTimeMultiplier = 1000f;

                    // move left
                    if (keyboardState.IsKeyDown(Keys.A))
                    {
                        this.Body.Velocity.X -= this.Body.Acceleration.X * (float)gameTime.ElapsedGameTime.TotalSeconds * ellapsedTimeMultiplier;
                        this.FacingDirection = -1;
                    }
                    // move right
                    if (keyboardState.IsKeyDown(Keys.D))
                    {
                        this.Body.Velocity.X += this.Body.Acceleration.X * (float)gameTime.ElapsedGameTime.TotalSeconds * ellapsedTimeMultiplier;
                        this.FacingDirection = 1;
                    }

                    if (keyboardState.IsKeyDown(Keys.Space) && !EnergyRecharging) // Basicly trigger
                    {
                        Floating = false;

                    } else
                    {
                        Floating = true;
                    }

                    //if (Press && keyboardState.IsKeyUp(Keys.Space)) //Switch entre estados
                    //{
                    //    Floating = !Floating;
                    //    Press = false;
                    //}

                    /* Floating */

                    if (Floating)
                    {
                        if (Body.Position.Y >= 0)
                        {
                            Body.Velocity.Y = -FloatingUpSpeed; //Floating Up
                        }

                        if (Energy < MaxEnergy)
                        {
                            Energy += EnergyGainRate;
                        }

                    } else 
                    {
                        if (Energy > 0f && !EnergyRecharging)
                        {
                            Body.Velocity.Y = AnchorDownSpeed; //Floating Down
                            Energy -= EnergyDrainRate;

                        } else
                        {
                            Floating = true;
                        }
                    }

                    if(Energy <= 0)
                    {
                        EnergyRecharging = true;

                    } else if(Energy >= MaxEnergy)
                    {
                        EnergyRecharging = false;
                    }

                    /* ---- */

                    // cap velocity
                    if (Body.Velocity.X > Body.MaxVelocity)
                    {
                        Body.Velocity.X = Body.MaxVelocity;
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
                    Body.Velocity.X *= Body.Drag;
                    Body.Velocity.Y = 0;

                    this.Body.Update(gameTime);

                    /* Shooting */

                    if (keyboardState.IsKeyDown(Keys.RightControl) && Energy >= BulletCost)
                    {

                        if (this.LastShot < gameTime.TotalGameTime.TotalMilliseconds)
                        {

                            //Console.WriteLine("Shooting at " + gameTime.TotalGameTime.TotalMilliseconds);
                            this.LastShot = (float)gameTime.TotalGameTime.TotalMilliseconds + this.ShootRate;

                            // get the first dead bullet
                            Bullet b = null;
                            for (int i = 0; i < Bullets.Count; i++)
                            {
                                if (!Bullets[i].Alive)
                                {
                                    b = Bullets[i];
                                    break;
                                }

                            }

                            if (b != null)
                            {

                                Random rnd = new Random();
                                int YVariation = 4;

                                b.Reset();
                                b.Revive();

                                b.ShotAtMilliseconds = gameTime.TotalGameTime.TotalMilliseconds;

                                b.Body.X = Body.X + (FacingDirection > 0 ? 24 : -2);
                                b.Body.Y = this.Body.Y + rnd.Next(-YVariation, YVariation) + 10;  //TODO: fix 16 offset with final sprites

                                b.Body.Velocity.X = (ShootingVelocity + (rnd.Next(-2, 2) * 0.1f)) * FacingDirection;  // some variation to the speed
                                b.Body.Velocity.Y = (rnd.Next(-3, -1) * 0.01f);  // make it float a bit

                                // subtract bullet cost to energy
                                Energy -= BulletCost;

                            }



                        }

                    }

                    foreach (Bullet b in Bullets)
                    {
                        b.Update(gameTime, keyboardState);
                    }
                    
                }

            }
            
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            if (FacingDirection < 0)
            {

                //spriteBatch.Draw(
                //     Texture2D texture,
                //     Rectangle destinationRectangle,
                //     Nullable<Rectangle> sourceRectangle,
                //     Color color,
                //     float rotation,
                //     Vector2 origin,
                //     SpriteEffects effects,
                //     float layerDepth
                //);

                spriteBatch.Draw(
                         Texture,
                         position: Body.Position,
                         sourceRectangle: TextureBoundingRect,
                         effects: SpriteEffects.FlipHorizontally,
                         color: Tint
                    );

            }
            else
            {
                spriteBatch.Draw(this.Texture, this.Body.Position, this.TextureBoundingRect, this.Tint);
            }

            foreach (Bullet b in Bullets)
            {
                if(b.Visible && b.Alive)
                {
                    b.Draw(gameTime, spriteBatch);
                }
            }

        }

    }
}
