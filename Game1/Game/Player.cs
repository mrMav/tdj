using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine;
using Engine.Tiled;
using Engine.Physics;
using System.Collections.Generic;
using System;
using Engine.Animations;
using TDJGame.Utils;
using Microsoft.Xna.Framework.Audio;
using Engine.Particles;

namespace TDJGame
{
    public class Player : Sprite
    {
        public bool Floating;
        public bool Press;

        public float Energy;
        public float FloatingUpSpeed;
        public float FloatingDownSpeed;
        public float MaxEnergy = 200f;
        public float EnergyDrain = 0.3f;
        public float EnergyGain = 0.8f;

        public bool IsBobing = false;
        public float BobStarted = 0f;
        public float BobAmplitude = 1f;
        public float BobFrequency = 0.005f;

        public float LastShot = 0f;
        public float ShootingVelocity = 3f;
        public float ShootRate = 80f;
        public float BulletCost = 10f;

        public float KnockBackAmmount = 10f;
        public float FireKnockBackAmmount = 2f;

        public List<Bullet> Bullets;
        public Vector2 Size;
        public ParticleEmitter movementParticleEmitter;
        public ParticleEmitter anchorParticleEmitter;

        public Player(GameState state, Texture2D texture, Vector2 position, int width, int height, bool isControllable = true)
            : base(state, texture, position, width, height, true)
        {

            FacingDirection = 1;

            Energy = MaxEnergy;
            Size = new Vector2(16, 32);

            Body.Velocity.X = 0;
            Body.Velocity.Y = -2f;

            FloatingUpSpeed = 0.8f;
            FloatingDownSpeed = FloatingUpSpeed * 2;

            Body.Acceleration.X = 1f;
            Body.MaxVelocity = 3f;
            Body.Drag.X = 0.6f;
            Body.Drag.Y = 0.6f;

            Body.SetSize(10, 26, 11, 3);

            Body.Enabled = true;
            Body.Tag = "player";

            /* Create a few bullets */
            Bullets = new List<Bullet>();
            for (int i = 0; i < 50; i++)
            {
                Bullet b = new Bullet(state, texture, Vector2.Zero, this);
                b.Animations.CurrentFrame = new Frame(0, 78, 16, 16);
                b.Body.SetSize(6, 6, 5, 5);
                b.Body.Drag.Y *= 1.1f;

                Bullets.Add(b);
            }

            movementParticleEmitter = new ParticleEmitter(State, 0, 0, 128);
            movementParticleEmitter.EmitterBox.Resize(1, 24);
            movementParticleEmitter.MakeParticles(texture, 16, 16);
            movementParticleEmitter.ParticleVelocity = new Vector2(0, 0.01f);
            movementParticleEmitter.SetAcceleration(0, -0.005f);                                            ////SEARCH HERE - Pardo////
            movementParticleEmitter.XVelocityVariationRange = new Vector2(-20f, 20f);
            movementParticleEmitter.YVelocityVariationRange = new Vector2(-40f, 40f);
            movementParticleEmitter.SetTextureCropRectangle(new Rectangle(0, 78, 16, 16));
            movementParticleEmitter.SpawnRate = 40f;
            movementParticleEmitter.ParticleLifespanMilliseconds = 750f;
            movementParticleEmitter.ParticleLifespanVariationMilliseconds = 50f;
            movementParticleEmitter.InitialScale = 0.5f;
            movementParticleEmitter.FinalScale = 1.1f;

            anchorParticleEmitter = new ParticleEmitter(State, 0, 0, 10);
            //anchorParticleEmitter.EmitterBox.Resize(1, 4);
            anchorParticleEmitter.EmitterBox.Resize(Body.Bounds.Width, Body.Bounds.Height);
            anchorParticleEmitter.MakeRandomParticles(texture, new Rectangle[] { new Rectangle(80, 256, 16, 16), new Rectangle(96, 256, 16, 16) });
            float dispersion = 200f;
            anchorParticleEmitter.XVelocityVariationRange = new Vector2(-dispersion, dispersion);
            anchorParticleEmitter.YVelocityVariationRange = new Vector2(-dispersion, dispersion);
            anchorParticleEmitter.SpawnRate = 150f;
            anchorParticleEmitter.ParticleLifespanMilliseconds = 750f;
            anchorParticleEmitter.ParticleLifespanVariationMilliseconds = 100f;
            anchorParticleEmitter.InitialScale = 0.5f;
            anchorParticleEmitter.FinalScale = 0.1f;
            anchorParticleEmitter.Burst = true;
            anchorParticleEmitter.ParticlesPerBurst = 5;
            anchorParticleEmitter.Activated = false;

            Floating = true;

            /*Animations*/

            Animations.CurrentFrame = new Frame(16, 64, 32, 32);

            Animation shootingAnim = Animations.Add("shooting", new Frame[] {new Frame(16,64,32,32), new Frame(288,64,32,32), new Frame(320,64,32,32), new Frame(352,64,32,32)
            , new Frame(384,64,32,32), new Frame(416,64,32,32) }, 10, false, false);

            Animation idleAnim = Animations.Add("idle", new Frame[] {new Frame(16,64,32,32), new Frame(288,64,32,32), new Frame(320,64,32,32), new Frame(352,64,32,32)
            , new Frame(384,64,32,32), new Frame(416,64,32,32) }, 14, false, false); //wrong values for now :D

            Animation walkingAnim = Animations.Add("walking", new Frame[] { new Frame(240, 64, 32, 32) , new Frame(80, 64, 32, 32), new Frame(112, 64, 32, 32), new Frame(144, 64, 32, 32), new Frame(176, 64, 32, 32),
            new Frame(208, 64, 32, 32)}, 8, false, false);

        }

        public void UpdateMotion(GameTime gameTime, KeyboardState keyboardState)
        {
            if (Alive)
            {
                base.Update(gameTime);

                if (this.IsControllable && keyboardState != null)
                {

                    float ellapsedTimeMultiplier = (float)gameTime.ElapsedGameTime.TotalSeconds * 1000f;

                    // move left
                    if (keyboardState.IsKeyDown(Keys.A))
                    {
                        this.Body.Velocity.X -= this.Body.Acceleration.X * ellapsedTimeMultiplier;
                        this.FacingDirection = -1;
                        this.movementParticleEmitter.Activated = true;                        
                    }
                    // move right
                    if (keyboardState.IsKeyDown(Keys.D))
                    {
                        this.Body.Velocity.X += this.Body.Acceleration.X * ellapsedTimeMultiplier;
                        this.FacingDirection = 1;
                        this.movementParticleEmitter.Activated = true;
                    }

                    if (keyboardState.IsKeyDown(Keys.Space)) // Basicly trigger
                    {
                        Press = true;                        
                    }

                    if (Press && keyboardState.IsKeyUp(Keys.Space) && !Floating) //Switch entre estados
                    {
                        Floating = !Floating;
                        Press = false;
                        //anchorParticleEmitter.Activated = true;
                    }
                    if (Press && keyboardState.IsKeyUp(Keys.Space) && Floating) //Switch entre estados
                    {
                        Floating = !Floating;
                        Press = false;
                        Energy -= 25f; // mudar para n remover valor quando player vai para cima
                        anchorParticleEmitter.Activated = true;
                        SoundEffect anchor;
                        State.SFX.TryGetValue("anchor", out anchor);
                        anchor?.Play(0.5f,-0.9f,0f);

                    }

                    Body.Drag.X = 0.6f;

                    /* Floating */

                    if (Floating)
                    {
                       

                        if (Body.Position.Y >= 0)
                        {
                            Body.Velocity.Y -= FloatingUpSpeed; //Floating Up
                        }

                        // bob a bit
                        if(Body.Position.Y <= 0f && !IsBobing)
                        {
                            IsBobing = true;
                            BobStarted = (float)gameTime.TotalGameTime.TotalMilliseconds;
                            Energy = MathHelper.Clamp(Energy + MaxEnergy / 2, 0, MaxEnergy);
                        }

                        // recharge
                        if (Energy < MaxEnergy)
                        {
                            Energy += EnergyGain;
                        }

                    } else
                    {
                        IsBobing = false;
                    }
                    
                    if (!Floating)
                    {

                        if((keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.D)) && Body.CollidingBottom)
                        {
                            Body.Drag.X = 0.055f;
                            Animations.Play("walking");
                        }

                        if (Energy <= 0)
                        { 
                            Energy = 0; //impedir que fique com valores negativos
                         
                            
                        }

                        if (Energy > 25f)
                        {
                            Body.Velocity.Y += FloatingDownSpeed; //Floating Down
                            Energy -= EnergyDrain;
                        }
                        else
                        {
                            Floating = true;
                        }
                    }

                    // makes the player bob on surface
                    if(IsBobing)
                    {

                        float x = (float)gameTime.TotalGameTime.TotalMilliseconds - BobStarted;

                        float phaseShift = 0.5f * (float)Math.PI;

                        Body.Velocity.Y = Math2.SinWave((x * BobFrequency - phaseShift), BobAmplitude);

                    }

                }

                // apply drag
                Body.Velocity.X *= Body.Drag.X;
                Body.Velocity.Y *= Body.Drag.Y;

                // cap velocity
                Body.Velocity.X = MathHelper.Clamp(Body.Velocity.X, -2f, 2f);
                Body.Velocity.Y = MathHelper.Clamp(Body.Velocity.Y, -4f, 4f);

            }
            
            // energy warning sfx
            if(Energy == 0)
            {
                SoundEffect energyWarning;
                State.SFX.TryGetValue("energyWarning", out energyWarning);
                energyWarning?.Play(0.5f, 0f, 0f);
            }
        }
        
        public bool UpdateCollisions(GameTime gameTime, Level level)
        {

            bool CameraShakeResponse = false;

            Body.PreCollisionUpdate(gameTime);
            Body.PreMovementUpdate(gameTime);

            // apply x velocity
            Body.X += Body.Velocity.X;

            // solve x collisions
            for (int i = 0; i < level.CollidableTiles.Count; i++)
            {
                if(level.CollidableTiles[i].Body.Enabled)
                {
                    Physics.Collide(this, level.CollidableTiles[i], 0); // collide in x
                }

            }

            // apply y velocity
            Body.Y += Body.Velocity.Y;

            // solve y collisions
            bool collided = false;
            for (int i = 0; i < level.CollidableTiles.Count; i++)
            {
                if (level.CollidableTiles[i].Body.Enabled)
                {

                    collided = Physics.Collide(this, level.CollidableTiles[i], 1); // collide in y           
                
                    //if the player was moving down:
                    if (collided && Body.MovingDown)
                    {
                        CameraShakeResponse = true;
                        SoundEffect fall;
                        State.SFX.TryGetValue("fall", out fall);
                        fall?.Play();
                    }

                }

            }

            // bound to world
            if (Body.Y < -16f)
            {
                Body.Y = -16f;
            }

            Body.Update(gameTime);

            return CameraShakeResponse;

        }

        public void UpdateProjectiles(GameTime gameTime, KeyboardState keyboardState)
        {

            
            if(Alive)
            {
                this.movementParticleEmitter.Update(gameTime);
                this.movementParticleEmitter.ForEachParticle(KillOutOfBoundsParticle);
                this.movementParticleEmitter.EmitterBox.X = Body.X + 11;
                this.movementParticleEmitter.EmitterBox.Y = Body.Y + 3;
                this.movementParticleEmitter.Activated = false;

                this.anchorParticleEmitter.Update(gameTime);
                this.anchorParticleEmitter.EmitterBox.X = Body.X + 8;
                this.anchorParticleEmitter.EmitterBox.Y = Body.Y + 16;
                this.anchorParticleEmitter.Activated = false;
            }

            if (keyboardState.IsKeyDown(Keys.RightControl) && Energy >= BulletCost)
            {
                Animations.Play("shooting");
                if (this.LastShot < gameTime.TotalGameTime.TotalMilliseconds)
                {
                    
                    this.LastShot = (float)gameTime.TotalGameTime.TotalMilliseconds + this.ShootRate;

                    // get the first dead bullet
                    Bullet b = null;
                    for (int i = 0; i < Bullets.Count; i++)
                    {
                        if (!Bullets[i].Alive)
                        {
                            b = Bullets[i];
                            FireKnockBack(this);
                            break;
                        }

                    }

                    if (b != null)
                    {

                        Random rnd = new Random();
                        int YVariation = 4;

                        b.Reset();
                        b.Revive();

                        b.ShotAtMilliseconds = gameTime.TotalGameTime.TotalMilliseconds;

                        b.Body.X = Body.X + (FacingDirection > 0 ? 24 : -2);
                        b.Body.Y = this.Body.Y + rnd.Next(-YVariation, YVariation) + 10;  //TODO: fix 16 offset with final sprites

                        b.Body.Velocity.X = (ShootingVelocity + (rnd.Next(-2, 2) * 0.1f)) * FacingDirection;  // some variation to the speed
                        b.Body.Velocity.Y = (rnd.Next(-3, -1) * 0.01f);  // make it float a bit

                        // subtract bullet cost to energy
                        Energy -= BulletCost;

                        float pitch = rnd.Next(-100, 10) * 0.01f;

                        SoundEffect sfx;
                        State.SFX.TryGetValue("bubble", out sfx);
                        sfx?.Play(1f, pitch, 0f);


                    }
                }

            }

            foreach (Bullet b in Bullets)
            {
                b.Update(gameTime);
            }

        }

        public int KillOutOfBoundsParticle(Particle p)
        {

            if (p.Body.Y <= 0f)
            {
                p.Kill();
            }

            return 0;

        }

        public void ApplyKnockBack(float multiplyer = 10f)
        {

            Floating = true;

            Body.Velocity.X = Body.Velocity.X * -multiplyer;
            Body.Velocity.Y = Body.Velocity.Y * -multiplyer;

        }

        public void ApplyKnockBackBasedOnSprite(Sprite sprite)
        {

            Floating = true;

            // apply based on sprite
            float intersectionAngle = (float)Math.Atan2((sprite.Body.Y - Body.Y), (sprite.Body.X - Body.X));

            Body.Velocity.X += (float)Math.Cos(intersectionAngle + Math.PI) * KnockBackAmmount;
            Body.Velocity.Y += (float)Math.Sin(intersectionAngle + Math.PI) * KnockBackAmmount;
            
        }

        public void FireKnockBack(Sprite sprite)
        {
            //apply based on sprite
            float intersectionAngle = (float)Math.Atan2((sprite.Body.Y - Body.Y), (sprite.Body.X - Body.X));

            if (this.FacingDirection == 1)
            {
                Body.Velocity.X += (float)Math.Cos(intersectionAngle + Math.PI) * FireKnockBackAmmount;
                Body.Velocity.Y += (float)Math.Sin(intersectionAngle + Math.PI) * FireKnockBackAmmount;
            }

            else
            {
                Body.Velocity.X -= (float)Math.Cos(intersectionAngle + Math.PI) * FireKnockBackAmmount;
                Body.Velocity.Y += (float)Math.Sin(intersectionAngle + Math.PI) * FireKnockBackAmmount;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            this.movementParticleEmitter.Draw(gameTime, spriteBatch);

            foreach (Bullet b in Bullets)
            {
                if(b.Visible && b.Alive)
                {
                    b.Draw(gameTime, spriteBatch);
                }
            }

            this.anchorParticleEmitter.Draw(gameTime, spriteBatch);

        }

    }
}
