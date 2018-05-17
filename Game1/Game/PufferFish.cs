using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine;
using Engine.Physics;
using System.Collections.Generic;
using System;
using Engine.Animations;

namespace TDJGame
{
    public class PufferFish : Sprite
    {
        float TravelSpeed;
        float TravelDistance;
        float CurrentDistance;
        int FacingDirection = 1;  // 1 is right, -1 is left

        public float LastShot = 0f;
        public float ShootingVelocity = 3f;
        public float ShootRate = 500f;

        public List<Bullet> Bullets;
        public Vector2 Size;

        public PufferFish(GameState state, Texture2D texture, Vector2 position, int width, int height, float travelDistance = 32f, float travelSpeed = 0.5f)
            : base(state, texture, position, width, height, false)
        {
            TravelDistance = travelDistance;
            TravelSpeed = travelSpeed;

            Animations.CurrentFrame = new Frame(9 * 16, 0, 32, 32);

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

            Body.X += Body.Velocity.X;
            Body.Y += Body.Velocity.Y;

            CurrentDistance += Body.Velocity.X;

            if (CurrentDistance <= 0)
            {
                FacingDirection = 1;  // go right now
                Body.Velocity.X *= -1;
            }
            else if (CurrentDistance + Body.Bounds.Width >= TravelDistance)
            {
                FacingDirection = -1;  // go left now
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

        // render
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Visible)
            {

                if(FacingDirection > 0)
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
                             sourceRectangle: this.Animations.CurrentFrame.TextureSourceRect,
                             effects: SpriteEffects.FlipHorizontally,
                             color: Tint
                        );

                } else
                {
                    spriteBatch.Draw(this.Texture, this.Body.Position, this.Animations.CurrentFrame.TextureSourceRect, this.Tint);
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
}
