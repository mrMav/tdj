using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDJGame.Utils
{
    public static class Math2
    {
        /// <summary>
        /// Maps a value from a range to another
        /// </summary>
        /// <see>https://forum.unity.com/threads/re-map-a-number-from-one-range-to-another.119437/</see>
        /// <param name="value"></param>
        /// <param name="from1"></param>
        /// <param name="to1"></param>
        /// <param name="from2"></param>
        /// <param name="to2"></param>
        /// <returns>The new value mapped</returns>
        public static float Map(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

    }
}
