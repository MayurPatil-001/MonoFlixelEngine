using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine.Inputs
{
    public class KeyboardManager: FlxBasic
    {
        public KeyboardState PreviousState;
        public KeyboardState CurrentState;

        public override void Update(GameTime gameTime)
        {
            if (!Enabled)
                return;

            PreviousState = CurrentState;
            CurrentState = Keyboard.GetState();
            base.Update(gameTime);
        }

        public void UpdateNull()
        {
           if (!Enabled)
                return;

            PreviousState = CurrentState;
            CurrentState = new KeyboardState();
        }

        public bool Pressed(Keys key)
        {
            return CurrentState.IsKeyDown(key);
        }

        public bool Released(Keys key)
        {
            return CurrentState.IsKeyUp(key);
        }

        public bool JustPressed(Keys key)
        {
            return Pressed(key) && PreviousState.IsKeyUp(key);
        }

        public bool JustReleased(Keys key)
        {
            return Released(key) && PreviousState.IsKeyDown(key);
        }

        public bool AnyPressed(Keys[] keys)
        {
            foreach (Keys key in keys)
                if (Pressed(key))
                    return true;
            return false;
        }

        public bool AnyReleased(Keys[] keys)
        {
            foreach (Keys key in keys)
                if (Released(key))
                    return true;
            return false;
        }

        public bool AnyJustPressed(Keys[] keys)
        {
            foreach (Keys key in keys)
                if (JustPressed(key))
                    return true;
            return false;
        }

        public bool AnyJustReleased(Keys[] keys)
        {
            foreach (Keys key in keys)
                if (JustReleased(key))
                    return true;
            return false;
        }

    }
}
