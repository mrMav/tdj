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
        public Vector2 TargetPosition;
        public Vector2 OriginalPosition;
        public Vector2 ShakeOffset;

        public float Drag = 0.8f;

        public float StartedShakeMilliseconds;
        public float ShackingInterval;
        public float ShackingAmmount;
        public float ShackingFrequency;
        public float ZoomShakeMultiplyer;

        public bool ShakeZoom;
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
            // need to reset the zoom;
            Zoom = 2.45f;

            if(Shaking)
            {
                if(gameTime.TotalGameTime.TotalMilliseconds > ShackingInterval + StartedShakeMilliseconds)
                {
                    Shaking = false;
                    ShakeOffset = Vector2.Zero;

                    StartedShakeMilliseconds = 0;
                    ShackingInterval = 0;
                    ShackingAmmount = 0;
                    ShackingFrequency = 0;
                    ZoomShakeMultiplyer = 0;

                    ShakeZoom = false;
                    
                } else
                {
                    UpdateShake(gameTime);

                    Position += ShakeOffset;

                    if(ShakeZoom)
                        Zoom += ShakeOffset.Y * ZoomShakeMultiplyer;

                }

            }

            // after shacking, we must reposition the camera.
            //Vector2 distToOrigin = OriginalPosition - Position;
            //distToOrigin *= 0.6f;
            //Move(distToOrigin.X, distToOrigin.Y);

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

        public void ActivateShake(GameTime gameTime, float interval = 150f, float ammount = 32f, float frequency = 0.01f, bool zoom = false, float zoommultiplyer = 0.01f)
        {

            this.Shaking = true;
            this.StartedShakeMilliseconds = (float)gameTime.TotalGameTime.TotalMilliseconds;
            this.ShackingInterval = interval;
            this.ShackingAmmount = ammount;
            this.ShackingFrequency = frequency;
            this.ShakeZoom = zoom;
            this.ZoomShakeMultiplyer = zoommultiplyer;

            this.OriginalPosition = Position;


        }

        public void UpdateShake(GameTime gameTime)
        {
            //https://www.desmos.com/calculator/7sgktk2drh

            Random rnd = new Random();

            float freqy = ShackingFrequency > 0 ? ShackingFrequency : rnd.Next(3, 5) * 0.1f;
            float freqx = ShackingFrequency > 0 ? ShackingFrequency : rnd.Next(3, 5) * 0.1f;

            float x = (float)gameTime.TotalGameTime.TotalMilliseconds - StartedShakeMilliseconds;
            float a = (float)(Math.Sqrt(ShackingAmmount));  // this is the magnitude
            float v = (float)(Math.Pow((a - x * (a / ShackingInterval)), 2));
            float q1 = (float)(freqy * Math.PI * x);
            float q2 = (float)(freqx * Math.PI * x);
            float d1 = (float)Math.Sin(q1);
            float d2 = (float)Math.Cos(q2);

            ShakeOffset.X = v * d1;
            ShakeOffset.Y = v * d2;

        }

        public string GetDebugString()
        {
            return $"Position: {Position}, Zoom {Zoom}\nShaking: {Shaking}, ShakeOffset: {ShakeOffset}\nShakeInterval: {ShackingInterval}, ShackingAmplitude: {ShackingAmmount}";
        }

    }
}
