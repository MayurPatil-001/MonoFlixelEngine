using Microsoft.Xna.Framework;
using System;

namespace Engine.MathUtils
{
    public class FlxMath
    {

        /// <summary>
        /// Used to account for floating-point inaccuracies.
        /// </summary>
        public const float EPSILON = 0.0000001f;
        public static float SQUARE_ROOT_OF_TWO = 1.41421356237f;

        public static bool PointInFlxRect(float pointX, float pointY, RectangleF rect)
        {
            return PointInFlxRect(new Vector2(pointX, pointY), rect);
        }

        public static bool PointInFlxRect(Vector2 vector, RectangleF rect)
        {
            return vector.X >= rect.X && vector.X <= rect.Right && vector.Y >= rect.Y && vector.Y <= rect.Bottom;
        }

        public static bool Equal(float aValueA, float aValueB, float aDiff = EPSILON)
        {
            return Math.Abs(aValueA - aValueB) <= aDiff;
        }
    }
}
