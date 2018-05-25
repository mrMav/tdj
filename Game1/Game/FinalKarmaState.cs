using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Engine;
using System;
using Microsoft.Xna.Framework.Media;

namespace TDJGame
{
    public class FinalKarmaState : GameState
    {

        #region [Properties]

        SpriteFont font;

        #endregion

        public FinalKarmaState(string key, GraphicsDeviceManager graphics)
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
            ContentLoaded = false;
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState kState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();

            Rectangle mouseRectangle = new Rectangle(mState.X, mState.Y, 1, 1);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(Color.Salmon);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "ShotsFired: " + Karma.playerShotsFired + "\nKarma: " + Karma.karma + "\nDamage Dealt: " + Karma.playerTotalDamage + "\nCollected: " + Karma.playerCollect, new Vector2(10,10), Color.Black);
            spriteBatch.End();
        }
    }
}