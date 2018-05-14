using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine;
using Engine.Animations;
using Engine.Tiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TDJGame.Utils;

namespace TDJGame
{
    public class TestObjectImport : GameState
    {

        #region [Properties]

        Camera2D Camera;
        SpriteFont font;
        Texture2D tilemapTexture;
        Level level;
        List<Sprite> enemies;

        #endregion

        public TestObjectImport(string key, GraphicsDeviceManager graphics)
        {
            Key = key;
            Graphics = graphics;
        }

        public override void Initialize()
        {
            base.Initialize();

            enemies = new List<Sprite>();

        }

        public override void LoadContent()
        {
            base.LoadContent();

            Camera = new Camera2D(Vector2.Zero);
            Camera.Zoom = 2.45f;

            font = content.Load<SpriteFont>("Font");
            tilemapTexture = this.content.Load<Texture2D>("spritesheet-jn");
                        
            /*
             * Level init
             */
            XMLLevelLoader XMLloader = new XMLLevelLoader();
            level = XMLloader.LoadLevel(this, @"Content\sampleTallLevel.tmx", tilemapTexture);
            level.SetCollisionTiles(new int[] { 1, 2, 17, 18, 33, 34 });
            
            /*
             * Enemies init
             */
            foreach(TiledObject obj in level.Objects)
            {

                if(obj.Name.ToLower() == "jellyfish")
                {
                    Vector2 center = new Vector2(obj.X + obj.Width / 2, obj.Y + obj.Height / 2);
                    Vector2 radius = new Vector2(obj.Width / 2, obj.Height / 2);
                    
                    float speed = float.Parse(obj.GetProperty("speed"));

                    JellyFish j = new JellyFish(this, tilemapTexture, Vector2.Zero, 16, 32, center, radius, speed);
                    j.Animations.CurrentFrame = new Frame(48, 112, 16, 32);

                    enemies.Add(j);

                    Console.WriteLine("added jelly");


                } else if(obj.Name.ToLower() == "pufferfish")
                {
                    Vector2 position = new Vector2(obj.X, obj.Y);

                    float speed = float.Parse(obj.GetProperty("speed"));

                    PufferFish p = new PufferFish(this, tilemapTexture, position, 32, 32, obj.Width, speed);
                    p.Animations.CurrentFrame = new Frame(0, 112, 32, 32);

                    enemies.Add(p);

                    Console.WriteLine("added puffer");

                }

            }
        
            

            ContentLoaded = true;

        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState kState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();

            #region [Camera Update]

            if (kState.IsKeyDown(Keys.Q))
            {
                Camera.Zoom += 0.1f;
            }
            else if (kState.IsKeyDown(Keys.E))
            {
                Camera.Zoom -= 0.1f;
            }

            float ammount = 4f;
            if (kState.IsKeyDown(Keys.W))
            {
                Camera.Position.Y -= ammount;
            }
            else if (kState.IsKeyDown(Keys.S))
            {
                Camera.Position.Y += ammount;
            }
            if (kState.IsKeyDown(Keys.A))
            {
                Camera.Position.X -= ammount;
            }
            else if (kState.IsKeyDown(Keys.D))
            {
                Camera.Position.X += ammount;
            }

            Camera.GetTransform(Graphics.GraphicsDevice);

            #endregion


            foreach(Sprite s in enemies)
            {
                s.Update(gameTime);
            }

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Camera.Transform);

            level.Draw(gameTime, spriteBatch);

            foreach (Sprite s in enemies)
            {
                s.Draw(gameTime, spriteBatch);
            }

            spriteBatch.End();
        }
    }
}
