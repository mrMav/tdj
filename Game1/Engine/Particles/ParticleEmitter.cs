using Engine.Animations;
using Engine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Particles
{
    public class ParticleEmitter
    {

        /*
         * The GameState that this emitter belongs too.
         */
        public GameState State { get; }

        /*
         * The area from which the particles 
         * will be emitted.
         */ 
        public AABB EmitterBox;

        /*
         * The maximum number of particles.
         */
        private int MaxParticles;

        /*
         * If this emitter is active.
         */
        public bool Activated { get; set; }

        /*
         * The array containing all the particles
         */
        public Particle[] Particles;

        /*
         * This emitter RNG
         */
        private Random Rnd;

        /*
         * Burst Mode Flag
         */
        public bool Burst;

        /*
         * How Many Particles per burst
         */
        private int _particlesPerBurst;
        public int ParticlesPerBurst
        {
            get
            {
                return _particlesPerBurst;
            }
            set
            {
                if(value > MaxParticles)
                {
                    _particlesPerBurst = MaxParticles;
                } else
                {
                    _particlesPerBurst = value;
                }
            }
        }

        #region [Particles Properties]

        public double ParticleLifespanMilliseconds;
        public double ParticleLifespanVariationMilliseconds;
        public double LastSpawnedParticleMilliseconds;
        public double SpawnRate;

        public Vector2 ParticleVelocity;
        public Vector2 XVelocityVariationRange;
        public Vector2 YVelocityVariationRange;
        public float VelocityMultiplier;

        public float InitialScale;
        public float FinalScale;

        public bool RandomizeParticlePositions;

        #endregion


        /*
         * Constructor
         */
        public ParticleEmitter(GameState state, float x, float y, int maxParticles = 100, int seed = 0)
        {
            State = state;

            EmitterBox = new AABB(x, y, 1, 1);

            MaxParticles = maxParticles;

            Rnd = new Random(seed);

            Activated = true;
            ParticleLifespanMilliseconds = 1000f;
            LastSpawnedParticleMilliseconds = 0f;
            ParticleLifespanVariationMilliseconds = 0f;
            SpawnRate = 500f;

            ParticleVelocity = Vector2.Zero;
            XVelocityVariationRange = Vector2.Zero;
            YVelocityVariationRange = Vector2.Zero;

            InitialScale = 1f;
            FinalScale = 1f;

            ParticlesPerBurst = 5;
            Burst = false;

            RandomizeParticlePositions = true;

        }

        public void MakeParticles(Texture2D particleTexture, int width, int height)
        {
            Particles = new Particle[MaxParticles];

            for (int i = 0; i < MaxParticles; i++)
            {
                Particles[i] = new Particle(State, particleTexture, new Vector2(0, 0), width, height);
                Particles[i].Spawner = this;
            }
        }

        public void MakeRandomParticles(Texture2D particleTexture, Rectangle[] rects)
        {
            Particles = new Particle[MaxParticles];
            
            for (int i = 0; i < MaxParticles; i++)
            {
                Rectangle picked = rects[Rnd.Next(0, rects.Length)];

                Particles[i] = new Particle(State, particleTexture, new Vector2(0, 0), picked.Width, picked.Height);
                Particles[i].Spawner = this;
                Particles[i].Animations.CurrentFrame = new Frame(picked.X, picked.Y, picked.Width, picked.Height);
            }
        }
        
        public void ParseTextureToParticles(Texture2D texture, int offsetX, int offsetY, int cropRectWith, int cropRectHeight, int chunkWidth = 1, int chunkHeight = 1)
        {

            MaxParticles = (cropRectWith / chunkWidth) * (cropRectHeight / chunkHeight);

            Particles = new Particle[MaxParticles];
            
            int currentParticleIndex = 0;

            for(int y = 0; y < cropRectHeight; y += chunkWidth)
            {

                for(int x = 0; x < cropRectWith; x += chunkHeight)
                {
                    Particles[currentParticleIndex] = new Particle(State, texture, new Vector2(x, y), chunkWidth, chunkHeight);
                    Particles[currentParticleIndex].Animations.SetCurrentFrame(new Frame(offsetX + x, offsetY + y, chunkWidth, chunkHeight));
                    Particles[currentParticleIndex].Spawner = this;

                    currentParticleIndex++;

                }

            }
                        
        }

        public void Update(GameTime gameTime)
        {
            if (Activated)
            {
                // spawn particle
                if (LastSpawnedParticleMilliseconds < gameTime.TotalGameTime.TotalMilliseconds)
                {
                    // update timer
                    LastSpawnedParticleMilliseconds = gameTime.TotalGameTime.TotalMilliseconds + SpawnRate;

                    if(Burst)
                    {
                        int count = 0;

                        for (int i = 0; i < MaxParticles; i++)
                        {

                            if(count >= ParticlesPerBurst)
                            {
                                break;
                            }

                            if (!Particles[i].Alive)
                            {
                                SetParticleReady(Particles[i], gameTime);
                                count++;
                            }
                        }

                        Activated = false;

                    } else
                    {
                        // get the first dead particle
                        Particle p = null;
                        for (int i = 0; i < MaxParticles; i++)
                        {
                            if (!Particles[i].Alive)
                            {
                                p = Particles[i];
                                break;
                            }
                        }

                        if (p != null)
                        {

                            SetParticleReady(p, gameTime);

                        }
                    }
                    
                }

            }

            // update all particles
            for (int i = 0; i < MaxParticles; i++)
            {
                Particle p = Particles[i];

                p.Update(gameTime);

            }

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for(int i = 0; i < MaxParticles; i++)
            {
                Particles[i].Draw(gameTime, spriteBatch);
            }
        }

        public void SetAcceleration(float x, float y)
        {
            for(int i = 0; i < MaxParticles; i++)
            {
                Particles[i].Body.Acceleration.X = x;
                Particles[i].Body.Acceleration.Y = y;
            }
        }

        public void SetDrag(float x, float y)
        {
            for (int i = 0; i < MaxParticles; i++)
            {
                Particles[i].Body.Drag.X = x;
                Particles[i].Body.Drag.Y = y;
            }
        }

        public void SetTextureCropRectangle(Rectangle rect)
        {
            for (int i = 0; i < MaxParticles; i++)
            {
                Particles[i].Animations.CurrentFrame = new Frame(rect.X, rect.Y, rect.Width, rect.Height, 0);
            }
        }

        public void SetParticleReady(Particle p, GameTime gameTime)
        {
            
            float velocityX = ParticleVelocity.X + Rnd.Next((int)XVelocityVariationRange.X, (int)XVelocityVariationRange.Y) * 0.01f;
            float velocityY = ParticleVelocity.Y + Rnd.Next((int)YVelocityVariationRange.X, (int)YVelocityVariationRange.Y) * 0.01f;

            p.Reset();
            p.Revive();

            p.InitialScale = InitialScale;
            p.FinalScale = FinalScale;

            p.SpawnedAtMilliseconds = gameTime.TotalGameTime.TotalMilliseconds;
            p.LifespanMilliseconds = ParticleLifespanMilliseconds + (float)Rnd.Next((int)-ParticleLifespanVariationMilliseconds,(int)ParticleLifespanVariationMilliseconds);

            p.Body.Velocity.X = velocityX;
            p.Body.Velocity.Y = velocityY;

            if (RandomizeParticlePositions)
            {
                p.Body.X = Rnd.Next((int)EmitterBox.Min.X, (int)EmitterBox.Max.X);
                p.Body.Y = Rnd.Next((int)EmitterBox.Min.Y, (int)EmitterBox.Max.Y);

            }
            else
            {
                p.Body.X = p.InitialPosition.X + EmitterBox.X;
                p.Body.Y = p.InitialPosition.Y + EmitterBox.Y;
            }

            // TODO: avoid square explosion by normalizing vector and multiplying by smth

        }

        public void ForEachParticle(Func<Particle, int> callback)
        {
            for(int i = 0; i < MaxParticles; i++)
            {
                callback(Particles[i]);
            }
        }
    }
}
