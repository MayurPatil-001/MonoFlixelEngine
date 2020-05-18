using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engine.Animations
{
    public class AnimationFrame
    {
        private Color[] _bitmapData;
        public TimeSpan Duration { get; set; }
        public Texture2D Texture { get; set; }
        public Rectangle SourceRectangle { get; set; }
        /// <summary>
        /// Color[] used for pixel perfect collisions
        /// </summary>
        public Color[] BitmapData 
        {
            get
            { 
                if(_bitmapData == null)
                {
                    _bitmapData = new Color[SourceRectangle.Width * SourceRectangle.Height];
                    Texture.GetData(0, SourceRectangle, _bitmapData, 0, _bitmapData.Length);
                }
                return _bitmapData;
            }
        }

        public AnimationFrame(Texture2D texture, Rectangle sourceRectangle, TimeSpan duration)
        {
            Texture = texture;
            SourceRectangle = sourceRectangle;
            Duration = duration;
        }
    }
}
