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
    public class TurtleX : Sprite
    {
        PlayState play;

        float TravelSpeed;
        float TravelDistance;
        public float CurrentDistance;
        public int FacingDirection = 1;  // 1 is right, -1 is left
        int detectSight;

        public Vector2 Size;

        public TurtleX(GameState state, Texture2D texture, Vector2 position, int width, int height, int sight, float travelDistance = 32f, float travelSpeed = 0.5f)
            : base(state, texture, position, width, height, false)
        {
            TravelDistance = travelDistance;
            TravelSpeed = travelSpeed;

            detectSight = sight;

            Animations.CurrentFrame = new Frame(9 * 16, 0, 32, 32);

            Body.Enabled = true;
            Body.Velocity.X = TravelSpeed;
            Body.Tag = "turtlex";
        }

        public void UpdateMov(GameTime gameTime, Player player)
        {
            base.Update(gameTime);

            Body.X += Body.Velocity.X;
            Body.Y += Body.Velocity.Y;

            CurrentDistance += Body.Velocity.X;

            if ((Math.Abs(player.Body.X - Body.X) < detectSight))
            {
                if ((Math.Abs(player.Body.X - Body.X) < 30) && (Math.Abs(player.Body.Y - Body.Y) < 30))
                {
                    player.ReceiveDamage(10);
                    Kill();
                }

                if (player.Body.X - Body.X <= 0)
                {
                    FacingDirection = 0;
                    Body.Velocity.X = -0.7f;
                }
                else if (player.Body.X - Body.X > 0)
                {
                    FacingDirection = 1;
                    Body.Velocity.X = 0.7f;
                }
            }
             
            else
            {
                if (CurrentDistance <= 0) //arranjar maneira do inimigo começar na posição final do x que é = obj.width ou seja andar no sentido oposto
                {
                    FacingDirection = 1;  // go right now
                    Body.Velocity.X *= -1;
                }
                else if (CurrentDistance + Body.Bounds.Width >= TravelDistance)
                {
                    FacingDirection = -1;  // go left now
                    Body.Velocity.X *= -1;
                }
            }
        }

        // render
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Visible)
            {

                if(FacingDirection > 0)
                {

                    //spriteBatch.Draw(
                    //     Texture2D texture,
                    //     Rectangle destinationRectangle,
                    //     Nullable<Rectangle> sourceRectangle,
                    //     Color color,
                    //     float rotation,
                    //     Vector2 origin,
                    //     SpriteEffects effects,
                    //     float layerDepth
                    //);

                    spriteBatch.Draw(
                             Texture,
                             position: Body.Position,
                             sourceRectangle: this.Animations.CurrentFrame.TextureSourceRect,
                             effects: SpriteEffects.FlipHorizontally,
                             color: Tint
                        );

                } else
                {
                    spriteBatch.Draw(this.Texture, this.Body.Position, this.Animations.CurrentFrame.TextureSourceRect, this.Tint);
                }
            }
        }

    }
}
