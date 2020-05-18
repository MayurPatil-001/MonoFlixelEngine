using Microsoft.Xna.Framework;

namespace Engine.Inputs
{
    public class FlxPointer
    {
        private Point _position;
        public Vector2 Position { get => _position.ToVector2(); }
        public int X { get => _position.X; set => _position.X = value; }
        public int Y { get => _position.Y; set => _position.Y = value; }
        public int ScreenX { get; private set; }
        public int ScreenY { get; private set; }
        private Point _globalScreenPosition;
        private Vector2 _cachedPoint = Vector2.Zero;

        public Vector2 GetWorldPosition(FlxCamera camera = null)
        {
            if (camera == null)
                camera = FlxG.Camera;
            _cachedPoint = GetScreenPosition(camera);
            return new Vector2(_cachedPoint.X + camera.Scroll.X, _cachedPoint.Y + camera.Scroll.Y);
        }

        public Vector2 GetScreenPosition(FlxCamera camera = null)
        {
            if (camera == null)
                camera = FlxG.Camera;
            Vector2 point;
            point.X = (_globalScreenPosition.X - camera.X + 0.5f * camera.Width * (camera.Zoom - camera.InitialZoom)) / camera.Zoom;
            point.Y = (_globalScreenPosition.Y - camera.Y + 0.5f * camera.Height * (camera.Zoom - camera.InitialZoom)) / camera.Zoom;
            return point;
        }


        public Vector2 GetPositionInCameraView(FlxCamera camera = null)
        {
            if (camera == null)
                camera = FlxG.Camera;
            Vector2 point;
            point.X = (_globalScreenPosition.X - camera.X) / camera.Zoom + camera.ViewOffset.X;
            point.Y = (_globalScreenPosition.Y - camera.Y) / camera.Zoom + camera.ViewOffset.Y;
            return point;
        }

        public void SetGlobalScreenPositionUnsafe(float newX, float newY)
        {
            _globalScreenPosition.X = (int)newX;
            _globalScreenPosition.Y = (int)newY;
            UpdatePositions();
        }
        private void UpdatePositions()
        {
            _cachedPoint = GetScreenPosition(FlxG.Camera);
            ScreenX = (int)_cachedPoint.X;
            ScreenY = (int)_cachedPoint.Y;

            _cachedPoint = GetWorldPosition(FlxG.Camera);
            _position = _cachedPoint.ToPoint();
        }
    }
}
