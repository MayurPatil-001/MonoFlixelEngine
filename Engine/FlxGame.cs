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
        /// <summary>
        /// Window Title
        /// </summary>
        public string Title { get => Window.Title; set => Window.Title = value; }

        // util
        private FlxState nextState;
        /// <summary>
        /// Current Game State
        /// </summary>
        public FlxState CurrentState { get; private set; }

        /// <summary>
        /// Requested Game State, Used internally to change Current Game State
        /// </summary>
        public FlxState RequestedState { set { nextState = value; } }

        //screen size
        private readonly int _width;
        private readonly int _height;

        /// <summary>
        /// Game Constructor
        /// </summary>
        /// <param name="width">Games Width</param>
        /// <param name="height">Game Height</param>
        /// <param name="windowWidth">Widnow Width</param>
        /// <param name="windowHeight">Window Height</param>
        /// <param name="windowTitle">Window TItle</param>
        /// <param name="fullscreen">Start in FullScreen</param>
        public FlxGame(int width, int height, int windowWidth, int windowHeight, string windowTitle = "Game", bool fullscreen = true)
        {
            Title = windowTitle;
            _width = width;
            _height = height;

            GraphicsDeviceManager graphics = new GraphicsDeviceManager(this);
            graphics.DeviceReset += OnGraphicsReset;
            graphics.DeviceCreated += OnGraphicsCreate;
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.PreferMultiSampling = false;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.PreferredBackBufferFormat = SurfaceFormat.Color;
            graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;

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
                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                graphics.IsFullScreen = true;
            }
            else
            {
                graphics.PreferredBackBufferWidth = windowWidth;
                graphics.PreferredBackBufferHeight = windowHeight;
                graphics.IsFullScreen = false;
            }
#endif
            graphics.ApplyChanges();
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Game Constructor
        /// Window width will be set to "Game Width"
        /// Window Height will be set to "Game Height"
        /// Window Title will be set to "Game"
        /// Window FullScreen will be set to "False"
        /// </summary>
        /// <param name="gameWidth">Game Width</param>
        /// <param name="gameHeight">Game Height</param>
        public FlxGame(int gameWidth, int gameHeight): this(gameWidth, gameHeight, gameWidth, gameHeight, "Game", false)
        {

        }

        /// <summary>
        /// Internal Initialize Method
        /// </summary>
        protected override void Initialize()
        {
            FlxG.Initialize(this, _width, _height, 1);
            base.Initialize();
        }

        /// <summary>
        /// Internal LoadContent Method, Create() will be called from this method
        /// </summary>
        protected override void LoadContent()
        {
            FlxG.LoadContent(GraphicsDevice);
            base.LoadContent();
            Create();
        }

        /// <summary>
        /// Override this to Add Initial Game State, will be Called by Game's LoadContent()
        /// </summary>
        protected virtual void Create() { }

        /// <summary>
        /// Internal Update Method
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            FlxG.Update(gameTime);

            if (CurrentState != nextState)
            {
                CurrentState?.Dispose();
                CurrentState = nextState;
                CurrentState?.Initialize();
            }

            CurrentState?.Update(gameTime);
            
            base.Update(gameTime);
        }

        /// <summary>
        /// Internal Draw Method
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            CurrentState?.Draw(gameTime);
            base.Draw(gameTime);
        }

        /// <summary>
        /// Internal Dispose Method
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            CurrentState?.Dispose();
            FlxG.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Add Initial State to Game
        /// </summary>
        /// <param name="state"></param>
        public void Add(FlxState state)
        {
            if (CurrentState != null)
                throw new InvalidOperationException("Inital State is already added");

            nextState = state;
        }

        protected override void OnActivated(object sender, EventArgs args)
        {
            base.OnActivated(sender, args);
            CurrentState?.OnFocus();
        }

        protected override void OnDeactivated(object sender, EventArgs args)
        {
            base.OnDeactivated(sender, args);
            CurrentState?.OnFocusLost();
        }

        private void OnClientSizeChanged(object sender, EventArgs e)
        {
            CurrentState?.OnResize(Window.ClientBounds.Width, Window.ClientBounds.Height);
        }

        private void OnGraphicsCreate(object sender, EventArgs e)
        {
            //Nothing
        }

        private void OnGraphicsReset(object sender, EventArgs e)
        {
            //Nothing
        }

    }
}
