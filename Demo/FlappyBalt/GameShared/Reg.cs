using Engine;
using Engine.Extensions;
using Engine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Demo
{
    class Reg
    {
        static Texture2D _bitmapData;
        static Color GREY_LIGHT = new Color(176, 176, 191);
	    static Color GREY_MED = new Color(100, 106, 125);
	    static Color GREY_DARK= new Color(53, 53, 61);

        public static FlxSave Save;
        public static SaveFile SaveFile;
        public static int Score = 0;
        public static int HighScore = 0;
        public static PlayState PS;


        public static Texture2D GetBounceImage(int height)
        {
            if (_bitmapData != null)
                return _bitmapData;
            _bitmapData = new Texture2D(FlxG.Game.GraphicsDevice, 8, height);
            Rectangle rect = new Rectangle(0, 0, _bitmapData.Width, _bitmapData.Height);
            _bitmapData.FillRect(rect, GREY_MED);
            rect = new Rectangle(4, 0, 4, height);
            _bitmapData.FillRect(rect, GREY_LIGHT);
            rect = new Rectangle(0, 1, 1, height - 2);
            _bitmapData.FillRect(rect, GREY_DARK);
            rect.X = 3;
            _bitmapData.FillRect(rect, GREY_DARK);
            rect = new Rectangle(1, 0, 2, 1);
            _bitmapData.FillRect(rect, GREY_DARK);
            rect.Y = height - 1;
            _bitmapData.FillRect(rect, GREY_DARK);
            rect = new Rectangle(4, 1, 1, height - 2);
            _bitmapData.FillRect(rect, Color.White);
            rect.X = 7;
            _bitmapData.FillRect(rect, Color.White);
            rect = new Rectangle(5, 0, 2, 1);
            _bitmapData.FillRect(rect, Color.White);
            rect.Y = height - 1;
            _bitmapData.FillRect(rect, Color.White);


            return _bitmapData;
        }

        public static void Dispose()
        {
            _bitmapData.Dispose();
        }
    }
}
