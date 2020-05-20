using Engine.Group;
using Microsoft.Xna.Framework;
using System;

namespace Engine.UI
{
    /// <summary>
    /// A gamepad which contains 4 directional buttons and 4 action buttons.
    /// It's easy to set the callbacks and to customize the layout.
    /// </summary>
    public class FlxVirtualPad : FlxSpriteGroup
    {
        public FlxButton ButtonA;
        public FlxButton ButtonB;
        public FlxButton ButtonC;
        public FlxButton ButtonX;
        public FlxButton ButtonY;
        public FlxButton ButtonLeft;
        public FlxButton ButtonUp;
        public FlxButton ButtonRight;
        public FlxButton ButtonDown;

        /// <summary>
        /// Group of directions buttons.
        /// </summary>
        public FlxSpriteGroup DPad;

        /// <summary>
        /// Group of action buttons.
        /// </summary>
        public FlxSpriteGroup Actions;

        /// <summary>
        /// Create a gamepad which contains 4 directional buttons and 4 action buttons.
        /// </summary>
        /// <param name="dPad">The D-Pad mode. `FULL` for example.</param>
        /// <param name="action">The action buttons mode. `A_B_C` for example.</param>
        public FlxVirtualPad(FlxDPadMode? dPad = null, FlxActionMode? action= null)
        {
            ScrollFactor = Vector2.Zero;

            if (dPad == null)
                dPad = FlxDPadMode.FULL;
            if (!action.HasValue)
                action = FlxActionMode.A_B_C;

            DPad = new FlxSpriteGroup();
            DPad.ScrollFactor = Vector2.Zero;

            Actions = new FlxSpriteGroup();
            Actions.ScrollFactor = Vector2.Zero;

            switch (dPad.Value)
            {
                case FlxDPadMode.UP_DOWN:
                    DPad.Add(Add(ButtonUp = CreateButton(0, FlxG.Height - 85, 44, 45, "up")));
                    DPad.Add(Add(ButtonDown = CreateButton(0, FlxG.Height - 45, 44, 45, "down")));
                    break;
                case FlxDPadMode.LEFT_RIGHT:
                    DPad.Add(Add(ButtonLeft = CreateButton(0, FlxG.Height - 45, 44, 45, "left")));
                    DPad.Add(Add(ButtonRight = CreateButton(42, FlxG.Height - 45, 44, 45, "right")));
                    break;
                case FlxDPadMode.UP_LEFT_RIGHT:
                    DPad.Add(Add(ButtonUp = CreateButton(35, FlxG.Height - 81, 44, 45, "up")));
                    DPad.Add(Add(ButtonLeft = CreateButton(0, FlxG.Height - 45, 44, 45, "left")));
                    DPad.Add(Add(ButtonRight = CreateButton(69, FlxG.Height - 45, 44, 45, "right")));
                    break;
                case FlxDPadMode.FULL:
                    DPad.Add(Add(ButtonUp = CreateButton(35, FlxG.Height - 116, 44, 45, "up")));
                    DPad.Add(Add(ButtonLeft = CreateButton(0, FlxG.Height - 81, 44, 45, "left")));
                    DPad.Add(Add(ButtonRight = CreateButton(69, FlxG.Height - 81, 44, 45, "right")));
                    DPad.Add(Add(ButtonDown = CreateButton(35, FlxG.Height - 45, 44, 45, "down")));
                    break;
                case FlxDPadMode.NONE:
                    break;
            }

            switch (action)
            {
                case FlxActionMode.A:
                    Actions.Add(Add(ButtonA = CreateButton(FlxG.Width - 44, FlxG.Height - 45, 44, 45, "a")));
                    break;
                case FlxActionMode.A_B:
                    Actions.Add(Add(ButtonA = CreateButton(FlxG.Width - 44, FlxG.Height - 45, 44, 45, "a")));
                    Actions.Add(Add(ButtonB = CreateButton(FlxG.Width - 86, FlxG.Height - 45, 44, 45, "b")));
                    break;
                case FlxActionMode.A_B_C:
                    Actions.Add(Add(ButtonA = CreateButton(FlxG.Width - 128, FlxG.Height - 45, 44, 45, "a")));
                    Actions.Add(Add(ButtonB = CreateButton(FlxG.Width - 86, FlxG.Height - 45, 44, 45, "b")));
                    Actions.Add(Add(ButtonC = CreateButton(FlxG.Width - 44, FlxG.Height - 45, 44, 45, "c")));
                    break;
                case FlxActionMode.A_B_X_Y:
                    Actions.Add(Add(ButtonY = CreateButton(FlxG.Width - 86, FlxG.Height - 85, 44, 45, "y")));
                    Actions.Add(Add(ButtonX = CreateButton(FlxG.Width - 44, FlxG.Height - 85, 44, 45, "x")));
                    Actions.Add(Add(ButtonB = CreateButton(FlxG.Width - 86, FlxG.Height - 45, 44, 45, "b")));
                    Actions.Add(Add(ButtonA = CreateButton(FlxG.Width - 44, FlxG.Height - 45, 44, 45, "a")));
                    break;
                case FlxActionMode.NONE:
                    break;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            DPad.Dispose();
            Actions.Dispose();
            DPad = null;
            Actions = null;
            ButtonA = null;
            ButtonB = null;
            ButtonC = null;
            ButtonX = null;
            ButtonY = null;
            ButtonLeft = null;
            ButtonRight = null;
            ButtonUp = null;
            ButtonDown = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">The x-position of the button.</param>
        /// <param name="y">The y-position of the button.</param>
        /// <param name="width">The width of the button.</param>
        /// <param name="height">The height of the button.</param>
        /// <param name="graphic">The image of the button. It must contains 3 frames (`NORMAL`, `HIGHLIGHT`, `PRESSED`).</param>
        /// <param name="onClick">The callback for the button.</param>
        /// <returns></returns>
        public FlxButton CreateButton(float x, float y, int width, int height, string graphic, Action onClick = null)
        {
            FlxButton button = new FlxButton(null, x, y);
            //TODO :button.Frames = null;
            button.ResetSizeFromFrame();
            button.Solid = false;
            button.Immovable = true;
            button.ScrollFactor = Vector2.Zero;

            if (onClick != null)
                button.OnDown.Callback = onClick;
            return button;
        }
    }

    public enum FlxDPadMode
    {
        NONE,
        UP_DOWN,
        LEFT_RIGHT,
        UP_LEFT_RIGHT,
        FULL
    }

    public enum FlxActionMode
    {
        NONE,
        A,
        A_B,
        A_B_C,
        A_B_X_Y
    }
}
