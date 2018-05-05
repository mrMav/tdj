using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine.Physics;

namespace Engine
{
    public class Sprite
    {
        // game reference
        public GraphicsDeviceManager Graphics;

        // display texture
        public Texture2D Texture;
        public Rectangle TextureBoundingRect;
        public Color Tint;

        public Body Body;

        public bool IsControllable;

        // constructor
        public Sprite(GraphicsDeviceManager graphics, Texture2D texture, Vector2 position, int width, int height, bool isControllable = false)
        {
            this.Graphics = graphics;

            this.Texture = texture;
            this.IsControllable = isControllable;

            this.TextureBoundingRect = new Rectangle((int)position.X, (int)position.Y, width, height);

            this.Body = new Body(position.X, position.Y, width, height);

            this.Tint = Color.White;
        }

        // logic update
        public virtual void Update(GameTime gameTime, KeyboardState keyboardState)
        {
           
        }

        // render
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.Texture, this.Body.Position, this.TextureBoundingRect, this.Tint);
        }
    }
}
