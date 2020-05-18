using Engine.Group;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    public class FlxState : FlxGroup
    {
        public FlxCamera Camera { get { return FlxG.Camera; } }
        public Color BackgroundColor { get; set; }

        #region Debugging
        public bool VisibleBoundingbox { get; set; } = false;
        public Color BoundingBoxColor { get; set; } = Color.Green;
        public bool VisibleHitbox { get; set; } = false;
        public Color HitBoxColor { get; set; } = Color.Green;
        #endregion

        public FlxState()
        {
            BackgroundColor = Color.Black;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            Create();
        }

        protected virtual void Create() { }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (FlxCamera camera in FlxG.Cameras.List)
            {
                CurrentCamera = camera;
                SpriteBatch = CurrentCamera.SpriteBatch;
                SpriteBatch.Begin(SpriteSortMode.Deferred, camera.BlendState, camera.SamplerState,DepthStencilState.None, RasterizerState.CullNone, camera.Effect, camera.ViewMatrix);
                base.Draw(gameTime);
                camera.Draw(gameTime);
                SpriteBatch.End();
            }
        }

#if DEBUG
        public override void DebugDrawBoundigbox(GameTime gameTime)
        {
            if (!VisibleBoundingbox)
                return;
            foreach (FlxCamera camera in FlxG.Cameras.List)
            {
                CurrentCamera = camera;
                SpriteBatch = CurrentCamera.SpriteBatch;
                SpriteBatch.Begin(SpriteSortMode.Deferred, camera.BlendState, camera.SamplerState, DepthStencilState.None, RasterizerState.CullNone, camera.Effect, camera.ViewMatrix);
                base.DebugDrawBoundigbox(gameTime);
                //camera.Draw(gameTime);
                SpriteBatch.End();
            }
        }

        public override void DebugDrawHitBox(GameTime gameTime)
        {
            if (!VisibleHitbox)
                return;
            foreach (FlxCamera camera in FlxG.Cameras.List)
            {
                CurrentCamera = camera;
                SpriteBatch = CurrentCamera.SpriteBatch;
                SpriteBatch.Begin(SpriteSortMode.Deferred, camera.BlendState, camera.SamplerState, DepthStencilState.None, RasterizerState.CullNone, camera.Effect, camera.ViewMatrix);
                base.DebugDrawHitBox(gameTime);
                //camera.Draw(gameTime);
                SpriteBatch.End();
            }
        }
#endif

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public override FlxBasic Add(FlxBasic obj, bool force = false)
        {
            FlxG.Log.Info("Adding " + obj);
            return base.Add(obj, force);
        }

        public override FlxBasic Remove(FlxBasic obj)
        {
            FlxG.Log.Info("Removing " + obj);
            return base.Remove(obj);
        }
    }
}
