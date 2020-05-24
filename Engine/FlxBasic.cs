using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Engine
{
    public enum FlxType : int
    {
        NONE = 0,
        OBJECT = 1,
        GROUP = 2,
        TILEMAP = 3,
        SPRITEGROUP = 4
    }

    public interface IFlxBasic
    {
        int Id { get; set; }
        bool Alive { get; set; }
        bool Exists { get; set; }
        bool Active { get; set; }
        SpriteBatch SpriteBatch { get; set; }
        FlxType FlixelType { get; set; }
        List<FlxCamera> Cameras { get; set; }
        void Kill();
        void Revive();
    }

    public class FlxBasic : CoreGameComponent, IFlxBasic
    {
        public int Id { get; set; } = -1;
        public bool Alive { get; set; } = true;
        public bool Exists { get; set; } = true;
        public bool Active { get { return Enabled; } set { Enabled = value; } }
        public SpriteBatch SpriteBatch { get; set; }

        public FlxType FlixelType { get; set; } = FlxType.NONE;

        /// <summary>
        /// Hepler function set this before actual Draw()
        /// </summary>
        public FlxCamera CurrentCamera { get; set; }
        public List<FlxCamera> Cameras 
        {
            get => (_cameras == null) ? FlxCamera.DefaultCameras : _cameras;
            set => _cameras = value;
        }
        private List<FlxCamera> _cameras;


        public FlxBasic()
        {
        }

        public virtual void Kill() { }
        public virtual void Revive() { }

#if DEBUG
        public virtual void DebugDrawBoundigbox(GameTime gameTime) { }
        public virtual void DebugDrawHitBox(GameTime gameTime) { }
#endif
    }
}
