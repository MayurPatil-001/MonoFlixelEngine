using Microsoft.Xna.Framework;
using System;
using System.Runtime.InteropServices;

namespace Engine.Extensions
{
    public static class ColorExtensions
    {
        public static Color Interpolate(this Color color, Color color1, Color color2, float factor = 0.5f)
        {
            int r = (int)((color2.R - color1.R) * factor + color1.R);
            int g = (int)((color2.G - color1.G) * factor + color1.G);
            int b = (int)((color2.B - color1.B) * factor + color1.B);
            int a = (int)((color2.A - color1.A) * factor + color1.A);
            return new Color(r, g, b, a);
        }


        public static Color FromHex(this Color color, string hexColor)
        {
            hexColor = hexColor.Replace("0x", "");
            hexColor = hexColor.Replace("#", "");
            
            int a;
            if (hexColor.Length == 8)
            {
                string alpha = hexColor.Substring(0, 2);
                a = Convert.ToInt32(alpha, 16);
                hexColor = hexColor.Replace(alpha, "");
            }
            else
            {
                a = 255;
            }
            int r = Convert.ToInt32(hexColor.Substring(0, 2), 16);
            int g = Convert.ToInt32(hexColor.Substring(2, 2), 16);
            int b = Convert.ToInt32(hexColor.Substring(4, 2), 16);
            return new Color(r, g, b, a);
        }
    }
}
