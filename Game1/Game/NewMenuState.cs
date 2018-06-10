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
    public class NewMenuState : GameState
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


        //Stopwatch stopwatch;

        //long tickCount = 0;
        //double sumOfMilliseconds = 0;
        //double averageMilliseconds = 0;
        //double maxMills = double.MinValue;
        //double minMills = double.MaxValue;

        #endregion

        public NewMenuState(string key, GraphicsDeviceManager graphics)
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

            Karma.Reset();

            //stopwatch = new Stopwatch();

        }

        public override void LoadContent()
        {
            base.LoadContent();

            camera = new Camera2D(Vector2.Zero);
            camera.Zoom = 1.5f;

            font = content.Load<SpriteFont>("Font");
            tilemapTexture = this.content.Load<Texture2D>("SpriteSheet");

            MediaPlayer.Volume = 0.3f;
            MediaPlayer.Play(Globals.MenuSong);
            MediaPlayer.IsRepeating = true;

            diveButton = new Trigger(Graphics.PreferredBackBufferWidth / 2 - 128 / 2, Graphics.PreferredBackBufferHeight - 48 - 32, 128f, 48f, "dive");  //Graphics.PreferredBackBufferWidth / 2 - 128 / 2, Graphics.PreferredBackBufferHeight - 48 - 32, 128, 48

            /*
             * Level init
             */
            XMLLevelLoader XMLloader = new XMLLevelLoader();

            // se o load do mapa falhar, well shit. vai para o menu.
            try
            {
                level = XMLloader.LoadLevel(this, @"Content\Menu.tmx", tilemapTexture);

            }
            catch (Exception e)
            {
                Console.WriteLine("Error Loading Map. Error message: " + e.Message);

                StateManager.Instance.StartGameState("MenuState");

            }

            level.SetCollisionTiles(new int[] { 2, 33, 34, 35, 47, 66, 15, 79 });

            /*
             * parse objects
             */
            foreach (TiledObject obj in level.Objects)
            {

                Console.WriteLine("parsing " + obj.Name);

                if (obj.Name.ToLower() == "jellyfish")
                {
                    Vector2 center = new Vector2(obj.X + obj.Width / 2, obj.Y + obj.Height / 2);
                    Vector2 radius = new Vector2(obj.Width / 2, obj.Height / 2);

                    float speed = float.Parse(obj.GetProperty("speed"));

                    JellyFish j = new JellyFish(this, tilemapTexture, Vector2.Zero, 16, 32, center, radius, speed);

                    Karma.maxKarma += j.Health;

                    // make it start on the right side of its path
                    if (obj.GetProperty("start_rotation") == "right")
                    {
                        j.FacingDirection = 1;
                    }
                    else
                    {
                        j.FacingDirection = -1;
                    }

                    Console.WriteLine(obj.GetProperty("start_rotation"));

                    enemies.Add(j);

                }
                else if (obj.Name.ToLower() == "pufferfish")
                {
                    Vector2 position = new Vector2(obj.X, obj.Y);

                    float speed = float.Parse(obj.GetProperty("speed"));

                    PufferFish p = new PufferFish(this, tilemapTexture, position, 32, 32, obj.Width, speed);
              
                    for (int i = 0; i < p.Bullets.Count; i++)
                    {
                        p.Bullets[i].Alive = true;
                        p.Bullets[i].Visible = false;
                    }

                    Karma.maxKarma += p.Health;

                    // make it start on the right side of its path
                    if (obj.GetProperty("start_side") == "right")
                    {
                        p.Body.X = obj.X + obj.Width;
                        p.CurrentDistance = obj.Width - 33;
                    }

                    enemies.Add(p);
                }
                else if (obj.Name.ToLower() == "turtlex")
                {
                    Vector2 position = new Vector2(obj.X, obj.Y);

                    float speed = float.Parse(obj.GetProperty("speed"));

                    TurtleX p = new TurtleX(this, tilemapTexture, position, 32, 32, 64, obj.Width, speed);
                    p.Animations.CurrentFrame = new Frame(96, 112, 32, 32);

                    Karma.maxKarma += p.Health;

                    // make it start on the right side of its path
                    if (obj.GetProperty("start_side") == "right")
                    {
                        p.Body.X = obj.X + obj.Width;
                        p.CurrentDistance = obj.Width - 33;
                    }

                    enemies.Add(p);

                }
                else if (obj.Name.ToLower() == "goldfish")
                {
                    goldenFishs.Add(new GoldFish(this, tilemapTexture, new Vector2(obj.X, obj.Y), 16, 16));
                }
                else if (obj.Name.ToLower() == "particles")
                {
                    if (obj.GetProperty("type") == "dark_ambient")
                    {

                        ParticleEmitter particleEmitter = new ParticleEmitter(this, obj.X, obj.Y, 256);
                        particleEmitter.EmitterBox.Resize(obj.Width, obj.Height);

                        particleEmitter.MakeRandomParticles(tilemapTexture, new Rectangle[]
                        {
                            new Rectangle(128, 257, 3, 3),
                            new Rectangle(132, 257, 3, 3),
                            new Rectangle(136, 257, 3, 3)
                        });

                        particleEmitter.ParticleVelocity = new Vector2(0, 0);
                        particleEmitter.SetAcceleration(0, 0);
                        particleEmitter.XVelocityVariationRange = new Vector2(-20f, 20f);
                        particleEmitter.YVelocityVariationRange = new Vector2(-20f, 20f);
                        particleEmitter.SpawnRate = 100f;
                        particleEmitter.ParticleLifespanMilliseconds = 5000f;
                        particleEmitter.ParticleLifespanVariationMilliseconds = 1000f;
                        particleEmitter.InitialScale = 1.5f;
                        particleEmitter.FinalScale = 0.5f;

                        backgroundParticles.Add(particleEmitter);

                    }
                    else
                    {
                        ParticleEmitter particleEmitter = new ParticleEmitter(this, obj.X, obj.Y, 256);
                        particleEmitter.EmitterBox.Resize(obj.Width, obj.Height);
                        particleEmitter.MakeParticles(tilemapTexture, 16, 16);
                        particleEmitter.ParticleVelocity = new Vector2(0, -0.01f);
                        particleEmitter.SetAcceleration(0, -0.005f);
                        particleEmitter.XVelocityVariationRange = new Vector2(-20f, 20f);
                        particleEmitter.YVelocityVariationRange = new Vector2(-20f, 0f);
                        particleEmitter.SetTextureCropRectangle(new Rectangle(0, 78, 16, 16));
                        particleEmitter.SpawnRate = 250f;
                        particleEmitter.ParticleLifespanMilliseconds = 5000f;
                        particleEmitter.ParticleLifespanVariationMilliseconds = 1000f;
                        particleEmitter.InitialScale = 0.1f;
                        particleEmitter.FinalScale = 1.5f;

                        particleEmitter.ForEachParticle(ChangeSpriteTintBlue);

                        backgroundParticles.Add(particleEmitter);
                    }

                }
                else if (obj.Name.ToLower() == "player_spawn")
                {
                    //player.Body.X = obj.X;
                    //player.Body.Y = obj.Y;
                }
                else if (obj.Name.ToLower() == "change_state_trigger")
                {
                    triggers.Add(new Trigger(obj.X, obj.Y, obj.Width, obj.Height, obj.GetProperty("value")));
                }
                else if (obj.Name.ToLower() == "speedbox")
                {

                    SpeedBox s = new SpeedBox(obj.X, obj.Y, obj.Width, obj.Height, float.Parse(obj.GetProperty("speedX")), float.Parse(obj.GetProperty("speedY")));

                    ParticleEmitter particleEmitter = new ParticleEmitter(this, obj.X, obj.Y, 512);
                    particleEmitter.EmitterBox.Resize(obj.Width, obj.Height);
                    particleEmitter.MakeRandomParticles(tilemapTexture, new Rectangle[]
                        {
                            new Rectangle(128, 257, 3, 3),
                            new Rectangle(132, 257, 3, 3),
                            new Rectangle(136, 257, 3, 3),
                            new Rectangle(128 - 16, 257, 3, 3),
                            new Rectangle(132 - 16, 257, 3, 3),
                            new Rectangle(136 - 16, 257, 3, 3)
                        });
                    particleEmitter.ParticleVelocity = new Vector2(s.SpeedIncrease.X * 10, s.SpeedIncrease.Y * 10);
                    particleEmitter.XVelocityVariationRange = new Vector2(-20f, 20f);
                    particleEmitter.YVelocityVariationRange = new Vector2(-20f, 20f);

                    particleEmitter.SpawnRate = 60f;
                    particleEmitter.ParticleLifespanMilliseconds = 5000f;
                    particleEmitter.ParticleLifespanVariationMilliseconds = 1000f;
                    particleEmitter.InitialScale = 1.0f;
                    particleEmitter.FinalScale = 0.5f;


                    backgroundParticles.Add(particleEmitter);
                    speedBoxes.Add(s);

                }

                Console.WriteLine("added " + obj.Name);

            }
            
            topWaterTiles = level.GetTilesListByID(new int[] { 97, 98, 99 });
            
            /*
             * Build Background Gradient
             */
            backgroundWaterGradientStrip = new Texture2D(Graphics.GraphicsDevice, 1, level.Height * level.TileHeight);

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

            #endregion

            foreach (Sprite s in enemies)
            {

                s.Update(gameTime);

            }

            // build loop on only the on screen tiles

            Vector2 screenTopLeftCornerToWorldCoords = camera.GetScreenToWorldPosition(Vector2.Zero);

            int topTileXIndex = (int)screenTopLeftCornerToWorldCoords.X / level.TileWidth;
            int topTileYIndex = (int)screenTopLeftCornerToWorldCoords.Y / level.TileHeight;

            int ammountOfTilesWidthOnScreen = (int)(Graphics.PreferredBackBufferWidth / camera.Zoom / level.TileWidth);
            int ammountOfTilesHeightOnScreen = (int)(Graphics.PreferredBackBufferHeight / camera.Zoom / level.TileHeight);

            for (int y = topTileYIndex; y < ammountOfTilesHeightOnScreen + topTileYIndex; y++)
            {

                for (int x = topTileXIndex; x < ammountOfTilesWidthOnScreen + topTileXIndex; x++)
                {

                    // layer 1 is the one we are using for collisions
                    if (y >= 0 && y < level.Layers[1].TileMap.GetLength(0) &&
                       x >= 0 && x < level.Layers[1].TileMap.GetLength(1))
                    {

                        Tile currentTile = level.Layers[1].TileMap[y, x];

                        if (currentTile == null)
                        {
                            continue;
                        }

                        if (!currentTile.Body.Enabled)
                        {
                            continue;
                        }
                        
                        // enemies projectiles

                        foreach (Sprite p in enemies)
                        {
                            if (p.Alive)
                            {

                                if (p.GetType() == typeof(PufferFish))
                                {
                                    PufferFish puf = (PufferFish)p;

                                    foreach (Bullet b in puf.Bullets)
                                    {
                                        if (b.Alive)
                                        {
                                            if (Physics.Overlap(b, currentTile))
                                            {
                                                b.Kill();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            /*
             * Particle Emitters, background assets, UI, etc..
             */
            #region [ Secondary Assets ]

            // water waves
            float count = 1f;
            foreach (Tile t in topWaterTiles)
            {
                float x = (float)gameTime.TotalGameTime.TotalMilliseconds;
                float phaseShift = (count / 30) * (float)Math.PI * 19.7f;
                float freq = 0.005f;
                float amp = 1f;

                // low freq motion, sin wave 1
                t.Body.Y = Math2.SinWave((x * freq - phaseShift), amp);

                count++;
            }

            foreach (ParticleEmitter p in backgroundParticles)
            {
                p.Update(gameTime);
                p.ForEachParticle(KillOutOfBoundsParticle);
            }

            foreach (GoldFish g in goldenFishs)
            {
                if (g.Alive)
                {

                    g.Update(gameTime);

                }

            }

            camera.Position = new Vector2(
                (float)Math.Sin(gameTime.TotalGameTime.TotalMilliseconds  * 0.0001) * 128 + (level.Width / 2 * level.TileWidth),
                (float)(Math.Cos(gameTime.TotalGameTime.TotalMilliseconds * 0.0001) * -1) * 128 + (level.Height / 2 * level.TileHeight)
            );


            // clamp camera position
            float halfscreenwidth = Graphics.PreferredBackBufferWidth / 2f;
            float halfscreenheight = Graphics.PreferredBackBufferHeight / 2f;
            camera.Position.X = MathHelper.Clamp(camera.Position.X, halfscreenwidth / camera.Zoom, (level.Width * level.TileWidth) - (halfscreenwidth / camera.Zoom));
            camera.Position.Y = MathHelper.Clamp(camera.Position.Y, -1000f, (level.Height * level.TileHeight) - (halfscreenheight / camera.Zoom));

            camera.Update(gameTime, Graphics.GraphicsDevice);
            camera.Zoom = 1.5f;

            Vector2 screenTopLeftCornerToWorldCoords2 = camera.GetScreenToWorldPosition(Vector2.Zero);

            camera.Bounds.X = screenTopLeftCornerToWorldCoords2.X + 16f;
            camera.Bounds.Y = screenTopLeftCornerToWorldCoords2.Y + 16f;
            
            #endregion

            /*
             * Game resolution
             */
            #region [ Game Resolution and other checks ]
            

            if (Physics.Overlap(new AABB(mState.Position.X, mState.Position.Y, 1, 1), diveButton))
            {
                // perform desired action from trigger

                StateManager.Instance.StartGameState("NewTestState");


            }
        
            #endregion

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
            float levelDepth = level.Height * level.TileHeight;
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
            level.Layers[0].Draw(gameTime, spriteBatch);

            // background particle effects
            foreach (ParticleEmitter p in backgroundParticles)
            {
                p.Draw(gameTime, spriteBatch);
            }

            // world tiles
            level.Layers[1].Draw(gameTime, spriteBatch);

            // enemies
            foreach (Sprite s in enemies)
            {
                s.Draw(gameTime, spriteBatch);
            }

            // the gold stuff
            foreach (Sprite s in goldenFishs)
            {
                s.Draw(gameTime, spriteBatch);
            }

            spriteBatch.End();

            #endregion

            #region [Layer 2 - UI - Static]

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            // button
            spriteBatch.Draw(tilemapTexture, new Rectangle(Graphics.PreferredBackBufferWidth / 2 - 128 / 2, Graphics.PreferredBackBufferHeight - 48 - 32, 128, 48), new Rectangle(224, 448, 128, 48), Color.White);

            // ink
            spriteBatch.Draw(tilemapTexture, new Rectangle(Graphics.PreferredBackBufferWidth / 2 - 320 / 2, 32, 320, 108), new Rectangle(160, 336, 320, 108), Color.White);

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