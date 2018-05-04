using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TDJGame.Engine;
using TDJGame.Engine.Physics;
using TDJGame.Engine.Tiled;
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
            //tilemapTexture = this.content.Load<Texture2D>("test_tilemap");
            tilemapTexture = this.content.Load<Texture2D>("WaterLvl1Test");

            // player
            this.player = new Player(tilemapTexture, Vector2.Zero, 16, 32, true);
            this.player.TextureBoundingRect = new Rectangle(176, 80, 16, 32);
            this.player.Body.Enabled = true;
            this.player.Body.Tag = "player";

            // specify acceleration
            this.player.Body.Acceleration = new Vector2(1f);

            // specify drag
            this.player.Body.Drag = 0.8f;

            XMLLevelLoader XMLloader = new XMLLevelLoader();
            //this.level = XMLloader.LoadLevel(@"Content\test_map.tmx", tilemapTexture);
            this.level = XMLloader.LoadLevel(@"Content\WaterLvl1Test.tmx", tilemapTexture);
            //this.level.SetCollisionTiles(new int[] {
            //    52, 53, 54,
            //    68, 69, 70,
            //    84, 85, 86,
            //    100, 101, 102,
            //    120, 135, 151
            //});
            
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

            float inc = 0.1f;

            if (kState.IsKeyDown(Keys.Q))
            {
                this.camera.Zoom += inc;
            }

            if (kState.IsKeyDown(Keys.E))
            {
                this.camera.Zoom -= inc;
            }

            this.player.Update(gameTime, kState);

            this.camera.Position = new Vector2(this.player.Body.Position.X, this.player.Body.Position.Y);
            
            this.camera.GetTransform(this.Game.GraphicsDevice);

            this.mouseWorldCoordinates = this.camera.GetScreenToWorldPosition(mState.Position.ToVector2());

            //Physics.CollideMovingSpriteWithListOfStaticObjects(this.player, this.level.CollidableTiles);
            //Console.WriteLine(this.player.Body.Velocity);
            
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
            spriteBatch.End();


            // GUI render
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            //spriteBatch.DrawString(font, $"This state key is {this.Key}! Play with WASD!\nIf you wnat to go back to the menu, press Enter. (disabled in source code)\nQ & E to zoom in and out!", Vector2.Zero, Color.LightGreen);
                        

            spriteBatch.DrawString(font, $"{(int)this.camera.Position.X}, {(int)this.camera.Position.Y}, {this.camera.Zoom}", new Vector2(0, this.Game.graphics.PreferredBackBufferHeight - 16), Color.Red);
        
            spriteBatch.DrawString(font, $"{Math.Round(frameCounter.AverageFramesPerSecond)}", Vector2.Zero, Color.LightGreen);
            

            spriteBatch.End();

        }
    }
}