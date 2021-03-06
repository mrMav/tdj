﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace TDJGame
{
    public static class Globals
    {

        public static int CurrentLevelNumber;
        public static string CurrentLevel;

        public static Song MenuSong;
        public static Song LevelSong;
        public static Song JingleSong;

    }

    public class TDJGame : Game
    {

        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        public TDJGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;

        }

        protected override void Initialize()
        {
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.PreferredBackBufferWidth = 1270;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

            StateManager.Instance.AddGameState(new StartupState("StartupState", graphics));
            StateManager.Instance.AddGameState(new NewMenuState("MenuState", graphics));
            //StateManager.Instance.AddGameState(new PlayState("PlayState", graphics));
            StateManager.Instance.AddGameState(new CreditsState("CreditsState", graphics));
            //StateManager.Instance.AddGameState(new TestParticleEmitter("TestParticles", graphics));
            //StateManager.Instance.AddGameState(new TestObjectImport("TestObjectImport", graphics));
            //StateManager.Instance.AddGameState(new TestAnimations("TestAnimations", graphics));
            //StateManager.Instance.AddGameState(new TestCamera("TestCamera", graphics));
            //StateManager.Instance.AddGameState(new FeatureTestMap("FeatureTestMap", graphics));
            //StateManager.Instance.AddGameState(new Level1State("Level1State", graphics));
            //StateManager.Instance.AddGameState(new Level2State("Level2State", graphics));
            StateManager.Instance.AddGameState(new NewTestState("NewTestState", graphics));
            //StateManager.Instance.AddGameState(new KarmaScreenState("KarmaScreenState", graphics));
            StateManager.Instance.AddGameState(new NewKarmaScreen("KarmaScreenState", graphics));

            Globals.CurrentLevel = "Level1";            

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Globals.MenuSong = Content.Load<Song>("menuSong");
            Globals.LevelSong = Content.Load<Song>("inkStuff");
            Globals.JingleSong = Content.Load<Song>("endlevelink");

            StateManager.Instance.LoadContent(Content);

            StateManager.Instance.StartGameState("MenuState");

        }

        protected override void UnloadContent()
        {
            StateManager.Instance.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if(this.IsActive)
                StateManager.Instance.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            StateManager.Instance.Draw(gameTime, spriteBatch, GraphicsDevice);

            base.Draw(gameTime);
        }

    }
}
