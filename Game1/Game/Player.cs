using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TDJGame.Engine;

namespace TDJGame
{
    public class Player : Sprite
    {

        public Player(Texture2D pTexture, Vector2 pPosition)
            : base(pTexture, pPosition, true)
        {

        }

        public override void Update(GameTime pGameTime, KeyboardState kState)
        {
            // rotate over time
            this.angle += 0.1f;

            // controlling 
            if (this.isControllable && kState != null)
            {

                float ellapsedTimeMultiplier = 100f;

                // move left
                if (kState.IsKeyDown(Keys.A))
                {
                    this.velocity.X -= this.acceleration.X * (float)pGameTime.ElapsedGameTime.TotalSeconds * ellapsedTimeMultiplier;
                }
                // move right
                if (kState.IsKeyDown(Keys.D))
                {
                    this.velocity.X += this.acceleration.X * (float)pGameTime.ElapsedGameTime.TotalSeconds * ellapsedTimeMultiplier;
                }
                // move up
                if (kState.IsKeyDown(Keys.W))
                {
                    this.velocity.Y -= this.acceleration.Y * (float)pGameTime.ElapsedGameTime.TotalSeconds * ellapsedTimeMultiplier;
                }
                // move down
                if (kState.IsKeyDown(Keys.S))
                {
                    this.velocity.Y += this.acceleration.Y * (float)pGameTime.ElapsedGameTime.TotalSeconds * ellapsedTimeMultiplier;
                }

                // apply velocity
                this.position = this.position + this.velocity;
                // apply drag
                this.velocity *= this.drag;

            }

        }

        public override void Draw(GameTime pGameTime, SpriteBatch pSpriteBatch)
        {
            base.Draw(pGameTime, pSpriteBatch);
        }


    }
}
