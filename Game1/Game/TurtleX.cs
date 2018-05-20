using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine;
using Engine.Physics;
using System.Collections.Generic;
using System;
using Engine.Animations;

namespace TDJGame
{
    public class TurtleX : Enemy
    {        
        float TravelSpeed;
        float TravelDistance;
        public float CurrentDistance;
        float DetectSight;

        bool PlayerDetected = false;
        double MillisecondsAfterDetect = 0f;
        double ExplostionTimeOut = 2000f;

        public TurtleX(GameState state, Texture2D texture, Vector2 position, int width, int height, float sight, float travelDistance = 32f, float travelSpeed = 0.5f)
            : base(state, texture, position, width, height, 96, 112, 16)
        {
            TravelDistance = travelDistance;
            TravelSpeed = travelSpeed;

            DetectSight = sight;
            
            Body.Enabled = true;
            Body.Velocity.X = TravelSpeed;
            Body.Tag = "turtlex";
        }

        public float UpdateMov(GameTime gameTime, Player player)
        {
            base.Update(gameTime);

            if (Alive)
            {

                /* Detect player in a radius = to sight */

                Vector2 distanceDiff = Body.Position - player.Body.Position;
                
                if (distanceDiff.Length() <= DetectSight)
                {
                    PlayerDetected = true;
                }

                if (PlayerDetected)
                {

                    MillisecondsAfterDetect += gameTime.ElapsedGameTime.TotalMilliseconds;

                    double percentOfTimePassedSinceDetect = MillisecondsAfterDetect / ExplostionTimeOut;

                    byte value = (byte)(255 * (1f - percentOfTimePassedSinceDetect));
                    Tint.R = 255;
                    Tint.G = value;
                    Tint.B = value;
                    Tint.A = 255;

                    if (percentOfTimePassedSinceDetect >= 1f)
                    {
                        // explode
                        float distancePercentage = distanceDiff.Length() / DetectSight;
                        float inflictDamage = (1f - distancePercentage) * Damage;
                        
                        return inflictDamage >= 0f ? inflictDamage : -1f;

                    }

                    // chase
                    if(distanceDiff.X <= 0f)
                    {
                        if(Body.Velocity.X <= 0f)
                        {
                            ChangeDirection(1);
                        }

                    } else
                    {
                        if(Body.Velocity.X > 0f)
                        {
                            ChangeDirection(-1);
                        }
                    }

                }

                /* Movement */

                Body.X += Body.Velocity.X;
                Body.Y += Body.Velocity.Y;

                CurrentDistance += Body.Velocity.X;

                if (CurrentDistance <= 0)
                {
                    ChangeDirection(1);
                }
                else if (CurrentDistance + Body.Bounds.Width >= TravelDistance)
                {
                    ChangeDirection(-1);
                }

            }

            return 0f;

        }

        public void ChangeDirection(int newDirection)
        {
            FacingDirection = newDirection;
            Body.Velocity.X *= -1;
        }
    }
}
