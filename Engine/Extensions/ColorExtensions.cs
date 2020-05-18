using Microsoft.Xna.Framework;
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
    }
}
