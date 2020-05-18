using Engine;
using Microsoft.Xna.Framework;

namespace FlxSnake
{
    public class Game1 : FlxGame
    {
        public Game1():base(320, 240, 640, 480, "Game", false)
        {

        }

        protected override void Create()
        {
            Add(new PlayState());
        }
    }
}
