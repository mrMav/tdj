using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TDJGame.Engine.Physics;

namespace TDJGame.Engine
{
    public class Sprite
    {
        // display texture
        public Texture2D Texture;
        public Rectangle TextureBoundingRect;

        public Body Body;

        public bool IsControllable;

        // constructor
        public Sprite(Texture2D texture, Vector2 position, int width, int height, bool isControllable = false)
        {
            this.Texture = texture;
            this.IsControllable = isControllable;

            this.TextureBoundingRect = new Rectangle((int)position.X, (int)position.Y, width, height);

            this.Body = new Body(position, width, height);                        
        }

        // logic update
        public virtual void Update(GameTime gameTime, KeyboardState keyboardState)
        {
           
        }

        // render
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.Texture, this.Body.Position, this.TextureBoundingRect, Color.White);
        }
    }
}
