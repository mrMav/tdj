using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine;
using Engine.Tiled;
using Engine.Physics;
using System.Collections.Generic;
using System;
using Engine.Animations;
using TDJGame.Utils;
using Microsoft.Xna.Framework.Audio;

namespace TDJGame
{
    public class Player : Sprite
    {
        public bool Floating;
        public bool Press;

        public float Energy;
        public float FloatingUpSpeed = 2f;
        public float FloatingDownSpeed = 4f;
        public float MaxEnergy = 200f;

        public bool IsBobing = false;
        public float BobStarted = 0f;
        public float BobAmplitude = 1f;
        public float BobFrequency = 0.005f;

        public float LastShot = 0f;
        public float ShootingVelocity = 3f;
        public float ShootRate = 80f;
        public float BulletCost = 10f;

        public float KnockBackAmmount = 64f;

        public List<Bullet> Bullets;
        public Vector2 Size;

        // 1 right -1 left
        public int FacingDirection = 1;

        public Player(GameState state, Texture2D texture, Vector2 position, int width, int height, bool isControllable = true)
            : base(state, texture, position, width, height, true)
        {

            Energy = MaxEnergy;
            Size = new Vector2(16, 32);

            Body.Velocity.X = 0;
            Body.Velocity.Y = -2f;

            Body.Acceleration.X = 0.1f;
            Body.MaxVelocity = 3f;
            Body.Drag.X = 0.9f;

            /* Create a few bullets */
            Bullets = new List<Bullet>();
            for(int i = 0; i < 50; i++)
            {
                Bullet b = new Bullet(state, texture, Vector2.Zero, this);
                b.Animations.CurrentFrame = new Frame(48, 96, 16, 16);
                b.Body.SetSize(6, 6, 5, 5);
                b.Body.Drag.Y *= 1.1f;
                b.Damage = 500f;

                Bullets.Add(b);
            }

            Floating = true;

        }

        public void UpdateMotion(GameTime gameTime, KeyboardState keyboardState, Level level)
        {

            if (Alive)
            {
                base.Update(gameTime);

                if (this.IsControllable && keyboardState != null)
                {

                    this.Body.PreMovementUpdate(gameTime);

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
                        if (Body.Position.Y >= 0)
                        {
                            Body.Velocity.Y = -FloatingUpSpeed; //Floating Up
                        }

                        // bob a bit
                        if(Body.Position.Y <= 0f && !IsBobing)
                        {
                            IsBobing = true;
                            BobStarted = (float)gameTime.TotalGameTime.TotalMilliseconds;
                            Energy = MathHelper.Clamp(Energy + MaxEnergy / 2, 0, MaxEnergy);
                        }

                        // recharge
                        if (Energy < MaxEnergy)
                        {
                            Energy += 1;
                        }

                    } else
                    {

                        IsBobing = false;

                    }
                    
                    if (!Floating && Body.Position.Y <= State.Graphics.PreferredBackBufferHeight - Size.Y)
                    {
                        if (Energy > 0f)
                        {
                            Body.Velocity.Y = FloatingDownSpeed; //Floating Down
                            Energy -= 1f;
                        }
                        else
                            Floating = true;
                    }

                    // makes the player bob on surface
                    if(IsBobing)
                    {

                        float x = (float)gameTime.TotalGameTime.TotalMilliseconds - BobStarted;

                        float phaseShift = 0.5f * (float)Math.PI;

                        Body.Velocity.Y = Math2.SinWave((x * BobFrequency - phaseShift), BobAmplitude);

                    }
                    
                    /* ---- */

                    // cap velocity
                    if (Body.Velocity.Length() > Body.MaxVelocity)
                    {
                        Body.Velocity.Normalize();
                        Body.Velocity *= Body.MaxVelocity;
                    }

                    this.Body.PreCollisionUpdate(gameTime);

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
                    Body.Velocity.X *= Body.Drag.X;
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
                        b.Update(gameTime);
                    }
                    
                }

            }
            
        }

        public void ApplyKnockBack(Sprite sprite)
        {

            Floating = true;

            float intersectionAngle = (float)Math.Atan2((sprite.Body.Y - Body.Y), (sprite.Body.X - Body.X));

            // apply 
            Body.Velocity.X += (float)Math.Cos(intersectionAngle + Math.PI) * KnockBackAmmount;
            Body.Velocity.Y += (float)Math.Sin(intersectionAngle + Math.PI) * KnockBackAmmount;
            
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            if (Visible)
            {
                if (FacingDirection < 0)
                {
                    spriteBatch.Draw(
                             Texture,
                             position: Body.Position,
                             sourceRectangle: Animations.CurrentFrame.TextureSourceRect,
                             effects: SpriteEffects.FlipHorizontally,
                             color: Tint
                        );

                }
                else
                {
                    spriteBatch.Draw(this.Texture, this.Body.Position, Animations.CurrentFrame.TextureSourceRect, this.Tint);
                }

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
