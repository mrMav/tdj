using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Particles
{
    public class Particle : Sprite
    {

        public ParticleEmitter Spawner;

        public double SpawnedAtMilliseconds = 0f;
        public double MillisecondsAfterSpawn = 0f;

        /*
         * Constructor
         */
        public Particle(GraphicsDeviceManager graphics, Texture2D texture, Vector2 position, int width, int height)
            : base(graphics, texture, position, width, height, false)
        {

            Body.Acceleration = Vector2.Zero;

            Body.Drag = new Vector2(1f, 1f);

            Kill();

        }

        public override void Update(GameTime gameTime)
        {
            // call base.Update() for blinking feature

            if(Alive)
            {
                MillisecondsAfterSpawn += gameTime.ElapsedGameTime.TotalMilliseconds;

                if(Body.Acceleration.Length() > 0f)
                {
                    Body.Velocity.X += Body.Acceleration.X;
                    Body.Velocity.Y += Body.Acceleration.Y;
                }

                Body.X += Body.Velocity.X;
                Body.Y += Body.Velocity.Y;

                Body.Velocity.X *= Body.Drag.X;
                Body.Velocity.Y *= Body.Drag.Y;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }

        public void Reset()
        {
            Alive = false;
            Visible = false;

            SpawnedAtMilliseconds = 0f;
            MillisecondsAfterSpawn = 0f;

            Body.X = 0f;
            Body.Y = 0f;

            Body.Velocity.X = 0f;
            Body.Velocity.Y = 0f;

        }

    }

}
