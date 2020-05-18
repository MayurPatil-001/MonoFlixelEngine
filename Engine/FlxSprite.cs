using Engine.Animations;
using Engine.Extensions;
using Engine.MathUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engine
{
    public class FlxSprite : FlxObject
    {
        private string _texturePath;
        private bool _isTextureConstructed;
        public float Alpha { get; set; } = 1.0f;
        public bool IsAnimated { get; private set; }
        public AnimationController Animation { get; private set; }
        public Texture2D Texture { get; private set; }
        public AnimationFrame CurrentFrame { get => Animation.CurrentAnimation.CurrentFrame; }
        public float FrameWidth { get => CurrentFrame.SourceRectangle.Width; }
        public float FrameHeight { get => CurrentFrame.SourceRectangle.Height; }
        private Vector2 HalfSize { get => new Vector2(FrameWidth * 0.5f, FrameHeight * 0.5f); }
        #region Collisions
        /// <summary>
        /// Gets Color[] of current Frame
        /// </summary>
        public Color[] BitmapData { get => CurrentFrame.BitmapData; }
        public Vector2 Offset;
        #endregion


        public FlxSprite(float x = 0, float y = 0, string graphicAssetPath = null):base(x, y, false)
        {
            _texturePath = graphicAssetPath;
            _isTextureConstructed = false;
            Initialize();
            if(_texturePath != null)
                LoadGraphic(_texturePath);
        }

        public FlxSprite(Vector2 position, string graphicAssetPath):this(position.X, position.Y, graphicAssetPath)
        {
        }

        protected override void LoadContent()
        {
            if (Texture != null || _texturePath == null)
                return;
            Texture = Game.Content.Load<Texture2D>(_texturePath);
            //BitmapData = new Color[Texture.Width * Texture.Height];
            //Texture.GetData(BitmapData);
#if DEBUG
            FlxG.Log.Info("Texture Loaded for" + this);
#endif
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            Animation.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Draw(CurrentFrame.Texture, RenderPosition, CurrentFrame.SourceRectangle,Color, Rotation, Origin, Scale, Effect, LayerDepth);
            base.Draw(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            if (_isTextureConstructed)
                Texture.Dispose();
#if DEBUG
            FlxG.Log.Info("Texture Disposed for" + this);
#endif
            base.Dispose(disposing);
        }

        #region Graphics
        public virtual FlxSprite LoadGraphic(string graphicAssetPath, bool isAnimated=false, int frameWidth = 0, int frameHeight = 0)
        {
            _texturePath = graphicAssetPath;
            IsAnimated = isAnimated;
            LoadContent();

            if(frameWidth == 0)
            {
                frameWidth = IsAnimated ? Texture.Height : Texture.Width;
                frameWidth = (frameWidth > Texture.Width) ? Texture.Width : frameWidth;
            }
            if (frameHeight == 0)
            {
                frameHeight = IsAnimated ? frameWidth : Texture.Height;
                frameHeight = (frameHeight > Texture.Height) ? Texture.Height : frameHeight;
            }
            Animation = new AnimationController(Texture, frameWidth, frameHeight);
            Width = frameWidth;
            Height = frameHeight;
            return this;
        }

        public virtual FlxSprite LoadGraphic(Texture2D texture, bool isAnimated = false, int frameWidth = 0, int frameHeight = 0)
        {
            _texturePath = texture.Name;
            IsAnimated = isAnimated;
            Texture = texture;
          
            if (frameWidth == 0)
            {
                frameWidth = IsAnimated ? Texture.Height : Texture.Width;
                frameWidth = (frameWidth > Texture.Width) ? Texture.Width : frameWidth;
            }
            if (frameHeight == 0)
            {
                frameHeight = IsAnimated ? frameWidth : Texture.Height;
                frameHeight = (frameHeight > Texture.Height) ? Texture.Height : frameHeight;
            }
            Animation = new AnimationController(Texture, frameWidth, frameHeight);
            Width = frameWidth;
            Height = frameHeight;
            return this;
        }

        public virtual FlxSprite MakeGraphic(int width, int height, Color color)
        {
            //TODO : Optimize : Use FlxG.PixelTexture => Create custom Animation Frame with Source Rect & Destination Rectangle
            _isTextureConstructed = true;

            Texture2D _bitmapData = new Texture2D(FlxGame.Instance.GraphicsDevice, width, height);
            Rectangle rect = new Rectangle(0, 0, _bitmapData.Width, _bitmapData.Height);
            _bitmapData.FillRect(rect, color);
#if DEBUG
            FlxG.Log.Info("Texture Created for" + this);
#endif
            return LoadGraphic(_bitmapData, false, width, height);
        }

        #endregion

        #region Utils
        public void CenterOffset(bool adjustPosition = false)
        {
            Offset.X = (FrameWidth - Width) * 0.5f;
            Offset.X = (FrameHeight - Height) * 0.5f;
            if (adjustPosition)
            {
                X += Offset.X;
                Y += Offset.Y;
            }
        }
        public void CenterOrigin()
        {
            Origin = new Vector2(FrameWidth * 0.5f, FrameHeight * 0.5f);
        }

        public Vector2 GetGraphicMidPoint()
        {
            return new Vector2(X + FrameWidth * 0.5f, Y + FrameHeight * 0.5f);
        }

        public void UpdateHitbox()
        {
            Width = Math.Abs(Scale.X) * FrameWidth;
            Height = Math.Abs(Scale.Y) * FrameHeight;
            Offset = new Vector2(-0.5f * (Width - FrameWidth), -0.5f * (Height - FrameHeight));
            CenterOrigin();
        }

        public override bool IsOnScreen(FlxCamera camera = null)
        {
            if (camera == null)
                camera = FlxG.Camera;

            float minX = X - Offset.X - camera.Scroll.X * ScrollFactor.X;
            float minY = Y - Offset.Y - camera.Scroll.Y * ScrollFactor.Y;

            if (Angle == 0 && Scale == Vector2.One)
            {
                return camera.ContainsPoint(new Vector2(minX, minY), FrameWidth, FrameHeight);
            }

            float radiusX = HalfSize.X;
            float radiusY = HalfSize.Y;

            float ox = Origin.X;
            if (ox != radiusX)
            {
                float x1 = Math.Abs(ox);
                float x2 = Math.Abs(FrameWidth - ox);
                radiusX = Math.Max(x2, x1);
            }

            float oy = Origin.Y;
            if (oy != radiusY)
            {
                float y1 = Math.Abs(oy);
                float y2 = Math.Abs(FrameHeight - oy);
                radiusY = Math.Max(y2, y1);
            }

            radiusX *= Math.Abs(Scale.X);
            radiusY *= Math.Abs(Scale.Y);
            float radius = Math.Max(radiusX, radiusY);
            radius *= FlxMath.SQUARE_ROOT_OF_TWO;

            minX += ox - radius;
            minY += oy - radius;

            float doubleRadius = 2 * radius;

            return camera.ContainsPoint(new Vector2(minX, minY), doubleRadius, doubleRadius);
        }
        #endregion
    }
}
