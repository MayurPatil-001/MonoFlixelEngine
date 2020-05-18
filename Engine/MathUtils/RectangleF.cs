using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Engine.MathUtils
{
    public class RectangleF
    {
        /// <summary>
        ///     The <see cref="RectangleF" /> with <see cref="X" />, <see cref="Y" />, <see cref="Width" /> and
        ///     <see cref="Height" /> all set to <code>0.0f</code>.
        /// </summary>
        public static readonly RectangleF Empty = new RectangleF();

        /// <summary>
        ///     The x-coordinate of the top-left corner position of this <see cref="RectangleF" />.
        /// </summary>
        public float X;

        /// <summary>
        ///     The y-coordinate of the top-left corner position of this <see cref="RectangleF" />.
        /// </summary>
        public float Y;

        /// <summary>
        ///     The width of this <see cref="RectangleF" />.
        /// </summary>
        public float Width;

        /// <summary>
        ///     The height of this <see cref="RectangleF" />.
        /// </summary>
        public float Height;

        /// <summary>
        ///     Gets the x-coordinate of the left edge of this <see cref="RectangleF" />.
        /// </summary>
        public float Left => X;

        /// <summary>
        ///     Gets the x-coordinate of the right edge of this <see cref="RectangleF" />.
        /// </summary>
        public float Right => X + Width;

        /// <summary>
        ///     Gets the y-coordinate of the top edge of this <see cref="RectangleF" />.
        /// </summary>
        public float Top => Y;

        /// <summary>
        ///     Gets the y-coordinate of the bottom edge of this <see cref="RectangleF" />.
        /// </summary>
        public float Bottom => Y + Height;

        /// <summary>
        ///     Gets a value indicating whether this <see cref="RectangleF" /> has a <see cref="X" />, <see cref="Y" />,
        ///     <see cref="Width" />,
        ///     <see cref="Height" /> all equal to <code>0.0f</code>.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty => Width.Equals(0) && Height.Equals(0) && X.Equals(0) && Y.Equals(0);

        /// <summary>
        ///     Gets the <see cref="Point2" /> representing the the top-left of this <see cref="RectangleF" />.
        /// </summary>
        public Vector2 Position
        {
            get { return new Vector2(X, Y); }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        /// <summary>
        ///     Gets the <see cref="Size2" /> representing the extents of this <see cref="RectangleF" />.
        /// </summary>
        public Vector2 Size
        {
            get { return new Vector2(Width, Height); }
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }

        /// <summary>
        ///     Gets the <see cref="Point2" /> representing the center of this <see cref="RectangleF" />.
        /// </summary>
        public Vector2 Center => new Vector2(X + Width * 0.5f, Y + Height * 0.5f);

        /// <summary>
        ///     Gets the <see cref="Point2" /> representing the top-left of this <see cref="RectangleF" />.
        /// </summary>
        public Vector2 TopLeft => new Vector2(X, Y);

        /// <summary>
        ///     Gets the <see cref="Point2" /> representing the bottom-right of this <see cref="RectangleF" />.
        /// </summary>
        public Vector2 BottomRight => new Vector2(X + Width, Y + Height);

        /// <summary>
        ///     Initializes a new instance of the <see cref="RectangleF" /> structure from the specified top-left xy-coordinate
        ///     <see cref="float" />s, width <see cref="float" /> and height <see cref="float" />.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public RectangleF(float x = 0, float y = 0, float width = 0, float height = 0)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RectangleF" /> structure from the specified top-left
        ///     <see cref="Point2" /> and the extents <see cref="Size2" />.
        /// </summary>
        /// <param name="position">The top-left point.</param>
        /// <param name="size">The extents.</param>
        public RectangleF(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
        }

        public RectangleF Set(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
            return this;
        }
        public RectangleF Set(float x = 0, float y = 0, float width = 0, float height = 0)
        {
            Set(new Vector2(x, y), new Vector2(width, height));
            return this;
        }

        public RectangleF CopyFrom(RectangleF other)
        {
            X = other.X;
            Y = other.Y;
            Width = other.Width;
            Height = other.Height;
            return this;
        }

        public bool Overlaps(RectangleF other)
        {
            return other.Right > X && other.X < Right && other.Bottom > Y && other.Y < Bottom;
        }

        public bool ContainsVector2(Vector2 vector)
        {
            return FlxMath.PointInFlxRect(vector, this);
        }
        public RectangleF Union(RectangleF other)
        {
            float minX = Math.Min(X, other.X);
            float minY = Math.Min(Y, other.Y);
            float maxX = Math.Min(Right, other.Right);
            float maxY = Math.Min(Bottom, other.Bottom);

            return Set(minX, minY, maxX, maxY);
        }
        public RectangleF Floor()
        {
            X = (float)Math.Floor(X);
            Y = (float)Math.Floor(Y);
            Width = (float)Math.Floor(Width);
            Height = (float)Math.Floor(Height);
            return this;
        }
        public RectangleF Ceil()
        {
            X = (float)Math.Ceiling(X);
            Y = (float)Math.Ceiling(Y);
            Width = (float)Math.Ceiling(Width);
            Height = (float)Math.Ceiling(Height);
            return this;
        }
        public RectangleF Round()
        {
            X = (float)Math.Round(X);
            Y = (float)Math.Round(Y);
            Width = (float)Math.Round(Width);
            Height = (float)Math.Round(Height);
            return this;
        }
        public RectangleF UnionWithPoint(Vector2 vector)
        {
            float minX = Math.Min(X, vector.X);
            float minY = Math.Min(Y, vector.Y);
            float maxX = Math.Min(Right, vector.X);
            float maxY = Math.Min(Bottom, vector.Y);
            return Set(minX, minY, maxX, maxY);
        }
        public RectangleF Offset(Vector2 vector)
        {
            X += vector.X;
            Y += vector.Y;
            return this;
        }

        public RectangleF Intersection(RectangleF other)
        {
            float x0 = X < other.X ? other.X : X;
            float x1 = Right > other.Right ? other.Right : Right;
            if (x1 <= x0)
                return Empty;

            float y0 = Y < other.Y ? other.Y : Y;
            float y1 = Bottom > other.Bottom ? other.Bottom : Bottom;
            if (y1 <= y0)
                return Empty;

            return new RectangleF(x0, y0, x1 - x0, y1 - y0);
        }

        /// <summary>
        ///     Determines whether the two specified <see cref="RectangleF" /> structures intersect.
        /// </summary>
        /// <param name="first">The first rectangle.</param>
        /// <param name="second">The second rectangle.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="first" /> intersects with the <see cref="second" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool Intersects(ref RectangleF first, ref RectangleF second)
        {
            return first.X < second.X + second.Width && first.X + first.Width > second.X &&
                   first.Y < second.Y + second.Height && first.Y + first.Height > second.Y;
        }

        /// <summary>
        ///     Determines whether the two specified <see cref="RectangleF" /> structures intersect.
        /// </summary>
        /// <param name="first">The first rectangle.</param>
        /// <param name="second">The second rectangle.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="first" /> intersects with the <see cref="second" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool Intersects(RectangleF first, RectangleF second)
        {
            return Intersects(ref first, ref second);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="RectangleF" /> intersects with this
        ///     <see cref="RectangleF" />.
        /// </summary>
        /// <param name="rectangle">The bounding rectangle.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="rectangle" /> intersects with this
        ///     <see cref="RectangleF" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        //public bool Intersects(RectangleF rectangle)
        //{
        //    return Intersects(ref this, ref rectangle);
        //}

        /// <summary>
        ///     Determines whether the specified <see cref="RectangleF" /> contains the specified
        ///     <see cref="Point2" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="rectangle" /> contains the <paramref name="point" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public static bool Contains(ref RectangleF rectangle, ref Vector2 point)
        {
            return rectangle.X <= point.X && point.X < rectangle.X + rectangle.Width && rectangle.Y <= point.Y && point.Y < rectangle.Y + rectangle.Height;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="RectangleF" /> contains the specified
        ///     <see cref="Point2" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="rectangle" /> contains the <paramref name="point" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public static bool Contains(RectangleF rectangle, Vector2 point)
        {
            return Contains(ref rectangle, ref point);
        }

        /// <summary>
        ///     Determines whether this <see cref="RectangleF" /> contains the specified
        ///     <see cref="Point2" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     <c>true</c> if the this <see cref="RectangleF"/> contains the <paramref name="point" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        //public bool Contains(Vector2 point)
        //{
        //    return Contains(ref this, ref point);
        //}


        ///// <summary>
        /////     Compares two <see cref="RectangleF" /> structures. The result specifies whether the values of the
        /////     <see cref="X" />, <see cref="Y"/>, <see cref="Width"/> and <see cref="Height" /> fields of the two <see cref="RectangleF" /> structures
        /////     are equal.
        ///// </summary>
        ///// <param name="first">The first rectangle.</param>
        ///// <param name="second">The second rectangle.</param>
        ///// <returns>
        /////     <c>true</c> if the values of the
        /////     <see cref="X" />, <see cref="Y"/>, <see cref="Width"/> and <see cref="Height" /> fields of the two <see cref="RectangleF" /> structures
        /////     are equal; otherwise, <c>false</c>.
        ///// </returns>
        //public static bool operator ==(RectangleF first, RectangleF second)
        //{
        //    return first.Equals(ref second);
        //}

        ///// <summary>
        /////     Compares two <see cref="RectangleF" /> structures. The result specifies whether the values of the
        /////     <see cref="X" />, <see cref="Y"/>, <see cref="Width"/> and <see cref="Height" /> fields of the two <see cref="RectangleF" /> structures
        /////     are unequal.
        ///// </summary>
        ///// <param name="first">The first rectangle.</param>
        ///// <param name="second">The second rectangle.</param>
        ///// <returns>
        /////     <c>true</c> if the values of the
        /////     <see cref="X" />, <see cref="Y"/>, <see cref="Width"/> and <see cref="Height" /> fields of the two <see cref="RectangleF" /> structures
        /////     are unequal; otherwise, <c>false</c>.
        ///// </returns>
        //public static bool operator !=(RectangleF first, RectangleF second)
        //{
        //    return !(first == second);
        //}

        /// <summary>
        ///     Indicates whether this <see cref="RectangleF" /> is equal to another <see cref="RectangleF" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="RectangleF" /> is equal to the <paramref name="rectangle" />; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(RectangleF rectangle)
        {
            return Equals(ref rectangle);
        }

        /// <summary>
        ///     Indicates whether this <see cref="RectangleF" /> is equal to another <see cref="RectangleF" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="RectangleF" /> is equal to the <paramref name="rectangle" />; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(ref RectangleF rectangle)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return X == rectangle.X && Y == rectangle.Y && Width == rectangle.Width && Height == rectangle.Height;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        ///     Returns a value indicating whether this <see cref="RectangleF" /> is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to make the comparison with.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="RectangleF" /> is equal to <paramref name="obj" />; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is RectangleF && Equals((RectangleF)obj);
        }

        /// <summary>
        ///     Returns a hash code of this <see cref="RectangleF" /> suitable for use in hashing algorithms and data
        ///     structures like a hash table.
        /// </summary>
        /// <returns>
        ///     A hash code of this <see cref="RectangleF" />.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Width.GetHashCode();
                hashCode = (hashCode * 397) ^ Height.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="Rectangle" /> to a <see cref="RectangleF" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>
        ///     The resulting <see cref="RectangleF" />.
        /// </returns>
        public static implicit operator RectangleF(Rectangle rectangle)
        {
            return new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="Rectangle" /> to a <see cref="RectangleF" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>
        ///     The resulting <see cref="RectangleF" />.
        /// </returns>
        /// <remarks>
        ///     <para>A loss of precision may occur due to the truncation from <see cref="float" /> to <see cref="int" />.</para>
        /// </remarks>
        public static explicit operator Rectangle(RectangleF rectangle)
        {
            return new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
        }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this <see cref="RectangleF" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this <see cref="RectangleF" />.
        /// </returns>
        public override string ToString()
        {
            return $"{{X: {X}, Y: {Y}, Width: {Width}, Height: {Height}";
        }


    }
}
