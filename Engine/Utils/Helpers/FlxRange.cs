namespace Engine.Utils.Helpers
{
    public class FlxRange<T>
    {
        public T Start;
        public T End;
        public bool Active { get; set; } = true;

        public FlxRange(T start, T end = default)
        {
            Start = start;
            End = end == null ? start : end;
        }

        public FlxRange<T> Set(T start, T end = default)
        {
            Start = start;
            End = end == null ? start : end;
            return this;
        }

        public override bool Equals(object obj)
        {
            FlxRange<T> o = (FlxRange<T>)obj;
            return Start.Equals(o.Start) && End.Equals(o.End);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
