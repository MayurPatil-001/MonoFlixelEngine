using Engine.Extensions;
using Engine.MathUtils;
using Engine.Utils;
using Engine.ViewportAdapters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Engine
{
    public enum FlxCameraFollowStyle
    {
        /**
	 * Camera has no deadzone, just tracks the focus object directly.
	 */
        LOCKON,

        /**
         * Camera's deadzone is narrow but tall.
         */
        PLATFORMER,

        /**
         * Camera's deadzone is a medium-size square around the focus object.
         */
        TOPDOWN,

        /**
         * Camera's deadzone is a small square around the focus object.
         */
        TOPDOWN_TIGHT,

        /**
         * Camera will move screenwise.
         */
        SCREEN_BY_SCREEN,

        /**
         * Camera has no deadzone, just tracks the focus object directly and centers it.
         */
        NO_DEAD_ZONE

    }
    public class FlxCamera : FlxBasic
    {
        #region Core Fields
        private float _maximumZoom = float.MaxValue;
        private float _minimumZoom;
        private float _zoom;
        #region Cache
        //TODO
        #endregion
        
        private float _layerDepth = 0;
        public BlendState BlendState;
        public SamplerState SamplerState;
        public Effect Effect;
        public SpriteEffects SpriteEffects = SpriteEffects.None;
        public Color BgColor;
        #endregion

        #region FX Fields

        private Color _fxFlashColor = Color.Transparent;
        private float _fxFlashDuration = 0f;
        private Action _fxFlashComplete;
        private float _fxFlashAlpha = 0f;

        private Color _fxFadeColor = Color.Transparent;
        private float _fxFadeDuration = 0f;
        private bool _fxFadeIn = false;
        private Action _fxFadeComplete;
        private bool _fxFadeCompleted = true;
        private float _fxFadeAlpha = 0f;

        private Vector2 _fxShakeOriginalPosition;
        private float _fxShakeIntensity = 0f;
        private float _fxShakeDuration = 0f;
        private Action _fxShakeComplete;
        private FlxAxes _fxShakeAxes = FlxAxes.XY;
        #endregion

        #region Core Properties
        public Vector2 Position { get; set; }
        public float X { get => Position.X; set => Position = new Vector2(value, Y); }
        public float Y { get => Position.Y; set => Position = new Vector2(X, value); }
        public float Rotation { get; set; }
        public Vector2 Origin { get; set; }
        public Vector2 Center => Position + Origin;
        public float Width { get => ViewportAdapter.VirtualWidth; }
        public float Height { get => ViewportAdapter.VirtualHeight; }
        public float InitialZoom { get; private set; }
        public Vector2 InitialPosition { get; private set; }

        public float Zoom
        {
            get => _zoom;
            set
            {
                if ((value < MinimumZoom) || (value > MaximumZoom))
                    throw new ArgumentException("Zoom must be between MinimumZoom and MaximumZoom");
                _zoom = value;
            }
        }

        public float MinimumZoom
        {
            get => _minimumZoom;
            set
            {
                if (value < 0)
                    throw new ArgumentException("MinimumZoom must be greater than zero");

                if (Zoom < value)
                    Zoom = MinimumZoom;
                _minimumZoom = value;
            }
        }

        public float MaximumZoom
        {
            get => _maximumZoom;
            set
            {
                if (value < 0)
                    throw new ArgumentException("MaximumZoom must be greater than zero");

                if (Zoom > value)
                    Zoom = value;
                _maximumZoom = value;
            }
        }
        public RectangleF BoundingRectangle
        {
            get
            {
                var frustum = GetBoundingFrustum();
                var corners = frustum.GetCorners();
                var topLeft = corners[0];
                var bottomRight = corners[2];
                var width = bottomRight.X - topLeft.X;
                var height = bottomRight.Y - topLeft.Y;
                return new RectangleF(topLeft.X, topLeft.Y, width, height);
            }
        }

        public Matrix ViewMatrix { get => GetViewMatrix(); }
        public ViewportAdapter ViewportAdapter { get; private set; }
        public static float DefaultZoom { get; set; }
        public static List<FlxCamera> DefaultCameras { get; set; }
        #endregion

        #region Util Properties
        public RectangleF ScrollBounds { get; set; }
        public Vector2 ViewOffset { get => InitialPosition - Position; }
        /// <summary>
        /// Stores the basic parallax scrolling values.
        /// This is basically the camera's top-left corner position in world coordinates.
        /// There is also `focusOn(point:FlxPoint)` which you can use to
        /// make the camera look at specified point in world coordinates.
        /// </summary>
        public Vector2 Scroll { get; set; } = Vector2.Zero;
        #endregion
        #region FX Properties
        public FlxCameraFollowStyle Style;
        public FlxObject Target { get; set; }
        public Vector2 TargetOffset { get; set; }
        public float FollowLerp { get; set; } = 1;
        public Vector2 FollowLead { get; set; }

        public Rectangle DeadZone { get; set; }
        public float? MinScrollX;
        public float? MaxScrollX;
        public float? MinScrollY;
        public float? MaxScrollY;

        #endregion

        #region Constructors
        public FlxCamera(GraphicsDevice graphicsDevice)
            : this(new DefaultViewportAdapter(graphicsDevice))
        {
        }

        public FlxCamera(ViewportAdapter viewportAdapter)
        {
            ViewportAdapter = viewportAdapter;

            Rotation = 0;
            Zoom = 1;
            InitialZoom = Zoom;
            Origin = new Vector2(viewportAdapter.VirtualWidth / 2f, viewportAdapter.VirtualHeight / 2f);
            Position = Vector2.Zero;
            BgColor = FlxG.Cameras.BgColor;
            InitialPosition = Position;
        }

        public FlxCamera() : this(new BoxingViewportAdapter(FlxG.Game.Window, FlxG.Game.GraphicsDevice, FlxG.Width, FlxG.Height))
        {
        }

        public FlxCamera(int width, int height) : this(new BoxingViewportAdapter(FlxG.Game.Window, FlxG.Game.GraphicsDevice, width, height))
        {
        }
        #endregion


        #region Overrides
        public override void Initialize()
        {
            BlendState = BlendState.AlphaBlend;
            SamplerState = SamplerState.LinearClamp;
            base.Initialize();
        }
        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(Game.GraphicsDevice);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (Target != null)
                UpdateFollow(gameTime);
            UpdateScorll(gameTime);
            UpdateFlash(gameTime);
            UpdateFade(gameTime);
            UpdateShake(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!Visible)
                return;

            // FX Flash
            if (_fxFlashAlpha > 0f)
                SpriteBatch.Draw(FlxG.PixelTexture, (Rectangle)BoundingRectangle, FlxG.PixelTexture.Bounds, _fxFlashColor * _fxFlashAlpha, Rotation, Origin, SpriteEffects, _layerDepth);

            base.Draw(gameTime);
        }
        #endregion

        #region Transform Methods
        public void LookAt(Vector2 position)
        {
            Position = position - new Vector2(ViewportAdapter.VirtualWidth / 2f, ViewportAdapter.VirtualHeight / 2f);
        }

        public Vector2 WorldToScreen(float x, float y)
        {
            return WorldToScreen(new Vector2(x, y));
        }

        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            var viewport = ViewportAdapter.Viewport;
            return Vector2.Transform(worldPosition + new Vector2(viewport.X, viewport.Y), GetViewMatrix());
        }

        public Vector2 ScreenToWorld(float x, float y)
        {
            return ScreenToWorld(new Vector2(x, y));
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            var viewport = ViewportAdapter.Viewport;
            return Vector2.Transform(screenPosition - new Vector2(viewport.X, viewport.Y),
                Matrix.Invert(GetViewMatrix()));
        }
        public void Move(Vector2 direction)
        {
            Position += Vector2.Transform(direction, Matrix.CreateRotationZ(-Rotation));
        }

        public void Rotate(float deltaRadians)
        {
            Rotation += deltaRadians;
        }

        public void ZoomIn(float deltaZoom)
        {
            ClampZoom(Zoom + deltaZoom);
        }

        public void ZoomOut(float deltaZoom)
        {
            ClampZoom(Zoom - deltaZoom);
        }

        #endregion

        /*
         *  Utils
         */
        #region Utils
        private Matrix GetVirtualViewMatrix(Vector2 parallaxFactor)
        {
            return
                Matrix.CreateTranslation(new Vector3(-Position * parallaxFactor, 0.0f)) *
                Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Zoom, Zoom, 1) *
                Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        }
        public Matrix GetViewMatrix(Vector2 parallaxFactor)
        {
            return GetVirtualViewMatrix(parallaxFactor) * ViewportAdapter.GetScaleMatrix();
        }

        private Matrix GetVirtualViewMatrix()
        {
            return GetVirtualViewMatrix(Vector2.One);
        }

        public Matrix GetViewMatrix()
        {
            return GetViewMatrix(Vector2.One);
        }

        public Matrix GetInverseViewMatrix()
        {
            return Matrix.Invert(GetViewMatrix());
        }

        private Matrix GetProjectionMatrix(Matrix viewMatrix)
        {
            var projection = Matrix.CreateOrthographicOffCenter(0, ViewportAdapter.VirtualWidth, ViewportAdapter.VirtualHeight, 0, -1, 0);
            Matrix.Multiply(ref viewMatrix, ref projection, out projection);
            return projection;
        }

        public BoundingFrustum GetBoundingFrustum()
        {
            var viewMatrix = GetVirtualViewMatrix();
            var projectionMatrix = GetProjectionMatrix(viewMatrix);
            return new BoundingFrustum(projectionMatrix);
        }

        public ContainmentType Contains(Point point)
        {
            return Contains(point.ToVector2());
        }

        public ContainmentType Contains(Vector2 vector2)
        {
            return GetBoundingFrustum().Contains(new Vector3(vector2.X, vector2.Y, 0));
        }

        public bool ContainsPoint(Vector2 vector2, float width = 0, float height = 0)
        {
            if (width == 0 || height == 0)
                return Contains(vector2) != ContainmentType.Disjoint;
            else
                return Contains(new Rectangle((int)vector2.X, (int)vector2.Y, (int)width, (int)height)) != ContainmentType.Disjoint;
        }

        public ContainmentType Contains(Rectangle rectangle)
        {
            var max = new Vector3(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, 0.5f);
            var min = new Vector3(rectangle.X, rectangle.Y, 0.5f);
            var boundingBox = new BoundingBox(min, max);
            return GetBoundingFrustum().Contains(boundingBox);
        }
        private void ClampZoom(float value)
        {
            if (value < MinimumZoom)
                Zoom = MinimumZoom;
            else
                Zoom = value > MaximumZoom ? MaximumZoom : value;
        }

        #endregion

        #region Camera Effects
        public void Flash(Color color, float duration, Action onComplete = null, bool force = false)
        {
            if (!force && _fxFlashAlpha > 0f)
                return;
            _fxFlashColor = color;
            if (duration < 0)
                duration = 0.000001f;
            _fxFlashDuration = duration;
            _fxFlashComplete = onComplete;
            _fxFlashAlpha = 1.0f;
        }

        public void Fade(Color color, float duration = 1f, bool fadeIn = false, Action onComplete = null, bool force = false)
        {
            if (!_fxFadeCompleted && !force)
                return;
            _fxFadeColor = color;
            if (duration <= 0)
                duration = 0.000001f;
            _fxFadeIn = fadeIn;
            _fxFadeDuration = duration;
            _fxFadeComplete = onComplete;

            _fxFadeAlpha = _fxFadeIn ? 0.999999f : 0.000001f;
            _fxFadeCompleted = false;
        }

        public void Fade(float duration = 1f, bool fadeIn = false, Action onComplete = null, bool force = false)
        {
            Fade(Color.Black, duration, fadeIn, onComplete, force);
        }

        public void Shake(float intensity = 0.05f, float duration = 0.5f, Action onComplete = null, bool force = false, FlxAxes axes = FlxAxes.XY)
        {
            if (!force && _fxShakeDuration > 0)
                return;
            _fxShakeIntensity = intensity;
            _fxShakeDuration = duration;
            _fxShakeComplete = onComplete;
            _fxShakeAxes = axes;
            _fxShakeOriginalPosition = Position;
        }

        private void UpdateScorll(GameTime gameTime)
        {
            //TODO
        }

        private void UpdateFlash(GameTime gameTime)
        {
            if (_fxFlashAlpha > 0f)
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                _fxFlashAlpha -= elapsed / _fxFlashDuration;
                if (_fxFlashAlpha <= 0)
                    _fxFlashComplete?.Invoke();
            }
        }

        private void UpdateFade(GameTime gameTime)
        {
            if (_fxFadeCompleted)
                return;
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_fxFadeIn)
            {
                _fxFadeAlpha -= elapsed / _fxFadeDuration;
                if (_fxFadeAlpha <= 0f)
                {
                    _fxFadeAlpha = 0f;
                    CompleteFade();
                }
            }
            else
            {
                _fxFadeAlpha += elapsed / _fxFadeDuration;
                if (_fxFadeAlpha >= 1f)
                {
                    _fxFadeAlpha = 1f;
                    CompleteFade();
                }
            }
        }
        private void CompleteFade()
        {
            _fxFadeCompleted = true;
            _fxFadeComplete?.Invoke();
        }

        private void UpdateShake(GameTime gameTime)
        {
            if (_fxShakeDuration > 0f)
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                _fxShakeDuration -= elapsed;
                if (_fxShakeDuration <= 0)
                {
                    Position = _fxShakeOriginalPosition;
                    _fxShakeComplete?.Invoke();
                }
                else
                {
                    Position = _fxShakeOriginalPosition;
                    if (_fxShakeAxes != FlxAxes.Y)
                        X += FlxG.Random.NextFloat(-_fxShakeIntensity * Width, _fxShakeIntensity * Width) * Zoom;
                    if (_fxShakeAxes != FlxAxes.X)
                        Y += FlxG.Random.NextFloat(-_fxShakeIntensity * Height, _fxShakeIntensity * Height) * Zoom;
                }
            }
        }

        public void UpdateFollow(GameTime gameTime)
        {

        }

        public void StopFX()
        {
            _fxFlashAlpha = 0f;
            _fxFadeAlpha = 0f;
            _fxShakeDuration = 0f;
        }
        #endregion
    }
}
