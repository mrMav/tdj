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
    public class Level2State : GameState
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
        List<Tile> spikesPointingLeft;
        List<Tile> spikesPointingRight;
        List<Tile> topWaterTiles;
        List<ParticleEmitter> backgroundParticles;
        List<Sprite> goldenFishs;
        List<Trigger> triggers;
        Song song;


        #endregion

        public Level2State(string key, GraphicsDeviceManager graphics)
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
            SFX = new Dictionary<string, SoundEffect>();



        }

        public override void LoadContent()
        {
            base.LoadContent();

            camera = new Camera2D(Vector2.Zero);
            camera.Zoom = (float)Graphics.PreferredBackBufferHeight * 2.45f / 600f;  // the ideal zoom is 2.45 at 600px of screen height

            font = content.Load<SpriteFont>("Font");
            tilemapTexture = this.content.Load<Texture2D>("SpriteSheet");
            song = content.Load<Song>("InkStuff");
            MediaPlayer.Volume = 0.3f;
            MediaPlayer.Play(song);
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
            /*player.Animations.CurrentFrame = new Frame(96, 176, 16, 32);*/  // woman
            player.Animations.CurrentFrame = new Frame(16, 64, 32, 32);  // actual player
            //player.Animations.Add("robot-idle", new int[] { 177, 178, 179, 180, 181, 182 }, 6, false, true);
            //player.Animations.Add("woman-run", new int[] { 183, 184, 185, 186, 187, 188 }, 12, true);
            player.Body.X = 16 * 150; /*330*/ //spawn x
            player.Body.Y = 16 * 6; //spawn y
            player.Body.SetSize(16, 32, 0, 0);  // woman
            player.Body.SetSize(10, 26, 11, 3);  // actual player

            //player.Animations.Play("woman-run");

            /*
             * Level init
             */
            XMLLevelLoader XMLloader = new XMLLevelLoader();
            level = XMLloader.LoadLevel(this, @"Content\Level2.tmx", tilemapTexture);
            level.SetCollisionTiles(new int[] { 2, 33, 34, 35, 47, 66, });


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

                    Console.WriteLine("added jelly");


                }
                else if (obj.Name.ToLower() == "pufferfish")
                {
                    Vector2 position = new Vector2(obj.X, obj.Y);

                    float speed = float.Parse(obj.GetProperty("speed"));

                    PufferFish p = new PufferFish(this, tilemapTexture, position, 32, 32, obj.Width, speed);


                    // make it start on the right side of its path
                    if (obj.GetProperty("start_side") == "right")
                    {
                        p.Body.X = obj.X + obj.Width;
                        p.CurrentDistance = obj.Width - 33;
                    }


                    enemies.Add(p);

                    Console.WriteLine("added puffer");

                }
                else if (obj.Name.ToLower() == "turtlex")
                {
                    Vector2 position = new Vector2(obj.X, obj.Y);

                    float speed = float.Parse(obj.GetProperty("speed"));

                    TurtleX p = new TurtleX(this, tilemapTexture, position, 32, 32, 64, obj.Width, speed);
                    

                    // make it start on the right side of its path
                    if (obj.GetProperty("start_side") == "right")
                    {
                        p.Body.X = obj.X + obj.Width;
                        p.CurrentDistance = obj.Width - 33;
                    }

                    enemies.Add(p);

                    Console.WriteLine("added turtlex");

                }

                else if (obj.Name.ToLower() == "goldfish")
                {
                    goldenFishs.Add(new GoldFish(this, tilemapTexture, new Vector2(obj.X, obj.Y), 16, 16));
                }
                else if (obj.Name.ToLower() == "particles")
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



                    Console.WriteLine("added particles");

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

            }


            // build spikes tiles list
            spikesPointingDown = level.GetTilesListByID(new int[] { 514 });
            spikesPointingUp = level.GetTilesListByID(new int[] { 515 });
            spikesPointingLeft = level.GetTilesListByID(new int[] { 516 });
            spikesPointingRight = level.GetTilesListByID(new int[] { 517 });

            foreach (Tile spike in spikesPointingDown)
            {
                spike.Body.SetSize(12, 6, 1, 0);
            }
            foreach (Tile spike in spikesPointingUp)
            {
                spike.Body.SetSize(12, 6, 2, 10);
            }

            foreach (Tile spike in spikesPointingLeft)
            {
                spike.Body.SetSize(6, 12, 10, 2);
            }

            foreach (Tile spike in spikesPointingRight)
            {
                spike.Body.SetSize(6, 12, 0, 2);
            }

            topWaterTiles = level.GetTilesListByID(new int[] { 97, 98, 99 });

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
            spikesPointingLeft = null;
            spikesPointingRight = null;
            enemies = null;
            SFX = null;

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

            /*
             * First of all, we update the player
             * and other sprites velocity vectors
             */
            #region [Update Sprites Velocity and Position]

            player.UpdateMotion(gameTime, kState);

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

                foreach (Tile spike in spikesPointingLeft)
                {
                    if (Physics.Overlap(spike, player))
                    {
                        TriggerPlayerHurt(gameTime, spike);
                        break;
                    }
                }

                foreach (Tile spike in spikesPointingRight)
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

            bool cameraShakeResponse = player.UpdateCollisions(gameTime, level);

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
                float phaseShift = (count / topWaterTiles.Count) * (float)Math.PI * 19.7f;
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
                g.Update(gameTime);

                if (Physics.Overlap(g, player))
                {
                    g.Kill();
                }
            }

            energyBar.SetPercent((int)(player.Energy * 100f / player.MaxEnergy));
            healthBar.SetPercent((int)(player.Health * 100f / player.MaxHealth));

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

            #endregion

            /*
             * Game resolution
             */
            #region [ Game Resolution and other checks ]

            if (!player.Alive)
            {
                // show end game screen
                // now we will just restart this state
                StateManager.Instance.StartGameState("Level2State");
                return;
            }

            foreach (Trigger t in triggers)
            {
                if (Physics.Overlap(player.Body.Bounds, t))
                {
                    StateManager.Instance.StartGameState(t.Value);
                    return;
                }
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

            //foreach (Sprite s in spikesPointingLeft)
            //{
            //    DrawBodyShape(s, spriteBatch, new Color(0, 0, 150, 150));
            //}
            //foreach (Sprite s in spikesPointingRight)
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

            spriteBatch.End();

            #endregion

            #region [Layer 2 - UI - Static]

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            energyBar.Draw(spriteBatch, gameTime);
            healthBar.Draw(spriteBatch, gameTime);

            //spriteBatch.DrawString(font, player.Body.GetDebugString(), new Vector2(0, 48), Color.Red);
            spriteBatch.DrawString(font, $"{Math.Round(frameCounter.CurrentFramesPerSecond)}", Vector2.Zero, Color.LightGreen);
            spriteBatch.DrawString(font, $"{(int)camera.Position.X}, {(int)camera.Position.Y}, {camera.Zoom}", new Vector2(0, graphicsDevice.Viewport.Height - 16), Color.Red);

            spriteBatch.End();

            #endregion

        }

        public void TriggerPlayerHurt(GameTime gameTime, Sprite theHurtingSprite = null, float damage = -1f)
        {
            player.StartBlinking(gameTime);
            camera.ActivateShake(gameTime, 250, 6, 0.015f);

            if (damage < 0f)
            {
                player.ReceiveDamage(theHurtingSprite.Damage);
            }
            else
            {
                player.ReceiveDamage(damage);
                Console.WriteLine("damage: " + damage);
            }

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