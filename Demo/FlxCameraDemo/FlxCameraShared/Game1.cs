

using Engine;

namespace FlxCameraShared
{
    public class Game1 : FlxGame
    {
        public Game1():base(640, 480)
        {

        }

        protected override void LoadContent()
        {
            base.LoadContent();
            Add(new PlayState());
        }
    }
}
