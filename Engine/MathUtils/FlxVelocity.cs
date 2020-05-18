using Microsoft.Xna.Framework;
using System;

namespace Engine.MathUtils
{
    public class FlxVelocity
    {
        public static float ComputeVelocity(float velocity, float acceleration, float drag, float max, float delta)
        {
            if (acceleration != 0)
                velocity += acceleration * delta;
            else if(drag != 0)
            {
                float dragDelta = drag * delta;
                if ((velocity - dragDelta) > 0)
                    velocity -= dragDelta;
                else if ((velocity + dragDelta) < 0)
                    velocity += dragDelta;
                else
                    velocity = 0;
            }

            if((velocity != 0) && (max != 0))
            {
                if (velocity > max)
                    velocity = max;
                else if (velocity < -max)
                    velocity = -max;
            }

            return velocity;
        }
        public static Vector2 VelocityFromRotation(float rotation, float speed)
        {
            return new Vector2((float)Math.Cos(rotation) * speed, (float)Math.Sin(rotation) * speed);
        }

        public static Vector2 VelocityFromAngle(float angle, float speed)
        {
            return VelocityFromRotation(MathHelper.ToRadians(angle), speed);
        }
    }
}
