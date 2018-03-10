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
        public int Height { get; set; }
        public bool Infinite { get; set; }
        public List<Layer> Layers { get; set; }
        public int Nextobjectid { get; set; }
        public string Orientation { get; set; }
        public string Renderorder { get; set; }
        public string Tiledversion { get; set; }
        public int Tileheight { get; set; }
        public List<Tileset> Tilesets { get; set; }
        public int Tilewidth { get; set; }
        public string Type { get; set; }
        public int Version { get; set; }
        public int Width { get; set; }


        public Level()
        {

            this.Layers = new List<Layer>();
            this.Tilesets = new List<Tileset>();

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
