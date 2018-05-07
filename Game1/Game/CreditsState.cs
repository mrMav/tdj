using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Engine;
using System;

namespace TDJGame
{
    public class CreditsState : GameState
    {

        #region [Properties]
        Texture2D image;
        Rectangle back;

        #endregion

        public CreditsState(string key, GraphicsDeviceManager graphics)
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
            image = this.content.Load<Texture2D>("Credits");
            back = new Rectangle(1695, 1000, 160, 35);

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

            Rectangle mouseRectangle = new Rectangle(mState.X, mState.Y, 1, 1);

            if (mouseRectangle.Intersects(back) && mState.LeftButton == ButtonState.Pressed)
            {
                StateManager.Instance.StartGameState("MenuState");
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(Color.Salmon);

            spriteBatch.Begin();
            spriteBatch.Draw(image, new Rectangle(0, 0, 1920, 1080), Color.White);
            spriteBatch.End();
        }
    }
}
