using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine;
using Engine.Physics;
using System;
using Engine.Animations;

namespace TDJGame
{
    public class JellyFish : Enemy
    {
        float TravelSpeed;
        Vector2 CenterPoint;
        Vector2 Radius;
        
        public JellyFish(GameState state, Texture2D texture, Vector2 position, int width, int height, Vector2 centerPoint, Vector2 radius, float travelSpeed = 0.5f)
            : base(state, texture, position, width, height, 48, 112)
        {
            TravelSpeed = travelSpeed;

            CenterPoint = centerPoint;
            Radius = radius;

            Animations.CurrentFrame = new Frame(12 * 16, 0 * 16, 16, 32);
            
            Body.Enabled = true;
            Body.Tag = "jellyfish";
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float threshold = 0.002f;

            Body.X = CenterPoint.X + (float)Math.Cos(gameTime.TotalGameTime.TotalMilliseconds * threshold * TravelSpeed) * Radius.X;
            Body.Y = CenterPoint.Y + (float)Math.Sin(gameTime.TotalGameTime.TotalMilliseconds * threshold * TravelSpeed) * Radius.Y;
            
        }
    }
}
