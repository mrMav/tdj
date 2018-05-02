using System;
using Microsoft.Xna.Framework;

namespace TDJGame.Engine.Physics
{
    /// <summary>
    /// Represents a Physics body
    /// </summary>
    public class Body
    {
        public string Tag;

        public Vector2 Position;
        public Vector2 Acceleration;
        public Vector2 Velocity;
        public Vector2 Origin;

        public Rectangle BoundingRect;

        public float X
        {
            get
            {
                return this.Position.X;
            }
            set
            {
                this.Position.X = value;
            }
        }

        public float Y
        {
            get
            {
                return this.Position.Y;
            }
            set
            {
                this.Position.Y = value;
            }
        }

        public float Width
        {
            get
            {
                return this.BoundingRect.Width;
            }
        }

        public float Height
        {
            get
            {
                return this.BoundingRect.Height;
            }
        }

        // TODO: min and max should be revisited for performance improvment
        public Vector2 Min
        {
            get
            {
                return new Vector2(X, Y);
            }
        }
        public Vector2 Max
        {
            get
            {
                return new Vector2(X + Width, Y + Height);
            }
        }

        public float HalfWidth { get; set; }
        public float HalfHeight { get; set; }
        
        public float Angle { get; set; }
        public float Drag { get; set; }

        public bool Enabled { get; set; }

        public bool CollidingUp { get; set; }
        public bool CollidingRight { get; set; }
        public bool CollidingBottom { get; set; }
        public bool CollidingLeft { get; set; }

        public bool MovingUp { get; set; }
        public bool MovingRight { get; set; }
        public bool MovingBottom { get; set; }
        public bool MovingLeft { get; set; }



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

            this.HalfWidth = width / 2;
            this.HalfHeight = height / 2;

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
            this.BoundingRect.X = (int)this.X;
            this.BoundingRect.Y = (int)this.Y;
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

            this.HalfWidth = width / 2;
            this.HalfHeight = height / 2;

        }

        public void ResetCollisions()
        {
            CollidingUp = false;
            CollidingRight = false;
            CollidingBottom = false;
            CollidingLeft = false;
        }

        public void ResetMovingDirections()
        {
            MovingUp = false;
            MovingRight = false;
            MovingBottom = false;
            MovingLeft = false;
        }

    }
}
