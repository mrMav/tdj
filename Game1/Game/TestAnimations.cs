using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine;
using Engine.Tiled;
using Engine.Animations;
using Engine.Particles;
using TDJGame.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TDJGame
{
    public class TestAnimations : GameState
    {

        #region [Properties]

        FrameCounter frameCounter = new FrameCounter();
        Camera2D Camera;
        SpriteFont font;
        Texture2D tilemapTexture;
        Level level;
        List<Sprite> enemies;
        Player player;
        Texture2D backgroundGradientStrip;
        ParticleEmitter emitter1;
        ParticleEmitter playerDeathEmitter;
        double updateTime;

        float startedShakeMilliseconds = 0;
        float currentShakeMilliseconds = 0;

        #endregion

        public TestAnimations(string key, GraphicsDeviceManager graphics)
        {
            Key = key;
            Graphics = graphics;
        }

        public override void Initialize()
        {
            base.Initialize();

            enemies = new List<Sprite>();
            
        }

        public override void LoadContent()
        {
            base.LoadContent();

            Camera = new Camera2D(Vector2.Zero);
            Camera.Zoom = 2.45f;

            font = content.Load<SpriteFont>("Font");
            tilemapTexture = this.content.Load<Texture2D>("spritesheet-jn");

            /*
             * Level init
             */
            XMLLevelLoader XMLloader = new XMLLevelLoader();
            level = XMLloader.LoadLevel(this, @"Content\tiledObjectTest.tmx", tilemapTexture);
            level.SetCollisionTiles(new int[] { 1, 2, 17, 18, 33, 34 });
            
            /*
             * Enemies init
             */
            foreach (TiledObject obj in level.Objects)
            {

                if (obj.Name.ToLower() == "jellyfish")
                {
                    Vector2 center = new Vector2(obj.X + obj.Width / 2, obj.Y + obj.Height / 2);
                    Vector2 radius = new Vector2(obj.Width / 2, obj.Height / 2);

                    float speed = float.Parse(obj.GetProperty("speed"));

                    JellyFish j = new JellyFish(this, tilemapTexture, Vector2.Zero, 16, 32, center, radius, speed);
                    j.Animations.CurrentFrame = new Frame(48, 112, 16, 32, 0);

                    enemies.Add(j);

                    Console.WriteLine("added jelly");


                }
                else if (obj.Name.ToLower() == "pufferfish")
                {
                    Vector2 position = new Vector2(obj.X, obj.Y);

                    float speed = float.Parse(obj.GetProperty("speed"));

                    PufferFish p = new PufferFish(this, tilemapTexture, position, 32, 32, obj.Width, speed);
                    p.Animations.CurrentFrame = new Frame(0, 112, 32, 32, 0);

                    enemies.Add(p);

                    Console.WriteLine("added puffer");

                }

            }

            /*
             *Player init
            */
            
            player = new Player(this, tilemapTexture, new Vector2(0, 0), 16, 32, true);
            player.Animations.CurrentFrame = new Frame(96, 176, 16, 32);
            player.Animations.Add("robot-idle", new int[] { 177, 178, 179, 180, 181, 182 }, 6, false, true);
            player.Animations.Add("woman-run", new int[] { 183, 184, 185, 186, 187, 188 }, 12, true);
            player.Body.X = 0;
            player.Body.Y = 0;
            player.Body.MaxVelocity = 3f;
            player.Body.Drag.X = 0.6f;
            player.Body.Enabled = true;
            player.Body.Tag = "player";
            player.Body.SetSize(16, 32, 0, 0);


            /*
             * Build Background Gradient
             */
            backgroundGradientStrip = new Texture2D(Graphics.GraphicsDevice, 1, level.Height * level.TileHeight);

            Color startColor = new Color(57, 92, 181);
            Color finishColor = new Color(17, 43, 104);
            Color currentColor;
            for (int i = 0; i < backgroundGradientStrip.Height; i++)
            {
                float ratio = Math2.Map(i, 0f, backgroundGradientStrip.Height, 0f, 1.0f);

                currentColor = Color.Lerp(startColor, finishColor, ratio);
                DrawMe.Pixel(backgroundGradientStrip, 0, i, currentColor);
            }

            emitter1 = new ParticleEmitter(this, Graphics.PreferredBackBufferWidth / 2, Graphics.PreferredBackBufferWidth / 2, 512);
            //emitter1.MakeParticles(tilemapTexture, 16, 16);
            //emitter1.SetTextureCropRectangle(new Rectangle(48, 96, 16, 16));
            emitter1.ParseTextureToParticles(tilemapTexture, 0, 0, 16, 32, 4, 4);
            emitter1.ParticleVelocity = new Vector2(0, 0f);
            emitter1.XVelocityVariationRange = new Vector2(-10f, 10f);
            emitter1.YVelocityVariationRange = new Vector2(-30f, 0f);
            emitter1.EmitterBox.X = 0;
            //emitter1.EmitterBox.Y = level.Height * level.TileHeight;
            emitter1.EmitterBox.Y = 0;
            //emitter1.EmitterBox.Resize(level.Width * level.TileWidth, 1f);
            emitter1.EmitterBox.Resize(1f, 1f);
            emitter1.SpawnRate = 0f;
            emitter1.ParticleLifespanMilliseconds = 5000f;
            emitter1.ParticleLifespanVariationMilliseconds = 500f;
            emitter1.Burst = false;
            //emitter1.Burst = true;
            emitter1.ParticlesPerBurst = 512;
            emitter1.InitialScale = 1.0f;
            emitter1.FinalScale = 0.1f;
            //emitter1.RandomizeParticlePositions = false;

            emitter1.SetAcceleration(0.0f, -0.01f);

            //emitter1.ForEachParticle(TintBlue);

            //playerDeathEmitter = new ParticleEmitter(this, 0, 0);

            ContentLoaded = true;

        }

        public int TintBlue(Particle p)
        {
            p.Tint = new Color(20, 10, 90, 50);
            return 0;
        }

        public override void Update(GameTime gameTime)
        {
            updateTime = 0;

            KeyboardState kState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();

            #region [Camera Update]

            if (kState.IsKeyDown(Keys.Q))
            {
                Camera.Zoom *= 1.1f;
            }
            else if (kState.IsKeyDown(Keys.E))
            {
                Camera.Zoom *= 0.9f;
            }

            //Camera.Position.X = player.Body.X + player.Body.Bounds.HalfWidth;
            //Camera.Position.Y = player.Body.Y + player.Body.Bounds.HalfHeight;

            if(mState.LeftButton == ButtonState.Pressed)
            {
                Vector2 newpos = Camera.GetScreenToWorldPosition(new Vector2(mState.X, mState.Y));

                Vector2 dist = newpos - Camera.Position;

                Camera.Position.X += dist.X * 0.2f;
                Camera.Position.Y += dist.Y * 0.2f;
            }

            if (kState.IsKeyDown(Keys.S))
            {
                if (!Camera.Shaking)
                    Camera.ActivateShake(gameTime);
            }

            Camera.Update(gameTime, Graphics.GraphicsDevice);

            #endregion

            if (kState.IsKeyDown(Keys.X))
            {
                player.Animations.Play("robot-idle");
            }
            else if (kState.IsKeyDown(Keys.C))
            {
                player.Animations.Play("woman-run");
            }

            if (kState.IsKeyDown(Keys.F))
            {
                emitter1.Activated = true;
            }

            player.UpdateMotion(gameTime, kState, level);

            foreach (Sprite s in enemies)
            {
                s.Update(gameTime);
            }

            #region [Shake]

            //if (kState.IsKeyDown(Keys.S))
            //{
            //    startedShakeMilliseconds = (float)gameTime.TotalGameTime.TotalMilliseconds;
            //}

            //player.Body.Y += UpdateShake(gameTime);

            #endregion


            //player.Body.Y = y;

            //emitter1.ParticleVelocity.X = (float)Math.Cos(gameTime.TotalGameTime.TotalMilliseconds);
            //emitter1.ParticleVelocity.Y = (float)Math.Sin(gameTime.TotalGameTime.TotalMilliseconds);
            //emitter1.ParticleVelocity.Y = -0.2f;

            emitter1.Update(gameTime);

            updateTime = gameTime.ElapsedGameTime.TotalMilliseconds;

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            frameCounter.Update(deltaTime);

            graphicsDevice.Clear(Color.Black);

            /*
             * Background
             */
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            spriteBatch.Draw(backgroundGradientStrip, new Rectangle(0, 0, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight), Color.White);
            spriteBatch.End();

            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Camera.Transform);


            level.Draw(gameTime, spriteBatch);

            player.Draw(gameTime, spriteBatch);
            emitter1.Draw(gameTime, spriteBatch);

            foreach (Sprite s in enemies)
            {
                s.Draw(gameTime, spriteBatch);
            }

            spriteBatch.End();

            spriteBatch.Begin();

            //spriteBatch.DrawString(font, "'X' and 'C' to switch between animations", new Vector2(8, 16), Color.LightGreen);

            spriteBatch.DrawString(font, $"{(frameCounter.CurrentFramesPerSecond)}", Vector2.Zero, Color.LightGreen);
            spriteBatch.DrawString(font, Camera.GetDebugString(), new Vector2(0, 16), Color.Red);

            if (player.Animations.CurrentAnimation != null)
            {

                spriteBatch.DrawString(font, player.Animations.CurrentAnimation.GetDebugInfo(), new Vector2(8, 48), Color.Red);

            }
            spriteBatch.End();

        }

        public float UpdateShake(GameTime gameTime)
        {

            // https://www.graphpad.com/guides/prism/7/curve-fitting/reg_damped_sine_wave.htm?toc=0&printWindow

            //currentShakeMilliseconds -= startedShakeMilliseconds; 

            float gameMs = (float)gameTime.TotalGameTime.TotalMilliseconds - startedShakeMilliseconds;
            float initialAmplitude = 3f;
            float decayConstant = 0.01f;
            float angularFrequency = 150f;
            float y = initialAmplitude *
                (float)Math.Exp(-decayConstant * gameMs) *
                (float)Math.Sin((2 * Math.PI * gameMs / angularFrequency) + 0);

            // Amplitude is the height of top of the waves, in Y units.
            // Wavelength is the time it takes for a complete cycle, in units of X
            // Frequency is the number of cycles per time unit.It is calculated as the reciprocal of wavelength, and is expressed in the inverse of the time units of X.
            // PhaseShift in radians.A phaseshift of 0 sets Y equal to 0 at X = 0.
            // K is the decay constant, in the reciprocal of the time units of the X axis.
            // HalfLlife is the time it takes for the maximum amplitude to decrease by a factor of 2.It is computed as 0.693 / K.

            Console.WriteLine($"{gameMs}, {y}");

            return y;

        }

    }
}
