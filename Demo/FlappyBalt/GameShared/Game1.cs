using Engine;

namespace Demo
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : FlxGame
    {
        public Game1():base(160, 240, 480, 720, "Flappybalt", false)
        {
        }

        protected override void Create()
        {
            Add(new PlayState());
        }
    }
}
