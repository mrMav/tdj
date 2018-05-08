using Microsoft.Xna.Framework;

namespace Engine.Physics
{
    public class AABB
    {
        protected float _x;
        protected float _y;

        protected float _width;
        protected float _height;

        protected float _halfwidth;
        protected float _halfheight;

        protected Vector2 _min;
        protected Vector2 _max;

        protected Vector2 _position;

        public float X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        public float Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }

        public float Width
        {
            get
            {
                return _width;
            }
        }

        public float Height
        {
            get
            {
                return _height;
            }
        }

        public float HalfWidth
        {
            get
            {
                return _halfwidth;
            }
        }

        public float HalfHeight
        {
            get
            {
                return _halfheight;
            }
        }

        public float CenterX
        {
            get
            {
                return _x + _halfwidth;
            }
        }

        public float CenterY
        {
            get
            {
                return _y + _halfheight;
            }
        }

        public Vector2 Min
        {
            get
            {
                _min.X = _x;
                _min.Y = _y;

                return _min;
            }
        }

        public Vector2 Max
        {
            get
            {
                _max.X = _x + _width;
                _max.Y = _y + _height;

                return _max;
            }
        }

        public Vector2 Position
        {
            get
            {
                _position.X = _x;
                _position.Y = _y;

                return _position;
            }
        }

        public AABB(float x, float y, float width, float height)
        {
            _x = x;
            _y = y;

            _width = width;
            _height = height;

            _halfwidth = width / 2;
            _halfheight = height / 2;
        }

        public void Resize(float width, float height)
        {
            _width = width;
            _height = height;

            _halfwidth = width / 2;
            _halfheight = height / 2;
        }

    }
}
