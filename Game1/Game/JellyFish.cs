using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine;
using Engine.Physics;
using System;
using Engine.Animations;
using Microsoft.Xna.Framework.Audio;

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

            Animations.CurrentFrame = new Frame(16, 176, 32, 32);
            Body.SetSize(16,16,9,6);
            
            Body.Enabled = true;
            Body.Tag = "jellyfish";
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float threshold = 0.002f;

            Body.X = CenterPoint.X + (float)Math.Cos(FacingDirection * gameTime.TotalGameTime.TotalMilliseconds * threshold * TravelSpeed) * Radius.X;
            Body.Y = CenterPoint.Y + (float)Math.Sin(FacingDirection * gameTime.TotalGameTime.TotalMilliseconds * threshold * TravelSpeed) * Radius.Y;
            
        }

        public override void Kill()
        {
            base.Kill();
            SoundEffect jDeath;
            State.SFX.TryGetValue("enemyDeath", out jDeath);
            jDeath?.Play(1, 0.65f, 0);
        }
    }
}
