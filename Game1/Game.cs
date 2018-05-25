using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace TDJGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class TDJGame : Game
    {

        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Song menuSong;
        
        public TDJGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;

        }

        protected override void Initialize()
        {
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();

            StateManager.Instance.AddGameState(new StartupState("StartupState", graphics));
            StateManager.Instance.AddGameState(new MenuState("MenuState", graphics));
            StateManager.Instance.AddGameState(new PlayState("PlayState", graphics));
            StateManager.Instance.AddGameState(new CreditsState("CreditsState", graphics));
            StateManager.Instance.AddGameState(new TestParticleEmitter("TestParticles", graphics));
            StateManager.Instance.AddGameState(new TestObjectImport("TestObjectImport", graphics));
            StateManager.Instance.AddGameState(new TestAnimations("TestAnimations", graphics));
            StateManager.Instance.AddGameState(new TestCamera("TestCamera", graphics));
            StateManager.Instance.AddGameState(new FeatureTestMap("FeatureTestMap", graphics));
            StateManager.Instance.AddGameState(new FinalKarmaState("FinalKarmaState", graphics));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //menuSong = Content.Load<Song>("menuSong");
            //MediaPlayer.Play(menuSong);

            StateManager.Instance.LoadContent(Content);

            StateManager.Instance.StartGameState("FeatureTestMap");

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
