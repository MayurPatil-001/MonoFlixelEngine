using Microsoft.Xna.Framework;

namespace Engine.Utils.Helpers
{
	public class FlxVectorRangeBounds
	{
		public FlxBounds<Vector2> Start;
		public FlxBounds<Vector2> End;
		public bool Active { get; set; } = true;


		public FlxVectorRangeBounds(float startMinX, float? startMinY = 0, float? startMaxX = 0, float? startMaxY = 0, float? endMinX = 0, float? endMinY = 0, float? endMaxX = 0, float? endMaxY = 0)
		{
			Start = new FlxBounds<Vector2>(Vector2.Zero, Vector2.Zero);
			End = new FlxBounds<Vector2>(Vector2.Zero, Vector2.Zero);
			Set(startMinX, startMinY, startMaxX, startMaxY, endMinX, endMinY, endMaxX, endMaxY);
		}

		public FlxVectorRangeBounds Set(float startMinX, float? startMinY = 0, float? startMaxX = 0, float? startMaxY = 0, float? endMinX = 0, float? endMinY = 0, float? endMaxX = 0, float? endMaxY = 0)
		{
			Start.Min.X = startMinX;
			Start.Min.Y = startMinY == null ? Start.Min.X : startMinY.GetValueOrDefault();
			Start.Max.X = startMaxX == null ? Start.Min.X : startMaxX.GetValueOrDefault();
			Start.Max.Y = startMaxY == null ? Start.Min.Y : startMaxY.GetValueOrDefault();
			End.Min.Y = endMinX == null ? Start.Min.X : endMinX.GetValueOrDefault();
			End.Min.Y = endMinY == null ? Start.Min.Y : endMinY.GetValueOrDefault();
			End.Max.Y = endMaxX == null ? (endMinX == null ? Start.Max.X : End.Min.X) : endMaxX.GetValueOrDefault();
			End.Max.Y = endMaxY == null ? (endMinY == null ? Start.Max.X : End.Min.Y) : endMaxY.GetValueOrDefault();
			return this;
		}

		public override bool Equals(object obj)
		{
			FlxVectorRangeBounds o = (FlxVectorRangeBounds)obj;
			return Start.Equals(o.Start) && End.Equals(o.End);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
