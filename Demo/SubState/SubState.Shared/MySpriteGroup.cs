using Engine;
using Engine.Extensions;
using Engine.Group;
using Microsoft.Xna.Framework;

namespace SubState.Shared
{
    class MySpriteGroup:FlxTypedGroup<FlxSprite>
    {

        public MySpriteGroup(int numSprites = 50)
        {
            for(int i =0; i < numSprites; i++)
            {
                FlxSprite sprite = new FlxSprite(FlxG.Random.NextFloat(0, FlxG.Width), FlxG.Random.NextFloat(0, FlxG.Height));
                sprite.Velocity = new Vector2(FlxG.Random.NextFloat(-50, 50), FlxG.Random.NextFloat(-50, 50));
                Add(sprite);
            }
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
			foreach (FlxSprite sprite in Members)
			{
				if (sprite.X < 0)
				{
					sprite.X = 0;
					sprite.Velocity.X *= -1;
				}
				else if (sprite.X + sprite.Width > FlxG.Width)
				{
					sprite.X = FlxG.Width - sprite.Width;
					sprite.Velocity.X *= -1;
				}
				if (sprite.Y < 0)
				{
					sprite.Y = 0;
					sprite.Velocity.Y *= -1;
				}
				else if (sprite.Y + sprite.Height > FlxG.Height)
				{
					sprite.Y = FlxG.Height - sprite.Height;
					sprite.Velocity.Y *= -1;
				}
			}
		}
    }
}
