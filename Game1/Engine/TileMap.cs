using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDJGame
{
    public class TileMap
    {

        protected Texture2D texture;
        protected float tileSize;
        
        private int textureTilesWidth;
        private int textureTilesHeight;

        public float scale = 3;

        public Tile[,] tileMap;

        // temp data
        public int[,] data =
        {
            {16,16,16,16,16,16,16,16,40,16,16,16,16,16,16 },
            {16,16,16,16,16,16,16,16,16,16,16,16,16,16,16 },
            {16,16,16,16,16,16,16,16,16,16,16,16,16,16,16 },
            {16,16,16,05,05,05,16,16,16,16,16,16,16,16,16 },
            {16,16,05,16,16,16,16,16,16,16,16,16,16,16,16 },
            {16,05,16,16,16,16,16,16,16,16,16,16,16,16,16 },
            {00,00,00,00,00,00,00,00,00,00,00,00,00,00,00 }

        };

        public TileMap(Texture2D texture, float tileSize)
        {
            this.texture = texture;
            this.tileSize = tileSize * this.scale;

            this.textureTilesWidth = texture.Width / (int)tileSize;
            this.textureTilesHeight = texture.Height / (int)tileSize;

            this.tileMap = new Tile[this.data.GetLength(0), this.data.GetLength(1)];

        }

        public void ParseData()
        {
            for(int y = 0; y < this.data.GetLength(0); y++ )
            {
                for(int x = 0; x < this.data.GetLength(1); x++)
                {
                    // Column = TileID mod Rows
                    // Row = TileID div Rows

                    this.tileMap[y, x] = new Tile(
                        texture,
                        new Vector2(this.data[y, x] % this.textureTilesWidth, this.data[y, x] / this.textureTilesWidth),
                        new Vector2(x * this.tileSize, y * this.tileSize),
                        this.tileSize
                    );

                    Console.WriteLine(this.tileMap[y, x].textureBoundingRect);

                }
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {

            foreach(Tile t in this.tileMap)
            {
                t.Draw(spriteBatch, scale);
            }

        }

    }
}
