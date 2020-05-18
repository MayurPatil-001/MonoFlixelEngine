using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Extensions
{
    public static class Texture2DExtension
    {
        public static Texture2D FillRect(this Texture2D texture, Rectangle rectangle, Color color)
        {
            Color[] cols = new Color[rectangle.Width * rectangle.Height];
            for (int i = 0; i < cols.Length; i++)
                cols[i] = color;
            texture.SetData(0, rectangle, cols, 0, rectangle.Width * rectangle.Height);
            return texture;
        }
    }
}
