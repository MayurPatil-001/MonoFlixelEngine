using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Engine.Inputs
{
    public class GamePadManager: CoreGameComponent
    {
        const int totalGamePads = 4;
        public static GamePadData[] GamePads { get; private set; }

        public GamePadManager()
        {
            GamePads = new GamePadData[4];
            for (int i = 0; i < totalGamePads; i++)
                GamePads[i] = new GamePadData((PlayerIndex)i);
        }
        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < totalGamePads; i++)
                GamePads[i].Update(gameTime);
            base.Update(gameTime);
        }

        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            base.OnEnabledChanged(sender, args);
            for (int i = 0; i < totalGamePads; i++)
                GamePads[i].Enabled = Enabled;
        }
        public static void UpdateNull(GameTime gameTime)
        {
            for (int i = 0; i < 4; i++)
                GamePads[i].UpdateNull(gameTime);
        }
    }
    public class GamePadData: CoreGameComponent
    {
        public PlayerIndex PlayerIndex { get; private set; }
        public GamePadState PreviousState;
        public GamePadState CurrentState;
        public bool Attached;
        public float DeadZone = 0.15f;

        private float rumbleStrength;
        private float rumbleTime;

        internal GamePadData(PlayerIndex playerIndex)
        {
            PlayerIndex = playerIndex;
        }

        public override void Update(GameTime gameTime)
        {
            PreviousState = CurrentState;
            CurrentState = GamePad.GetState(PlayerIndex);
            Attached = CurrentState.IsConnected;

            if (rumbleTime > 0)
            {
                rumbleTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (rumbleTime <= 0)
                    GamePad.SetVibration(PlayerIndex, 0, 0);
            }
        }

        public void UpdateNull(GameTime gameTime)
        {
            PreviousState = CurrentState;
            CurrentState = new GamePadState();
            Attached = GamePad.GetState(PlayerIndex).IsConnected;

            if (rumbleTime > 0)
                rumbleTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            GamePad.SetVibration(PlayerIndex, 0, 0);
        }

        public void Rumble(float strength, float time)
        {
            if (rumbleTime <= 0 || strength > rumbleStrength || (strength == rumbleStrength && time > rumbleTime))
            {
                GamePad.SetVibration(PlayerIndex, strength, strength);
                rumbleStrength = strength;
                rumbleTime = time;
            }
        }

        public void StopRumble()
        {
            GamePad.SetVibration(PlayerIndex, 0, 0);
            rumbleTime = 0;
        }

        #region Buttons

        public bool Check(Buttons button)
        {
            if (!Enabled)
                return false;

            return CurrentState.IsButtonDown(button);
        }

        public bool Pressed(Buttons button)
        {
            if (!Enabled)
                return false;

            return CurrentState.IsButtonDown(button) && PreviousState.IsButtonUp(button);
        }

        public bool Released(Buttons button)
        {
            if (!Enabled)
                return false;

            return CurrentState.IsButtonUp(button) && PreviousState.IsButtonDown(button);
        }

        public bool Check(Buttons buttonA, Buttons buttonB)
        {
            return Check(buttonA) || Check(buttonB);
        }

        public bool Pressed(Buttons buttonA, Buttons buttonB)
        {
            return Pressed(buttonA) || Pressed(buttonB);
        }

        public bool Released(Buttons buttonA, Buttons buttonB)
        {
            return Released(buttonA) || Released(buttonB);
        }

        public bool Check(Buttons buttonA, Buttons buttonB, Buttons buttonC)
        {
            return Check(buttonA) || Check(buttonB) || Check(buttonC);
        }

        public bool Pressed(Buttons buttonA, Buttons buttonB, Buttons buttonC)
        {
            return Pressed(buttonA) || Pressed(buttonB) || Check(buttonC);
        }

        public bool Released(Buttons buttonA, Buttons buttonB, Buttons buttonC)
        {
            return Released(buttonA) || Released(buttonB) || Check(buttonC);
        }

        #endregion

        #region Sticks

        public Vector2 GetLeftStick()
        {
            Vector2 ret = CurrentState.ThumbSticks.Left;
            ret.Y = -ret.Y;
            return ret;
        }

        public Vector2 GetLeftStick(float deadzone)
        {
            Vector2 ret = CurrentState.ThumbSticks.Left;
            if (ret.LengthSquared() < deadzone * deadzone)
                ret = Vector2.Zero;
            else
                ret.Y = -ret.Y;
            return ret;
        }

        public Vector2 GetRightStick()
        {
            Vector2 ret = CurrentState.ThumbSticks.Right;
            ret.Y = -ret.Y;
            return ret;
        }

        public Vector2 GetRightStick(float deadzone = 0.15f)
        {
            Vector2 ret = CurrentState.ThumbSticks.Right;
            if (ret.LengthSquared() < deadzone * deadzone)
                ret = Vector2.Zero;
            else
                ret.Y = -ret.Y;
            return ret;
        }

        #region Left Stick Directions

        public bool LeftStickLeftCheck(float deadzone = 0.15f)
        {
            return CurrentState.ThumbSticks.Left.X <= -deadzone;
        }

        public bool LeftStickLeftPressed(float deadzone)
        {
            return CurrentState.ThumbSticks.Left.X <= -deadzone && PreviousState.ThumbSticks.Left.X > -deadzone;
        }

        public bool LeftStickLeftReleased(float deadzone)
        {
            return CurrentState.ThumbSticks.Left.X > -deadzone && PreviousState.ThumbSticks.Left.X <= -deadzone;
        }

        public bool LeftStickRightCheck(float deadzone)
        {
            return CurrentState.ThumbSticks.Left.X >= deadzone;
        }

        public bool LeftStickRightPressed(float deadzone)
        {
            return CurrentState.ThumbSticks.Left.X >= deadzone && PreviousState.ThumbSticks.Left.X < deadzone;
        }

        public bool LeftStickRightReleased(float deadzone)
        {
            return CurrentState.ThumbSticks.Left.X < deadzone && PreviousState.ThumbSticks.Left.X >= deadzone;
        }

        public bool LeftStickDownCheck(float deadzone)
        {
            return CurrentState.ThumbSticks.Left.Y <= -deadzone;
        }

        public bool LeftStickDownPressed(float deadzone)
        {
            return CurrentState.ThumbSticks.Left.Y <= -deadzone && PreviousState.ThumbSticks.Left.Y > -deadzone;
        }

        public bool LeftStickDownReleased(float deadzone)
        {
            return CurrentState.ThumbSticks.Left.Y > -deadzone && PreviousState.ThumbSticks.Left.Y <= -deadzone;
        }

        public bool LeftStickUpCheck(float deadzone)
        {
            return CurrentState.ThumbSticks.Left.Y >= deadzone;
        }

        public bool LeftStickUpPressed(float deadzone)
        {
            return CurrentState.ThumbSticks.Left.Y >= deadzone && PreviousState.ThumbSticks.Left.Y < deadzone;
        }

        public bool LeftStickUpReleased(float deadzone)
        {
            return CurrentState.ThumbSticks.Left.Y < deadzone && PreviousState.ThumbSticks.Left.Y >= deadzone;
        }

        public float LeftStickHorizontal(float deadzone)
        {
            float h = CurrentState.ThumbSticks.Left.X;
            if (Math.Abs(h) < deadzone)
                return 0;
            else
                return h;
        }

        public float LeftStickVertical(float deadzone)
        {
            float v = CurrentState.ThumbSticks.Left.Y;
            if (Math.Abs(v) < deadzone)
                return 0;
            else
                return -v;
        }

        #endregion

        #region Right Stick Directions

        public bool RightStickLeftCheck(float deadzone)
        {
            return CurrentState.ThumbSticks.Right.X <= -deadzone;
        }

        public bool RightStickLeftPressed(float deadzone)
        {
            return CurrentState.ThumbSticks.Right.X <= -deadzone && PreviousState.ThumbSticks.Right.X > -deadzone;
        }

        public bool RightStickLeftReleased(float deadzone)
        {
            return CurrentState.ThumbSticks.Right.X > -deadzone && PreviousState.ThumbSticks.Right.X <= -deadzone;
        }

        public bool RightStickRightCheck(float deadzone)
        {
            return CurrentState.ThumbSticks.Right.X >= deadzone;
        }

        public bool RightStickRightPressed(float deadzone)
        {
            return CurrentState.ThumbSticks.Right.X >= deadzone && PreviousState.ThumbSticks.Right.X < deadzone;
        }

        public bool RightStickRightReleased(float deadzone)
        {
            return CurrentState.ThumbSticks.Right.X < deadzone && PreviousState.ThumbSticks.Right.X >= deadzone;
        }

        public bool RightStickUpCheck(float deadzone)
        {
            return CurrentState.ThumbSticks.Right.Y <= -deadzone;
        }

        public bool RightStickUpPressed(float deadzone)
        {
            return CurrentState.ThumbSticks.Right.Y <= -deadzone && PreviousState.ThumbSticks.Right.Y > -deadzone;
        }

        public bool RightStickUpReleased(float deadzone)
        {
            return CurrentState.ThumbSticks.Right.Y > -deadzone && PreviousState.ThumbSticks.Right.Y <= -deadzone;
        }

        public bool RightStickDownCheck(float deadzone)
        {
            return CurrentState.ThumbSticks.Right.Y >= deadzone;
        }

        public bool RightStickDownPressed(float deadzone)
        {
            return CurrentState.ThumbSticks.Right.Y >= deadzone && PreviousState.ThumbSticks.Right.Y < deadzone;
        }

        public bool RightStickDownReleased(float deadzone)
        {
            return CurrentState.ThumbSticks.Right.Y < deadzone && PreviousState.ThumbSticks.Right.Y >= deadzone;
        }

        public float RightStickHorizontal(float deadzone)
        {
            float h = CurrentState.ThumbSticks.Right.X;
            if (Math.Abs(h) < deadzone)
                return 0;
            else
                return h;
        }

        public float RightStickVertical(float deadzone)
        {
            float v = CurrentState.ThumbSticks.Right.Y;
            if (Math.Abs(v) < deadzone)
                return 0;
            else
                return -v;
        }

        #endregion

        #endregion

        #region DPad

        public int DPadHorizontal
        {
            get
            {
                return CurrentState.DPad.Right == ButtonState.Pressed ? 1 : (CurrentState.DPad.Left == ButtonState.Pressed ? -1 : 0);
            }
        }

        public int DPadVertical
        {
            get
            {
                return CurrentState.DPad.Down == ButtonState.Pressed ? 1 : (CurrentState.DPad.Up == ButtonState.Pressed ? -1 : 0);
            }
        }

        public Vector2 DPad
        {
            get
            {
                return new Vector2(DPadHorizontal, DPadVertical);
            }
        }

        public bool DPadLeftCheck
        {
            get
            {
                return CurrentState.DPad.Left == ButtonState.Pressed;
            }
        }

        public bool DPadLeftPressed
        {
            get
            {
                return CurrentState.DPad.Left == ButtonState.Pressed && PreviousState.DPad.Left == ButtonState.Released;
            }
        }

        public bool DPadLeftReleased
        {
            get
            {
                return CurrentState.DPad.Left == ButtonState.Released && PreviousState.DPad.Left == ButtonState.Pressed;
            }
        }

        public bool DPadRightCheck
        {
            get
            {
                return CurrentState.DPad.Right == ButtonState.Pressed;
            }
        }

        public bool DPadRightPressed
        {
            get
            {
                return CurrentState.DPad.Right == ButtonState.Pressed && PreviousState.DPad.Right == ButtonState.Released;
            }
        }

        public bool DPadRightReleased
        {
            get
            {
                return CurrentState.DPad.Right == ButtonState.Released && PreviousState.DPad.Right == ButtonState.Pressed;
            }
        }

        public bool DPadUpCheck
        {
            get
            {
                return CurrentState.DPad.Up == ButtonState.Pressed;
            }
        }

        public bool DPadUpPressed
        {
            get
            {
                return CurrentState.DPad.Up == ButtonState.Pressed && PreviousState.DPad.Up == ButtonState.Released;
            }
        }

        public bool DPadUpReleased
        {
            get
            {
                return CurrentState.DPad.Up == ButtonState.Released && PreviousState.DPad.Up == ButtonState.Pressed;
            }
        }

        public bool DPadDownCheck
        {
            get
            {
                return CurrentState.DPad.Down == ButtonState.Pressed;
            }
        }

        public bool DPadDownPressed
        {
            get
            {
                return CurrentState.DPad.Down == ButtonState.Pressed && PreviousState.DPad.Down == ButtonState.Released;
            }
        }

        public bool DPadDownReleased
        {
            get
            {
                return CurrentState.DPad.Down == ButtonState.Released && PreviousState.DPad.Down == ButtonState.Pressed;
            }
        }

        #endregion

        #region Triggers

        public bool LeftTriggerCheck(float threshold)
        {
            if (!Enabled)
                return false;

            return CurrentState.Triggers.Left >= threshold;
        }

        public bool LeftTriggerPressed(float threshold)
        {
            if (!Enabled)
                return false;

            return CurrentState.Triggers.Left >= threshold && PreviousState.Triggers.Left < threshold;
        }

        public bool LeftTriggerReleased(float threshold)
        {
            if (!Enabled)
                return false;

            return CurrentState.Triggers.Left < threshold && PreviousState.Triggers.Left >= threshold;
        }

        public bool RightTriggerCheck(float threshold)
        {
            if (!Enabled)
                return false;

            return CurrentState.Triggers.Right >= threshold;
        }

        public bool RightTriggerPressed(float threshold)
        {
            if (!Enabled)
                return false;

            return CurrentState.Triggers.Right >= threshold && PreviousState.Triggers.Right < threshold;
        }

        public bool RightTriggerReleased(float threshold)
        {
            if (!Enabled)
                return false;

            return CurrentState.Triggers.Right < threshold && PreviousState.Triggers.Right >= threshold;
        }

        #endregion
    }
}
