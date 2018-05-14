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

        Camera2D Camera;
        SpriteFont font;
        Texture2D tilemapTexture;
        Level level;
        List<Sprite> enemies;
        Player player;
        Texture2D backgroundGradientStrip;
        ParticleEmitter emitter1;
        ParticleEmitter playerDeathEmitter;

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
            player.Animations.CurrentFrame = new Frame(0, 176, 16, 32);
            player.Animations.Add("robot-idle", new int[] { 177, 178, 179, 180, 181, 182 }, 6, false, true);
            player.Animations.Add("woman-run", new int[] { 183, 184, 185, 186, 187, 188 }, 12, true);
            player.Body.X = 16 * 3;
            player.Body.Y = 16 * 3;
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
            emitter1.ParseTextureToParticles(tilemapTexture, 96, 176, 16, 32);
            emitter1.ParticleVelocity = new Vector2(0, -0.1f);
            emitter1.XVelocityVariationRange = new Vector2(-30f, 30f);
            emitter1.YVelocityVariationRange = new Vector2(-30f, 0f);
            //emitter1.SetTextureCropRectangle(new Rectangle(48, 96, 16, 16));
            emitter1.EmitterBox.X = 0;
            //emitter1.EmitterBox.Y = level.Height * level.TileHeight;
            emitter1.EmitterBox.Y = 0;
            //emitter1.EmitterBox.Resize(level.Width * level.TileWidth, 1f);
            emitter1.EmitterBox.Resize(50f, 50f);
            emitter1.SpawnRate = 1500f;
            emitter1.ParticleLifespanMilliseconds = 1000f;
            emitter1.ParticleLifespanVariationMilliseconds = 150f;
            //emitter1.Burst = false;
            emitter1.Burst = true;
            emitter1.ParticlesPerBurst = 512;
            emitter1.InitialScale = 1.0f;
            emitter1.FinalScale = 0.1f;
            emitter1.RandomizeParticlePositions = false;

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

            Camera.GetTransform(Graphics.GraphicsDevice);

            #endregion

            if (kState.IsKeyDown(Keys.X))
            {
                player.Animations.Play("robot-idle");
            }
            else if (kState.IsKeyDown(Keys.C))
            {
                player.Animations.Play("woman-run");
            }

            player.UpdateMotion(gameTime, kState, level);

            foreach (Sprite s in enemies)
            {
                s.Update(gameTime);
            }

            emitter1.Update(gameTime);

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(Color.Black);

            /*
             * Background
             */
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            spriteBatch.Draw(backgroundGradientStrip, new Rectangle(0, 0, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight), Color.White);
            spriteBatch.End();

            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Camera.Transform);

            emitter1.Draw(gameTime, spriteBatch);

            //level.Draw(gameTime, spriteBatch);

            player.Draw(gameTime, spriteBatch);

            foreach (Sprite s in enemies)
            {
                s.Draw(gameTime, spriteBatch);
            }

            spriteBatch.End();

            spriteBatch.Begin();

            spriteBatch.DrawString(font, "'X' and 'C' to switch between animations", new Vector2(8, 16), Color.LightGreen);

            if (player.Animations.CurrentAnimation != null)
            {

                spriteBatch.DrawString(font, player.Animations.CurrentAnimation.GetDebugInfo(), new Vector2(8, 48), Color.Red);

            }
            spriteBatch.End();

        }
    }
}
