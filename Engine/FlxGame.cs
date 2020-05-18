#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

#endregion

namespace Engine
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class FlxGame : Game
    {
        public string Title;

        // references
        public static FlxGame Instance { get; private set; }
        public static GraphicsDeviceManager Graphics { get; private set; }
        
        // util
        public Color BackgroundColor;
        private FlxState nextState;
        public FlxState CurrentState { get; private set; }
        public FlxState RequestedState { set { nextState = value; } }

        // screen size
        private int _width;
        private int _height;
        //public static int ViewWidth { get; private set; }
        //public static int ViewHeight { get; private set; }
        //public static Viewport Viewport { get; private set; }
        public static Matrix ScreenMatrix;
        public RenderTarget2D RenderTarget2D;


        public FlxGame(int width, int height, int windowWidth, int windowHeight, string windowTitle = "Game", bool fullscreen = true)
        {

            Instance = this;

            Title = Window.Title = windowTitle;
            _width = width;
            _height = height;
            BackgroundColor = Color.Black;
            
            Graphics = new GraphicsDeviceManager(this);
            Graphics.DeviceReset += OnGraphicsReset;
            Graphics.DeviceCreated += OnGraphicsCreate;
            Graphics.SynchronizeWithVerticalRetrace = true;
            Graphics.PreferMultiSampling = false;
            Graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Graphics.PreferredBackBufferFormat = SurfaceFormat.Color;
            Graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;

#if PS4 || XBOXONE
            Graphics.PreferredBackBufferWidth = 1920;
            Graphics.PreferredBackBufferHeight = 1080;
#elif NSWITCH
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;
#else
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnClientSizeChanged;

            if (fullscreen)
            {
                Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                Graphics.IsFullScreen = true;
            }
            else
            {
                Graphics.PreferredBackBufferWidth = windowWidth;
                Graphics.PreferredBackBufferHeight = windowHeight;
                Graphics.IsFullScreen = false;
            }
#endif

            Graphics.ApplyChanges();
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        public FlxGame(int gameWidth, int gameHeight): this(gameWidth, gameHeight, gameWidth, gameHeight, "Game", false)
        {

        }

        protected override void Initialize()
        {
            FlxG.Initialize(this, _width, _height, 1);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            FlxG.LoadContent(GraphicsDevice);
            RenderTarget2D = new RenderTarget2D(GraphicsDevice, FlxG.Width, FlxG.Height);
            base.LoadContent();
            Create();
        }

        protected virtual void Create() { }

        protected override void Update(GameTime gameTime)
        {
            FlxG.Update(gameTime);

            if (CurrentState != nextState)
            {
                if (CurrentState != null) 
                {
                    CurrentState.Dispose();
                }
                CurrentState = nextState;
                CurrentState.Initialize();
            }

            if (CurrentState != null)
                CurrentState.Update(gameTime);
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Viewport = Viewport;
            //GraphicsDevice.SetRenderTarget(RenderTarget2D);
            GraphicsDevice.Clear(CurrentState.BackgroundColor);
            CurrentState.Draw(gameTime);

            if (CurrentState.VisibleHitbox)
                CurrentState.DebugDrawHitBox(gameTime);

            if (CurrentState.VisibleBoundingbox)
                CurrentState.DebugDrawBoundigbox(gameTime);

            base.Draw(gameTime);

            //GraphicsDevice.SetRenderTarget(null);
            //GraphicsDevice.Clear(BackgroundColor);
            //FlxG.Camera.SpriteBatch.Begin();
            //FlxG.Camera.SpriteBatch.Draw(RenderTarget2D, GraphicsDevice.Viewport.Bounds, Color.White);
            //FlxG.Camera.SpriteBatch.End();
        }

        protected override void Dispose(bool disposing)
        {
            CurrentState.Dispose();
            RenderTarget2D.Dispose();
            FlxG.Dispose();
            base.Dispose(disposing);
        }

        public void Add(FlxState state)
        {
            nextState = state;
        }

        protected override void OnActivated(object sender, EventArgs args)
        {
            base.OnActivated(sender, args);

            //if (currentState != null)
            //    currentState.GainFocus();
        }

        protected override void OnDeactivated(object sender, EventArgs args)
        {
            base.OnDeactivated(sender, args);

            //if (currentState != null)
            //    currentState.LoseFocus();
        }

        private void OnClientSizeChanged(object sender, EventArgs e)
        {
            //if (RenderTarget2D != null)
            //    RenderTarget2D.Dispose();
            //RenderTarget2D = new RenderTarget2D(GraphicsDevice, Window.ClientBounds.Width, Window.ClientBounds.Height);
        }

        private void OnGraphicsCreate(object sender, EventArgs e)
        {
            //if (RenderTarget2D != null)
            //    RenderTarget2D.Dispose();
            //RenderTarget2D = new RenderTarget2D(GraphicsDevice, Window.ClientBounds.Width, Window.ClientBounds.Height);
        }

        private void OnGraphicsReset(object sender, EventArgs e)
        {
            //if (RenderTarget2D != null)
            //    RenderTarget2D.Dispose();
            //RenderTarget2D = new RenderTarget2D(GraphicsDevice, Window.ClientBounds.Width, Window.ClientBounds.Height);
        }

    }
}
