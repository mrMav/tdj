using Microsoft.Xna.Framework;
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

        public List<Tile> CollidableTiles;


        public Level()
        {

            this.Layers = new List<Layer>();
            this.CollidableTiles = new List<Tile>();
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            foreach (Layer layer in this.Layers)
            {
                layer.Draw(spriteBatch);
            }

        }

        /// <summary>
        /// Set all tiles body with the specified id enabled
        /// </summary>
        /// <param name="id" type="int[]"></param>
        public void SetCollisionTiles(int[] id)
        {            
            foreach(Layer layer in Layers)
            {
                for(int y = 0; y < layer.TileMap.GetLength(0); y++)
                {
                    for(int x = 0; x < layer.TileMap.GetLength(1); x++)
                    {
                        for(int i = 0; i < id.Length; i++)
                        {
                            Tile t = layer.TileMap[y, x];

                            if (t != null && t.ID == id[i])
                            {
                                t.Body.Enabled = true;

                                this.CollidableTiles.Add(t);

                                t.Tint = Color.Red;
                            }
                        }
                    }
                }
            }
        }

        
        /// <returns>Returns a List of Collide Enabled tiles</returns>
        public List<Tile> GetCollideEnabledTiles()
        {
            List<Tile> tiles = new List<Tile>();

            foreach (Layer layer in Layers)
            {
                for (int y = 0; y < layer.TileMap.GetLength(0); y++)
                {
                    for (int x = 0; x < layer.TileMap.GetLength(1); x++)
                    {
                        Tile t = layer.TileMap[y, x];

                        if (t.Body.Enabled)
                        {
                            tiles.Add(t);
                        }
                    }
                }
            }

            return tiles;

        }
    }
}
