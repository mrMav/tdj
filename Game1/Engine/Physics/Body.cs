using Microsoft.Xna.Framework;

namespace TDJGame.Engine.Physics
{
    /// <summary>
    /// Represents a Physics body
    /// </summary>
    public class Body
    {

        public Vector2 Position;
        public Vector2 Acceleration;
        public Vector2 Velocity;
        public Vector2 Origin;

        public Rectangle BoundingRect;

        public float Angle { get; set; }
        public float Drag { get; set; }

        public bool Enabled { get; set; }
        
        /// <summary>
        /// Creates a body at the position
        /// </summary>
        /// <param name="position">The body position</param>
        public Body(Vector2 position, int width, int height)
        {
            Position = position;
            Acceleration = Vector2.Zero;
            Velocity = Vector2.Zero;
            Origin = new Vector2(0.5f, 0.5f);

            BoundingRect = new Rectangle((int)position.X, (int)position.Y, width, height);

            Angle = 0.0f;
            Drag = 1.0f;

            Enabled = false;
        }

        /// <summary>
        /// Updates this body logic.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (Enabled)
            {
                
            }
        }

        /// <summary>
        /// Resize this body size.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void ResizeBody(int width, int height)
        {
            BoundingRect.Width = width;
            BoundingRect.Height = height;
        }

    }
}
