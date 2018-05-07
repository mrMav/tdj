using Microsoft.Xna.Framework;
using System;
using Engine;

namespace Engine.Physics
{
    /// <summary>
    /// Physics class
    /// </summary>
    public class Physics
    {
        public static void Collide(Sprite a, Sprite b, int side)
        {
            AABB intersection = MinkowskiDifference(a.Body.Bounds, b.Body.Bounds);

            if (intersection.X <= 0 &&
               intersection.X + intersection.Width >= 0 &&
               intersection.Y <= 0 &&
               intersection.Y + intersection.Height >= 0)
            {

                SetCollisionSide(intersection, a.Body, b.Body);

                AABBPenetrationCollisionResponse(intersection, a.Body, side);
            }

        }

        public static void Collide(Sprite a, Sprite[] list, int side)
        {
            for (int i = 0; i < list.GetLength(0); i++)
            {
                if (list[i] != null)
                {
                    Collide(a, list[i], side);
                }
            }

        }

        public static bool Overlap(Sprite a, Sprite b)
        {

            AABB intersection = MinkowskiDifference(a.Body.Bounds, b.Body.Bounds);

            return intersection.X <= 0 && intersection.X + intersection.Width >= 0 && intersection.Y <= 0 && intersection.Y + intersection.Height >= 0;

        }

        public static void AABBPenetrationCollisionResponse(AABB intersection, Body body, int side)
        {
            Vector2 penetration = Vector2.Zero;
            GetMinimumPenetrationDepth(intersection, Vector2.Zero, out penetration);

            body.Intersection = penetration;

            // solve
            if (side == 0)
            {
                body.X -= penetration.X;                
            }
            else
            {
                body.Y -= penetration.Y;                
            }

        }

        public static void SetCollisionSide(AABB minkowski, Body a, Body b)
        {

            float dx = a.Bounds.CenterX - b.Bounds.CenterX;
            float dy = a.Bounds.CenterY - b.Bounds.CenterY;

            float wy = minkowski.Width * dy;
            float hx = minkowski.Height * dx;

            if (wy > hx)
            {
                if (wy > -hx)
                {
                    a.CollidingUp = true;
                    b.CollidingBottom = true;
                }
                else
                {
                    a.CollidingLeft = true;
                    b.CollidingRight = true;
                }
            }
            else
            {
                if (wy > -hx)
                {
                    a.CollidingRight = true;
                    b.CollidingLeft = true;
                }
                else
                {
                    a.CollidingBottom = true;
                    b.CollidingUp = true;
                }
            }
        }

        public static void GetMinimumPenetrationDepth(AABB rect, Vector2 point, out Vector2 result)
        {
            Vector2 min = rect.Min;
            Vector2 max = rect.Max;

            float minDist = Math.Abs(point.X - min.X);

            result.X = min.X;
            result.Y = point.Y;

            if (Math.Abs(max.X - point.X) < minDist)
            {
                minDist = Math.Abs(max.X - point.X);

                result.X = max.X;
                result.Y = point.Y;
            }
            if (Math.Abs(max.Y - point.Y) < minDist)
            {
                minDist = Math.Abs(max.Y - point.Y);

                result.X = point.X;
                result.Y = max.Y;
            }
            if (Math.Abs(min.Y - point.Y) < minDist)
            {
                minDist = Math.Abs(min.Y - point.Y);

                result.X = point.X;
                result.Y = min.Y;
            }

        }

        public static AABB MinkowskiDifference(AABB a, AABB b)
        {
            Vector2 topleft = a.Min - b.Max;
            Vector2 size = new Vector2(a.Width + b.Width, a.Height + b.Height);

            return new AABB(
                topleft.X,
                topleft.Y,
                size.X,
                size.Y
            );
        }
    }
}
