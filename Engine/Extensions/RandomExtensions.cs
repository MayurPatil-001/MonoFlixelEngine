using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Engine.Extensions
{
    public static class RandomExtensions
    {
        public static float NextFloat(this Random random, float minValue, float maxValue)
        {
            return (float)random.NextDouble() * (maxValue - minValue) + minValue;
        }

        public static T GetObject<T>(this Random random, List<T> objects, float[] weightsArray = null, int startIndex = 0, int endIndex = 0)
        {
            T selected = default;
            if (objects.Count > 0)
            {
                if (endIndex <= 0)
                    endIndex = objects.Count;
                startIndex = MathHelper.Clamp(startIndex, 0, objects.Count - 1);
                endIndex = MathHelper.Clamp(endIndex, 0, objects.Count);

                //TODO : use weightsArray
                
                int index = random.Next(startIndex, endIndex);
                selected = objects[index];
            }
            return selected;
        }

        public static Color NextColor(this Random random, Color? min, Color? max, int? alpha = null, bool greyScale = false)
        {
            int r, g, b, a;
            if(min == null && max == null)
            {
                r = random.Next(0, 255);
                g = random.Next(0, 255);
                b = random.Next(0, 255);
                a = alpha == null ? random.Next(0, 255) : alpha.Value;
            }
            else if(max == null)
            {
                Color m = min.Value;
                r = random.Next(m.R, 255);
                g = greyScale ? r : random.Next(m.G, 255);
                b = greyScale ? r : random.Next(m.B, 255);
                a = alpha == null ? random.Next(m.A, 255) : alpha.Value;
            }
            else if (min == null)
            {
                Color m = max.Value;
                r = random.Next(0, m.R);
                g = greyScale ? r : random.Next(0, m.G);
                b = greyScale ? r : random.Next(0, m.B);
                a = alpha == null ? random.Next(0, m.A) : alpha.Value;
            }
            else
            {
                Color m = min.Value;
                Color n = max.Value;
                r = random.Next(m.R, n.R);
                g = greyScale ? r : random.Next(m.G, n.G);
                b = greyScale ? r : random.Next(m.B, n.B);
                a = alpha == null ? random.Next(m.A, n.A) : alpha.Value;
            }
            return new Color(r,g,b,a);
        }
    }
}
