using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDJGame.Utils
{
    public static class Utility
    {

        public static Color FadeColor(Color color, byte amount)
        {

            return new Color(color.R - amount, color.G - amount, color.B - amount, color.A - amount);

        }

    }
}
