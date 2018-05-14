using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Animations
{
    /// <summary>
    /// Represents a single frame.
    /// </summary>
    public class Frame
    {
        /*
         * X position in the texture.
         */
        public int X { get; set; }

        /*
         * Y position in the texture.
         */
        public int Y { get; set; }

        /*
         * Width of the frame.
         */
        public int Width { get; set; }

        /*
         * Height of the frame.
         */
        public int Height { get; set; }

        /*
         * Index of this frame in the animation.
         */
        public int Index { get; set; }

        /*
         * This frame source rectangle.
         */
        public Rectangle TextureSourceRect { get; }
        
        public Frame(int x, int y, int width, int height, int index = 0)
        {

            X = x;
            Y = y;

            Width = width;
            Height = height;

            Index = index;

            TextureSourceRect = new Rectangle(x, y, width, height);

        }
    }
}
