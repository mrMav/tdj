using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDJGame.Engine.Tiled
{
    public class Level
    {

        public int Width { get; set; }
        public int Height { get; set; }
        public bool Infinite { get; set; }

        public List<Layer> Layers { get; set; }
        public int NextObjectId { get; set; }

        public string Orientation { get; set; }
        public string RenderOrder { get; set; }
        public int TileHeight { get; set; }
        public int TileWidth { get; set; }

        public string Type { get; set; }
        public int Version { get; set; }
        public string TiledVersion { get; set; }


        public Level()
        {

            this.Layers = new List<Layer>();

        }

        public void Draw(SpriteBatch spriteBatch)
        {

            foreach (Layer layer in this.Layers)
            {
                layer.Draw(spriteBatch);
            }

        }

    }

}
