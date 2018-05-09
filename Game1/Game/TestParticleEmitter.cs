using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Engine;
using System;
using Microsoft.Xna.Framework.Media;
using Engine.Particles;

namespace TDJGame
{
    public class TestParticleEmitter : GameState
    {

        #region [Properties]

        Texture2D SpriteSheet;

        ParticleEmitter Emitter;

        #endregion

        public TestParticleEmitter(string key, GraphicsDeviceManager graphics)
        {
            this.Key = key;
            this.Graphics = graphics;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            SpriteSheet = content.Load<Texture2D>("SpriteSheetDraft");

            Emitter = new ParticleEmitter(this, Graphics.PreferredBackBufferWidth / 2, Graphics.PreferredBackBufferWidth / 2);
            Emitter.MakeParticles(SpriteSheet, 16, 16);
            Emitter.ParticleVelocity = new Vector2(2, -2);
            Emitter.XVelocityVariationRange = new Vector2(-5f, 5f);
            Emitter.YVelocityVariationRange = new Vector2(-5f, 5f);
            Emitter.SetTextureCropRectangle(new Rectangle(0, 32, 16, 16));
            Emitter.EmitterBox.Resize(2f, 2f);
            Emitter.SpawnRate = 50;

            this.ContentLoaded = true;

        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            ContentLoaded = false;
        }

        public override void Update(GameTime gameTime)
        {
            Emitter.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            Emitter.Draw(gameTime, spriteBatch);
            spriteBatch.End();

        }
    }
}