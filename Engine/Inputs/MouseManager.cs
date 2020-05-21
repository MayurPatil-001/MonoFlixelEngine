using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Engine.Inputs
{
    public class MouseManager: CoreGameComponent
    {
        public MouseState PreviousState;
        public MouseState CurrentState;
        #region Position
        public float X
        {
            get { return Position.X; }
            set { Position = new Vector2(value, Position.Y); }
        }

        public float Y
        {
            get { return Position.Y; }
            set { Position = new Vector2(Position.X, value); }
        }
        public float ScreenX
        {
            get { return ScreenPosition.X; }
            set { ScreenPosition = new Vector2(value, ScreenPosition.Y); }
        }

        public float ScreenY
        {
            get { return ScreenPosition.Y; }
            set { ScreenPosition = new Vector2(ScreenPosition.X, value); }
        }
        public Vector2 Position
        {
            get
            {
                return GetPositionInCameraView();
                //return Vector2.Transform(new Vector2(CurrentState.X, CurrentState.Y) - FlxGame.Viewport.Bounds.Location.ToVector2(), Matrix.Invert(FlxGame.ScreenMatrix));
            }

            set
            {
                ScreenPosition = FlxG.Camera.ScreenToWorld(value);
                //var vector = Vector2.Transform(value + FlxGame.Viewport.Bounds.Location.ToVector2(), FlxGame.ScreenMatrix);
                //Mouse.SetPosition((int)Math.Round(vector.X), (int)Math.Round(vector.Y));
            }
        }

        public Vector2 ScreenPosition
        {
            get
            {
                return new Vector2(CurrentState.X, CurrentState.Y);
            }
            set
            {
                Mouse.SetPosition((int)Math.Round(value.X), (int)Math.Round(value.Y));
            }
        }

        public Vector2 WorldPosition
        {
            get { return Position; }
            set { Position = value; }
        }

        public bool JustMoved
        {
            get
            {
                return CurrentState.X != PreviousState.X
                    || CurrentState.Y != PreviousState.Y;
            }
        }

        #endregion

        #region Wheel
        public int Wheel
        {
            get { return CurrentState.ScrollWheelValue - PreviousState.ScrollWheelValue; }
        }
        #endregion

        #region Buttons
        public bool Pressed
        {
            get { return CurrentState.LeftButton == ButtonState.Pressed; }
        }

        public bool PressedRight
        {
            get { return CurrentState.RightButton == ButtonState.Pressed; }
        }

        public bool PressedMiddle
        {
            get { return CurrentState.MiddleButton == ButtonState.Pressed; }
        }

        public bool JustPressed
        {
            get { return CurrentState.LeftButton == ButtonState.Pressed && PreviousState.LeftButton == ButtonState.Released; }
        }

        public bool JustPressedRight
        {
            get { return CurrentState.RightButton == ButtonState.Pressed && PreviousState.RightButton == ButtonState.Released; }
        }

        public bool JustPressedMiddle
        {
            get { return CurrentState.MiddleButton == ButtonState.Pressed && PreviousState.MiddleButton == ButtonState.Released; }
        }

        public bool JustReleased
        {
            get { return CurrentState.LeftButton == ButtonState.Released && PreviousState.LeftButton == ButtonState.Pressed; }
        }

        public bool JustReleasedRight
        {
            get { return CurrentState.RightButton == ButtonState.Released && PreviousState.RightButton == ButtonState.Pressed; }
        }

        public bool JustReleasedMiddle
        {
            get { return CurrentState.MiddleButton == ButtonState.Released && PreviousState.MiddleButton == ButtonState.Pressed; }
        }

        #endregion
        public override void Update(GameTime gameTime)
        {
            if (!Enabled)
                return;

            PreviousState = CurrentState;
            CurrentState = Mouse.GetState();
            base.Update(gameTime);
        }

        public void UpdateNull()
        {
            if (!Enabled)
                return;

            PreviousState = CurrentState;
            CurrentState = new MouseState();
        }

        public Vector2 GetPositionInCameraView(FlxCamera camera = null)
        {
            if (camera == null)
                camera = FlxG.Camera;
            return camera.ViewportAdapter.PointToScreen(ScreenPosition.ToPoint()).ToVector2();
        }

    }
}
