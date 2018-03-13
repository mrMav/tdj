using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDJGame.Engine
{
    public class GameState
    {
        #region [Properties]

        // a reference to the game
        public Game Game;

        public string Key { get; set; }
        protected ContentManager content;
        public bool ContentLoaded { get; set; }

        protected GraphicsDeviceManager Graphics;

        #endregion
        public GameState() { }

        public GameState(string key, Game game)
        {
            this.Key = key;
            this.Game = game;

            this.ContentLoaded = false;
        }

        public virtual void Initialize()
        {

            Console.WriteLine($"GameState {Key} initialized.");

        }

        public virtual void LoadContent()
        {
            this.content = new ContentManager(StateManager.Instance.Content.ServiceProvider, "Content");
        }

        public virtual void UnloadContent()
        {
            this.content.Unload();
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {

        }

    }
}
