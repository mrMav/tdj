/*
 * This is a simple class to generate geometric shapes in a Monogame Texture2D.
 * 
 * Create a new Texture2D and use as follows: 
 * 
 * int TileSize = 64;
 * 
 * // create the new texture
 * block = new Texture2D(graphics.GraphicsDevice, TileSize, TileSize);   
 * 
 * // Paint the entire canvas black
 * DrawMe.FillRect(block, 0, 0, TileSize, TileSize, Color.Black);
 * 
 * // Draw some Lines
 * DrawMe.Line(block, 0, 0, TileSize - 1, TileSize - 1, green);
 * DrawMe.Line(block, 0, TileSize - 1, TileSize - 1, 0, green);
 * 
 * // Draw a circle. This circle will wrap: geometry outside of the bounds will wrap the texture
 * DrawMe.FillCircle(block, 0, 0, 30, Color.Firebrick);
 * 
 * Feel free to try the various methods.
 * This was not made by a professional, I just needed an image and was 
 * too lazy to open an image editor, make one, and load it.
 * Some error handling is needed, use at your own risk.
 * Will update with more methods maybe. One day.
 * 
 * Made with love and laziness by 'mrmav'
 * Based on the works of Alois Zingl. 
 * 2018
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

/// <summary>
/// This is a class to draw geometric shapes into monogame textures
/// </summary>
public static class DrawMe
{
    /// <summary>
    /// This enum is used when drawing something in the texture. 
    /// OverFlow.Wrap will wrap the geometry (not true wrap);
    /// OverFlow.Discard will ignore pixels outside the texture
    /// </summary>
    public enum OverFlow
    {
        Wrap = 1,
        Discard = 2
    }

    private static OverFlow OverFlowMethod = OverFlow.Discard;

    public static void SetOverFlowMethod(OverFlow method)
    {
        OverFlowMethod = method;
    }

    /// <summary>
    /// Gets the pixel data of the texture as Color objects
    /// </summary>
    /// <param name="texture"></param>
    /// <returns></returns>
    private static Color[] GetData(Texture2D texture)
    {
        Color[] data = new Color[texture.Width * texture.Height];
        texture.GetData<Color>(data);
        return data;
    }

    /// <summary>
    /// Puts the pixel data of the texture
    /// </summary>
    /// <param name="texture"></param>
    /// <returns></returns>
    private static void SetData(Texture2D texture, Color[] data)
    {
        texture.SetData<Color>(data);
    }

    /// <summary>
    /// Sets the data pixel to the provided color
    /// </summary>
    /// <param name="data"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="w"></param>
    /// <param name="color"></param>
    private static void SetPixel(Color[] data, int x, int y, int w, Color color)
    {
        if (OverFlowMethod == OverFlow.Discard)
        {
            int h = data.GetLength(0) / w;

            if (x < 0 || x >= w || y < 0 || y >= h)
            {
                // pixel out of texture bounds
                Console.WriteLine("Pixel out of texture bounds, ignoring this pixel.");
            }
            else
            {
                data[y * w + x] = color;
            }

        }
        else if (OverFlowMethod == OverFlow.Wrap)
        {
            int index = y * w + x;

            if (index >= 0 && index < data.GetLength(0))
            {
                data[y * w + x] = color;
            }
            else
            {
                Console.WriteLine("Index out of bounds, ignoring this pixel.");
            }

        }

    }

    /// <summary>
    /// Set the color of the pixel in the texture at coordinates
    /// </summary>
    /// <param name="texture">The texture to modify</param>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <param name="color">The color</param>
    public static void Pixel(Texture2D texture, int x, int y, Color color)
    {
        Color[] data = GetData(texture);

        SetPixel(data, x, y, texture.Width, color);

        SetData(texture, data);
    }

    /// <summary>
    /// Fills an entire texture with a color
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="color"></param>
    public static void Fill(Texture2D texture, Color color)
    {
        Color[] data = new Color[texture.Width * texture.Height];

        for (int i = 0; i < data.GetLength(0); i++)
        {
            data[i] = color;
        }

        SetData(texture, data);

    }

    /// <summary>
    /// Draws a fileed axis aligned rectangle
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="w"></param>
    /// <param name="h"></param>
    /// <param name="color"></param>
    public static void FillRect(Texture2D texture, int x, int y, int w, int h, Color color)
    {
        Color[] data = GetData(texture);

        for (int xx = 0; xx < w; xx++)
        {
            for (int yy = 0; yy < h; yy++)
            {
                SetPixel(data, xx + x, yy + y, texture.Width, color);
            }
        }

        SetData(texture, data);

    }

    /// <summary>
    /// Draws the bounds of an axis aligned rectangle
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="w"></param>
    /// <param name="h"></param>
    /// <param name="color"></param>
    public static void StrokeRect(Texture2D texture, int x, int y, int w, int h, Color color)
    {
        int left = x, up = y, right = x + w - 1, down = y + h - 1;

        Line(texture, left, up, right, up, color);
        Line(texture, left, up, left, down, color);
        Line(texture, right, down, right, up, color);
        Line(texture, right, down, left, down, color);
    }

    /// <summary>
    /// Draws a line from point 1 to point 2
    /// </summary>
    /// <see cref="http://members.chello.at/~easyfilter/bresenham.html"/>
    /// <param name="texture"></param>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <param name="color"></param>
    public static void Line(Texture2D texture, int x1, int y1, int x2, int y2, Color color)
    {

        Color[] data = GetData(texture);

        int dx = Math.Abs(x2 - x1);
        int sx = x1 < x2 ? 1 : -1;

        int dy = -Math.Abs(y2 - y1);
        int sy = y1 < y2 ? 1 : -1;

        int err = dx + dy;
        int err2;

        while (true)
        {
            SetPixel(data, x1, y1, texture.Width, color);

            if (x1 == x2 && y1 == y2)
                break;

            err2 = 2 * err;

            if (err2 >= dy)
            {
                err += dy;
                x1 += sx;
            }

            if (err2 <= dx)
            {
                err += dx;
                y1 += sy;
            }
        }

        SetData(texture, data);

    }

    /// <summary>
    /// Strokes a circle on the texture
    /// </summary>
    /// <see cref="http://members.chello.at/~easyfilter/bresenham.html"/>
    /// <param name="texture"></param>
    /// <param name="centerx"></param>
    /// <param name="centery"></param>
    /// <param name="radius"></param>
    /// <param name="color"></param>
    public static void StrokeCircle(Texture2D texture, int centerx, int centery, int radius, Color color)
    {
        Color[] data = GetData(texture);

        int x = -radius;
        int y = 0;
        int err = 2 - 2 * radius;

        do
        {
            SetPixel(data, centerx - x, centery + y, texture.Width, color);
            SetPixel(data, centerx - y, centery - x, texture.Width, color);
            SetPixel(data, centerx + x, centery - y, texture.Width, color);
            SetPixel(data, centerx + y, centery + x, texture.Width, color);

            radius = err;

            if (radius <= y)
            {
                err += ++y * 2 + 1;
            }

            if (radius > x || err > y)
            {
                err += ++x * 2 + 1;
            }

        } while (x < 0);

        SetData(texture, data);
    }

    /// <summary>
    /// Fills a circle on the texture
    /// </summary>
    /// <see cref="http://members.chello.at/~easyfilter/bresenham.html"/>
    /// <param name="texture"></param>
    /// <param name="centerx"></param>
    /// <param name="centery"></param>
    /// <param name="radius"></param>
    /// <param name="color"></param>
    public static void FillCircle(Texture2D texture, int centerx, int centery, int radius, Color color)
    {

        int x = -radius;
        int y = 0;
        int err = 2 - 2 * radius;

        do
        {
            // scanning

            Line(texture, centerx, centery + y, centerx - x, centery + y, color);
            Line(texture, centerx, centery - x, centerx - y, centery - x, color);
            Line(texture, centerx, centery - y, centerx + x, centery - y, color);
            Line(texture, centerx, centery + x, centerx + y, centery + x, color);

            radius = err;

            if (radius <= y)
            {
                err += ++y * 2 + 1;
            }

            if (radius > x || err > y)
            {
                err += ++x * 2 + 1;
            }

        } while (x < 0);

    }

}