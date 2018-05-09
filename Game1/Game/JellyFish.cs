using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine;
using Engine.Physics;
using System;

namespace TDJGame
{
    public class JellyFish : Sprite
    {
        float TravelSpeed;
        Vector2 CenterPoint;
        Vector2 Radius;
        
        public JellyFish(GraphicsDeviceManager graphics, Texture2D texture, Vector2 position, int width, int height, Vector2 centerPoint, Vector2 radius, float travelSpeed = 0.5f)
            : base(graphics, texture, position, width, height, false)
        {
            TravelSpeed = travelSpeed;

            CenterPoint = centerPoint;
            Radius = radius;

            TextureBoundingRect = new Rectangle(12 * 16, 0 * 16, 16, 32);
            
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
