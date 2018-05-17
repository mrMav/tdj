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
        Texture2D backgroundGradientStrip;
        EnergyBar energyBar;
        EnergyBar healthBar;
        Player player;
        List<Sprite> enemies;
        List<Tile> spikesPointingDown;
        List<Tile> spikesPointingUp;
        List<ParticleEmitter> particleEmitters;

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
            particleEmitters = new List<ParticleEmitter>();

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
             * Player init
             */
            player = new Player(this, tilemapTexture, Vector2.Zero, 32, 32, true);
            player.Animations.CurrentFrame = new Frame(96, 176, 16, 32);
            //player.Animations.Add("robot-idle", new int[] { 177, 178, 179, 180, 181, 182 }, 6, false, true);
            player.Animations.Add("woman-run", new int[] { 183, 184, 185, 186, 187, 188 }, 12, true);
            player.Body.X = 16 * 7;
            player.Body.Y = 16 * 5;
            player.Body.MaxVelocity = 3f;
            player.Body.Drag.X = 0.6f;
            player.Body.Enabled = true;
            player.Body.Tag = "player";
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

                    particleEmitters.Add(particleEmitter);



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

            /*
             * UI Elements init
             */
            healthBar = new EnergyBar(Graphics, new Vector2(16, 16), 256, 16, new Color(0, 255, 0));
            energyBar = new EnergyBar(Graphics, new Vector2(16, 32 + 4), 256, 16, new Color(255, 0, 0));

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
            backgroundGradientStrip = null;
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
            KeyboardState kState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();

            if (kState.IsKeyDown(Keys.Q))
            {
                player.StartBlinking(gameTime);
            }

            /*
             * Player Update
             */
            player.UpdateMotion(gameTime, kState, level, bubble);

            /*
             * AI (lol) Update
             */
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
            //foreach (Sprite s in pufferFish)
            //{
            //    if (s.Alive)
            //    {
            //        s.Update(gameTime);

            //        if (Physics.Overlap(s, player) && !player.IsBlinking)  // when blinking, take no damage
            //        {
            //            player.ReceiveDamage(s.Damage);
            //            player.StartBlinking(gameTime);
            //        }
            //    }
            //}

            foreach (Tile spike in spikesPointingDown)
            {
                if (Physics.Overlap(spike, player) && !player.IsBlinking)  // when blinking, take no damage
                {
                    TriggerPlayerHurt(gameTime, spike);
                }
            }
            foreach (Tile spike in spikesPointingUp)
            {
                if (Physics.Overlap(spike, player) && !player.IsBlinking)  // when blinking, take no damage
                {
                    TriggerPlayerHurt(gameTime, spike);
                }
            }

            foreach (ParticleEmitter p in particleEmitters)
                p.Update(gameTime);

            /*
             * Player Projectiles
             */
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

            /*
             * UI Update
             */
            energyBar.SetPercent((int)(player.Energy * 100f / player.MaxEnergy));
            healthBar.SetPercent((int)(player.Health * 100f / player.MaxHealth));

            camera.Position = new Vector2(player.Body.Position.X + player.Body.Bounds.HalfWidth, player.Body.Position.Y + player.Body.Bounds.HalfHeight);
            camera.Update(gameTime, Graphics.GraphicsDevice);

            /*
             * End Condition
             */
            if (!player.Alive)
            {
                // show end game screen
                // now we will just restart this state
                StateManager.Instance.StartGameState("FeatureTestMap");
            }

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            frameCounter.Update(deltaTime);

            graphicsDevice.Clear(Color.CornflowerBlue);

            /*
             * Background
             */
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            spriteBatch.Draw(backgroundGradientStrip, new Rectangle(0, 0, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight), Color.White);
            spriteBatch.End();

            /*
             * World Render
             */
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.Transform);

            foreach (ParticleEmitter p in particleEmitters)
                p.Draw(gameTime, spriteBatch);

            level.Draw(gameTime, spriteBatch);
            foreach (Sprite s in enemies)
            {
                s.Draw(gameTime, spriteBatch);
                //DrawBodyShape(s, spriteBatch, new Color(0, 150, 0, 150));
            }

            foreach (Sprite s in spikesPointingDown)
            {
                //DrawBodyShape(s, spriteBatch, new Color(150, 0, 0, 150));
            }
            foreach (Sprite s in spikesPointingUp)
            {
                //DrawBodyShape(s, spriteBatch, new Color(150, 0, 0, 150));
            }

            player.Draw(gameTime, spriteBatch);
            //DrawBodyShape(player, spriteBatch, new Color(0, 0, 150, 150));

            spriteBatch.End();

            /* 
             * GUI render
             */
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            energyBar.Draw(spriteBatch, gameTime);
            healthBar.Draw(spriteBatch, gameTime);
            //spriteBatch.DrawString(font, $"{(int)camera.Position.X}, {(int)camera.Position.Y}, {camera.Zoom}", new Vector2(0, graphicsDevice.Viewport.Height - 16), Color.Red);
            spriteBatch.DrawString(font, player.Body.GetDebugString(), new Vector2(0, 48), Color.Red);
            spriteBatch.DrawString(font, $"{Math.Round(frameCounter.CurrentFramesPerSecond)}", Vector2.Zero, Color.LightGreen);

            spriteBatch.End();

        }

        public void TriggerPlayerHurt(GameTime gameTime, Sprite theHurtingSprite)
        {
            //player.ReceiveDamage(theHurtingSprite.Damage);
            player.StartBlinking(gameTime);
            //camera.ActivateShake(gameTime, 350, 8, 0.02f);
            player.ApplyKnockBack(theHurtingSprite);
        }

        /*
         * SERIOUS MEMORY LEAK HERE!
         */
        void DrawBodyShape(Sprite sprite, SpriteBatch spriteBatch, Color color)
        {
            Texture2D texture = new Texture2D(Graphics.GraphicsDevice, (int)sprite.Body.CollisionRect.Width, (int)sprite.Body.CollisionRect.Height);
            DrawMe.Fill(texture, color);

            spriteBatch.Draw(texture, sprite.Body.CollisionRect.Position, Color.White);
        }

    }
}

