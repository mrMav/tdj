using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Engine;
using System;
using Microsoft.Xna.Framework.Media;

namespace TDJGame
{
    public class MenuState : GameState
    {

        #region [Properties]

        SpriteFont font;
        Texture2D image, text, submarine, front;
        Vector2 position;
        Rectangle dive, options, credits, quit;

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
            image = this.content.Load<Texture2D>("Menu");
            submarine = this.content.Load<Texture2D>("sub");
            front = this.content.Load<Texture2D>("Underwater");
            text = this.content.Load<Texture2D>("MenuText");


            dive = new Rectangle(90, 900, 338, 110);
            credits = new Rectangle(1130, 955, 300, 60);

            this.ContentLoaded = true;

        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            ContentLoaded = false;
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState kState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();

            Rectangle mouseRectangle = new Rectangle(mState.X, mState.Y, 1, 1);

            Vector2 offset = new Vector2(0, 450);
            Vector2 radius = new Vector2(0, 70);

            float threshold = 0.0012f;

            position.Y = offset.Y + (float)Math.Sin(gameTime.TotalGameTime.TotalMilliseconds * threshold) * radius.Y;

            if (mouseRectangle.Intersects(dive) && mState.LeftButton == ButtonState.Pressed)
            {
                StateManager.Instance.StartGameState("PlayState");
            }
            if (mouseRectangle.Intersects(credits) && mState.LeftButton == ButtonState.Pressed)
            {
                StateManager.Instance.StartGameState("CreditsState");
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(Color.Salmon);

            spriteBatch.Begin();
            spriteBatch.Draw(image, new Rectangle(0, 0, 1920, 1080), Color.White);
            spriteBatch.Draw(submarine, new Rectangle(750, (int)position.Y, 402, 301), Color.White);
            spriteBatch.Draw(front, new Rectangle(0, 0, 1920, 1080), Color.White);
            spriteBatch.Draw(text, new Rectangle(0, 0, 1920, 1080), Color.White);
            spriteBatch.End();
        }
    }
}