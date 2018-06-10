using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Engine;

namespace TDJGame
{
    public class KarmaScreenState : GameState
    {

        #region [Properties]
        SpriteFont font;

        double started;
        double timeout;

        #endregion

        public KarmaScreenState(string key, GraphicsDeviceManager graphics)
        {
            this.Key = key;
            this.Graphics = graphics;
        }

        public override void Initialize()
        {
            base.Initialize();
            started = 0;
            timeout = 5000;
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
            if(started <= 0)
            {
                started = gameTime.TotalGameTime.TotalMilliseconds;
            }

            if (started + timeout < gameTime.TotalGameTime.TotalMilliseconds)
            {
                StateManager.Instance.StartGameState("MenuState");
            }


        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(Color.Salmon);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "RANK:\n" + Karma.DetermineRank() + "\n\nShotsFired: " + Karma.playerShotsFired + "\nKarma: " + Karma.karma + "\nDamage Dealt: " + Karma.playerTotalDamage + "\nCollected: " + Karma.playerCollect, new Vector2(10, 10), Color.Black);
            spriteBatch.End();
        }
    }
}