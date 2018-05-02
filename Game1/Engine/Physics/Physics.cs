using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TDJGame.Engine.Tiled;

namespace TDJGame.Engine.Physics
{
    /// <summary>
    /// Physics class
    /// </summary>
    public class Physics
    {
        public static void CollideMovingSpriteWithListOfStaticObjects(Sprite sprite, List<Tile> list)
        {

            Console.WriteLine("----- Init collision check");
            
            foreach(Tile tile in list)
            {

                if(tile.Body.Enabled && sprite.Body.Enabled)
                {
                    Vector2 hitNormal = Vector2.Zero;
                    Vector2 outVel = Vector2.Zero;

                    bool result = Physics.SweptAABB2(sprite.Body, tile.Body, out outVel, out hitNormal);
                    //bool hasCollision = Physics.CheckCollision(sprite.Body, tile.Body);

                    Console.WriteLine();
                    Console.WriteLine(result.ToString() + hitNormal + outVel);
                    
                }

            }

        }

        public static bool CheckCollision(Body a, Body b)
        {
            a.ResetCollisions();
            b.ResetCollisions();

            bool hasCollision = a.BoundingRect.Intersects(b.BoundingRect);

            if (hasCollision)
            {
                /* collision bellow b
                 * -----
                 * | b |
                 * -----
                 * -----
                 * | a |
                 * -----
                 */
                if(a.Y <= b.Y + b.Height)
                {
                    a.CollidingUp = true;

                    b.CollidingBottom = true;
                }

                /* collision above b
                 * -----
                 * | a |
                 * -----
                 * -----
                 * | b |
                 * -----
                 */
                if (a.Y + a.Height >= b.Y)
                {
                    a.CollidingBottom = true;

                    b.CollidingUp = true;                                        
                }

                /* collision left of b
                 * ----- -----
                 * | a | | b |
                 * ----- -----
                 */
                if (a.X + a.Width >= b.X)
                {
                    a.CollidingRight = true;

                    b.CollidingLeft = true;
                }

                /* collision right of b
                 * ----- -----
                 * | b | | a |
                 * ----- -----
                 */
                if (a.X <= b.X + b.Height)
                {
                    a.CollidingRight = true;

                    b.CollidingLeft = true;
                }


                //// solve when b is moving up
                //if(b.MovingUp && b.CollidingUp)
                //{
                //    b.Y = a.Y + a.Height;
                //}

                //// solve when b is moving down
                //if (b.MovingBottom && b.CollidingBottom)
                //{
                //    b.Y = a.Y - b.Height;
                //}

                //// solve when b is moving left
                //if (b.MovingLeft && b.CollidingLeft)
                //{
                //    b.X = a.X + a.Width;
                //}

                //// solve when b is moving right
                //if (b.MovingRight && b.CollidingRight)
                //{
                //    b.X = a.X - b.Width;
                //}

            }

            return hasCollision;
        }

        /// <see>
        /// https://www.gamedev.net/articles/programming/general-and-gameplay-programming/swept-aabb-collision-detection-and-response-r3084/?page=2&tab=comments
        /// </see>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static float SweptAABB(Body a, Body b, out Vector2 normal)
        {
            Vector2 invEntry = new Vector2();
            Vector2 invExit = new Vector2();

            // find the distance between the objects on the near and far sides for both x and y
            if (a.Velocity.X > 0.0f)
            {
                invEntry.X = b.X - (a.X + a.Width);
                invExit.X = (b.X + b.Width) - a.X;
            }
            else
            {
                invEntry.X = (b.X + b.Width) - a.X;
                invExit.X = b.X - (a.X + a.Width);
            }

            if(a.Velocity.Y > 0.0f)
            {
                invEntry.Y = b.Y - (a.Y + a.Height);
                invExit.Y = (b.Y + b.Height) - a.Y;
            }
            else
            {
                invEntry.Y = (b.Y + b.Height) - a.Y;
                invExit.Y = b.Y - (a.Y + a.Height);
            }


            // find time of collision and time of leaving for each axis
            // (if statement is to prevent divide by zero)
            Vector2 entry = new Vector2();
            Vector2 exit = new Vector2();

            if (a.Velocity.X == 0.0f)
            {
                entry.X = float.MinValue;
                exit.X = float.MaxValue;
            }
            else
            {
                entry.X = invEntry.X / a.Velocity.X;
                exit.X = invExit.X / a.Velocity.X;                
            }

            if (a.Velocity.Y == 0.0f)
            {
                entry.Y = float.MinValue;
                exit.Y = float.MaxValue;
            }
            else
            {
                entry.Y = invEntry.Y / a.Velocity.Y;
                exit.X = invExit.Y / a.Velocity.Y;
            }

            if (entry.X > 1.0f) entry.X = float.MinValue;
            if (entry.Y > 1.0f) entry.Y = float.MinValue;

            // find the earliest/latest times of collision
            float entryTime = Math.Max(entry.X, entry.Y);
            float exitTime = Math.Min(exit.X, exit.Y);

            // if there was no collision            
            //if(entryTime > exitTime || entry.X < 0.0f && entry.Y < 0.0f || entry.X > 1.0f || entry.Y > 1.0f)
            if(
                (entryTime > exitTime || entry.X < 0.0f && entry.Y < 0.0f) ||
                (entry.Y < 0.0f && (a.X + a.Width < b.X || a.X > b.X + b.Width)) ||
                entry.Y < 0.0f && (a.Y + a.Height < b.Y || a.Y > b.X + b.Height))
            {
                //Console.WriteLine("No Collision!");

                normal.X = 0.0f;
                normal.Y = 0.0f;

                return 1.0f;
            }
            else // if there was indeed a collision
            {

                Console.WriteLine("Collision!");

                // calcultate normal of collided surface
                if(entry.X > entry.Y)
                {
                    if(invEntry.X < 0.0f)
                    {
                        normal.X = 1.0f;
                        normal.Y = 0.0f;
                    }
                    else
                    {
                        normal.X = -1.0f;
                        normal.Y = 0.0f;
                    }
                }
                else
                {
                    if(invEntry.Y < 0.0f)
                    {
                        normal.X = 0.0f;
                        normal.Y = 1.0f;
                    }
                    else
                    {
                        normal.X = 0.0f;
                        normal.Y = -1.0f;
                    }
                }

            }

            return entryTime;

        }

        /// <summary>
        /// Sweep a in the direction of v against b, returns true & info if there was a hit
        /// </summary>
        /// <see>
        /// https://gamedev.stackexchange.com/questions/28577/2d-aabb-vs-aabb-sweep-how-to-calculate-hit-normal
        /// </see>
        /// <returns>True if collision</returns>
        public static bool SweptAABB2(Body a, Body b, out Vector2 outVel, out Vector2 hitNormal)
        {
            //Initialise out info
            Vector2 v = new Vector2(a.Velocity.X, a.Velocity.Y);

            outVel = v;
            hitNormal = Vector2.Zero;

            // Return early if a & b are already overlapping
            if (a.BoundingRect.Intersects(b.BoundingRect)) return false;

            // Treat b as stationary, so invert v to get relative velocity
            v = -v;

            float hitTime = 0.0f;
            float outTime = 1.0f;
            Vector2 overlapTime = Vector2.Zero;

            /* (Min) ___
             *      |   |
             *      |___|
             *           (Max)
             */

            // X axis overlap
            if (v.X < 0)
            {
                if (b.Max.X < a.Min.X) return false;
                if (b.Max.X > a.Min.X) outTime = Math.Min((a.Min.X - b.Max.X) / v.X, outTime);

                if (a.Max.X < b.Min.X)
                {
                    overlapTime.X = (a.Max.X - b.Min.X) / v.X;
                    hitTime = Math.Max(overlapTime.X, hitTime);
                }
            }
            else if (v.X > 0)
            {
                if (b.Min.X > a.Max.X) return false;
                if (a.Max.X > b.Min.X) outTime = Math.Min((a.Max.X - b.Min.X) / v.X, outTime);

                if (b.Max.X < a.Min.X)
                {
                    overlapTime.X = (a.Min.X - b.Max.X) / v.X;
                    hitTime = Math.Max(overlapTime.X, hitTime);
                }
            }

            if (hitTime > outTime) return false;

            //=================================

            // Y axis overlap
            if (v.Y < 0)
            {
                if (b.Max.Y < a.Min.Y) return false;
                if (b.Max.Y > a.Min.Y) outTime = Math.Min((a.Min.Y - b.Max.Y) / v.Y, outTime);

                if (a.Max.Y < b.Min.Y)
                {
                    overlapTime.Y = (a.Max.Y - b.Min.Y) / v.Y;
                    hitTime = Math.Max(overlapTime.Y, hitTime);
                }
            }
            else if (v.Y > 0)
            {
                if (b.Min.Y > a.Max.Y) return false;
                if (a.Max.Y > b.Min.Y) outTime = Math.Min((a.Max.Y - b.Min.Y) / v.Y, outTime);

                if (b.Max.Y < a.Min.Y)
                {
                    overlapTime.Y = (a.Min.Y - b.Max.Y) / v.Y;
                    hitTime = Math.Max(overlapTime.Y, hitTime);
                }
            }

            if (hitTime > outTime) return false;

            // Scale resulting velocity by normalized hit time
            outVel = -v * hitTime;

            // Hit normal is along axis with the highest overlap time
            if (overlapTime.X > overlapTime.Y)
            {
                hitNormal = new Vector2(Math.Sign(v.X), 0);
            }
            else
            {
                hitNormal = new Vector2(0, Math.Sign(v.Y));
            }

            return true;
        }

    }
}
