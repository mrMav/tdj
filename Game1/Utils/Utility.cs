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

            return new Color(
                (color.R - amount >= 0 ? color.R - amount : 0),
                (color.G - amount >= 0 ? color.G - amount : 0),
                (color.B - amount >= 0 ? color.B - amount : 0),
                (color.A - amount >= 0 ? color.A - amount : 0));

        }

    }
}
