using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine;
using Engine.Physics;
using System.Collections.Generic;
using System;
using Engine.Animations;
using Engine.Particles;

namespace TDJGame
{
    public class PufferFish : Enemy
    {
        float TravelSpeed;
        float TravelDistance;
        public float CurrentDistance;

        public float LastShot = 0f;
        public float ShootingVelocity = 3f;
        public float ShootRate = 500f;

        public List<Bullet> Bullets;
        public Vector2 Size;
        
        public PufferFish(GameState state, Texture2D texture, Vector2 position, int width, int height, float travelDistance = 32f, float travelSpeed = 0.5f)
            : base(state, texture, position, width, height, 0, 112)
        {
            TravelDistance = travelDistance;
            TravelSpeed = travelSpeed;
                        
            Body.SetSize(18, 13, 5, 9);
            Body.Enabled = true;
            Body.Velocity.X = TravelSpeed;
            Body.Tag = "pufferfish";

            /* Create a few bullets */
            Bullets = new List<Bullet>();
            for(int i = 0; i < 50; i++)
            {
                Bullet b = new Bullet(state, texture, Vector2.Zero, this);
                b.Animations.CurrentFrame = new Frame(32, 112, 16, 32);
                b.Body.SetSize(7, 9, 4, 12);

                Bullets.Add(b);
            }

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Alive)
            {                
                Body.X += Body.Velocity.X;
                Body.Y += Body.Velocity.Y;

                CurrentDistance += Body.Velocity.X;

                if (CurrentDistance <= 0) //arranjar maneira do inimigo começar na posição final do x que é = obj.width ou seja andar no sentido oposto
                {
                    FacingDirection = 1;  // go left now
                    Body.Velocity.X *= -1;
                }
                else if (CurrentDistance + Body.Bounds.Width >= TravelDistance)
                {
                    FacingDirection = -1;  // go right now
                    Body.Velocity.X *= -1;
                }

                /* Shooting */
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

                        b.Body.Velocity.X = 0;
                        b.Body.Velocity.Y = ShootingVelocity;
                    }
                }

                foreach (Bullet b in Bullets)
                {
                    b.Update(gameTime);
                }
            }
        }

        // render
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            if (Visible)
            {
                if (Alive)
                {
                    foreach (Bullet b in Bullets)
                    {
                        if (b.Visible && b.Alive)
                        {
                            b.Draw(gameTime, spriteBatch);
                        }
                    }
                }                
            }
        }
    }
}
