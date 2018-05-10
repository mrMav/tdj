using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Tiled
{
    public class TiledObject
    {   
        public string Name { get; }
        public string Type { get; }

        public float X { get; }
        public float Y { get; }

        public float Width  { get; }
        public float Height { get; }

        Dictionary<string, string> Properties;

        public TiledObject(string name, string type, float x, float y, float width, float height)
        {
            Name = name;
            Type = type;

            X = x;
            Y = y;

            Width  = width;
            Height = height;

            Properties = new Dictionary<string, string>();
        }

        public void SetProperty(string key, string value)
        {
            Properties.Add(key, value);
        }

        public string GetProperty(string key)
        {
            string value = "";

            if(Properties.TryGetValue(key, out value))
            {
                return value;
            }

            return "";

        }
    }
}
