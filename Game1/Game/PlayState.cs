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

namespace TDJGame
{
    public class PlayState : GameState
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
        List<Sprite> pufferFish;
        List<Tile> spikesPointingDown;
        List<Tile> spikesPointingUp;

        #endregion

        public PlayState(string key, GraphicsDeviceManager graphics)
        {
            Key = key;
            Graphics = graphics;
        }

        public override void Initialize()
        {
            base.Initialize();

            frameCounter = new FrameCounter();
            enemies = new List<Sprite>();
            pufferFish = new List<Sprite>();

        }

        public override void LoadContent()
        {
            base.LoadContent();
            MediaPlayer.Stop();

            camera = new Camera2D(Vector2.Zero);
            camera.Zoom = 2.45f;

            font = content.Load<SpriteFont>("Font");
            tilemapTexture = this.content.Load<Texture2D>("SpriteSheetDraft");

            /*
             * A single pixel to draw lines and stuff
             */
            pixel = new Texture2D(Graphics.GraphicsDevice, 1, 1);
            DrawMe.Fill(pixel, Color.White);

            /*
             * Player init
             */ 
            player = new Player(this, tilemapTexture, Vector2.Zero, 32, 32, true);
            player.TextureBoundingRect = new Rectangle(96, 0, 32, 32);
            player.Body.X = 16 * 3;
            player.Body.Y = 16 * 3;
            player.Body.MaxVelocity = 3f;
            player.Body.Drag.X = 0.6f;
            player.Body.Enabled = true;
            player.Body.Tag = "player";
            player.Body.SetSize(16, 29, 9, 2);

            /*
             * Enemies
             */

            pufferFish.Add(new PufferFish(this, tilemapTexture, new Vector2(149 * 16, 11 * 16), 32, 32, 174 * 16 - 149 * 16, 1.5f));
            pufferFish.Add(new PufferFish(this, tilemapTexture, new Vector2(150 * 16, 2  * 16), 32, 32, 174 * 16 - 150 * 16, 1.5f));
            pufferFish.Add(new PufferFish(this, tilemapTexture, new Vector2(191 * 16, 2  * 16), 32, 32, 210 * 16 - 191 * 16, 1.5f));
            pufferFish.Add(new PufferFish(this, tilemapTexture, new Vector2(181 * 16, 11 * 16), 32, 32, 210 * 16 - 181 * 16, 1.5f));
            pufferFish.Add(new PufferFish(this, tilemapTexture, new Vector2(219 * 16, 5  * 16), 32, 32, 235 * 16 - 219 * 16, 1.5f));

            enemies.Add(new JellyFish(this, tilemapTexture, Vector2.Zero, 16, 32, new Vector2(60  * 16, 6 * 16), new Vector2(4  * 16, 4 * 16), 0.5f));
            enemies.Add(new JellyFish(this, tilemapTexture, Vector2.Zero, 16, 32, new Vector2(120 * 16, 6 * 16), new Vector2(10 * 16, 5 * 16), 0.5f));

            /*
             * Level init
             */
            XMLLevelLoader XMLloader = new XMLLevelLoader();
            level = XMLloader.LoadLevel(this, @"Content\prototipo.tmx", tilemapTexture);
            level.SetCollisionTiles(new int[] { 1, 2, 4, 6 });

            // build spikes tiles list
            spikesPointingDown = level.GetTilesListByID(new int[] { 3 });
            spikesPointingUp   = level.GetTilesListByID(new int[] { 5 });

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
            for(int i = 0; i < backgroundGradientStrip.Height; i++)
            {
                float ratio = Math2.Map(i, 0f, backgroundGradientStrip.Height, 0f, 1.0f);

                currentColor = Color.Lerp(startColor, finishColor, ratio);
                DrawMe.Pixel(backgroundGradientStrip, 0, i, currentColor);
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
            backgroundGradientStrip = null;
            energyBar = null;
            healthBar = null;
            player = null;
            spikesPointingDown = null;
            spikesPointingUp = null;
            enemies = null;
            pufferFish = null;

        }

        public override void Update(GameTime gameTime)
        {
            /*
             * Input State refresh
             */ 
            KeyboardState kState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();
            
            if(kState.IsKeyDown(Keys.Q))
            {
                player.StartBlinking(gameTime);
            }

            /*
             * Player Update
             */ 
            player.UpdateMotion(gameTime, kState, level);

            /*
             * AI (lol) Update
             */
            foreach (Sprite s in enemies)
            {
                if(s.Alive)
                {
                    s.Update(gameTime);

                    if(Physics.Overlap(s, player) && !player.IsBlinking)  // when blinking, take no damage
                    {
                        player.ReceiveDamage(s.Damage);
                        player.StartBlinking(gameTime);
                    }
                }
            }
            foreach (Sprite s in pufferFish)
            {
                if (s.Alive)
                {
                    s.Update(gameTime);

                    if (Physics.Overlap(s, player) && !player.IsBlinking)  // when blinking, take no damage
                    {
                        player.ReceiveDamage(s.Damage);
                        player.StartBlinking(gameTime);
                    }
                }
            }

            foreach (Tile spike in spikesPointingDown)
            {
                if (Physics.Overlap(spike, player) && !player.IsBlinking)  // when blinking, take no damage
                {
                    player.ReceiveDamage(10);
                    player.StartBlinking(gameTime);
                }
            }
            foreach (Tile spike in spikesPointingUp)
            {
                if (Physics.Overlap(spike, player) && !player.IsBlinking)  // when blinking, take no damage
                {
                    player.ReceiveDamage(10);
                    player.StartBlinking(gameTime);
                }
            }


            /*
             * Projectiles
             */
            foreach (Bullet b in player.Bullets)
            {
                if(b.Alive)
                {
                    foreach (Tile t in level.CollidableTiles)
                    {
                        if (Physics.Overlap(b, t))
                        {
                            b.Kill();
                        }
                    }
                    foreach (Sprite s in pufferFish)
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
             * Projectiles
             */
            foreach (PufferFish p in pufferFish)
            {
                foreach (Bullet b in p.Bullets)
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

                                player.ReceiveDamage(10);
                                player.StartBlinking(gameTime);

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

            camera.Position = new Vector2(player.Body.Position.X, 7 * 16 );
            camera.GetTransform(Graphics.GraphicsDevice);

            /*
             * End Condition
             */
            if(!player.Alive)
            {
                // show end game screen
                // now we will just restart this state
                StateManager.Instance.StartGameState("PlayState");
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

            level.Draw(spriteBatch);
            foreach (Sprite s in enemies)
            {
                s.Draw(gameTime, spriteBatch);
            }
            foreach (Sprite s in pufferFish)
            {
                s.Draw(gameTime, spriteBatch);
            }
            player.Draw(gameTime, spriteBatch);

            //DrawBodyShape(player, spriteBatch, new Color(0, 160, 0, 170));
            //DrawBodyShape(spikesPointingDown[0], spriteBatch, new Color(160, 0, 0, 170));
            //DrawBodyShape(spikesPointingUp[0], spriteBatch, new Color(160, 0, 0, 170));

            spriteBatch.End();

            /* 
             * GUI render
             */ 
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            
            energyBar.Draw(spriteBatch, gameTime);
            healthBar.Draw(spriteBatch, gameTime);
            //spriteBatch.DrawString(font, $"{(int)camera.Position.X}, {(int)camera.Position.Y}, {camera.Zoom}", new Vector2(0, graphicsDevice.Viewport.Height - 16), Color.Red);
            //spriteBatch.DrawString(font, player.Body.GetDebugString(), new Vector2(0, 48), Color.Red);
            spriteBatch.DrawString(font, $"{Math.Round(frameCounter.AverageFramesPerSecond)}", Vector2.Zero, Color.LightGreen);
            
            spriteBatch.End();

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
 
