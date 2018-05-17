using System;
using System.Xml;
using System.IO;
using Engine.Tiled;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using Engine;

namespace TDJGame.Utils
{
    public class XMLLevelLoader
    {

        public XMLLevelLoader()
        {

        }

        public Level LoadLevel(GameState state, string path, Texture2D texture)
        {

            if (File.Exists(path))
            {

                // load xml file
                XmlDocument doc = new XmlDocument();
                doc.Load(path);

                Level level = new Level(state);

                // get the map node for atributtes extraction
                XmlNode node = doc.DocumentElement.SelectSingleNode("/map");

                // somthing happened with the xml map node
                if (node == null)
                {

                    Debug.Print("somthing happened with the xml map node");

                    return null;
                }

                level.Height = int.Parse(node.Attributes["height"]?.InnerText);
                level.Width = int.Parse(node.Attributes["width"]?.InnerText);

                level.TileWidth = int.Parse(node.Attributes["tilewidth"]?.InnerText);
                level.TileHeight = int.Parse(node.Attributes["tileheight"]?.InnerText);

                level.NextObjectId = int.Parse(node.Attributes["nextobjectid"]?.InnerText);

                // get the layers
                XmlNodeList layers = doc.DocumentElement.SelectNodes("/map/layer");

                // this will look just plain ugly
                foreach (XmlNode layer in layers)
                {

                    int width = int.Parse(layer.Attributes["width"]?.InnerText);
                    int height = int.Parse(layer.Attributes["height"]?.InnerText);

                    Layer l = new Layer(level, texture, width, height);

                    l.Name = layer.Attributes["name"]?.InnerText;

                    XmlNodeList children = layer.ChildNodes;

                    string[] data = children.Item(0).InnerText.Split(',');
                    l.Data = new int[data.Length];

                    for (int i = 0; i < data.Length; i++)
                    {
                        l.Data[i] = int.Parse(data[i]);
                    }

                    l.MakeTiles();

                    level.Layers.Add(l);

                }

                // get the objects groups
                XmlNodeList objectGroups = doc.DocumentElement.SelectNodes("/map/objectgroup");

                foreach (XmlNode group in objectGroups)
                {
                    foreach (XmlNode obj in group)
                    {

                        string name = obj.Attributes["name"]?.InnerText;
                        string type = obj.Attributes["type"]?.InnerText;

                        float x = float.Parse(obj.Attributes["x"]?.InnerText);
                        float y = float.Parse(obj.Attributes["y"]?.InnerText);
                        
                        float width = 1;
                        float height = 1;

                        try
                        {
                            width = float.Parse(obj.Attributes["width"]?.InnerText);
                            height = float.Parse(obj.Attributes["height"]?.InnerText);
                        }
                        catch { };

                        TiledObject newObj = new TiledObject(name, type, x, y, width, height);

                        

                        // fetch custom properties
                        XmlNode props = obj.SelectSingleNode("properties");

                        if (props != null)
                        {

                            foreach (XmlNode prop in props)
                            {
                                string key = prop.Attributes["name"]?.InnerText;
                                string value = prop.Attributes["value"]?.InnerText;

                                newObj.SetProperty(key, value);
                            }

                        }

                        level.Objects.Add(newObj);

                    }
                }

                return level;

            }
            else
            {

                Debug.Print("xml file does not exist");

                return null;

            }

        }


    }
}
