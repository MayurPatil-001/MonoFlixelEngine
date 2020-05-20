using Engine.Extensions;
using Engine.MathUtils;
using Engine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engine
{
    public enum FlxObjectDirection : int
    {
        LEFT = 0x0001,
        RIGHT = 0x0010,
        UP = 0x0100,
        DOWN = 0x1000,
        NONE = 0x0000,
        CEILING = UP,
        FLOOR = DOWN,
        WALL = LEFT | RIGHT,
        ANY = LEFT | RIGHT | UP | DOWN
    }

    public class FlxObject : FlxBasic
    {

        public static bool DefaultPixelPerfectPosition = false;
        /**
         * This value dictates the maximum number of pixels two objects have to intersect
         * before collision stops trying to separate them.
         * Don't modify this unless your objects are passing through each other.
         */
        public static float SEPARATE_BIAS = 4;

        #region Core FlxObject
        private Vector2 _position;
        private Vector2 _size;
        private float _rotation;
        private Vector2 _scale;
        private Vector2 _originRatio;
        public virtual Vector2 Position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                {
                    _isTransformMatrixDirty = true;
                    _position = value;
                }
            }
        }
        public Vector2 Size
        {
            get { return _size; }
            set
            {
                if (_size != value)
                {
                    _isTransformMatrixDirty = true;
                    _size = value;
                }
            }
        }
        public bool PixelPerfectPosition { get; set; } = true;
        /// <summary>
        /// Rotation in Radians
        /// </summary>
        public float Rotation
        {
            get { return _rotation; }
            set
            {
                value = MathHelper.WrapAngle(value);
                if (_rotation != value)
                {
                    _isTransformMatrixDirty = true;
                    _rotation = value;
                }
            }
        }
        public Vector2 OriginRatio
        {
            get { return _originRatio; }
            set
            {
                if (_originRatio != value)
                {
                    _isTransformMatrixDirty = true;
                    _originRatio = value;
                }
            }
        }
        public Vector2 Scale
        {
            get { return _scale; }
            set
            {

                if (_scale != value)
                {
                    _isTransformMatrixDirty = true;
                    _scale = value;
                }
            }
        }
        public Color Color;
        public Color DebugColor { get; set; } = Color.Green;
        public int DebugLineWidth { get; set; } = 1;
        public float LayerDepth;
        public SpriteEffects Effect = SpriteEffects.None;

        public virtual float X
        {
            get { return Position.X; }
            set
            {
                if (_position.X != value)
                {
                    _isTransformMatrixDirty = true;
                    _position.X = value;
                }
            }
        }
        public virtual float Y
        {
            get { return Position.Y; }
            set
            {
                if (_position.Y != value)
                {
                    _isTransformMatrixDirty = true;
                    _position.Y = value;
                }
            }
        }
        public virtual float Width
        {
            get { return Size.X; }
            set
            {
                if (_size.X != value)
                {
                    _isTransformMatrixDirty = true;
                    _size.X = value;
                }
            }
        }
        public virtual float Height
        {
            get { return Size.Y; }
            set
            {
                if (_size.Y != value)
                {
                    _isTransformMatrixDirty = true;
                    _size.Y = value;
                }
            }
        }
        public Vector2 Origin 
        { 
            get { return OriginRatio * Size; }
            set 
            {
                float xpos = value.X == 0 ? 0 : value.X / Size.X;
                float ypos = value.Y == 0 ? 0 : value.Y / Size.Y;
                OriginRatio = new Vector2(xpos, ypos); 
            }
        }
        /// <summary>
        /// Angle in Degrees
        /// </summary>
        public float Angle { get { return MathHelper.ToDegrees(Rotation); } set { Rotation = MathHelper.ToRadians(value); } }

        /// <summary>
        /// Returns Render Position in Current Camera
        /// </summary>
        public Vector2 RenderPosition 
        { 
            get 
            {
                return GetScreenPosition(CurrentCamera) + Origin; 
            }
        }
        public bool FlipX
        {
            get { return (Effect & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally; }
            set { Effect = value ? (Effect | SpriteEffects.FlipHorizontally) : (Effect & ~SpriteEffects.FlipHorizontally); }
        }
        public bool FlipY
        {
            get { return (Effect & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically; }
            set { Effect = value ? (Effect | SpriteEffects.FlipVertically) : (Effect & ~SpriteEffects.FlipVertically); }
        }
        public Vector2 MidPoint { get { return new Vector2(X + Width * 0.5f, Y + Height * 0.5f); } }
        public float Health { get; set; }
        #endregion

        #region Movement + Velocity
        public bool Moves { get; set; } = true;
        public bool Immovable { get; set; } = false;
        public Vector2 ScrollFactor;
        public Vector2 Velocity;
        public Vector2 Acceleration;
        public Vector2 Drag;
        public Vector2 MaxVelocity;
        public Vector2 Last;
        /// <summary>
        /// In Degrees
        /// </summary>
        public float AngularVelocity { get { return MathHelper.ToDegrees(AngularVelocityRadians); } set { AngularVelocityRadians = MathHelper.ToRadians(value); } }
        /// <summary>
        /// In Radians
        /// </summary>
        public float AngularVelocityRadians;
        /// <summary>
        /// In Degrees
        /// </summary>
        public float AngularAcceleration { get { return MathHelper.ToDegrees(AngularAccelerationRadians); } set { AngularAccelerationRadians = MathHelper.ToRadians(value); } }
        public float AngularAccelerationRadians;
        /// <summary>
        /// In Degrees
        /// </summary>
        public float AngularDrag { get { return MathHelper.ToDegrees(AngularDragRadians); } set { AngularDragRadians = MathHelper.ToRadians(value); } }
        public float AngularDragRadians;
        /// <summary>
        /// In Degrees
        /// </summary>
        public float MaxAngular { get { return MathHelper.ToDegrees(MaxAngularRadians); } set { MaxAngularRadians = MathHelper.ToRadians(value); } }
        public float MaxAngularRadians;
        #endregion

        #region Collisions
        private static RectangleF _firstSeparateFlxRect = new RectangleF();
        private static RectangleF _secondSeparateFlxRect = new RectangleF();
        private bool _isTransformMatrixDirty = true;
        private Matrix _transformMatrix = Matrix.Identity;
        private Rectangle _boundingBox;
        public Rectangle Hitbox { get => new Rectangle((int)X, (int)Y, (int)Width, (int)Height); }
        public Matrix TransformMatrix
        {
            get
            {
                if (_isTransformMatrixDirty && Solid)
                {
                    CreateTransformMatrix();
                }

                return _transformMatrix;
            }
        }
        /// <summary>
        /// Axis aligned Bounding Box
        /// </summary>
        public Rectangle BoundingBox
        {
            get
            {
                if (_isTransformMatrixDirty)
                    if (Rotation == 0)
                        _boundingBox = new Rectangle((int)X, (int)Y, (int)Width, (int)Height);
                    else
                        _boundingBox = CalculateBoundingRectangle(new Rectangle(0, 0, (int)Width, (int)Height), TransformMatrix);
                return _boundingBox;
            }
        }

        public Rectangle HitBox { get => new Rectangle((int)X, (int)Y, (int)Width, (int)Height); }

        /// <summary>
        /// Whether the object collides or not. For more control over what directions the object will collide from,
        /// use collision constants (like `LEFT`, `FLOOR`, etc) to set the value of `AllowCollisions` directly.
        /// </summary>
        public bool Solid
        {
            get { return (AllowCollisions & FlxObjectDirection.ANY) > FlxObjectDirection.NONE; }
            set { AllowCollisions = value ? FlxObjectDirection.NONE : FlxObjectDirection.ANY; }
        }
        /// <summary>
        /// Bit field of flags (use with UP, DOWN, LEFT, RIGHT, etc) indicating collision directions. Use bitwise operators to check the values stored here.
        /// Useful for things like one-way platforms (e.g. allowCollisions = UP;). The accessor "solid" just flips this variable between NONE and ANY.
        /// </summary>
        public FlxObjectDirection AllowCollisions { get; set; } = FlxObjectDirection.ANY;
        public float Elasticity { get; set; }
        public float Mass { get; set; } = 1;
        public FlxObjectDirection Touching = FlxObjectDirection.NONE;
        public FlxObjectDirection WasTouching = FlxObjectDirection.NONE;
        /// <summary>
        /// Whether this sprite is dragged along with the horizontal movement of objects it collides with
        /// (makes sense for horizontally-moving platforms in platformers for example).
        /// </summary>
        public bool CollisonXDrag { get; set; } = true;

        #endregion

        public FlxObject(float x, float y, bool initialize = true)
        {
            Color = Color.White;
            Position = new Vector2(x, y);
            Scale = new Vector2(1, 1);
            OriginRatio = new Vector2(0.5f, 0.5f);
            Alive = true;
            Exists = true;
            Health = 1;
            if(initialize)
                Initialize();
        }

        public FlxObject(Vector2 position, bool initialize = true) : this(position.X, position.Y, initialize)
        {

        }

        public override void Initialize()
        {
            InitVars();
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            Last = Position;

            if (Moves)
                UpdateMotion(gameTime);

            base.Update(gameTime);

            WasTouching = Touching;
            Touching = FlxObjectDirection.NONE;
        }

#if DEBUG
        public override void DebugDrawBoundigbox(GameTime gameTime)
        {
            Rectangle _top = new Rectangle(BoundingBox.X, BoundingBox.Y, BoundingBox.Width, DebugLineWidth);
            Rectangle _right = new Rectangle(BoundingBox.Right, BoundingBox.Y, DebugLineWidth, BoundingBox.Height);
            Rectangle _bottom = new Rectangle(BoundingBox.X, BoundingBox.Bottom, BoundingBox.Width, DebugLineWidth);
            Rectangle _left = new Rectangle(BoundingBox.X, BoundingBox.Y, DebugLineWidth, BoundingBox.Height);
            SpriteBatch.Draw(FlxG.PixelTexture, _top, DebugColor);
            SpriteBatch.Draw(FlxG.PixelTexture, _right, DebugColor);
            SpriteBatch.Draw(FlxG.PixelTexture, _bottom, DebugColor);
            SpriteBatch.Draw(FlxG.PixelTexture, _left, DebugColor);
        }

        public override void DebugDrawHitBox(GameTime gameTime)
        {
            Rectangle _top = new Rectangle(HitBox.X, HitBox.Y, HitBox.Width, DebugLineWidth);
            Rectangle _right = new Rectangle(HitBox.Right, HitBox.Y, DebugLineWidth, HitBox.Height);
            Rectangle _bottom = new Rectangle(HitBox.X, HitBox.Bottom, HitBox.Width, DebugLineWidth);
            Rectangle _left = new Rectangle(HitBox.X, HitBox.Y, DebugLineWidth, HitBox.Height);
            SpriteBatch.Draw(FlxG.PixelTexture, _top, DebugColor);
            SpriteBatch.Draw(FlxG.PixelTexture, _right, DebugColor);
            SpriteBatch.Draw(FlxG.PixelTexture, _bottom, DebugColor);
            SpriteBatch.Draw(FlxG.PixelTexture, _left, DebugColor);
        }
#endif

        #region Core FlxObject
        public override void Kill()
        {
            Alive = false;
            Exists = false;
        }

        public override void Revive()
        {
            Alive = true;
            Exists = true;
        }

        public virtual void Hurt(float damage)
        {
            Health -= damage;
            if (Health <= 0)
                Kill();
        }
        public virtual void Reset(Vector2 position)
        {
            Position = position;
            Last = position;
            Velocity = Vector2.Zero;
            Revive();
        }

        public virtual void Reset(float x, float y)
        {
            Reset(new Vector2(x, y));
        }

        public Vector2 GetScreenPosition(FlxCamera camera = null)
        {
            if (camera == null)
                camera = FlxG.Camera;
            Vector2 screenPosition = new Vector2(X, Y);

            if (PixelPerfectPosition)
                screenPosition = screenPosition.Floor();

            screenPosition.X += camera.Scroll.X * (1 - ScrollFactor.X);
            screenPosition.Y += camera.Scroll.Y * (1 - ScrollFactor.Y);
            return screenPosition;//.Subtract(camera.Scroll.X * ScrollFactor.X, camera.Scroll.Y * ScrollFactor.Y);
        }

        public virtual bool IsOnScreen(FlxCamera camera = null)
        {
            if (camera == null)
                camera = FlxG.Camera;
            Vector2 screenPosition = GetScreenPosition(camera);
            return camera.ContainsPoint(screenPosition, Width, Height);
        }

        public virtual void SetPosition(float x, float y)
        {
            X = x;
            Y = y;
        }

        public void SetSize(float width, float height)
        {
            Width = width;
            Height = height;
        }
        public virtual FlxObject ScreenCenter(FlxAxes? axes = null)
        {
            if (axes == null)
                axes = FlxAxes.XY;
            if (axes != FlxAxes.Y)
                X = (FlxG.Width / 2) - (Width / 2);
            if (axes != FlxAxes.X)
                Y = (FlxG.Height / 2) - (Height / 2);
            return this;
        }

        public bool IsTouching(FlxObjectDirection direction)
        {
            return (Touching & direction) > FlxObjectDirection.NONE;
        }

        public bool JustTouching(FlxObjectDirection direction)
        {
            return IsTouching(direction) && (WasTouching & direction) > FlxObjectDirection.NONE;
        }
        #endregion

        #region Movement + Velocity
        protected virtual void InitVars()
        {
            Last = new Vector2(X, Y);
            ScrollFactor = new Vector2(1, 1);
            PixelPerfectPosition = DefaultPixelPerfectPosition;
            InitMotionVars();
        }

        protected void InitMotionVars()
        {
            Velocity = new Vector2();
            Acceleration = new Vector2();
            Drag = new Vector2();
            MaxVelocity = new Vector2(10000, 10000);
        }

        protected void UpdateMotion(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float velocityDelta = 0.5f * (FlxVelocity.ComputeVelocity(AngularVelocity, AngularAcceleration, AngularDrag, MaxAngular, delta) - AngularVelocity);
            AngularVelocity += velocityDelta;
            Angle += AngularVelocity * delta;
            AngularVelocity += velocityDelta;

            velocityDelta = 0.5f * (FlxVelocity.ComputeVelocity(Velocity.X, Acceleration.X, Drag.X, MaxVelocity.X, delta) - Velocity.X);
            Velocity.X += velocityDelta;
            float xDelta = Velocity.X * delta;
            Velocity.X += velocityDelta;
            X += xDelta;

            velocityDelta = 0.5f * (FlxVelocity.ComputeVelocity(Velocity.Y, Acceleration.Y, Drag.Y, MaxVelocity.Y, delta) - Velocity.Y);
            Velocity.Y += velocityDelta;
            float yDelta = Velocity.Y * delta;
            Velocity.Y += velocityDelta;
            Y += yDelta;
        }
        #endregion

        #region Collisions
        private void CreateTransformMatrix()
        {
            _transformMatrix = Matrix.CreateTranslation(new Vector3(-Origin, 0)) *
                Matrix.CreateScale(new Vector3(Scale, 1f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateTranslation(new Vector3(RenderPosition, 0f));
            _isTransformMatrixDirty = false;
        }

        /// <summary>
        /// Calculates an axis aligned rectangle which fully contains an arbitrarily
        /// transformed axis aligned rectangle.
        /// </summary>
        /// <param name="rectangle">Original bounding rectangle.</param>
        /// <param name="transform">World transform of the rectangle.</param>
        /// <returns>A new rectangle which contains the trasnformed rectangle.</returns>
        private static Rectangle CalculateBoundingRectangle(Rectangle rectangle,
                                                           Matrix transform)
        {
            // Get all four corners in local space
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            // Return that as a rectangle
            return new Rectangle((int)min.X, (int)min.Y,
                                 (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        public static bool Separate(FlxObject object1, FlxObject object2)
        {
            bool separateX = SeparateX(object1, object2);
            bool separateY = SeparateY(object1, object2);
            return separateX || separateY;
        }

        public static bool UpdateTouchingFlags(FlxObject object1, FlxObject object2)
        {
            bool touchingX = UpdateTouchingFlagsX(object1, object2);
            bool touchingY = UpdateTouchingFlagsY(object1, object2);
            return touchingX || touchingY;
        }

        private static float ComputeOverlapX(FlxObject object1, FlxObject object2, bool checkMaxOverlap = true)
        {
            float overlap = 0f;
            float obj1delta = object1.X - object1.Last.X;
            float obj2delta = object2.X - object2.Last.X;
            if (obj1delta != obj2delta)
            {
                // Check if the X hulls actually overlap
                float obj1deltaAbs = (obj1delta > 0) ? obj1delta : -obj1delta;
                float obj2deltaAbs = (obj2delta > 0) ? obj2delta : -obj2delta;

                RectangleF obj1rect = _firstSeparateFlxRect.Set(object1.X - ((obj1delta > 0) ? obj1delta : 0), object1.Last.Y, object1.Width + obj1deltaAbs,
                    object1.Height);
                RectangleF obj2rect = _secondSeparateFlxRect.Set(object2.X - ((obj2delta > 0) ? obj2delta : 0), object2.Last.Y, object2.Width + obj2deltaAbs,
                    object2.Height);

                if ((obj1rect.X + obj1rect.Width > obj2rect.X)
                    && (obj1rect.X < obj2rect.X + obj2rect.Width)
                    && (obj1rect.Y + obj1rect.Height > obj2rect.Y)
                    && (obj1rect.Y < obj2rect.Y + obj2rect.Height))
                {
                    float maxOverlap = checkMaxOverlap ? (obj1deltaAbs + obj2deltaAbs + SEPARATE_BIAS) : 0;

                    // If they did overlap (and can), figure out by how much and flip the corresponding flags
                    if (obj1delta > obj2delta)
                    {
                        overlap = object1.X + object1.Width - object2.X;
                        if ((checkMaxOverlap && (overlap > maxOverlap))
                            || ((object1.AllowCollisions & FlxObjectDirection.RIGHT) == 0)
                            || ((object2.AllowCollisions & FlxObjectDirection.LEFT) == 0))
                        {
                            overlap = 0;
                        }
                        else
                        {
                            object1.Touching |= FlxObjectDirection.RIGHT;
                            object2.Touching |= FlxObjectDirection.LEFT;
                        }
                    }
                    else if (obj1delta < obj2delta)
                    {
                        overlap = object1.X - object2.Width - object2.X;
                        if ((checkMaxOverlap && (-overlap > maxOverlap))
                            || ((object1.AllowCollisions & FlxObjectDirection.LEFT) == 0)
                            || ((object2.AllowCollisions & FlxObjectDirection.RIGHT) == 0))
                        {
                            overlap = 0;
                        }
                        else
                        {
                            object1.Touching |= FlxObjectDirection.LEFT;
                            object2.Touching |= FlxObjectDirection.RIGHT;
                        }
                    }
                }
            }
            return overlap;
        }
        /**
	 * The X-axis component of the object separation process.
	 *
	 * @param   Object1   Any `FlxObject`.
	 * @param   Object2   Any other `FlxObject`.
	 * @return  Whether the objects in fact touched and were separated along the X axis.
	 */
        public static bool SeparateX(FlxObject Object1, FlxObject Object2)
        {
            // can't separate two immovable objects
            bool obj1immovable = Object1.Immovable;
            bool obj2immovable = Object2.Immovable;
            if (obj1immovable && obj2immovable)
            {
                return false;
            }

            // If one of the objects is a tilemap, just pass it off.
            if (Object1.FlixelType == FlxType.TILEMAP)
            {
                throw new NotImplementedException();
                //var tilemap:FlxBaseTilemap<object> = cast Object1;
                //return tilemap.overlapsWithCallback(Object2, separateX);
            }
            if (Object2.FlixelType == FlxType.TILEMAP)
            {

                throw new NotImplementedException();
                //             var tilemap:FlxBaseTilemap<Dynamic> = cast Object2;
                //return tilemap.overlapsWithCallback(Object1, separateX, true);
            }

            float overlap = ComputeOverlapX(Object1, Object2);
            // Then adjust their positions and velocities accordingly (if there was any overlap)
            if (overlap != 0)
            {
                float obj1v = Object1.Velocity.X;
                float obj2v = Object2.Velocity.X;

                if (!obj1immovable && !obj2immovable)
                {
                    overlap *= 0.5f;
                    Object1.X = Object1.X - overlap;
                    Object2.X += overlap;

                    float obj1velocity = (float)Math.Sqrt((obj2v * obj2v * Object2.Mass) / Object1.Mass) * ((obj2v > 0) ? 1f : -1f);
                    float obj2velocity = (float)Math.Sqrt((obj1v * obj1v * Object1.Mass) / Object2.Mass) * ((obj1v > 0) ? 1f : -1f);
                    float average = (obj1velocity + obj2velocity) * 0.5f;
                    obj1velocity -= average;
                    obj2velocity -= average;
                    Object1.Velocity.X = average + obj1velocity * Object1.Elasticity;
                    Object2.Velocity.X = average + obj2velocity * Object2.Elasticity;
                }
                else if (!obj1immovable)
                {
                    Object1.X = Object1.X - overlap;
                    Object1.Velocity.X = obj2v - obj1v * Object1.Elasticity;
                }
                else if (!obj2immovable)
                {
                    Object2.X += overlap;
                    Object2.Velocity.X = obj1v - obj2v * Object2.Elasticity;
                }
                return true;
            }

            return false;
        }
        /**
	 * Checking overlap and updating `touching` variables, X-axis part used by `updateTouchingFlags`.
	 *
	 * @param   Object1   Any `FlxObject`.
	 * @param   Object2   Any other `FlxObject`.
	 * @return  Whether the objects in fact touched along the X axis.
	 */
        public static bool UpdateTouchingFlagsX(FlxObject Object1, FlxObject Object2)
        {
            // If one of the objects is a tilemap, just pass it off.
            if (Object1.FlixelType == FlxType.TILEMAP)
            {
                throw new NotImplementedException();
                //var tilemap:FlxBaseTilemap<Dynamic> = cast Object1;
                //return tilemap.overlapsWithCallback(Object2, updateTouchingFlagsX);
            }
            if (Object2.FlixelType == FlxType.TILEMAP)
            {
                throw new NotImplementedException();
                //var tilemap:FlxBaseTilemap<Dynamic> = cast Object2;
                //return tilemap.overlapsWithCallback(Object1, updateTouchingFlagsX, true);
            }
            // Since we are not separating, always return any amount of overlap => false as last parameter
            return ComputeOverlapX(Object1, Object2, false) != 0;
        }

        /**
	 * Internal function that computes overlap among two objects on the Y axis. It also updates the `touching` variable.
	 * `checkMaxOverlap` is used to determine whether we want to exclude (therefore check) overlaps which are
	 * greater than a certain maximum (linked to `SEPARATE_BIAS`). Default is `true`, handy for `separateY` code.
	 */
        private static float ComputeOverlapY(FlxObject Object1, FlxObject Object2, bool checkMaxOverlap = true)
        {
            float overlap = 0f;
            // First, get the two object deltas
            float obj1delta = Object1.Y - Object1.Last.Y;
            float obj2delta = Object2.Y - Object2.Last.Y;

            if (obj1delta != obj2delta)
            {
                // Check if the Y hulls actually overlap
                float obj1deltaAbs = (obj1delta > 0) ? obj1delta : -obj1delta;
                float obj2deltaAbs = (obj2delta > 0) ? obj2delta : -obj2delta;

                RectangleF obj1rect = _firstSeparateFlxRect.Set(Object1.X, Object1.Y - ((obj1delta > 0) ? obj1delta : 0), Object1.Width,
                    Object1.Height + obj1deltaAbs);
                RectangleF obj2rect = _secondSeparateFlxRect.Set(Object2.X, Object2.Y - ((obj2delta > 0) ? obj2delta : 0), Object2.Width,
                Object2.Height + obj2deltaAbs);

                if ((obj1rect.X + obj1rect.Width > obj2rect.X)
                    && (obj1rect.X < obj2rect.X + obj2rect.Width)
                    && (obj1rect.Y + obj1rect.Height > obj2rect.Y)
                    && (obj1rect.Y < obj2rect.Y + obj2rect.Height))
                {
                    float maxOverlap = checkMaxOverlap ? (obj1deltaAbs + obj2deltaAbs + SEPARATE_BIAS) : 0;

                    // If they did overlap (and can), figure out by how much and flip the corresponding flags
                    if (obj1delta > obj2delta)
                    {
                        overlap = Object1.Y + Object1.Height - Object2.Y;
                        if ((checkMaxOverlap && (overlap > maxOverlap))
                            || ((Object1.AllowCollisions & FlxObjectDirection.DOWN) == 0)
                            || ((Object2.AllowCollisions & FlxObjectDirection.UP) == 0))
                        {
                            overlap = 0;
                        }
                        else
                        {
                            Object1.Touching |= FlxObjectDirection.DOWN;
                            Object2.Touching |= FlxObjectDirection.UP;
                        }
                    }
                    else if (obj1delta < obj2delta)
                    {
                        overlap = Object1.Y - Object2.Height - Object2.Y;
                        if ((checkMaxOverlap && (-overlap > maxOverlap))
                            || ((Object1.AllowCollisions & FlxObjectDirection.UP) == 0)
                            || ((Object2.AllowCollisions & FlxObjectDirection.DOWN) == 0))
                        {
                            overlap = 0;
                        }
                        else
                        {
                            Object1.Touching |= FlxObjectDirection.UP;
                            Object2.Touching |= FlxObjectDirection.DOWN;
                        }
                    }
                }
            }
            return overlap;
        }
        /**
	 * The Y-axis component of the object separation process.
	 *
	 * @param   Object1   Any `FlxObject`.
	 * @param   Object2   Any other `FlxObject`.
	 * @return  Whether the objects in fact touched and were separated along the Y axis.
	 */
        public static bool SeparateY(FlxObject Object1, FlxObject Object2)
        {
            // can't separate two immovable objects
            bool obj1immovable = Object1.Immovable;
            bool obj2immovable = Object2.Immovable;
            if (obj1immovable && obj2immovable)
            {
                return false;
            }

            // If one of the objects is a tilemap, just pass it off.
            if (Object1.FlixelType == FlxType.TILEMAP)
            {
                throw new NotImplementedException();
                //var tilemap:FlxBaseTilemap<Dynamic> = cast Object1;
                //return tilemap.overlapsWithCallback(Object2, separateY);
            }
            if (Object2.FlixelType == FlxType.TILEMAP)
            {
                throw new NotImplementedException();
                //var tilemap:FlxBaseTilemap<Dynamic> = cast Object2;
                //return tilemap.overlapsWithCallback(Object1, separateY, true);
            }

            float overlap = ComputeOverlapY(Object1, Object2);
            // Then adjust their positions and velocities accordingly (if there was any overlap)
            if (overlap != 0)
            {
                float obj1delta = Object1.Y - Object1.Last.Y;
                float obj2delta = Object2.Y - Object2.Last.Y;
                float obj1v = Object1.Velocity.Y;
                float obj2v = Object2.Velocity.Y;

                if (!obj1immovable && !obj2immovable)
                {
                    overlap *= 0.5f;
                    Object1.Y = Object1.Y - overlap;
                    Object2.Y += overlap;

                    float obj1velocity = (float)Math.Sqrt((obj2v * obj2v * Object2.Mass) / Object1.Mass) * ((obj2v > 0) ? 1 : -1);
                    float obj2velocity = (float)Math.Sqrt((obj1v * obj1v * Object1.Mass) / Object2.Mass) * ((obj1v > 0) ? 1 : -1);
                    float average = (obj1velocity + obj2velocity) * 0.5f;
                    obj1velocity -= average;
                    obj2velocity -= average;
                    Object1.Velocity.Y = average + obj1velocity * Object1.Elasticity;
                    Object2.Velocity.Y = average + obj2velocity * Object2.Elasticity;
                }
                else if (!obj1immovable)
                {
                    Object1.Y = Object1.Y - overlap;
                    Object1.Velocity.Y = obj2v - obj1v * Object1.Elasticity;
                    // This is special case code that handles cases like horizontal moving platforms you can ride
                    if (Object1.CollisonXDrag && Object2.Active && Object2.Moves && (obj1delta > obj2delta))
                    {
                        Object1.X += Object2.X - Object2.Last.X;
                    }
                }
                else if (!obj2immovable)
                {
                    Object2.Y += overlap;
                    Object2.Velocity.Y = obj1v - obj2v * Object2.Elasticity;
                    // This is special case code that handles cases like horizontal moving platforms you can ride
                    if (Object2.CollisonXDrag && Object1.Active && Object1.Moves && (obj1delta < obj2delta))
                    {
                        Object2.X += Object1.X - Object1.Last.X;
                    }
                }
                return true;
            }

            return false;
        }
        /**
	 * Checking overlap and updating touching variables, Y-axis part used by `updateTouchingFlags`.
	 *
	 * @param   Object1   Any `FlxObject`.
	 * @param   Object2   Any other `FlxObject`.
	 * @return  Whether the objects in fact touched along the Y axis.
	 */
        public static bool UpdateTouchingFlagsY(FlxObject Object1, FlxObject Object2)
        {
            // If one of the objects is a tilemap, just pass it off.
            if (Object1.FlixelType == FlxType.TILEMAP)
            {
                throw new NotImplementedException();
                //var tilemap:FlxBaseTilemap<Dynamic> = cast Object1;
                //return tilemap.overlapsWithCallback(Object2, updateTouchingFlagsY);
            }
            if (Object2.FlixelType == FlxType.TILEMAP)
            {
                throw new NotImplementedException();
                //var tilemap:FlxBaseTilemap<Dynamic> = cast Object2;
                //return tilemap.overlapsWithCallback(Object1, updateTouchingFlagsY, true);
            }
            // Since we are not separating, always return any amount of overlap => false as last parameter
            return ComputeOverlapY(Object1, Object2, false) != 0;
        }


        #endregion
    }
}