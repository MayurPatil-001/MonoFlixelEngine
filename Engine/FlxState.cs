using Engine.Group;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    public class FlxState : FlxGroup
    {
        public FlxCamera Camera { get { return FlxG.Camera; } }
        /// <summary>
        /// The natural background color the cameras default to. In `AARRGGBB` format.
        /// </summary>
        public Color BackgroundColor { get => FlxG.Cameras.BgColor; set => FlxG.Cameras.BgColor = value; }

        #region SubState
        /// <summary>
        /// Determines whether or not this state is updated even when it is not the active state.
        /// For example, if you have your game state first, and then you push a menu state on top of it,
        /// if this is set to `true`, the game state would continue to update in the background.
        /// By default this is `false`, so background states will be "paused" when they are not active.
        /// </summary>
        public bool PersistentUpdate = false;
        /// <summary>
        /// Determines whether or not this state is updated even when it is not the active state.
        /// For example, if you have your game state first, and then you push a menu state on top of it,
        /// if this is set to `true`, the game state would continue to be drawn behind the pause state.
        /// By default this is `true`, so background states will continue to be drawn behind the current state.
        /// 
        /// If background states are not `visible` when you have a different state on top,
        /// you should set this to `false` for improved performance.
        /// </summary>
        public bool PersistentDraw = true;
        /// <summary>
        /// If substates get destroyed when they are closed, setting this to
        /// `false` might reduce state creation time, at greater memory cost.
        /// </summary>
        public bool DestroySubStates = true;
        /// <summary>
        /// Current substate. Substates also can be nested.
        /// </summary>
        public FlxSubState SubState { get; set; }
        /// <summary>
        /// If a state change was requested, the new state object is stored here until we switch to it.
        /// </summary>
        private FlxSubState _requestedSubState;
        /// <summary>
        /// Whether to reset the substate (when it changes, or when it's closed).
        /// </summary>
        private bool _requestSubStateReset = false;
        #endregion

        #region Debugging
        public bool VisibleBoundingbox { get; set; } = false;
        public Color BoundingBoxColor { get; set; } = Color.Green;
        public bool VisibleHitbox { get; set; } = false;
        public Color HitBoxColor { get; set; } = Color.Green;
        #endregion

        public FlxState()
        {
#if DEBUG
            FlxG.Log.Info("FlxState instance Created : "+ this + "(" + GetHashCode() + ")");
#endif
        }

        /// <summary>
        /// We do NOT recommend overriding the LoadContent, unless you want some crazy unpredictable things to happen!
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            Create();
        }

        /// <summary>
        /// This function is called after the game engine successfully switches states.
        /// Override this function, NOT the constructor, to initialize or set up your game state.
        /// We do NOT recommend overriding the constructor, unless you want some crazy unpredictable things to happen!
        /// </summary>
        protected virtual void Create() { }

        public override void Update(GameTime gameTime)
        {
            if(PersistentUpdate || SubState == null)
                base.Update(gameTime);
            if (_requestSubStateReset)
            {
                _requestSubStateReset = false;
                ResetSubState();
            }
            if (SubState != null)
                SubState.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if(PersistentDraw || SubState == null) { 
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
            if (SubState != null)
                SubState.Draw(gameTime);
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
            if(SubState != null)
            {
                SubState.Dispose();
                SubState = null;
            }
            base.Dispose(disposing);
#if DEBUG
            FlxG.Log.Info("FlxState instance Discposed : " + this + "(" + GetHashCode() + ")");
#endif
        }

        public override FlxBasic Add(FlxBasic obj, bool force = false)
        {
#if DEBUG
            FlxG.Log.Info("FlxState Adding : " + obj + "("+obj.GetHashCode()+")  in " +this +"(" + GetHashCode()+")");
#endif
            return base.Add(obj, force);
        }

        public override FlxBasic Remove(FlxBasic obj)
        {
#if DEBUG
            FlxG.Log.Info("FlxState Removing : " + obj + "(" + obj.GetHashCode() + ")  from " + this + "(" + GetHashCode() + ")");
#endif
            return base.Remove(obj);
        }

        #region SubState
        public void OpenSubState(FlxSubState subState)
        {
            _requestSubStateReset = true;
            _requestedSubState = subState;
#if DEBUG
            FlxG.Log.Info("FlxState OpenSubState : " + subState + "(" + subState.GetHashCode() + ")  in " + this + "(" + GetHashCode() + ")");
#endif
        }
        public void CloseSubState()
        {
            _requestSubStateReset = true;
        }
        public void ResetSubState()
        {
            if(SubState != null)
            {
                SubState.CloseCallback?.Invoke();

                if (DestroySubStates)
                    SubState.Dispose();
            }

            SubState = _requestedSubState;
            _requestedSubState = null;

            if(SubState != null)
            {
                //TODO: Reset Inputs

                SubState.ParentState = this;
                if (!SubState.Created)
                {
                    SubState.Created = true;
                    SubState.Create();
                }

                SubState.OpenCallback?.Invoke();
            }
        }
        /// <summary>
        /// Called from `FlxG.switchState()`. If `false` is returned, the state
        /// switch is cancelled - the default implementation returns `true`.
        /// 
        /// Useful for customizing state switches, e.g. for transition effects.
        /// </summary>
        /// <param name="nextState"></param>
        /// <returns></returns>
        public bool SwitchTo(FlxState nextState)
        {
            return true;
        }
        #endregion

        #region Window Events
        /// <summary>
        /// This method is called after the game loses focus.
        /// Can be useful for third party libraries, such as tweening engines.
        /// </summary>
        public virtual void OnFocusLost() { }
        /// <summary>
        /// This method is called after the game receives focus.
        /// Can be useful for third party libraries, such as tweening engines.
        /// </summary>
        public virtual void OnFocus() { }
        /// <summary>
        /// This function is called whenever the window size has been changed.
        /// </summary>
        /// <param name="width">The new window width</param>
        /// <param name="height">The new window Height</param>
        public virtual void OnResize(int width, int height) { }
        #endregion
    }
}
