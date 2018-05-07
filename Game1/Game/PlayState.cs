using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine;
using Engine.Physics;
using Engine.Tiled;
using TDJGame.Utils;

namespace TDJGame
{
    public class PlayState : GameState
    {

        #region [Properties]

        Camera2D camera;
        FrameCounter frameCounter = new FrameCounter();
        SpriteFont font;
        Texture2D tilemapTexture;
        Vector2 mouseWorldCoordinates;
        Level level;
        Texture2D pixel;
        EnergyBar bar;

        Player player;

        #endregion

        public PlayState(string key, GraphicsDeviceManager graphics)
        {
            Key = key;
            Graphics = graphics;
        }

        public override void Initialize()
        {
            base.Initialize();
                        
        }

        public override void LoadContent()
        {
            base.LoadContent();

            camera = new Camera2D(Vector2.Zero);
            camera.Zoom = 2.45f;

            font = content.Load<SpriteFont>("Font");
            tilemapTexture = content.Load<Texture2D>("tilemap");
            //tilemapTexture = this.content.Load<Texture2D>("SpriteSheetDraft");

            /*
             * A single pixel to draw lines and stuff
             */
            pixel = new Texture2D(Graphics.GraphicsDevice, 1, 1);
            DrawMe.Fill(pixel, Color.White);

            /*
             * Player init
             */ 
            player = new Player(Graphics, tilemapTexture, Vector2.Zero, 16, 32, true);
            player.TextureBoundingRect = new Rectangle(176, 80, 16, 32);
            player.Body.X = 16 * 3;
            player.Body.Y = 16 * 3;
            player.Body.MaxVelocity = 3f;
            player.Body.Enabled = true;
            player.Body.Tag = "player";

            /*
             * Level init
             */
            XMLLevelLoader XMLloader = new XMLLevelLoader();
            level = XMLloader.LoadLevel(@"Content\test_map_3.tmx", tilemapTexture);
            //this.level = XMLloader.LoadLevel(@"Content\prototipo.tmx", tilemapTexture);            
            //this.level.SetCollisionTiles(new int[] { 1, 2, 4, 5 });
            level.SetCollisionTiles(new int[] { 21 });

            /*
             * UI Elements init
             */ 
            bar = new EnergyBar(Graphics, new Vector2(16, 16), Graphics.PreferredBackBufferWidth - 32, 16, Color.MonoGameOrange);



            ContentLoaded = true;
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            /*
             * Input State refresh
             */ 
            KeyboardState kState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();
            
            /*
             * Player Update
             */ 
            player.UpdateMotion(gameTime, kState, level);

            foreach (Bullet b in player.Bullets)
            {
                foreach(Tile t in level.CollidableTiles)
                {
                    if(Physics.Overlap(b, t))
                    {
                        b.Kill();
                    }
                }
            }

            /*
             * UI Update
             */ 
            bar.SetPercent((int)(player.Energy * 100f / player.MaxEnergy));
            
            camera.Position = new Vector2(player.Body.Position.X, 8 * 16 );
            camera.GetTransform(Graphics.GraphicsDevice);                        
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            frameCounter.Update(deltaTime);

            graphicsDevice.Clear(Color.CornflowerBlue);

            /*
             * World Render
             */ 
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.Transform);

            level.Draw(spriteBatch);
            player.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            /* 
             * GUI render
             */ 
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            bar.Draw(spriteBatch, gameTime);
            spriteBatch.DrawString(font, $"{(int)camera.Position.X}, {(int)camera.Position.Y}, {camera.Zoom}", new Vector2(0, graphicsDevice.Viewport.Height - 16), Color.Red);
            spriteBatch.DrawString(font, $"{Math.Round(frameCounter.AverageFramesPerSecond)}", Vector2.Zero, Color.LightGreen);
            
            spriteBatch.End();

        }

        void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
        {
            Vector2 edge = end - start;

            float angle = (float)Math.Atan2(edge.Y, edge.X);

            spriteBatch.Draw(pixel,
                new Rectangle(
                    (int)start.X,
                    (int)start.Y,
                    (int)edge.Length(),
                    1),
                null,
                color,
                angle,
                new Vector2(0, 0),
                SpriteEffects.None,
                0);

        }
    }
}
 