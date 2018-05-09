using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Engine;
using System;
using Microsoft.Xna.Framework.Media;
using Engine.Particles;
using System.Collections.Generic;

namespace TDJGame
{
    public class TestParticleEmitter : GameState
    {

        #region [Properties]

        Texture2D SpriteSheet;

        List<ParticleEmitter> Emitters;

        #endregion

        public TestParticleEmitter(string key, GraphicsDeviceManager graphics)
        {
            this.Key = key;
            this.Graphics = graphics;
        }

        public override void Initialize()
        {
            base.Initialize();

            Emitters = new List<ParticleEmitter>();

        }

        public override void LoadContent()
        {
            base.LoadContent();

            SpriteSheet = content.Load<Texture2D>("SpriteSheetDraft");

            Emitters.Add(new ParticleEmitter(this, Graphics.PreferredBackBufferWidth / 2, Graphics.PreferredBackBufferWidth / 2, 100));
            Emitters[0].MakeParticles(SpriteSheet, 16, 16);
            Emitters[0].ParticleVelocity = new Vector2(2, 2);
            Emitters[0].XVelocityVariationRange = new Vector2(-80f, 80f);
            Emitters[0].YVelocityVariationRange = new Vector2(-80f, 80f);
            Emitters[0].SetTextureCropRectangle(new Rectangle(0, 32, 16, 16));
            Emitters[0].EmitterBox.X = 0;
            Emitters[0].EmitterBox.Y = 0;
            Emitters[0].EmitterBox.Resize(1f, 1f);
            Emitters[0].SpawnRate = 50f;
            Emitters[0].ParticleLifespanMilliseconds = 1000f;
            Emitters[0].ParticleLifespanVariationMilliseconds = 100f;
            Emitters[0].Burst = false;

            Emitters.Add(new ParticleEmitter(this, Graphics.PreferredBackBufferWidth / 2, Graphics.PreferredBackBufferWidth / 2, 100));
            Emitters[1].MakeParticles(SpriteSheet, 16, 16);
            Emitters[1].ParticleVelocity = new Vector2(-2, 2);
            Emitters[1].XVelocityVariationRange = new Vector2(-80f, 80f);
            Emitters[1].YVelocityVariationRange = new Vector2(-80f, 80f);
            Emitters[1].SetTextureCropRectangle(new Rectangle(0, 32, 16, 16));
            Emitters[1].EmitterBox.X = 800f;
            Emitters[1].EmitterBox.Y = 0;
            Emitters[1].EmitterBox.Resize(1f, 1f);
            Emitters[1].SpawnRate = 50f;
            Emitters[1].ParticleLifespanMilliseconds = 1000f;
            Emitters[1].ParticleLifespanVariationMilliseconds = 100f;
            Emitters[1].Burst = false;

            Emitters.Add(new ParticleEmitter(this, Graphics.PreferredBackBufferWidth / 2, Graphics.PreferredBackBufferWidth / 2, 100));
            Emitters[2].MakeParticles(SpriteSheet, 16, 16);
            Emitters[2].ParticleVelocity = new Vector2(-2, -2);
            Emitters[2].XVelocityVariationRange = new Vector2(-80f, 80f);
            Emitters[2].YVelocityVariationRange = new Vector2(-80f, 80f);
            Emitters[2].SetTextureCropRectangle(new Rectangle(0, 32, 16, 16));
            Emitters[2].EmitterBox.X = 800f;
            Emitters[2].EmitterBox.Y = 600f;
            Emitters[2].EmitterBox.Resize(1f, 1f);
            Emitters[2].SpawnRate = 50f;
            Emitters[2].ParticleLifespanMilliseconds = 1000f;
            Emitters[2].ParticleLifespanVariationMilliseconds = 100f;
            Emitters[2].Burst = false;

            Emitters.Add(new ParticleEmitter(this, Graphics.PreferredBackBufferWidth / 2, Graphics.PreferredBackBufferWidth / 2, 100));
            Emitters[3].MakeParticles(SpriteSheet, 16, 16);
            Emitters[3].ParticleVelocity = new Vector2(2, -2);
            Emitters[3].XVelocityVariationRange = new Vector2(-80f, 80f);
            Emitters[3].YVelocityVariationRange = new Vector2(-80f, 80f);
            Emitters[3].SetTextureCropRectangle(new Rectangle(0, 32, 16, 16));
            Emitters[3].EmitterBox.X = 0f;
            Emitters[3].EmitterBox.Y = 600f;
            Emitters[3].EmitterBox.Resize(1f, 1f);
            Emitters[3].SpawnRate = 50f;
            Emitters[3].ParticleLifespanMilliseconds = 1000f;
            Emitters[3].ParticleLifespanVariationMilliseconds = 100f;
            Emitters[3].Burst = false;

            Emitters.Add(new ParticleEmitter(this, Graphics.PreferredBackBufferWidth / 2, Graphics.PreferredBackBufferWidth / 2, 100));
            Emitters[4].MakeParticles(SpriteSheet, 16, 16);
            Emitters[4].ParticleVelocity = new Vector2(0, 0);
            Emitters[4].XVelocityVariationRange = new Vector2(-80f, 80f);
            Emitters[4].YVelocityVariationRange = new Vector2(-80f, 80f);
            Emitters[4].SetTextureCropRectangle(new Rectangle(0, 32, 16, 16));
            Emitters[4].EmitterBox.X = 800f / 2;
            Emitters[4].EmitterBox.Y = 600f / 2;
            Emitters[4].EmitterBox.Resize(1f, 1f);
            Emitters[4].SpawnRate = 1000f;
            Emitters[4].ParticleLifespanMilliseconds = 2000f;
            Emitters[4].ParticleLifespanVariationMilliseconds = 800f;
            Emitters[4].ParticlesPerBurst = 50;
            Emitters[4].Burst = true;

            this.ContentLoaded = true;

        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            ContentLoaded = false;
        }

        public override void Update(GameTime gameTime)
        {
            foreach(ParticleEmitter Emitter in Emitters)
                Emitter.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            foreach (ParticleEmitter Emitter in Emitters)
                Emitter.Draw(gameTime, spriteBatch);
            spriteBatch.End();

        }
    }
}