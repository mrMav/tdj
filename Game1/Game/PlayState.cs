using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TDJGame.Engine.Tiled;
using TDJGame.Utils;

namespace TDJGame
{
    public class PlayState : GameState
    {

        #region [Porperties]

        SpriteFont font;
        Texture2D ball;
        Texture2D tilemapTexture;

        Level level;

        Player player;

        #endregion

        public PlayState(string key, GraphicsDeviceManager graphics)
        {
            this.Key = key;
            this.Graphics = graphics;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            font = this.content.Load<SpriteFont>("Font");
            ball = this.content.Load<Texture2D>("ball");
            tilemapTexture = this.content.Load<Texture2D>("tilemap");

            this.player = new Player(
                ball,
                new Vector2(
                    this.Graphics.PreferredBackBufferWidth / 2,
                    this.Graphics.PreferredBackBufferHeight / 2)
                );

            XMLLevelLoader XMLloader = new XMLLevelLoader();
            this.level = XMLloader.LoadLevel("C:\\Users\\Noro\\Desktop\\TestMap\\test_map.tmx", tilemapTexture);
            
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

            if (kState.IsKeyDown(Keys.Enter))
            {
                StateManager.Instance.StartGameState("MenuState");
            }

            this.player.Update(gameTime, kState);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(Color.CornflowerBlue);
            
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            this.level.Draw(spriteBatch);
            spriteBatch.DrawString(font, $"This state key is {this.Key}! Play with WASD!\nIf you wnat to go back to the menu, press Enter.", Vector2.Zero, Color.OrangeRed);
            this.player.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }
    }
}