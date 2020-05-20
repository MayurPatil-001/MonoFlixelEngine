#region Using Statements
using Engine;

#endregion

namespace SubState.Shared
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : FlxGame
    {
        public Game1() : base(640, 480)
        {

        }

        protected override void Create()
        {
            Add(new MenuState());
            base.Create();
        }
    }
}
