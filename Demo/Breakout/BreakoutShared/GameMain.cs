
using Engine;

namespace BreakoutShared
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameMain : FlxGame
    {
        public GameMain() : base(320, 240, 640, 480, "Game", false)
        {
        }

        protected override void Create()
        {
            Add(new PlayState());
        }
    }
}
