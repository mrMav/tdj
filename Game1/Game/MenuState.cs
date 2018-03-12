using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using TDJGame.Engine;

namespace TDJGame
{
    public class MenuState : GameState
    {

        #region [Porperties]

        SpriteFont font;

        #endregion

        public MenuState(string key, GraphicsDeviceManager graphics)
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

            if (mState.LeftButton == ButtonState.Pressed)
            {
                StateManager.Instance.StartGameState("PlayState");
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(Color.Salmon);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, $"This state key is {this.Key}! Click to go to play state!", Vector2.Zero, Color.White);
            spriteBatch.End();
        }
    }
}
