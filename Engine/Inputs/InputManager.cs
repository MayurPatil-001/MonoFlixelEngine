using Microsoft.Xna.Framework;

namespace Engine.Inputs
{
    public class InputManager: CoreGameComponent
    {
        public KeyboardManager Keyboard;
        public MouseManager Mouse;
        public GamePadManager GamePads;
        public InputManager()
        {
            Keyboard = new KeyboardManager();
            Mouse = new MouseManager();
            GamePads = new GamePadManager();
        }
        public override void Update(GameTime gameTime)
        {
            Keyboard.Update(gameTime);
            Mouse.Update(gameTime);
            GamePads.Update(gameTime);
            base.Update(gameTime);
        }
    }
}
