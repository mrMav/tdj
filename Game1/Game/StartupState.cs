using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Engine;

namespace TDJGame
{
    public class StartupState : GameState
    {

        #region [Porperties]

        SpriteFont font;

        #endregion

        public StartupState(string key, GraphicsDeviceManager graphics)
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

            if (kState.IsKeyDown(Keys.Enter))
            {
                StateManager.Instance.StartGameState("MenuState");
            }

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {

            graphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, $"This state key is {this.Key}! Press enter to go to 'menu'", Vector2.Zero, Color.White);
            spriteBatch.End();
        }
    }
}
