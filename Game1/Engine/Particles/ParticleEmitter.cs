using Engine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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
        public int MaxParticles { get; }

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

            ParticlesPerBurst = 5;
            Burst = false;
        }

        public void MakeParticles(Texture2D particleTexture, int width, int height)
        {
            Particles = new Particle[MaxParticles];

            for (int i = 0; i < MaxParticles; i++)
            {
                Particles[i] = new Particle(State.Graphics, particleTexture, new Vector2(0, 0), width, height);
                Particles[i].Spawner = this;
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
                Particles[i].TextureBoundingRect = rect;
            }
        }

        public void SetParticleReady(Particle p, GameTime gameTime)
        {
            float spawnAtX = Rnd.Next((int)EmitterBox.Min.X, (int)EmitterBox.Max.X);
            float spawnAtY = Rnd.Next((int)EmitterBox.Min.Y, (int)EmitterBox.Max.Y);

            float velocityX = ParticleVelocity.X + Rnd.Next((int)XVelocityVariationRange.X, (int)XVelocityVariationRange.Y) * 0.01f;
            float velocityY = ParticleVelocity.Y + Rnd.Next((int)YVelocityVariationRange.X, (int)YVelocityVariationRange.Y) * 0.01f;

            p.Reset();
            p.Revive();

            p.SpawnedAtMilliseconds = gameTime.TotalGameTime.TotalMilliseconds;
            p.LifespanMilliseconds = ParticleLifespanMilliseconds + (float)Rnd.Next((int)-ParticleLifespanVariationMilliseconds,(int)ParticleLifespanVariationMilliseconds);

            p.Body.Velocity.X = velocityX;
            p.Body.Velocity.Y = velocityY;

            p.Body.X = spawnAtX;
            p.Body.Y = spawnAtY;
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
