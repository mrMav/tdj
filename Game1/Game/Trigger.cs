using Engine.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDJGame
{
    class Trigger : AABB
    {

        public string Value { get; }

        public Trigger(float x, float y, float width, float height, string value) : base(x, y, width, height)
        {
            Value = value;
        }

    }
}
