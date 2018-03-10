using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using TDJGame.Engine.Tiled;
using System.Diagnostics;

namespace TDJGame.Utils
{
    public class XMLLevelLoader
    {
        
        public XMLLevelLoader()
        {

        }

        public Level LoadLevel(string path)
        {
            
            if(File.Exists(path))
            {

                // load xml file
                XmlDocument doc = new XmlDocument();
                doc.Load(path);

                Level level = new Level();

                // get the map node for atributtes extraction
                XmlNode node = doc.DocumentElement.SelectSingleNode("/map");

                // somthing happened with the xml map node
                if(node == null)
                {

                    Debug.Print("somthing happened with the xml map node");

                    return null;
                }
                
                level.Height = int.Parse(node.Attributes["height"]?.InnerText);
                level.Width  = int.Parse(node.Attributes["width"]?.InnerText);

                level.Tilewidth = int.Parse(node.Attributes["tilewidth"]?.InnerText);
                level.Tileheight = int.Parse(node.Attributes["tileheight"]?.InnerText);

                // get the layers
                XmlNodeList layers = doc.DocumentElement.SelectNodes("/map/layer");

                foreach(XmlNode layer in layers)
                {
                    Layer l = new Layer();

                    l.Name = layer.Attributes["name"]?.InnerText;

                    l.Width = int.Parse(layer.Attributes["width"]?.InnerText);
                    l.Height = int.Parse(layer.Attributes["height"]?.InnerText);

                    XmlNodeList children = layer.ChildNodes;

                    string[] data = children.Item(0).InnerText.Split(',');
                    l.Data = new int[data.Length];

                    for(int i = 0; i < data.Length; i++)
                    {
                        l.Data[i] = int.Parse(data[i]);
                    }

                    level.Layers.Add(l);

                }

                return level;

            } else
            {

                Debug.Print("xml file does not exist");

                return null;

            }

        }
        
        
    }
}
