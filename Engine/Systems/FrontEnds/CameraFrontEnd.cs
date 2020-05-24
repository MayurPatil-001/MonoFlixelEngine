using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Engine.Systems.FrontEnds
{
    public interface ICameraFrontEnd { }
    public class CameraFrontEnd: CoreGameComponent
    {
        public List<FlxCamera> List { get; private set; }

        public Color BgColor 
        {
            get => FlxG.Camera == null ? Color.Black : FlxG.Camera.BackgroundColor;
           set
            {
                foreach (FlxCamera camera in List)
                    camera.BackgroundColor = value;
            }
        }

        public CameraFrontEnd()
        {
            List = new List<FlxCamera>();
            FlxCamera.DefaultCameras = List;
        }

        public T Add<T>(T newCamera) where T : FlxCamera
        {
            newCamera.Id = FlxG.Cameras.List.Count - 1;
            FlxG.Cameras.List.Add(newCamera);
            return newCamera;
        }

        public void Remove(FlxCamera camera, bool destroy = true)
        {
            List.Remove(camera);

            for(int i = 0; i < List.Count; i++)
                List[i].Id = i;

            if (destroy)
                camera.Dispose();
        }


        public override void Update(GameTime gameTime)
        {
            foreach(FlxCamera camera in List)
            {
                if (camera != null && camera.Exists && camera.Active)
                    camera.Update(gameTime);
            }
            base.Update(gameTime);
        }

    }
}
