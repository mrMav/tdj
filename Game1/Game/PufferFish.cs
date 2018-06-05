using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine;
using Engine.Physics;
using System.Collections.Generic;
using System;
using Engine.Animations;
using Engine.Particles;
using Microsoft.Xna.Framework.Audio;

namespace TDJGame
{
    public class PufferFish : Enemy
    {
        public float TravelSpeed;
        public float TravelDistance;
        public float CurrentDistance;

        public float LastShot = 0f;
        public float ShootingVelocity = 3f;
        public float ShootRate = 1250f;

        public List<Bullet> Bullets;
        public Vector2 Size;
        
        public PufferFish(GameState state, Texture2D texture, Vector2 position, int width, int height, float travelDistance = 32f, float travelSpeed = 0.5f)
            : base(state, texture, position, width, height, 64, 144)
        {
            TravelDistance = travelDistance;
            TravelSpeed = travelSpeed;

            Animations.CurrentFrame = new Frame(64, 144, 32, 32);
            Body.SetSize(25, 18, 6, 9);
            Body.Enabled = true;
            Body.Velocity.X = TravelSpeed;
            Body.Tag = "pufferfish";

            /* Create a few bullets */
            Bullets = new List<Bullet>();
            for(int i = 0; i < 50; i++)
            {
                Bullet b = new Bullet(state, texture, Vector2.Zero, this);
                b.Animations.CurrentFrame = new Frame(0, 144, 16, 16);
                b.Body.SetSize(7, 9, 5, 3); //due to a change

                Bullets.Add(b);
            }

            /* Animations */

            Animation swimAnim = Animations.Add("swim", new Frame[] { new Frame(96, 144, 32, 32), new Frame(128, 144, 32, 32), new Frame(160, 144, 32, 32) , new Frame(192, 144, 32, 32)}, 5,true,false);
            Animation shootAnim = Animations.Add("shoot", new Frame[] { new Frame(240, 144, 32, 32), new Frame(272, 144, 32, 32), new Frame(302, 144, 32, 32), new Frame(334, 144, 32, 32) }, 5, false, false);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Alive)
            {
                Animations.Play("swim");

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

                    b = null;
                    // get the first dead bullet                    
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

                        Animations.Play("shoot"); //probabbly not here

                        Random rnd = new Random();
                        int YVariation = 4;

                        b.Reset();
                        b.Revive();

                        b.ShotAtMilliseconds = gameTime.TotalGameTime.TotalMilliseconds;

                        b.Body.X = Body.X + (FacingDirection > 0 ? 24 : -2);
                        b.Body.Y = this.Body.Y + rnd.Next(-YVariation, YVariation) + 10;  //TODO: fix 16 offset with final sprites

                        b.Body.Velocity.X = 0;
                        b.Body.Velocity.Y = ShootingVelocity * -1;
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


        public override void Kill()
        {
            base.Kill();
            SoundEffect pDeath;
            State.SFX.TryGetValue("enemyDeath", out pDeath);
            pDeath?.Play(1,0f,0);

        }

    }
}
