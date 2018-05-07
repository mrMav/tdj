using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine;
using Engine.Physics;

namespace TDJGame
{
    public class PufferFish : Sprite
    {
        float TravelSpeed;
        float TravelDistance;
        float CurrentDistance;
        int FacingDirection = 1;  // 1 is right, -1 is left

        public PufferFish(GraphicsDeviceManager graphics, Texture2D texture, Vector2 position, int width, int height, float travelDistance = 32f, float travelSpeed = 0.5f)
            : base(graphics, texture, position, width, height, false)
        {
            TravelDistance = travelDistance;
            TravelSpeed = travelSpeed;

            Body.Velocity.X = TravelSpeed;
        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            Body.X += Body.Velocity.X;
            Body.Y += Body.Velocity.Y;

            CurrentDistance += Body.Velocity.X;

            if (CurrentDistance <= 0)
            {
                FacingDirection = 1;  // go right now
                Body.Velocity.X *= -1;
            }
            else if (CurrentDistance >= TravelDistance)
            {
                FacingDirection = -1;  // go left now
                Body.Velocity.X *= -1;
            }
            
        }
    }
}
