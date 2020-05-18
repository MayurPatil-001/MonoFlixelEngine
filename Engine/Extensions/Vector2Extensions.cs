using Microsoft.Xna.Framework;
using System;

namespace Engine.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector2 Floor(this Vector2 vector)
        {
            return new Vector2((float)Math.Floor(vector.X), (float)Math.Floor(vector.Y));
        }
        public static Vector2 Subtract(this Vector2 vector, float x, float y)
        {
            return new Vector2(vector.X - x, vector.Y - y);
        }
    }
}
