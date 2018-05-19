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
    public class Enemy : Sprite
    {
        private ParticleEmitter deathParticles;

        public Enemy(GameState state, Texture2D texture, Vector2 position, int width, int height, int xCoordinateParticles = 0, int yCoordinateParticles = 0, int ParticlesSize = 2)
            : base(state, texture, position, width, height, false)
        {
            
            // this animation doesn't really play anything, it just gives us time to 
            // make some effects until it is killed. It should be a transparent frame
            Animation deathAnim = Animations.Add("death_interval", new Frame[] { new Frame(128, 112, 32, 32) }, 1, false, true);
            deathAnim.Delay = 1000f; // animation duration in milliseconds

            #region [Particles Death Animation]

            int psize = ParticlesSize;
            int maxp = (int)Math.Pow(width / psize, 2);
            deathParticles = new ParticleEmitter(state, 0, 0, (int)Math.Pow(width / 2, 2));
            deathParticles.ParseTextureToParticles(texture, xCoordinateParticles, yCoordinateParticles, width, height, psize, psize);
            float dispersion = 500f;
            deathParticles.XVelocityVariationRange = new Vector2(-dispersion, dispersion);
            deathParticles.YVelocityVariationRange = new Vector2(-dispersion, dispersion);
            deathParticles.ParticleLifespanMilliseconds = deathAnim.Delay;
            deathParticles.ParticleLifespanVariationMilliseconds = 500f;
            deathParticles.Burst = true;
            deathParticles.ParticlesPerBurst = maxp;
            deathParticles.InitialScale = 1.0f;
            deathParticles.FinalScale = 5.0f;
            deathParticles.RandomizeParticlePositions = false;

            #endregion

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            if (!Alive)
            {
                deathParticles.EmitterBox.X = Body.X;
                deathParticles.EmitterBox.Y = Body.Y;
                deathParticles.Update(gameTime);
            }

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            if (Visible)
            {
                deathParticles.Draw(gameTime, spriteBatch);
            }
        }

        public override void Kill()
        {
            Alive = false;

            Animations.Play("death_interval");
        }

    }
}
