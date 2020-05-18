namespace Engine.Utils.Helpers
{
    public class FlxRangeBounds<T>
    {
        public FlxBounds<T> Start;
        public FlxBounds<T> End;
        public bool Active { get; set; } = true;

        public FlxRangeBounds(T startMin, T startMax = default, T endMin = default, T endMax = default)
        {
            Start = new FlxBounds<T>(startMin, startMax == null ? startMin : startMax);
            End = new FlxBounds<T>(endMin == null ? startMin : endMin, endMax == null ? Start.Max : endMax);
        }

        public FlxRangeBounds<T> Set(T startMin, T startMax = default, T endMin = default, T endMax = default)
        {
            Start.Min = startMin;
            Start.Max = startMax == null ? Start.Min : startMax;
            End.Min = endMin == null ? Start.Min : endMin;
            End.Max = endMax == null ? (endMin == null ? Start.Max : End.Min) : endMax;

            return this;
        }

        public override bool Equals(object obj)
        {
            FlxRangeBounds<T> o = (FlxRangeBounds<T>)obj;
            return Start.Equals(o.Start) && End.Equals(o.End);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
