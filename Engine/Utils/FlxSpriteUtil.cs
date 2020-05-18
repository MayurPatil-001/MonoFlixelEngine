namespace Engine.Utils
{
	public class FlxSpriteUtil
	{
		public static FlxSprite ScreenWrap(FlxSprite sprite, bool left = true, bool right = true, bool top = true, bool bottom = true)
		{
			if (left && ((sprite.X + sprite.FrameWidth / 2) <= 0))
			{
				sprite.X = FlxG.Width;
			}
			else if (right && (sprite.X >= FlxG.Width))
			{
				sprite.X = 0;
			}

			if (top && ((sprite.Y + sprite.FrameHeight / 2) <= 0))
			{
				sprite.Y = FlxG.Height;
			}
			else if (bottom && (sprite.Y >= FlxG.Height))
			{
				sprite.Y = 0;
			}
			return sprite;
		}

	}
}
