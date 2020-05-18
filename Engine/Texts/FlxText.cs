using Engine.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engine.Texts
{
    public class FlxText: FlxObject
    {
        private readonly string _fontPath;
        private string _text;
        private float _fieldWidth;
        public string Text 
        {
            get { return _text; }
            set
            {
                if (_text.Equals(value))
                    return;
                _text = value;
                UpdateSize();
            }
        }

        public SpriteFont Font;
        public float Alpha { get; set; } = 1.0f;
        public Rectangle DisplayBounds;
        private FlxTextAlign _alignment;
        public FlxTextAlign Alignment 
        { 
            get => _alignment; 
            set 
            {
                if (_alignment == value)
                    return;
                _alignment = value; 
                UpdatePosition(); 
            } 
        }
        public float FieldWidth 
        { 
            get => _fieldWidth; 
            set 
            {
                if (_fieldWidth == value)
                    return;
                _fieldWidth = value; 
                UpdateSize(); 
            }
        }
        public FlxTextBorderStyle BorderStyle { get; set; } = FlxTextBorderStyle.NONE;
        public Color BorderColor { get; set; } = Color.Transparent;
        public float BorderSize { get; set; } = 1;
        public float BorderQuality { get; set; } = 1;

        public FlxText(string fontPath, float x = 0, float y = 0, float fieldWidth = 0, string text = "") : base(x, y, false)
        {
            _fontPath = fontPath;
            _fieldWidth = fieldWidth;
            _text = text;
            if (_fontPath == null)
            {
                throw new ArgumentNullException("fontPath is null");
            }
            Alignment = FlxTextAlign.LEFT;
            Alpha = 1;
            DisplayBounds = new Rectangle((int)X, (int)Y, (int)FieldWidth, (int)Size.Y);
            Initialize();
        }

        protected override void LoadContent()
        {
            if (_fontPath == null)
                return;
            Font = Game.Content.Load<SpriteFont>(_fontPath);
            UpdateSize();
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            if (!Visible)
                return;
            ApplyBorderStyle();
            SpriteBatch.DrawString(Font, Text, RenderPosition.Floor(), Color * Alpha, Rotation, Origin.Floor(), Scale, Effect, LayerDepth);
        }

        private void ApplyBorderStyle()
        {
            if (BorderStyle == FlxTextBorderStyle.NONE)
                return;
            if (BorderColor == Color.Transparent)
                return;
            int iterations = (int)(BorderSize * BorderQuality);
            if (iterations <= 0)
                iterations = 1;
            float delta = BorderSize / iterations;
            switch (BorderStyle)
            {
                case FlxTextBorderStyle.SHADOW:
                    {
                        //Render a shadow beneath the text
                        //(do one lower-right offset draw call)
                        Vector2 oldPosition = Position;
                        Color oldColor = Color;
                        Color = BorderColor;
                        for (int i =0; i < iterations; i++)
                        {
                            DrawWithOffset(delta, delta);
                        }
                        Position = oldPosition;
                        Color = oldColor;
                        break;
                    }
                case FlxTextBorderStyle.OUTLINE:
                    {
                        // Render an outline around the text
                        // (do 8 offset draw calls)
                        Vector2 oldPosition = Position;
                        Color oldColor = Color;
                        Color = BorderColor;
                        float curDelta = delta;
                        for (int i = 0; i < iterations; i++)
                        {
                            DrawWithOffset(-curDelta, -curDelta);// upper-left
                            DrawWithOffset(curDelta, 0); // upper-middle
                            DrawWithOffset(curDelta, 0);// upper-right
                            DrawWithOffset(0, curDelta); // middle-right
                            DrawWithOffset(0, curDelta);// lower-right
                            DrawWithOffset(-curDelta, 0);// lower-middle
                            DrawWithOffset(-curDelta, 0);// lower-left
                            DrawWithOffset(0, -curDelta);// lower-left

                            Position = oldPosition;  // return to center

                            curDelta += delta;
                        }
                        Color = oldColor;
                        break;
                    }
                case FlxTextBorderStyle.OUTLINE_FAST:
                    {
                        // Render an outline around the text
                        // (do 4 diagonal offset draw calls)
                        // (this method might not work with certain narrow fonts)
                        Vector2 oldPosition = Position;
                        Color oldColor = Color;
                        Color = BorderColor;
                        float curDelta = delta;
                        for (int i = 0; i < iterations; i++)
                        {
                            DrawWithOffset(-curDelta, -curDelta);// upper-left
                            DrawWithOffset(curDelta * 2, 0);// lower-middle
                            DrawWithOffset(0, curDelta * 2);// lower-left
                            DrawWithOffset(-curDelta * 2, 0);// lower-left

                            Position = oldPosition; //// return to center
                            curDelta += delta;
                        }
                        Color = oldColor;
                        break;
                    }
            }
        }

        private void DrawWithOffset(float x, float y)
        {
            X += x;
            Y += y;
            SpriteBatch.DrawString(Font, Text, RenderPosition, Color * Alpha, Rotation, Origin.Floor(), Scale, Effect, LayerDepth);
        }

        private void UpdateSize()
        {
            Size = Font.MeasureString(Text);
            DisplayBounds.Height = (int)Size.Y;
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            if (Size == Vector2.Zero)
                return;
            if (Alignment == FlxTextAlign.LEFT)
            {
                X = DisplayBounds.X;
            }
            else if (Alignment == FlxTextAlign.RIGHT)
            {
                X = DisplayBounds.Right - Size.X;
            }
            else if (Alignment == FlxTextAlign.CENTER)
            {
                X = DisplayBounds.X + (DisplayBounds.Width / 2) - Size.X / 2;
            }
        }
    }

    public enum FlxTextBorderStyle 
    {
        NONE,
        SHADOW,
        OUTLINE,
        OUTLINE_FAST
    }
    public enum FlxTextAlign
    {
        LEFT, RIGHT, CENTER
    }
}
