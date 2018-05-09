using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Engine;
using System;
using Microsoft.Xna.Framework.Media;
using Engine.Particles;
using System.Collections.Generic;
using TDJGame.Utils;

namespace TDJGame
{
    public class TestParticleEmitter : GameState
    {

        #region [Properties]

        FrameCounter FPS;
        Texture2D SpriteSheet;
        SpriteFont Font;
        Camera2D Camera;
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

            FPS = new FrameCounter();
            SpriteSheet = content.Load<Texture2D>("SpriteSheetDraft");
            Camera = new Camera2D(Vector2.Zero);
            Font = content.Load<SpriteFont>("Font");

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

            Emitters.Add(new ParticleEmitter(this, Graphics.PreferredBackBufferWidth / 2, Graphics.PreferredBackBufferWidth / 2, 500));
            Emitters[5].MakeParticles(SpriteSheet, 16, 16);
            Emitters[5].ParticleVelocity = new Vector2(0, 0);
            float variation = 10f;
            Emitters[5].XVelocityVariationRange = new Vector2(-variation, variation);
            Emitters[5].YVelocityVariationRange = new Vector2(-variation, variation);
            Emitters[5].SetTextureCropRectangle(new Rectangle(0, 32, 16, 16));
            Emitters[5].SpawnRate = 0f;
            Emitters[5].ParticleLifespanMilliseconds = 5000f;
            Emitters[5].ParticleLifespanVariationMilliseconds = 1000f;
            Emitters[5].InitialScale = 0.5f;
            Emitters[5].FinalScale = 3.0f;

            Emitters[5].ForEachParticle(ChangeSpriteTintRed);

            Emitters.Add(new ParticleEmitter(this, Graphics.PreferredBackBufferWidth / 2, Graphics.PreferredBackBufferWidth / 2, 100));
            Emitters[6].MakeParticles(SpriteSheet, 16, 16);
            Emitters[6].ParticleVelocity = new Vector2(0, 0);
            variation = 1000f;
            Emitters[6].XVelocityVariationRange = new Vector2(-variation, variation);
            Emitters[6].YVelocityVariationRange = new Vector2(-variation, variation);
            Emitters[6].SetTextureCropRectangle(new Rectangle(0, 32, 16, 16));
            Emitters[6].EmitterBox.Resize(1f, 1f);
            Emitters[6].SpawnRate = 1000f;
            Emitters[6].ParticleLifespanMilliseconds = 2000f;
            Emitters[6].ParticleLifespanVariationMilliseconds = 800f;
            Emitters[6].ParticlesPerBurst = 50;
            Emitters[6].Burst = true;
            Emitters[6].InitialScale = 0.5f;
            Emitters[6].FinalScale = 3.0f;

            Emitters[6].ForEachParticle(ChangeSpriteTintGreen);

            this.ContentLoaded = true;

        }

        public int ChangeSpriteTintRed(Sprite s)
        {
            s.Tint = new Color(255, 0, 0, 255);
            return 0;
        }
        public int ChangeSpriteTintGreen(Sprite s)
        {
            s.Tint = new Color(0, 255, 0, 255);
            return 0;
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            ContentLoaded = false;
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState kState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();

            #region [Emitters Update]

            if(mState.LeftButton == ButtonState.Pressed)
            {

                Vector2 cursorScreenToWorldPosition = Camera.GetScreenToWorldPosition(new Vector2(mState.X, mState.Y));

                Emitters[5].EmitterBox.X = cursorScreenToWorldPosition.X;
                Emitters[5].EmitterBox.Y = cursorScreenToWorldPosition.Y;

                Emitters[5].Activated = true;

            } else
            {
                Emitters[5].Activated = false;
            }

            if (mState.RightButton == ButtonState.Pressed)
            {

                Vector2 cursorScreenToWorldPosition = Camera.GetScreenToWorldPosition(new Vector2(mState.X, mState.Y));

                Emitters[6].EmitterBox.X = cursorScreenToWorldPosition.X;
                Emitters[6].EmitterBox.Y = cursorScreenToWorldPosition.Y;

                Emitters[6].Activated = true;

            }
            else
            {
            Emitters[6].Activated = false;
            }

            foreach (ParticleEmitter Emitter in Emitters)
            {
                Emitter.Update(gameTime);
            }
            
            #endregion

            #region [Camera Update]

            if (kState.IsKeyDown(Keys.Q))
            {
                Camera.Zoom += 0.1f;
            } else if(kState.IsKeyDown(Keys.E))
            {
                Camera.Zoom -= 0.1f;
            }

            float ammount = 4f;
            if(kState.IsKeyDown(Keys.W))
            {
                Camera.Position.Y -= ammount;
            } else if (kState.IsKeyDown(Keys.S))
            {
                Camera.Position.Y += ammount;
            }
            if (kState.IsKeyDown(Keys.A))
            {
                Camera.Position.X -= ammount;
            }
            else if (kState.IsKeyDown(Keys.D))
            {
                Camera.Position.X += ammount;
            }

            Camera.GetTransform(Graphics.GraphicsDevice);

            #endregion

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            FPS.Update(deltaTime);
            
            graphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Camera.Transform);
            foreach (ParticleEmitter Emitter in Emitters)
                Emitter.Draw(gameTime, spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.DrawString(Font, $"{(int)FPS.CurrentFramesPerSecond}", new Vector2(8, 8), new Color(0, 255, 0));
            spriteBatch.End();

        }
    }
}