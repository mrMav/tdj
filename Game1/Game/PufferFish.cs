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

            TextureBoundingRect = new Rectangle(9 * 16, 0, 32, 32);

            Body.Enabled = true;
            Body.Velocity.X = TravelSpeed;
            Body.Tag = "pufferfish";
        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            base.Update(gameTime, keyboardState);

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
                             sourceRectangle: TextureBoundingRect,
                             effects: SpriteEffects.FlipHorizontally,
                             color: Tint
                        );

                } else
                {
                    spriteBatch.Draw(this.Texture, this.Body.Position, this.TextureBoundingRect, this.Tint);
                }

            }
        }

    }
}
