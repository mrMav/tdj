using Microsoft.Xna.Framework;

namespace Engine.Physics
{
    /// <summary>
    /// Represents a Physics body
    /// </summary>
    public class Body
    {
        public string Tag;

        public Vector2 Acceleration;
        public Vector2 Velocity;
        public Vector2 Origin;
        public float MaxVelocity;

        public Vector2 PreviousPosition;

        public Vector2 Delta;

        public Vector2 Intersection;

        public AABB Bounds;

        public float X
        {
            get
            {
                return Bounds.X;
            }
            set
            {
                Bounds.X = value;
            }
        }

        public float Y
        {
            get
            {
                return Bounds.Y;
            }
            set
            {
                Bounds.Y = value;
            }
        }

        public Vector2 Position
        {
            get
            {
                return Bounds.Position;
            }
        }

        public float Angle { get; set; }
        public float Drag { get; set; }

        public bool Enabled { get; set; }

        public bool CollidingUp { get; set; }
        public bool CollidingRight { get; set; }
        public bool CollidingBottom { get; set; }
        public bool CollidingLeft { get; set; }

        public bool MovingUp { get; set; }
        public bool MovingRight { get; set; }
        public bool MovingDown { get; set; }
        public bool MovingLeft { get; set; }

        public bool IsOnFloor { get; set; }

        /// <summary>
        /// Creates a body at the position
        /// </summary>
        /// <param name="position">The body position</param>
        public Body(float x, float y, int width, int height)
        {
            Acceleration = Vector2.Zero;
            Velocity = Vector2.Zero;
            Origin = new Vector2(0.5f, 0.5f);

            Bounds = new AABB(x, y, width, height);

            Angle = 0.0f;
            Drag = 1.0f;
            MaxVelocity = 1000f;

            Enabled = false;
        }
        
        /// <summary>
        /// Updates this body logic.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {

            ResetMovingDirections();

            // determing which side the body is moving
            if (DeltaY() > 0)
            {
                MovingDown = true;
                MovingUp = false;
            }
            else if (DeltaY() < 0)
            {
                MovingUp = true;
                MovingDown = false;
            }
            else
            {
                MovingUp = false;
                MovingDown = false;
            }

            if (DeltaX() > 0)
            {
                MovingRight = true;
                MovingLeft = false;
            }
            else if (DeltaX() < 0)
            {
                MovingLeft = true;
                MovingRight = false;
            }
            else
            {
                MovingRight = false;
                MovingLeft = false;
            }

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
            MovingDown = false;
            MovingLeft = false;
        }

        public float DeltaX()
        {
            return PreviousPosition.X - Position.X;
        }

        public float DeltaY()
        {
            return PreviousPosition.Y - Position.Y;
        }

        public string GetDebugString()
        {
            string debug = $"Moving Up: {MovingUp}, Moving Down: {MovingDown}, Moving Right: {MovingRight}, Moving Left: {MovingLeft}\n";
            debug += $"Colliding Up: {CollidingUp}, Colliding Down: {CollidingBottom}, Colliding Right: {CollidingRight}, Colliding Left: {CollidingLeft}\n";
            debug += $"Position: {Position}\n";
            debug += $"Prev Pos: {PreviousPosition}\n";
            debug += $"DeltaX: {DeltaX()}\n";
            debug += $"DeltaY: {DeltaY()}\n";
            debug += $"Velocity: {Velocity}\n";

            return debug;

        }

    }
}
