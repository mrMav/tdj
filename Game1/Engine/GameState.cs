using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Engine
{
    public class GameState
    {
        #region [Properties]

        // a reference to the game
        public Game Game;

        public string Key { get; set; }
        protected ContentManager content;
        public bool ContentLoaded { get; set; }

        public GraphicsDeviceManager Graphics { get; set; }

        public Dictionary<string, SoundEffect> SFX;

        #endregion
        public GameState() { }

        public GameState(string key, Game game)
        {
            this.Key = key;
            this.Game = game;

            SFX = new Dictionary<string, SoundEffect>();

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
            this.ContentLoaded = false;
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {

        }

    }
}
