using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine;

namespace TDJGame
{
    public class EnergyBar
    {

        public GraphicsDeviceManager Graphics;
        public Texture2D Texture;

        public Rectangle DestinationRectangle;

        public Vector2 Position;
        public int MaxWidth;

        private int _width;
        public int Width {
            get
            {
                return _width;

            }                
            set
            {
                _width = value;
                DestinationRectangle.Width = value;
            }
        }
        private int _height;
        public int Height
        {
            get
            {
                return _height;

            }
            set
            {
                _height = value;
                DestinationRectangle.Height = value;
            }
        }

        private Color _color;
        public Color Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                DrawMe.Fill(Texture, _color);
            }
        }
        
        public EnergyBar(GraphicsDeviceManager graphics, Vector2 position, int maxWidth, int height, Color color)
        {

            Texture = new Texture2D(graphics.GraphicsDevice, 1, 1);

            Graphics = graphics;
            Position = position;
            MaxWidth = maxWidth;
            Width = MaxWidth;
            Height = height;
            Color = color;
            DestinationRectangle = new Rectangle((int)position.X, (int)position.Y, Width, height);

        }

        public void SetPercent(int percent)
        {

            Width = percent * MaxWidth / 100;

        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(Texture, DestinationRectangle, Color.White);
        }

    }
}
