using System.Collections.Generic;

namespace Engine.Utils.Helpers
{
    public class FlxBounds<T>
    {
        public T Min;
        public T Max;
        public bool Active { get; set; } = true;

        public FlxBounds(T min, T max = default)
        {
            Min = min;
            Max = max == null ? min : max;
        }

        public FlxBounds<T> Set(T min, T max = default)
        {
            Min = min;
            Max = max == null ? min : max;
            return this;
        }

        public override bool Equals(object obj)
        {
            FlxBounds<T> o = (FlxBounds<T>)obj;
            return Min.Equals(o.Min) && Max.Equals(o.Max);
        }

        public override int GetHashCode()
        {
            int hashCode = 368138711;
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(Min);
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(Max);
            return hashCode;
        }
    }
}
