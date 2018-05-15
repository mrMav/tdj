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
    public class TestCamera : GameState
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

        float duration = 350f;
        float amplitude = 8f;
        float frequency = 0.03f;
        float zoommultiplyer = 0.01f;
        bool zoom = false;

        #endregion

        public TestCamera(string key, GraphicsDeviceManager graphics)
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
            player.Body.X = 15*16;
            player.Body.Y = 8*16;
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

            #region [Other Camera Controls]

            //if (kState.IsKeyDown(Keys.Q))
            //{
            //    Camera.Zoom *= 1.1f;
            //}
            //else if (kState.IsKeyDown(Keys.E))
            //{
            //    Camera.Zoom *= 0.9f;
            //}

            //if (mState.LeftButton == ButtonState.Pressed)
            //{
            //    Vector2 newpos = Camera.GetScreenToWorldPosition(new Vector2(mState.X, mState.Y));

            //    Vector2 dist = newpos - Camera.Position;

            //    Camera.Position.X += dist.X * 0.2f;
            //    Camera.Position.Y += dist.Y * 0.2f;
            //}

            if (kState.IsKeyDown(Keys.S))
            {
                if (!Camera.Shaking)
                    Camera.ActivateShake(gameTime, duration, amplitude, frequency, zoom, zoommultiplyer);
            }

            #endregion

            float ammount;

            // update duration
            if (kState.IsKeyDown(Keys.D1))
            {
                ammount = 10f;

                if (kState.IsKeyDown(Keys.Up))
                    duration += ammount;
                else if (kState.IsKeyDown(Keys.Down))
                    duration -= ammount;
                                
            }

            // update amplitude
            if (kState.IsKeyDown(Keys.D2))
            {
                ammount = 1f;

                if (kState.IsKeyDown(Keys.Up))
                    amplitude += ammount;
                else if (kState.IsKeyDown(Keys.Down))
                    amplitude -= ammount;
            }

            // update frequency
            if (kState.IsKeyDown(Keys.D3))
            {
                ammount = 0.01f;

                if (kState.IsKeyDown(Keys.Up))
                    frequency += ammount;
                else if (kState.IsKeyDown(Keys.Down))
                    frequency -= ammount;
            }

            // update zoom?
            if (kState.IsKeyDown(Keys.D4))
            {
                if (kState.IsKeyDown(Keys.Up))
                    zoom = true;
                else if (kState.IsKeyDown(Keys.Down))
                    zoom = false;
            }

            // update zoom multi
            if (kState.IsKeyDown(Keys.D5))
            {
                ammount = 0.01f;
                if (kState.IsKeyDown(Keys.Up))
                    zoommultiplyer += ammount;
                else if (kState.IsKeyDown(Keys.Down))
                    zoommultiplyer -= ammount;
            }

            // update rotation?
            //if (kState.IsKeyDown(Keys.D5))
            //{
            //    ammount = 0.01f;

            //    if (kState.IsKeyDown(Keys.Up))
            //        zoom = true;
            //    else if (kState.IsKeyDown(Keys.Down))
            //        zoom = false;
            //}

            player.UpdateMotion(gameTime, kState, level);

            foreach (Sprite s in enemies)
            {
                s.Update(gameTime);
            }


            Camera.Position.X = player.Body.X + player.Body.Bounds.HalfWidth;
            Camera.Position.Y = player.Body.Y + player.Body.Bounds.HalfHeight;

            Camera.Update(gameTime, Graphics.GraphicsDevice);

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            frameCounter.Update(deltaTime);

            graphicsDevice.Clear(Color.Black);

            #region [Background]

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            spriteBatch.Draw(backgroundGradientStrip, new Rectangle(0, 0, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight), Color.White);
            spriteBatch.End();

            #endregion

            #region [World]

            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Camera.Transform);

            level.Draw(gameTime, spriteBatch);

            player.Draw(gameTime, spriteBatch);

            foreach (Sprite s in enemies)
            {
                s.Draw(gameTime, spriteBatch);
            }

            spriteBatch.End();

            #endregion

            #region [UI]

            spriteBatch.Begin();
            
            spriteBatch.DrawString(font, "Hold 1-4 and press up and down to update shake settings:\n", new Vector2(8, 16), Color.LightGreen);
            spriteBatch.DrawString(font, $"1:duration (ms): {duration}\n2:amplitude (px): {amplitude}\n3:frequency: {frequency}\n4:affect zoom: {zoom}\n5:zoom multiplyer: {zoommultiplyer}", new Vector2(8, 32+8), Color.LightGreen);

            spriteBatch.End();
            #endregion

        }

    }
}
