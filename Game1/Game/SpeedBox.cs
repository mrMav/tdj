
using Engine;
using Engine.Physics;
using Microsoft.Xna.Framework;

namespace TDJGame
{
    public class SpeedBox
    {

        public Vector2 SpeedIncrease;

        public AABB Bounds;

        public SpeedBox(float x, float y, float width, float height, float speedXIncrease, float speedYIncrease)
        {

            Bounds = new AABB(x, y, width, height);

            SpeedIncrease.X = speedXIncrease;
            SpeedIncrease.Y = speedYIncrease;

        }

        public void ApplySpeed(GameTime gameTime, Sprite a)
        {

            float ellapsedTimeMultiplier = (float)gameTime.ElapsedGameTime.TotalSeconds * 1000f;

            a.Body.Velocity.X += SpeedIncrease.X * ellapsedTimeMultiplier;
            a.Body.Velocity.Y += SpeedIncrease.Y * ellapsedTimeMultiplier;

        }

    }
}
