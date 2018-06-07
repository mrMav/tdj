using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Engine
{
    public class StateManager
    {
        #region [Properties]
        private static StateManager instance;
        public static StateManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new StateManager();

                return instance;
            }
        }

        public ContentManager Content { get; set; }

        public List<GameState> GameStatesList;
        private GameState currentGameState;
        private bool PendingStateChange = false;

        #endregion

        // constructor
        private StateManager()
        {
            this.GameStatesList = new List<GameState>();
        }

        // add a game state
        public bool AddGameState(GameState gs)
        {
            if (this.FindState(gs.Key) == null)
            {

                this.GameStatesList.Add(gs);

                return true;

            }
            else
            {

                Console.WriteLine($"GameState {gs.Key} already exists.");

                return false;
            }

        }

        //switch gamestate
        public void StartGameState(string key)
        {

            GameState g = this.FindState(key);

            if (g != null)
            {
                if (this.currentGameState != null)
                {
                    // shutdown current
                    this.UnloadContent();
                }

                // change current to the new one
                this.currentGameState = g;
                
                // flag a reset
                this.PendingStateChange = true;

            }

        }

        //find state by key
        public GameState FindState(string key)
        {

            GameState gs = null;

            foreach (GameState g in this.GameStatesList)
            {
                if (g.Key.ToLower() == key.ToLower())
                {
                    gs = g;
                }
            }

            if (gs != null)
            {
                return gs;
            }
            else
            {
                Console.WriteLine($"No GameState '{key}' found.");
                return null;
            }

        }

        public void LoadContent(ContentManager Content)
        {
            this.Content = new ContentManager(Content.ServiceProvider, "Content");
        }

        public void UnloadContent()
        {
            this.currentGameState.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {

            // if there is a state change
            if (this.PendingStateChange)
            {
                this.currentGameState.Initialize();

                this.currentGameState.LoadContent();

                // state is now started
                this.PendingStateChange = false;

            }
            else if (this.currentGameState.ContentLoaded)
            {

                this.currentGameState.Update(gameTime);

            }

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            if (this.currentGameState.ContentLoaded)
            {

                this.currentGameState.Draw(gameTime, spriteBatch, graphicsDevice);

            }
        }
    }
}
