using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine;
using Engine.Physics;
using Engine.Tiled;
using TDJGame.Utils;

namespace TDJGame
{
    public class PlayState : GameState
    {

        #region [Properties]

        Camera2D camera;
        FrameCounter frameCounter = new FrameCounter();
        SpriteFont font;
        Texture2D tilemapTexture;
        Vector2 mouseWorldCoordinates;
        Level level;
        Texture2D pixel;

        Player player;

        #endregion

        public PlayState(string key, Game game)
        {
            this.Key = key;
            this.Game = game;
        }

        public override void Initialize()
        {
            base.Initialize();
            
        }

        public override void LoadContent()
        {
            base.LoadContent();

            camera = new Camera2D(Vector2.Zero);
            camera.Zoom = 3.0f;

            font = this.content.Load<SpriteFont>("Font");
            tilemapTexture = this.content.Load<Texture2D>("tilemap");

            /*
             * A single pixel to draw lines and stuff
             */
            pixel = new Texture2D(Game.GraphicsDevice, 1, 1);
            DrawMe.Fill(pixel, Color.White);

            // player
            this.player = new Player(tilemapTexture, Vector2.Zero, 16, 32, true);
            this.player.TextureBoundingRect = new Rectangle(176, 80, 16, 32);
            this.player.Body.Enabled = true;
            this.player.Body.Tag = "player";

            // specify acceleration
            this.player.Body.Acceleration = new Vector2(0.5f);

            // specify drag
            this.player.Body.Drag = 0.8f;

            XMLLevelLoader XMLloader = new XMLLevelLoader();
            //this.level = XMLloader.LoadLevel(@"Content\test_map.tmx", tilemapTexture);
            this.level = XMLloader.LoadLevel(@"Content\test_map_2.tmx", tilemapTexture);

            this.level.SetCollisionTiles(new int[] {
                52, 53, 54,
                68, 69, 70,
                84, 85, 86,
                100, 101, 102,
                120, 135, 151, 10
            });

            this.ContentLoaded = true;

        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState kState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();
            
            //if (kState.IsKeyDown(Keys.Enter))
            //{
            //    StateManager.Instance.StartGameState("MenuState");
            //}

            //float inc = 0.1f;

            //if (kState.IsKeyDown(Keys.Q))
            //{
            //    this.camera.Zoom += inc;
            //}

            //if (kState.IsKeyDown(Keys.E))
            //{
            //    this.camera.Zoom -= inc;
            //}

            this.player.UpdateMotion(gameTime, kState, this.level);

            this.camera.Position = new Vector2(this.player.Body.Position.X, this.player.Body.Position.Y);
            this.camera.GetTransform(this.Game.GraphicsDevice);
            this.mouseWorldCoordinates = this.camera.GetScreenToWorldPosition(mState.Position.ToVector2());
                        
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            frameCounter.Update(deltaTime);

            graphicsDevice.Clear(Color.CornflowerBlue);

            //world render
            spriteBatch.Begin(
                samplerState: SamplerState.PointClamp,                
                transformMatrix: this.camera.Transform
            );
            this.level.Draw(spriteBatch);
            this.player.Draw(gameTime, spriteBatch);

            DrawLine(spriteBatch, player.Body.Position, (player.Body.Position + player.Body.Velocity), Color.Blue);

            spriteBatch.End();

            // GUI render
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            //spriteBatch.DrawString(font, $"This state key is {this.Key}! Play with WASD!\nIf you wnat to go back to the menu, press Enter. (disabled in source code)\nQ & E to zoom in and out!", Vector2.Zero, Color.LightGreen);
            
            spriteBatch.DrawString(font, $"{(int)this.camera.Position.X}, {(int)this.camera.Position.Y}, {this.camera.Zoom}", new Vector2(0, graphicsDevice.Viewport.Height - 16), Color.Red);
            spriteBatch.DrawString(font, $"{Math.Round(frameCounter.AverageFramesPerSecond)}", Vector2.Zero, Color.LightGreen);


            spriteBatch.End();

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