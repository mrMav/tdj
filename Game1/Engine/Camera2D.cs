using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engine
{
    
    /// <see>
    /// http://www.david-amador.com/2009/10/xna-camera-2d-with-zoom-and-rotation/
    /// https://gamedev.stackexchange.com/questions/59301/xna-2d-camera-scrolling-why-use-matrix-transform
    /// </see>
    public class Camera2D
    {

        public float _zoom;
        public float Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                if(value >= 0.1f)
                {
                    _zoom = value;
                } else
                {
                    _zoom = 0.1f;
                }
            }
        }

        protected float Rotation { get; set; }

        public Matrix Transform { get; set; }

        public Vector2 Position;
        public Vector2 ShakeOffset;

        public float StartedShakeMilliseconds;
        public float ShackingInterval;
        public float ShackingAmplitude;

        public bool Shaking;

        public Camera2D(Vector2 position)
        {
            this._zoom = 1.0f;
            this.Rotation = 0.0f;
            this.Position = position == null ? Vector2.Zero : position;
            this.ShakeOffset = Vector2.Zero;
            this.Shaking = false;
        }

        public void Move(float x, float y)
        {
            this.Position.X += x;
            this.Position.Y += y;
        }

        public void Update(GameTime gameTime, GraphicsDevice graphicsDevice)
        {

            if(gameTime.TotalGameTime.TotalMilliseconds > ShackingInterval + StartedShakeMilliseconds)
            {
                Shaking = false;                
                ShakeOffset = Vector2.Zero;

            } else
            {
                UpdateShake(gameTime);
                Position += ShakeOffset;
            }


            GetTransform(graphicsDevice);

        }

        public void GetTransform(GraphicsDevice graphicsDevice)
        {
            Matrix invpos = Matrix.CreateTranslation(new Vector3(-this.Position.X, -this.Position.Y, 0));
            Matrix scale = Matrix.CreateScale(new Vector3(this.Zoom, this.Zoom, 1.0f));
            Matrix rot = Matrix.CreateRotationZ(this.Rotation);
            Matrix trans = Matrix.CreateTranslation(new Vector3(
                    graphicsDevice.Viewport.Width * 0.5f,
                    graphicsDevice.Viewport.Height * 0.5f,
                    0));

            this.Transform = invpos * rot * scale * trans;

        }

        /// <summary>
        /// Returns the given screen space position, transformed to world space position
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector2 GetScreenToWorldPosition(Vector2 v)
        {            
            return Vector2.Transform(v, Matrix.Invert(this.Transform));
        }

        /// <summary>
        /// Returns the given world space position, transformed to screen space position
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector2 GetWorldToScreenPosition(Vector2 v)
        {
            return Vector2.Transform(v, this.Transform);
        }

        public void ActivateShake(GameTime gameTime, float interval = 150f, float amplitude = 16f)
        {

            this.Shaking = true;
            this.StartedShakeMilliseconds = (float)gameTime.TotalGameTime.TotalMilliseconds;
            this.ShackingInterval = interval;
            this.ShackingAmplitude = amplitude;

        }

        public void UpdateShake(GameTime gameTime)
        {

            // https://www.graphpad.com/guides/prism/7/curve-fitting/reg_damped_sine_wave.htm?toc=0&printWindow

            float gameMs = (float)gameTime.TotalGameTime.TotalMilliseconds - StartedShakeMilliseconds;
            float initialAmplitude = ShackingAmplitude;
            float decayConstant = 0.001f;
            float angularFrequency = ShackingInterval;
            float y = initialAmplitude *
                (float)Math.Exp(-decayConstant * gameMs) *
                (float)Math.Sin((2 * Math.PI * gameMs / angularFrequency) + Math.PI * 2);

            // Amplitude is the height of top of the waves, in Y units.
            // Wavelength is the time it takes for a complete cycle, in units of X
            // Frequency is the number of cycles per time unit.It is calculated as the reciprocal of wavelength, and is expressed in the inverse of the time units of X.
            // PhaseShift in radians.A phaseshift of 0 sets Y equal to 0 at X = 0.
            // K is the decay constant, in the reciprocal of the time units of the X axis.
            // HalfLlife is the time it takes for the maximum amplitude to decrease by a factor of 2.It is computed as 0.693 / K.
            
            this.ShakeOffset.Y = y;

        }

        public string GetDebugString()
        {
            return $"Position: {Position}, Zoom {Zoom}\nShaking: {Shaking}, ShakeOffset: {ShakeOffset}\nShakeInterval: {ShackingInterval}, ShackingAmplitude: {ShackingAmplitude}";
        }

    }
}
