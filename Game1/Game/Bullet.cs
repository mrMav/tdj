using Engine;
using TDJGame.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace TDJGame
{
    public class Bullet : Sprite
    {
        
        public Sprite Spawner;
        public float MaxDistanceFromSpawner = 100f;  // using kill based on this makes it possible to keep a bullet alive by moving along with it
        public double ShotAtMilliseconds = 0f;
        public double TimeAfterShot = 0f;
        public double StartingFadeAnimAt = 750f;
        public double MaxAliveMilliseconds = 1000f;
        
        public Bullet(GameState state, Texture2D texture, Vector2 position, Sprite spawner)
            : base(state, texture, position, 16, 16, false)
        {

            Spawner = spawner;

            Alive = false;
            Visible = false;
        }

        public override void Update(GameTime gameTime)
        {
            if(Alive)
            {

                Random rnd = new Random();

                TimeAfterShot += gameTime.ElapsedGameTime.TotalMilliseconds;

                Body.X += Body.Velocity.X;
                Body.Y += Body.Velocity.Y;

                Body.Velocity.Y *= 1.1f;

                //if(Vector2.Distance(Spawner.Body.Position, Body.Position) >= MaxDistanceFromSpawner)
                //{
                //    Kill();
                //}

                if(TimeAfterShot >= MaxAliveMilliseconds)
                {
                    Kill();
                }

                // fade anim
                if(TimeAfterShot >= StartingFadeAnimAt)
                {
                    float alpha = Math2.Map((float)TimeAfterShot, (float)StartingFadeAnimAt, (float)MaxAliveMilliseconds, 255f, 0f);
                    Tint = Utility.FadeColor(Tint, (byte)(255 - (byte)(alpha >= 0f && alpha <= 255f ? alpha : 255f))); 
                }
                
            }
        }

        public void Reset()
        {
            Alive = false;
            Visible = false;

            Body.X = 0f;
            Body.Y = 0f;

            ShotAtMilliseconds = 0f;
            TimeAfterShot = 0f;

            Body.Velocity.X = 0f;
            Body.Velocity.Y = 0f;

            Body.Acceleration.X = 0f;
            Body.Acceleration.Y = 0f;

            Tint = Color.White;

        }

    }

}
