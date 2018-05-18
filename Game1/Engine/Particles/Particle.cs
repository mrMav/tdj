using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDJGame.Utils;

namespace Engine.Particles
{
    public class Particle : Sprite
    {

        public ParticleEmitter Spawner;

        public double SpawnedAtMilliseconds = 0f;
        public double MillisecondsAfterSpawn = 0f;
        public double LifespanMilliseconds = 0f;

        public float Scale = 1f;
        public float InitialScale = 1f;
        public float FinalScale = 1f;

        public Vector2 InitialPosition;

        /*
         * Constructor
         */
        public Particle(GameState state, Texture2D texture, Vector2 position, int width, int height)
            : base(state, texture, position, width, height, false)
        {

            Body.Acceleration = Vector2.Zero;

            Body.Drag = new Vector2(1f, 1f);

            InitialPosition.X = position.X;
            InitialPosition.Y = position.Y;

            Kill();

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(Alive)
            {
                MillisecondsAfterSpawn += gameTime.ElapsedGameTime.TotalMilliseconds;

                double percent = MillisecondsAfterSpawn / LifespanMilliseconds;

                Body.Velocity.X += Body.Acceleration.X;
                Body.Velocity.Y += Body.Acceleration.Y;
                
                Body.X += Body.Velocity.X;
                Body.Y += Body.Velocity.Y;

                Body.Velocity.X *= Body.Drag.X;
                Body.Velocity.Y *= Body.Drag.Y;

                if(InitialScale != FinalScale)
                    Scale = Math2.Map((float)MillisecondsAfterSpawn, 0f, (float)LifespanMilliseconds, InitialScale, FinalScale);

            }

            if (MillisecondsAfterSpawn >= LifespanMilliseconds)
            {
                this.Kill();
            }

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                spriteBatch.Draw(Texture, Body.Position, Animations.CurrentFrame.TextureSourceRect, Tint, 0f, Body.Origin, Scale, SpriteEffects.None, 0f);
            }
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
