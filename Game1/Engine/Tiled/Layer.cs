using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TDJGame.Engine.Tiled
{
    public class Layer
    {
        public int[] Data { get; set; }
        public int Height { get; set; }
        public string Name { get; set; }
        public int Opacity { get; set; }
        public string Type { get; set; }
        public bool Visible { get; set; }
        public int Width { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        // TODO: adicionar suporte do xml do tiled
        public Texture2D Texture { get; set; }
        public Tile[,] TileMap;

        public void Draw(SpriteBatch spriteBatch)
        {

            // Column = TileID mod Rows
            // Row = TileID div Rows

            //this.tileMap[y, x] = new Tile(
            //    texture,
            //    new Vector2(this.data[y, x] % this.textureTilesWidth, this.data[y, x] / this.textureTilesWidth),
            //    new Vector2(x * this.tileSize, y * this.tileSize),
            //    this.tileSize
            //);

            int l = this.Data.Length;
            for(int i = 0; i < l; i++)
            {
                
                spriteBatch.Draw(this.Texture, this.worldBoundingRect, this.textureBoundingRect, Color.White);

            }

        }
    }
}
