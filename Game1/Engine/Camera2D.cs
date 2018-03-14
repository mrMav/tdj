using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDJGame.Engine
{

    //http://www.david-amador.com/2009/10/xna-camera-2d-with-zoom-and-rotation/
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
        public Vector2 Position { get; set; }

        public Camera2D(Vector2 position)
        {
            this._zoom = 1.0f;
            this.Rotation = 0.0f;
            this.Position = position == null ? Vector2.Zero : position;
        }



        public void Move(Vector2 amount)
        {
            this.Position += amount;
        }

        public void GetTransform(GraphicsDevice graphicsDevice)
        {
            // I bet this is eating a lot of res
            // try and use the ref and outs of the functions

            Matrix invpos = Matrix.CreateTranslation(new Vector3(-this.Position.X, -this.Position.Y, 0));
            Matrix scale = Matrix.CreateScale(new Vector3(this.Zoom, this.Zoom, 0));
            Matrix rot = Matrix.CreateRotationZ(this.Rotation);
            Matrix trans = Matrix.CreateTranslation(new Vector3(
                    graphicsDevice.Viewport.Width * 0.5f,
                    graphicsDevice.Viewport.Height * 0.5f,
                    0));

            this.Transform = invpos * rot * scale * trans;

        }

    }
}
