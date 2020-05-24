using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.ViewportAdapters
{
    public class DefaultViewportAdapter : ViewportAdapter
    {
        
        public DefaultViewportAdapter(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
        }

        public override int VirtualWidth => Viewport.Width;
        public override int VirtualHeight => Viewport.Height;
        public override int ViewportWidth => Viewport.Width;
        public override int ViewportHeight => Viewport.Height;

        public override Matrix GetScaleMatrix()
        {
            return Matrix.Identity;
        }
    }
}
