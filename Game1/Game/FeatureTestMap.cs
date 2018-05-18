using System;
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
    public class FeatureTestMap : GameState
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
        EnergyBar healthBar;
        Player player;
        List<Sprite> enemies;
        List<Tile> spikesPointingDown;
        List<Tile> spikesPointingUp;
        List<Tile> topWaterTiles;
        List<ParticleEmitter> backgroundParticles;
        float c = 19.7f;
        float a = 1f;
        SoundEffect bubble;

        #endregion

        public FeatureTestMap(string key, GraphicsDeviceManager graphics)
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

        }

        public override void LoadContent()
        {
            base.LoadContent();
            MediaPlayer.Stop();

            camera = new Camera2D(Vector2.Zero);
            camera.Zoom = 2.45f;

            font = content.Load<SpriteFont>("Font");
            tilemapTexture = this.content.Load<Texture2D>("spritesheet-jn");

            bubble = this.content.Load<SoundEffect>("sfx_bubble");

            /*
             * A single pixel to draw lines and stuff
             */
            pixel = new Texture2D(Graphics.GraphicsDevice, 1, 1);
            DrawMe.Fill(pixel, Color.White);

            /*
             * Player init
             */
            player = new Player(this, tilemapTexture, Vector2.Zero, 32, 32, true);
            player.Animations.CurrentFrame = new Frame(96, 176, 16, 32);
            //player.Animations.Add("robot-idle", new int[] { 177, 178, 179, 180, 181, 182 }, 6, false, true);
            player.Animations.Add("woman-run", new int[] { 183, 184, 185, 186, 187, 188 }, 12, true);
            player.Body.X = 16 * 7;
            player.Body.Y = 16 * 5;
            player.Body.SetSize(16, 32, 0, 0);
            //player.Animations.Play("woman-run");

            /*
             * Level init
             */
            XMLLevelLoader XMLloader = new XMLLevelLoader();
            level = XMLloader.LoadLevel(this, @"Content\featureTestMap.tmx", tilemapTexture);
            level.SetCollisionTiles(new int[] { 1, 2, 17, 18, 33, 34 });

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
                    j.Animations.CurrentFrame = new Frame(48, 112, 16, 32);

                    enemies.Add(j);

                    Console.WriteLine("added jelly");


                }
                else if (obj.Name.ToLower() == "pufferfish")
                {
                    Vector2 position = new Vector2(obj.X, obj.Y);

                    float speed = float.Parse(obj.GetProperty("speed"));

                    PufferFish p = new PufferFish(this, tilemapTexture, position, 32, 32, obj.Width, speed);
                    p.Animations.CurrentFrame = new Frame(0, 112, 32, 32);

                    enemies.Add(p);

                    Console.WriteLine("added puffer");

                }
                else if(obj.Name.ToLower() == "particles")
                {

                    ParticleEmitter particleEmitter = new ParticleEmitter(this, obj.X, obj.Y, 256);
                    particleEmitter.EmitterBox.Resize(obj.Width, obj.Height);
                    particleEmitter.MakeParticles(tilemapTexture, 16, 16);
                    particleEmitter.ParticleVelocity = new Vector2(0, -0.01f);
                    particleEmitter.SetAcceleration(0, -0.005f);
                    particleEmitter.XVelocityVariationRange = new Vector2(-20f, 20f);
                    particleEmitter.YVelocityVariationRange = new Vector2(-20f, 0f);
                    particleEmitter.SetTextureCropRectangle(new Rectangle(3*16, 6*16, 16, 16));
                    particleEmitter.SpawnRate = 250f;
                    particleEmitter.ParticleLifespanMilliseconds = 5000f;
                    particleEmitter.ParticleLifespanVariationMilliseconds = 1000f;
                    particleEmitter.InitialScale = 0.1f;
                    particleEmitter.FinalScale = 1.5f;
                    
                    particleEmitter.ForEachParticle(ChangeSpriteTintBlue);

                    backgroundParticles.Add(particleEmitter);



                    Console.WriteLine("added particles");

                }

            }


            // build spikes tiles list
            spikesPointingDown = level.GetTilesListByID(new int[] { 97 });
            spikesPointingUp = level.GetTilesListByID(new int[] { 98 });

            foreach (Tile spike in spikesPointingDown)
            {
                spike.Body.SetSize(12, 6, 2, 0);
            }
            foreach (Tile spike in spikesPointingUp)
            {
                spike.Body.SetSize(12, 6, 2, 10);
            }

            topWaterTiles = level.GetTilesListByID(new int[] { 49, 50, 51 });

            /*
             * UI Elements init
             */
            healthBar = new EnergyBar(Graphics, new Vector2(16, 16), 256, 16, new Color(0, 255, 0));
            energyBar = new EnergyBar(Graphics, new Vector2(16, 32 + 4), 256, 16, new Color(255, 0, 0));

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

        public int ChangeSpriteTintBlue(Sprite s)
        {
            s.Tint = new Color(17, 43, 104, 20);
            return 0;
        }

        public override void UnloadContent()
        {
            base.UnloadContent();

            camera = null;
            frameCounter = null;
            font = null;
            tilemapTexture = null;
            level = null;
            pixel = null;
            backgroundWaterGradientStrip = null;
            energyBar = null;
            healthBar = null;
            player = null;
            spikesPointingDown = null;
            spikesPointingUp = null;
            enemies = null;

        }

        public override void Update(GameTime gameTime)
        {
            /*
             * Input State refresh
             */
            #region [System Status Refresh]

            KeyboardState kState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();

            if (kState.IsKeyDown(Keys.Up))
            {
                c += 0.1f;
            }
            if (kState.IsKeyDown(Keys.Down))
            {
                c -= 0.1f;
            }
            if (kState.IsKeyDown(Keys.NumPad1))
            {
                a += 0.1f;
            }
            if (kState.IsKeyDown(Keys.NumPad2))
            {
                a -= 0.1f;
            }
            #endregion

            /*
             * First of all, we update the player
             * and other sprites velocity vectors
             */
            #region [Update Sprites Velocity and Position]

            player.UpdateMotion(gameTime, kState);

            foreach (Sprite s in enemies)
            {
                if (s.Alive)
                {
                    s.Update(gameTime);

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

            foreach (Tile spike in spikesPointingDown)
            {
                if (Physics.Overlap(spike, player) && !player.IsBlinking)
                {
                    TriggerPlayerHurt(gameTime, spike);
                }
            }
            foreach (Tile spike in spikesPointingUp)
            {
                if (Physics.Overlap(spike, player) && !player.IsBlinking)
                {
                    TriggerPlayerHurt(gameTime, spike);
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
                    foreach (Tile t in level.CollidableTiles)
                    {
                        if (Physics.Overlap(b, t))
                        {
                            b.Kill();
                        }
                    }
                    foreach (Sprite s in enemies)
                    {
                        if (s.Alive)
                        {
                            if (Physics.Overlap(b, s))
                            {
                                b.Kill();
                                s.ReceiveDamage(b.Damage);
                                s.StartBlinking(gameTime);
                                // we should remove dead actors from the list

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
                                foreach (Tile t in level.CollidableTiles)
                                {
                                    if (Physics.Overlap(b, t))
                                    {
                                        b.Kill();
                                    }
                                }

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

            player.UpdateCollisions(gameTime, level);

            #endregion

            /*
             * Particle Emitters, background assets, UI, etc..
             */
            #region [ Secondary Assets ]

            // water waves
            float count = 1f;
            foreach(Tile t in topWaterTiles)
            {
                float x = (float)gameTime.TotalGameTime.TotalMilliseconds;
                float phaseShift = (count / topWaterTiles.Count) * (float)Math.PI * c;
                float freq = 0.005f;
                float amp = a;

                // low freq motion, sin wave 1
                t.Body.Y = Math2.SinWave((x * freq - phaseShift), amp);

                count++;
            }

            foreach (ParticleEmitter p in backgroundParticles)
            {
                p.Update(gameTime);
                p.ForEachParticle(KillOutOfBoundsParticle);
            }

            energyBar.SetPercent((int)(player.Energy * 100f / player.MaxEnergy));
            healthBar.SetPercent((int)(player.Health * 100f / player.MaxHealth));

            camera.Position = new Vector2(player.Body.Position.X + player.Body.Bounds.HalfWidth, player.Body.Position.Y + player.Body.Bounds.HalfHeight);

            // clamp camera position
            float halfscreenwidth = Graphics.PreferredBackBufferWidth / 2f;
            float halfscreenheight = Graphics.PreferredBackBufferHeight / 2f;
            camera.Position.X = MathHelper.Clamp(camera.Position.X, halfscreenwidth / camera.Zoom, (level.Width * level.TileWidth) - (halfscreenwidth / camera.Zoom));
            camera.Position.Y = MathHelper.Clamp(camera.Position.Y, -1000f, (level.Height * level.TileHeight) - (halfscreenheight / camera.Zoom));

            camera.Update(gameTime, Graphics.GraphicsDevice);

            #endregion

            /*
             * Game resolution
             */
            #region [ Game Resolution and other checks ]

            if (!player.Alive)
            {
                // show end game screen
                // now we will just restart this state
                StateManager.Instance.StartGameState("FeatureTestMap");
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
            float multiplyer = (1f - player.Body.Y / (level.Height * level.TileHeight));

            if (multiplyer >= 1f)
                multiplyer = 1f;

            byte colorvalue = (byte)(255f * multiplyer);

            depthbasedtint.R = colorvalue;
            depthbasedtint.G = colorvalue;
            depthbasedtint.B = colorvalue;
            spriteBatch.Draw(backgroundWaterGradientStrip, new Rectangle(0, 0, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight), depthbasedtint);

            Console.WriteLine(1f - player.Body.Y / (level.Height * level.TileHeight));

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

            // player
            player.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            #endregion

            #region [Layer 2 - UI - Static]

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            energyBar.Draw(spriteBatch, gameTime);
            healthBar.Draw(spriteBatch, gameTime);

            //spriteBatch.DrawString(font, player.Body.GetDebugString(), new Vector2(0, 48), Color.Red);
            spriteBatch.DrawString(font, $"{Math.Round(frameCounter.CurrentFramesPerSecond)} {c},{a}", Vector2.Zero, Color.LightGreen);
            spriteBatch.DrawString(font, $"{(int)camera.Position.X}, {(int)camera.Position.Y}, {camera.Zoom}", new Vector2(0, graphicsDevice.Viewport.Height - 16), Color.Red);

            spriteBatch.End();

            #endregion

        }

        public void TriggerPlayerHurt(GameTime gameTime, Sprite theHurtingSprite)
        {
            //player.ReceiveDamage(theHurtingSprite.Damage);
            player.StartBlinking(gameTime);
            camera.ActivateShake(gameTime, 250, 6, 0.015f);
            player.ApplyKnockBack(theHurtingSprite);
        }

        public int KillOutOfBoundsParticle(Particle p)
        {

            if(p.Body.Y <= 0f)
            {
                p.Kill();
            }

            return 0;

        }

        void DrawBodyShape(Sprite sprite, SpriteBatch spriteBatch, Color color)
        {
            
            spriteBatch.Draw(pixel, new Rectangle((int)sprite.Body.CollisionRect.Position.X,
                                                  (int)sprite.Body.CollisionRect.Position.Y,
                                                  (int)sprite.Body.CollisionRect.Width,
                                                  (int)sprite.Body.CollisionRect.Height),  color);
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

