using Engine;
using Engine.Animations;
using Engine.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDJGame.Utils;

namespace TDJGame
{
    /*
     * This thing is a class just so if we want to add more funcs to the 
     * little guy
     */ 
    class GoldFish : Sprite
    {

        Vector2 InitialPosition;
        ParticleEmitter emitter;

        public GoldFish(GameState state, Texture2D texture, Vector2 position, int width, int height)
            : base(state, texture, position, width, height, false)
        {

            InitialPosition = position;

            emitter = new ParticleEmitter(state, position.X, position.Y, 5);

            emitter.MakeRandomParticles(texture, new Rectangle[]
            {
                new Rectangle(48, 80, 3, 3),
                new Rectangle(52, 80, 3, 3),
                new Rectangle(56, 80, 3, 3)
            });

            emitter.EmitterBox.Resize(width, height);

            //emitter.MakeParticles(texture, 16, 16);
            //emitter.SetTextureCropRectangle(new Rectangle(3 * 16, 6 * 16, 16, 16));

            emitter.SetAcceleration(0, -0.001f);
            emitter.XVelocityVariationRange = new Vector2(-10, 10);
            emitter.YVelocityVariationRange = new Vector2(-10, 0);
            emitter.SpawnRate = 300f;
            emitter.ParticleLifespanMilliseconds = 1000f;
            emitter.ParticleLifespanVariationMilliseconds = 100f;
            emitter.InitialScale = 1.0f;
            emitter.FinalScale = 0.2f;
            emitter.Activated = true;

            Animations.CurrentFrame = new Frame(64, 96, 16, 16);
        }

        public override void Update(GameTime gameTime)
        {
            if(Alive)
            {
                base.Update(gameTime);
            
                float x = (float)gameTime.TotalGameTime.TotalMilliseconds;
                Body.Velocity.Y = Math2.SinWave((x * 0.001f), 4f);

                Body.Y = InitialPosition.Y + Body.Velocity.Y;

                emitter.Update(gameTime);
            }

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            if(Alive && Visible)
            {
                base.Draw(gameTime, spriteBatch);

                emitter.Draw(gameTime, spriteBatch);

            }

        }
        
    }
}
