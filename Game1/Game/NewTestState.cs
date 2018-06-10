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
    public class NewTestState : GameState
    {

        #region [Properties]
        
        Camera2D camera;
        FrameCounter frameCounter;
        SpriteFont font;
        Texture2D tilemapTexture;
        Level level;
        Texture2D pixel;
        Texture2D backgroundWaterGradientStrip;
        Texture2D backgroundSkyGradientStrip;
        EnergyBar energyBar;
        Player player;
        List<Sprite> enemies;
        List<Tile> spikesPointingDown;
        List<Tile> spikesPointingUp;
        List<Tile> topWaterTiles;
        List<ParticleEmitter> backgroundParticles;
        List<Sprite> goldenFishs;
        List<Trigger> triggers;
        List<SpeedBox> speedBoxes;

        //Stopwatch stopwatch;

        //long tickCount = 0;
        //double sumOfMilliseconds = 0;
        //double averageMilliseconds = 0;
        //double maxMills = double.MinValue;
        //double minMills = double.MaxValue;

        #endregion

        public NewTestState(string key, GraphicsDeviceManager graphics)
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
            camera.Zoom = (float)Graphics.PreferredBackBufferHeight * 2.45f / 600f;  // the ideal zoom is 2.45 at 600px of screen height

            font = content.Load<SpriteFont>("Font");
            tilemapTexture = this.content.Load<Texture2D>("SpriteSheet");

            MediaPlayer.Volume = 0.3f;
            MediaPlayer.Play(Globals.LevelSong);
            MediaPlayer.IsRepeating = true;
            
            /*
             * A single pixel to draw lines and stuff
             */
            pixel = new Texture2D(Graphics.GraphicsDevice, 1, 1);
            DrawMe.Fill(pixel, Color.White);

            /*
             * Load sfx
             */
            SFX.Add("bubble", content.Load<SoundEffect>("sfx_bubble"));
            SFX.Add("bubble_noise_single", content.Load<SoundEffect>("BubbleNoisesSingle"));
            SFX.Add("anchor", content.Load<SoundEffect>("Anchor3"));
            SFX.Add("fall", content.Load<SoundEffect>("Falling"));
            SFX.Add("enemyDeath", content.Load<SoundEffect>("EnemyDeath"));
            /*
             * Player init
             */
            player = new Player(this, tilemapTexture, Vector2.Zero, 32, 32, true);
            
            /*
             * Level init
             */
            XMLLevelLoader XMLloader = new XMLLevelLoader();

            // se o load do mapa falhar, well shit. vai para o menu.
            try
            {
                level = XMLloader.LoadLevel(this, @"Content\" + Globals.CurrentLevel +".tmx", tilemapTexture);

            } catch(Exception e)
            {
                Console.WriteLine("Error Loading Map. Error message: " + e.Message);

                StateManager.Instance.StartGameState("MenuState");

            }

            level.SetCollisionTiles(new int[] { 2, 33, 34, 35, 47, 66, 15, 79});


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
                    player.Body.X = obj.X;
                    player.Body.Y = obj.Y;
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
            
            // build spikes tiles list
            spikesPointingDown = level.GetTilesListByID(new int[] { 514 });
            spikesPointingUp = level.GetTilesListByID(new int[] { 515 });

            foreach (Tile spike in spikesPointingDown)
            {
                spike.Body.SetSize(12, 6, 1, 0);
            }
            foreach (Tile spike in spikesPointingUp)
            {
                spike.Body.SetSize(12, 6, 2, 10);
            }

            topWaterTiles = level.GetTilesListByID(new int[] { 97, 98, 99 });

            /*
             * UI Elements init
             */
            //healthBar = new EnergyBar(Graphics, new Vector2(16, 16), 256, 16, new Color(0, 255, 0));
            energyBar = new EnergyBar(Graphics, new Vector2(16+6, 16 + 25), 16*10 - 9, 16, new Color(255, 0, 0));  //16+2, 16 + 8, 16*10, 32+16

            Karma.karma = Karma.maxKarma;

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
            pixel = null;
            backgroundWaterGradientStrip = null;
            energyBar = null;
            player = null;
            spikesPointingDown = null;
            spikesPointingUp = null;
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

            //stopwatch = Stopwatch.StartNew();

            /*
             * Input State refresh
             */
            #region [System Status Refresh]

            KeyboardState kState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();

            #endregion

            /*
             * First of all, we update the player
             * and other sprites velocity vectors
             */
            #region [Update Sprites Velocity and Position]



            player.UpdateMotion(gameTime, kState);

            foreach (SpeedBox s in speedBoxes)
            {
                if (Physics.Overlap(player.Body.Bounds, s.Bounds))
                {
                     s.ApplySpeed(gameTime, player);
                }
            }

            foreach (Sprite s in enemies)
            {

                if (s.Body.Tag == "turtlex")
                {
                    TurtleX t = (TurtleX)s;
                    float inflictDamage = t.UpdateMov(gameTime, player);

                    if (inflictDamage > 0f)
                    {
                        TriggerPlayerHurt(gameTime, t, inflictDamage);
                        t.Kill();
                    }

                    if (inflictDamage == -1f)
                    {
                        t.Kill();
                        camera.ActivateShake(gameTime, 400f, 6, 0.01f, true, -0.02f);
                    }

                    continue;
                }

                s.Update(gameTime);

                if (s.Alive)
                {
                    if (Physics.Overlap(s, player) && !player.IsBlinking)  // when blinking, take no damage
                    {
                        TriggerPlayerHurt(gameTime, s);
                    }
                }
            }

            #endregion

            /*
             * Because the hazards may cause velocity changes,
             * we overlap the player with them, BEFORE checking
             * for collisions with the world.
             */
            #region [Hazards Overlap]

            if (!player.IsBlinking)
            {

                foreach (Tile spike in spikesPointingDown)
                {
                    if (Physics.Overlap(spike, player))
                    {
                        TriggerPlayerHurt(gameTime, spike);
                        break;  // breaking at the first overlap
                                // avoids various knockback forces
                                // being applied.
                                // this solved the issue (as far as I tested)
                                // of glitching through the solid tiles
                    }
                }
                foreach (Tile spike in spikesPointingUp)
                {
                    if (Physics.Overlap(spike, player))
                    {
                        TriggerPlayerHurt(gameTime, spike);
                        break;
                    }
                }

            }

            /*
             * Player Projectiles
             */
            player.UpdateProjectiles(gameTime, kState);

            foreach (Bullet b in player.Bullets)
            {
                if (b.Alive)
                {                    
                    foreach (Sprite s in enemies)
                    {
                        if (s.Alive)
                        {
                            if (Physics.Overlap(b, s))
                            {
                                b.Kill();
                                s.ReceiveDamage(b.Damage);
                                s.StartBlinking(gameTime);
                                Karma.ReduceKarma(1f);
                                Karma.AddPlayerDamage(1f);

                                if (!s.Alive)
                                {
                                    camera.ActivateShake(gameTime, 150, 6, 0.015f, true, -0.01f);
                                }


                            }
                        }
                    }
                }
            }

            /*
             * Enemies Projectiles
             */
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
                                if (player.Alive)
                                {
                                    if (Physics.Overlap(b, player) && !player.IsBlinking)
                                    {
                                        TriggerPlayerHurt(gameTime, b);

                                    }
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            /*
             * Next up, we have the world collisions
             * and resolution.
             */
            #region [World Collisions]

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

                        // collide player projectiles

                        foreach (Bullet b in player.Bullets)
                        {
                            if (b.Alive)
                            {
                                if (Physics.Overlap(b, currentTile))
                                {
                                    b.Kill();
                                }
                            }
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

            // apply player velocities and collide
            // in order to avoid wall sticking
            // it's necessary to first move in x
            // and solve and then move in y and solve again

            #region [Player World Collisions]

            bool cameraShakeResponse = false;

            player.Body.PreCollisionUpdate(gameTime);
            player.Body.PreMovementUpdate(gameTime);

            // apply x velocity
            player.Body.X += player.Body.Velocity.X;

            for (int y = topTileYIndex; y < ammountOfTilesHeightOnScreen + topTileYIndex; y++)
            {

                for (int x = topTileXIndex; x < ammountOfTilesWidthOnScreen + topTileXIndex; x++)
                {

                    // layer 1 is the one we are using for collisions
                    int xLength = level.Layers[1].TileMap.GetLength(1);
                    int yLength = level.Layers[1].TileMap.GetLength(0);

                    if (y >= 0 && y < yLength &&
                       x >= 0 && x < xLength)
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
                        
                        // solve x collisions
                        Physics.Collide(player, currentTile, 0); // collide in x

                    }
                }
            }

            // apply y velocity
            player.Body.Y += player.Body.Velocity.Y;

            for (int y = topTileYIndex; y < ammountOfTilesHeightOnScreen + topTileYIndex; y++)
            {

                for (int x = topTileXIndex; x < ammountOfTilesWidthOnScreen + topTileXIndex; x++)
                {

                    // layer 1 is the one we are using for collisions
                    int xLength = level.Layers[1].TileMap.GetLength(1);
                    int yLength = level.Layers[1].TileMap.GetLength(0);

                    if (y >= 0 && y < yLength &&
                       x >= 0 && x < xLength)
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
                        
                        // solve y collisions
                        bool collided = Physics.Collide(player, currentTile, 1); // collide in y           

                        //if the player was moving down:
                        if (collided && player.Body.MovingDown)
                        {
                            cameraShakeResponse = true;
                            SoundEffect fall;
                            SFX.TryGetValue("fall", out fall);
                            fall?.Play();
                        }                        
                    }
                }
            }

            // bound to world
            if (player.Body.Y < -16f)
            {
                player.Body.Y = -16f;
            }

            player.Body.Update(gameTime);

            #endregion


            //bool cameraShakeResponse = player.UpdateCollisions(gameTime, level);

            RepositionOutOfBoundsPlayer(gameTime);

            #endregion

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
                if(g.Alive)
                {

                    g.Update(gameTime);

                    if (Physics.Overlap(g, player))
                    {
                        g.Kill();
                        Karma.AddCollectable();
                    }
                }

            }

            energyBar.SetPercent((int)(player.Energy * 100f / player.MaxEnergy));
            //healthBar.SetPercent((int)(player.Health * 100f / player.MaxHealth));

            if (cameraShakeResponse && !camera.Shaking)
            {
                //camera.ActivateShake(gameTime, 250f, 4f, 0.05f, true, 0.01f); // static
                camera.ActivateShake(gameTime, 250f, Math.Abs(player.Body.DeltaY()) * 3, 0.05f, true, 0.01f); // based on delta
                //Console.WriteLine(Math.Abs(player.Body.DeltaY()));
            }

            camera.Position = new Vector2(player.Body.Position.X + player.Body.Bounds.HalfWidth, player.Body.Position.Y + player.Body.Bounds.HalfHeight);

            // clamp camera position
            float halfscreenwidth = Graphics.PreferredBackBufferWidth / 2f;
            float halfscreenheight = Graphics.PreferredBackBufferHeight / 2f;
            camera.Position.X = MathHelper.Clamp(camera.Position.X, halfscreenwidth / camera.Zoom, (level.Width * level.TileWidth) - (halfscreenwidth / camera.Zoom));
            camera.Position.Y = MathHelper.Clamp(camera.Position.Y, -1000f, (level.Height * level.TileHeight) - (halfscreenheight / camera.Zoom));

            camera.Update(gameTime, Graphics.GraphicsDevice);
            
            Vector2 screenTopLeftCornerToWorldCoords2 = camera.GetScreenToWorldPosition(Vector2.Zero);

            camera.Bounds.X = screenTopLeftCornerToWorldCoords2.X + 16f;
            camera.Bounds.Y = screenTopLeftCornerToWorldCoords2.Y + 16f;

            // build loop on only the visible tiles

            //Vector2 screenTopLeftCornerToWorldCoords = camera.GetScreenToWorldPosition(Vector2.Zero);

            //int topTileXIndex = (int)screenTopLeftCornerToWorldCoords.X / level.TileWidth;
            //int topTileYIndex = (int)screenTopLeftCornerToWorldCoords.Y / level.TileHeight;

            //int ammountOfTilesWidthOnScreen = (int)(Graphics.PreferredBackBufferWidth / camera.Zoom / level.TileWidth);
            //int ammountOfTilesHeightOnScreen = (int)(Graphics.PreferredBackBufferHeight / camera.Zoom / level.TileHeight);
            
            //for (int y = topTileYIndex; y < ammountOfTilesHeightOnScreen + topTileYIndex; y++)
            //{

            //    for (int x = topTileXIndex; x < ammountOfTilesWidthOnScreen + topTileXIndex; x++)
            //    {

            //        // layer 1 is the one we are using for collisions
            //        if(y >= 0 && y < level.Layers[1].TileMap.GetLength(0) &&
            //           x >= 0 && x < level.Layers[1].TileMap.GetLength(1))
            //        {

            //            if(level.Layers[1].TileMap[y, x] != null)
            //            {
            //                level.Layers[1].TileMap[y, x].Tint = new Color(0, 240, 0, 240);
            //            }

            //        }

            //    }

            //}


            #endregion

            /*
             * Game resolution
             */
            #region [ Game Resolution and other checks ]

            if (!player.Alive)
            {
                // show end game screen
                // now we will just restart this state
                StateManager.Instance.StartGameState("KarmaScreenState");

                // reboot this level
                //StateManager.Instance.StartGameState(this.Key);

                return;
            }

            foreach (Trigger t in triggers)
            {
                if (Physics.Overlap(player.Body.Bounds, t))
                {
                    Globals.CurrentLevel = t.Value;

                    StateManager.Instance.StartGameState(this.Key);

                    return;
                }
            }

            #endregion

            //stopwatch.Stop();

            //++tickCount;

            //sumOfMilliseconds += stopwatch.Elapsed.TotalMilliseconds;
            //averageMilliseconds = sumOfMilliseconds / tickCount;

            //maxMills = stopwatch.Elapsed.TotalMilliseconds > maxMills && tickCount > 20 ? stopwatch.Elapsed.TotalMilliseconds : maxMills;
            //minMills = stopwatch.Elapsed.TotalMilliseconds < minMills && tickCount > 20 ? stopwatch.Elapsed.TotalMilliseconds : minMills;

            //Console.WriteLine(
            //    $"RealTime: {stopwatch.Elapsed.TotalMilliseconds:0.0000}, Avg: {averageMilliseconds:0.0000}, Min: {minMills}, Max: {maxMills} "
            //);

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
            float multiplyer = (1f - player.Body.Y / (depthToPitchBlack < levelDepth ? levelDepth : depthToPitchBlack));

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
                //DrawBodyShape(s, spriteBatch, new Color(150, 0, 0, 150));
            }

            //foreach (Sprite s in spikesPointingDown)
            //{
            //    DrawBodyShape(s, spriteBatch, new Color(0, 0, 150, 150));
            //}
            //foreach (Sprite s in spikesPointingUp)
            //{
            //    DrawBodyShape(s, spriteBatch, new Color(0, 0, 150, 150));
            //}

            // the gold stuff
            foreach (Sprite s in goldenFishs)
            {
                s.Draw(gameTime, spriteBatch);
            }

            // player
            player.Draw(gameTime, spriteBatch);
            //DrawBodyShape(player, spriteBatch, new Color(100, 0, 0, 150));


            // draw camera bounds for testing purposes
            //spriteBatch.Draw(pixel, new Rectangle((int)camera.Bounds.X, (int)camera.Bounds.Y, (int)camera.Bounds.Width, (int)camera.Bounds.Height), new Color(20, 0, 0, 20));

            spriteBatch.End();

            #endregion

            #region [Layer 2 - UI - Static]

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            energyBar.Draw(spriteBatch, gameTime);
            //healthBar.Draw(spriteBatch, gameTime);

            // energy asset
            spriteBatch.Draw(tilemapTexture, new Rectangle(16+2, 16 + 8, 16*10, 32+16), new Rectangle(16, 432, 64, 16), Color.White);

            Rectangle lifeHeart = new Rectangle(0, 432, 16, 16);
            for (int i = 1; i <= player.Health; i++)
                spriteBatch.Draw(tilemapTexture, new Rectangle(32 * i - 16, 8, 32, 32), lifeHeart, Color.White);

            //spriteBatch.DrawString(font, player.Body.GetDebugString(), new Vector2(0, 48), Color.Red);
            spriteBatch.DrawString(font, $"{Math.Round(frameCounter.CurrentFramesPerSecond)}", Vector2.Zero, Color.LightGreen);
            //spriteBatch.DrawString(font, $"{(int)camera.Position.X}, {(int)camera.Position.Y}, {camera.Zoom} Bounds: {(int)camera.Bounds.X}, {(int)camera.Bounds.Y}, {(int)camera.Bounds.Width}, {(int)camera.Bounds.Height}", new Vector2(0, graphicsDevice.Viewport.Height - 16), Color.Red);

            spriteBatch.End();

            #endregion

        }

        public void TriggerPlayerHurt(GameTime gameTime, Sprite theHurtingSprite = null, float damage = -1f)
        {
            player.StartBlinking(gameTime);
            camera.ActivateShake(gameTime, 250, 6, 0.015f);

            player.ReceiveDamage(1);

            if (theHurtingSprite != null)
            {
                player.ApplyKnockBackBasedOnSprite(theHurtingSprite);
            }

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

        /// <summary>
        /// This functions will be called to detect if the player glitched out of the world.
        /// It's necessary to prevent unknown bugs from breaking the user experience.
        /// </summary>
        /// <param name="gameTime"></param>
        public void RepositionOutOfBoundsPlayer(GameTime gameTime)
        {

            float levelWidth = level.Width * level.TileWidth;
            float levelHeight = level.Height * level.TileHeight;

            if (player.Body.X < 0 || player.Body.X > levelWidth || player.Body.Y > levelHeight)
            {

                foreach (TiledObject obj in level.Objects)
                {
                    if (obj.Name.ToLower() == "player_spawn")
                    {
                        player.Body.X = obj.X;
                        player.Body.Y = obj.Y;
                    }
                }

                player.Floating = true;
                player.Energy = player.MaxEnergy;

                player.StartBlinking(gameTime);

            }
        }

        void DrawBodyShape(Sprite sprite, SpriteBatch spriteBatch, Color color)
        {

            spriteBatch.Draw(pixel, new Rectangle((int)sprite.Body.CollisionRect.Position.X,
                                                  (int)sprite.Body.CollisionRect.Position.Y,
                                                  (int)sprite.Body.CollisionRect.Width,
                                                  (int)sprite.Body.CollisionRect.Height), color);
        }

        void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
        {
            Vector2 edge = end - start;

            float angle = (float)Math.Atan2(edge.Y, edge.X);

            spriteBatch.Draw(pixel,
                new Rectangle(
                    (int)start.X,
                    (int)start.Y,
                    (int)edge.Length(),
                    1),
                null,
                color,
                angle,
                new Vector2(0, 0),
                SpriteEffects.None,
                0);

        }

    }
}