using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine;
using Engine.Physics;
using Engine.Tiled;
using TDJGame.Utils;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using Engine.Animations;
using Engine.Particles;
using Microsoft.Xna.Framework.Audio;

namespace TDJGame
{
    public class NewKarmaScreen : GameState
    {

        #region [Properties]

        Camera2D camera;
        FrameCounter frameCounter;
        SpriteFont font;
        Texture2D tilemapTexture;
        Level level;
        Texture2D backgroundWaterGradientStrip;
        Texture2D backgroundSkyGradientStrip;
        List<Sprite> enemies;
        List<Tile> topWaterTiles;
        List<ParticleEmitter> backgroundParticles;
        List<Sprite> goldenFishs;
        List<Trigger> triggers;
        List<SpeedBox> speedBoxes;

        Trigger diveButton;

        List<Sprite> topTiles;
        List<Sprite> bottomTiles;
        List<Sprite> boxes;


        //Stopwatch stopwatch;

        //long tickCount = 0;
        //double sumOfMilliseconds = 0;
        //double averageMilliseconds = 0;
        //double maxMills = double.MinValue;
        //double minMills = double.MaxValue;

        #endregion

        public NewKarmaScreen(string key, GraphicsDeviceManager graphics)
        {
            Key = key;
            Graphics = graphics;
        }

        public override void Initialize()
        {
            base.Initialize();

            frameCounter = new FrameCounter();
            enemies = new List<Sprite>();
            backgroundParticles = new List<ParticleEmitter>();
            goldenFishs = new List<Sprite>();
            triggers = new List<Trigger>();
            speedBoxes = new List<SpeedBox>();
            SFX = new Dictionary<string, SoundEffect>();

            topTiles = new List<Sprite>();
            bottomTiles = new List<Sprite>();
            boxes = new List<Sprite>();


            //stopwatch = new Stopwatch();

        }

        public override void LoadContent()
        {
            base.LoadContent();

            camera = new Camera2D(new Vector2(12 * 16, 12 * 16));
            camera.Zoom = 2.45f;

            font = content.Load<SpriteFont>("Font");
            tilemapTexture = this.content.Load<Texture2D>("SpriteSheet");

            MediaPlayer.Volume = 0.3f;
            MediaPlayer.Play(Globals.JingleSong);
            MediaPlayer.IsRepeating = false;

            
            for(int i = 0; i < Graphics.PreferredBackBufferWidth; i += 16)
            {

                Sprite s = new Sprite(this, tilemapTexture, new Vector2(i, 0), 16, 16);
                s.Animations.CurrentFrame = new Frame(16, 0, 16, 16);

                topTiles.Add(s);

                s = new Sprite(this, tilemapTexture, new Vector2(i, Graphics.PreferredBackBufferHeight / camera.Zoom - 32 - 4), 16, 16);
                s.Animations.CurrentFrame = new Frame(16, 0, 16, 16);

                bottomTiles.Add(s);
            }


            // build boxes
            Sprite box = new Sprite(this, tilemapTexture, new Vector2(16 * 2, 16 * 5), 96+16, 32);
            box.Animations.CurrentFrame = new Frame(160, 288, 96 + 16, 32);
            boxes.Add(box);

            box = new Sprite(this, tilemapTexture, new Vector2(16 * 2, 16 * 8), 96 + 16, 32);
            box.Animations.CurrentFrame = new Frame(160, 288, 96 + 16, 32);
            boxes.Add(box);

            box = new Sprite(this, tilemapTexture, new Vector2(16 * 2, 16 * 11), 96 + 16, 32);
            box.Animations.CurrentFrame = new Frame(160, 288, 96 + 16, 32);
            boxes.Add(box);

            ///*
            // * Level init
            // */
            //XMLLevelLoader XMLloader = new XMLLevelLoader();

            //// se o load do mapa falhar, well shit. vai para o menu.
            //try
            //{
            //    level = XMLloader.LoadLevel(this, @"Content\KarmaScreen.tmx", tilemapTexture);

            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("Error Loading Map. Error message: " + e.Message);

            //    StateManager.Instance.StartGameState("MenuState");

            //}

            /*
             * Build Background Gradient
             */
            backgroundWaterGradientStrip = new Texture2D(Graphics.GraphicsDevice, 1, Graphics.PreferredBackBufferHeight / 2);

            Color startColor = new Color(57, 92, 181);
            Color finishColor = new Color(17, 43, 104);
            Color currentColor;
            for (int i = 0; i < backgroundWaterGradientStrip.Height; i++)
            {
                float ratio = Math2.Map(i, 0f, backgroundWaterGradientStrip.Height, 0f, 1.0f);

                currentColor = Color.Lerp(startColor, finishColor, ratio);
                DrawMe.Pixel(backgroundWaterGradientStrip, 0, i, currentColor);
            }

            /*
             * Build Background Gradient
             */
            backgroundSkyGradientStrip = new Texture2D(Graphics.GraphicsDevice, 1, Graphics.PreferredBackBufferHeight / 2);

            startColor = new Color(61, 28, 111);
            finishColor = new Color(158, 98, 123);
            for (int i = 0; i < backgroundSkyGradientStrip.Height; i++)
            {
                float ratio = Math2.Map(i, 0f, backgroundSkyGradientStrip.Height, 0f, 1.0f);

                currentColor = Color.Lerp(startColor, finishColor, ratio);
                DrawMe.Pixel(backgroundSkyGradientStrip, 0, i, currentColor);
            }

            ContentLoaded = true;
        }

        public override void UnloadContent()
        {


            camera = null;
            frameCounter = null;
            font = null;
            tilemapTexture = null;
            level = null;
            backgroundWaterGradientStrip = null;
            enemies = null;
            SFX = null;

            //MediaPlayer.Stop();
            //song.Dispose();
            //song = null;

            //Console.WriteLine(song);
            //Console.WriteLine("HHERERERasdasd");

            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {

            /*
             * Input State refresh
             */
            #region [System Status Refresh]

            KeyboardState kState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();

            if (kState.IsKeyDown(Keys.Space))
            {
                if (Globals.CurrentLevel == "featureTestMap")
                {

                    StateManager.Instance.StartGameState("MenuState");

                }
                else
                {

                StateManager.Instance.StartGameState("NewTestState");                    

                }

                return;
            }

            #endregion


            camera.Position = new Vector2(0, 8*16);

            // clamp camera position
            float halfscreenwidth = Graphics.PreferredBackBufferWidth / 2f;
            float halfscreenheight = Graphics.PreferredBackBufferHeight / 2f;
            camera.Position.X = MathHelper.Clamp(camera.Position.X, halfscreenwidth / camera.Zoom, Graphics.PreferredBackBufferWidth - (halfscreenwidth / camera.Zoom));
            camera.Position.Y = MathHelper.Clamp(camera.Position.Y, -1000f, Graphics.PreferredBackBufferHeight - (halfscreenheight / camera.Zoom));

            camera.Update(gameTime, Graphics.GraphicsDevice);



        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            frameCounter.Update(deltaTime);

            graphicsDevice.Clear(Color.CornflowerBlue);

            #region [Layer 0 - Behind World Map - Static]

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            Color depthbasedtint = Color.White;
            float depthToPitchBlack = 32 * 16;
            float levelDepth = Graphics.PreferredBackBufferHeight;
            float multiplyer = (1f - camera.Position.Y / (depthToPitchBlack < levelDepth ? levelDepth : depthToPitchBlack));

            if (multiplyer >= 1f)
                multiplyer = 1f;

            byte colorvalue = (byte)(255f * multiplyer);

            depthbasedtint.R = colorvalue;
            depthbasedtint.G = colorvalue;
            depthbasedtint.B = colorvalue;
            spriteBatch.Draw(backgroundWaterGradientStrip, new Rectangle(0, 0, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight), depthbasedtint);

            Vector2 skypos = camera.GetWorldToScreenPosition(new Vector2(0, 3f));
            spriteBatch.Draw(backgroundSkyGradientStrip, new Rectangle(0, (int)skypos.Y - Graphics.PreferredBackBufferHeight, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight), Color.White);

            spriteBatch.End();

            #endregion

            #region [Layer 1 - World Map - Camera]

            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.Transform);

            // beackground tiles
            //level.Layers[0].Draw(gameTime, spriteBatch);                      

            // world tiles
            //level.Layers[1].Draw(gameTime, spriteBatch);

            foreach(Sprite s in topTiles)
            {
                s.Draw(gameTime, spriteBatch);
            }
            foreach (Sprite s in bottomTiles)
            {
                s.Draw(gameTime, spriteBatch);
            }
            foreach (Sprite s in boxes)
            {
                s.Draw(gameTime, spriteBatch);
            }

            spriteBatch.Draw(tilemapTexture, new Vector2(16 * 7.5f, 16 * 8.5f), new Rectangle(0, 256, 16, 16), Color.White);
            spriteBatch.Draw(tilemapTexture, new Vector2(Graphics.PreferredBackBufferWidth / camera.Zoom / 2 - 16, bottomTiles[0].Body.Y - 32), new Rectangle(16, 64, 32, 32), Color.White);

            spriteBatch.Draw(tilemapTexture, new Vector2(Graphics.PreferredBackBufferWidth / camera.Zoom / 2 + 32 , bottomTiles[0].Body.Y - 16 - 8), new Rectangle(0, 455, 32, 18), Color.White);

            double time = (Karma.totalTime - Karma.startTime);
            spriteBatch.DrawString(font, "Time: " + (int)(time / 60) + ":" + (int)(time % 60), new Vector2(16 * 2 + 4, 16 * 5 + 4), Color.White);
            //spriteBatch.DrawString(font, "Time: " + (Karma.totalTime - Karma.startTime), new Vector2(16 * 2 + 4, 16 * 5 + 4), Color.White);
            spriteBatch.DrawString(font, Karma.playerCollect + "/" + Karma.maxCollectables, new Vector2(16 * 2 + 16, 16 * 8 + 4), Color.White);
            spriteBatch.DrawString(font, "Rank: " + Karma.DetermineRank(), new Vector2(16 * 2 + 16, 16 * 11 + 4), Color.White);

            //spriteBatch.DrawString(font, "Press space to exit", new Vector2(Graphics.PreferredBackBufferWidth / camera.Zoom / 2, 10), Color.White);

            spriteBatch.End();

            #endregion

            #region [Layer 2 - UI - Static]

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            //spriteBatch.DrawString(font, "RANK:\n" + Karma.DetermineRank() + "\n\nShotsFired: " + Karma.playerShotsFired + "\nKarma: " + Karma.karma + "\nDamage Dealt: " + Karma.playerTotalDamage + "\nCollected: " + Karma.playerCollect, new Vector2(10, 10), Color.Black);
            //spriteBatch.DrawString(font, "Press space to exit", new Vector2(256, 10), Color.Black);

            spriteBatch.End();

            #endregion

        }

        public int ChangeSpriteTintBlue(Sprite s)
        {
            s.Tint = new Color(255, 255, 255, 255);
            return 0;
        }

        public int KillOutOfBoundsParticle(Particle p)
        {

            if (p.Body.Y <= 0f)
            {
                p.Kill();
            }

            return 0;

        }
    }
}