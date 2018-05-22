using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine.Physics;
using Engine.Animations;
using TDJGame.Utils;
using System;

namespace Engine
{
    public class Sprite
    {
        // game reference
        public GameState State;

        // display texture
        public Texture2D Texture;
        public Rectangle TextureBoundingRect;
        public Color Tint;

        public AnimationManager Animations;

        // physics body
        public Body Body;

        // gameplay handy properties
        public float MaxHealth = 30f;
        public float Health;
        public int Damage = 1;
        public bool Alive = true;
        public bool Visible = true;

        public int playerHealth, playerMaxHealth = 4;

        public bool IsControllable;
        public bool IsBlinking = false;
        public double BlinkingTimer = 0;
        public double BlinkingInterval = 1000;

        public int FacingDirection = 1;

        // constructor
        public Sprite(GameState state, Texture2D texture, Vector2 position, int width, int height, bool isControllable = false)
        {
            State = state;
            Texture = texture;
            Tint = Color.White;

            Animations = new AnimationManager(this, state);

            Body = new Body(position.X, position.Y, width, height);

            Health = MaxHealth;
            playerHealth = playerMaxHealth;
            IsControllable = isControllable;
                        
        }
                
        // logic update
        public virtual void Update(GameTime gameTime)
        {
            Animations.Update(gameTime);

            if (IsBlinking)
            {

                if (gameTime.TotalGameTime.TotalMilliseconds % 20 > 10)
                {

                    Tint = Utility.FadeColor(Tint, 250);

                }
                else
                {

                    Tint = Color.White;

                }

                if (gameTime.TotalGameTime.TotalMilliseconds > BlinkingTimer)
                {
                    IsBlinking = false;
                    Tint = Color.White;
                }

            }

        }

        // render
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (FacingDirection < 0)
            {

                spriteBatch.Draw(
                         Texture,
                         position: Body.Position,
                         sourceRectangle: this.Animations.CurrentFrame.TextureSourceRect,
                         effects: SpriteEffects.FlipHorizontally,
                         color: Tint
                    );

            }
            else
            {
                spriteBatch.Draw(this.Texture, this.Body.Position, this.Animations.CurrentFrame.TextureSourceRect, this.Tint);
            }
        }

        public virtual void Kill()
        {
            Alive = false;
            Visible = false;
        }

        public virtual void Revive()
        {
            Alive = true;
            Visible = true;
            Health = MaxHealth;
            playerHealth = playerMaxHealth;
        }

        public virtual void ReceiveDamage(float ammount)
        {
            Health -= ammount;

            if(Health <= 0f)
            {
                Kill();
            }

        }

        public virtual void PlayerReceiveDamage(int ammount)
        {
            playerHealth -= ammount;

            if (playerHealth <= 0)
            {
                Kill();
            }

        }

        public void StartBlinking(GameTime gameTime)
        {
            BlinkingTimer = gameTime.TotalGameTime.TotalMilliseconds + BlinkingInterval;
            IsBlinking = true;
        }

    }

}
