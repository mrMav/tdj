using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TDJGame
{
    public class Bullet : Sprite
    {

        public bool Alive;
        public bool Visible;

        public Sprite Spawner;
        public float MaxDistanceFromSpawner = 100f;  // using kill based on this makes it possible to keep a bullet alive by moving along with it
        public double ShotAtMilliseconds = 0f;
        public double TimeAfterShot = 0f;
        public double MaxAliveMilliseconds = 1000f;


        public Bullet(GraphicsDeviceManager graphics, Texture2D texture, Vector2 position, Sprite spawner)
            : base(graphics, texture, position, 16, 16, false)
        {

            Spawner = spawner;

            Alive = false;
            Visible = false;
        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            if(Alive)
            {
                TimeAfterShot += gameTime.ElapsedGameTime.TotalMilliseconds;

                Body.X += Body.Velocity.X;
                Body.Y += Body.Velocity.Y;

                //if(Vector2.Distance(Spawner.Body.Position, Body.Position) >= MaxDistanceFromSpawner)
                //{
                //    Kill();
                //}

                if(TimeAfterShot >= MaxAliveMilliseconds)
                {
                    Kill();
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
        }

        public void Revive()
        {
            Alive = true;
            Visible = true;
        }

        public void Kill()
        {
            Alive = false;
            Visible = false;
        }

    }

}
