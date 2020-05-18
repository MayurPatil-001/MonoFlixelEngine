using Engine;
using Microsoft.Xna.Framework;

namespace Sandbox
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : FlxGame
    {
        
        public Game1():base(640, 480)
        {
            
        }

        protected override void Initialize()
        {
            Add(new PlayState());
            base.Initialize();
        }

    }
}
