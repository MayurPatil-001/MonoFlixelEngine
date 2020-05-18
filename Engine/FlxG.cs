using Engine.Inputs;
using Engine.Systems;
using Engine.Systems.FrontEnds;
using Engine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engine
{
    public class FlxG
    {
        /// <summary>
        /// How many times the quad tree should divide the world on each axis.
        /// Generally, sparse collisions can have fewer divisons,
        /// while denser collision activity usually profits from more. Default value is `6`.
        /// </summary>
        public static int WorldDivisions = 6;
        public static int UpdateFrameRate = 60;
        public static FlxGame Game { get; private set; }
        public static int InitialWidth { get; private set; }
        public static int InitialHeight { get; private set; }
        /// <summary>
        /// Game Width in Pixels
        /// Do not Modify, Set in in BaseScaleMode.cs
        /// </summary>
        public static int Width { get; private set; }
        /// <summary>
        /// Game Height in Pixels
        /// Do not Modify, Set in in BaseScaleMode.cs
        /// </summary>
        public static int Height { get; private set; }
        public static float InitialZoom { get; private set; }
        public static Texture2D PixelTexture { get; private set; }
        public static LogFrontEnd Log { get; private set; } = new LogFrontEnd();
        public static FlxCamera Camera { get; private set; }
        public static CameraFrontEnd Cameras { get; private set; } = new CameraFrontEnd();
        public static InputManager InputManager { get; private set; }
        public static KeyboardManager Keyboard { get { return InputManager.Keyboard; } }
        public static MouseManager Mouse { get { return InputManager.Mouse; } }
        public static GamePadManager GamePads { get { return InputManager.GamePads; } }
        public static Rectangle WorldBounds { get; private set; }
        //public static TouchesManager Touches { get { return InputManager.Touches; } }

        public static Random Random { get; private set; }

        public static PluginFrontEnd Plugins { get; private set; }

        internal static void Initialize(FlxGame game, int width, int height, float zoom)
        {
            Game = game;
            Width = width;
            Height = height;
            InitialWidth = Width;
            InitialHeight = Height;
            InitialZoom = zoom;

            Camera = new FlxCamera();
            Camera.SamplerState = SamplerState.PointClamp;
            Camera.BlendState = BlendState.AlphaBlend;

            //Cameras = new List<FlxCamera>();
            Cameras.Add(Camera);

            InputManager = new InputManager();

            Random = new Random();

            WorldBounds = new Rectangle(0, 0, Width, Height);

            Plugins = new PluginFrontEnd();
        }

        internal static void LoadContent(GraphicsDevice graphicsDevice)
        {
            PixelTexture = new Texture2D(graphicsDevice, 1, 1);
            PixelTexture.SetData(new Color[] { Color.White });
            Camera.SpriteBatch = new SpriteBatch(graphicsDevice);
        }


        #region State Management
        public static void SwitchState(FlxState nextState)
        {
            FlxGame.Instance.RequestedState = nextState;
        }

        public static void ResetState()
        {

            FlxState state = (FlxState)Activator.CreateInstance(FlxGame.Instance.CurrentState.GetType());
            SwitchState(state);
        }
        #endregion

        public static void Update(GameTime gameTime)
        {
            InputManager.Update(gameTime);
            Plugins.Update(gameTime);
            Cameras.Update(gameTime);
        }

        public static void Dispose()
        {
            PixelTexture.Dispose();
        }

        #region Collisions

        public static bool Overlap(FlxBasic objectOrGroup1 = null, FlxBasic objectOrGroup2 = null, NotifyCallback notifyCallback = null, ProcessingCallback processingCallback = null)
        {
            if (objectOrGroup1 == null)
                objectOrGroup1 = FlxGame.Instance.CurrentState;
            if (objectOrGroup1 == objectOrGroup2)
                objectOrGroup2 = null;
            FlxQuadTree.Divisions = WorldDivisions;
            FlxQuadTree quadTree = FlxQuadTree.Recycle(WorldBounds.X, WorldBounds.Y, WorldBounds.Width, WorldBounds.Height);
            quadTree.Load(objectOrGroup1, objectOrGroup2, notifyCallback, processingCallback);
            bool result = quadTree.Execute();
            quadTree.Dispose();
            return result;
        }

        public static bool Collide(FlxBasic objectOrGroup1 = null, FlxBasic objectOrGroup2 = null, NotifyCallback notifyCallback = null)
        {
            return Overlap(objectOrGroup1, objectOrGroup2, notifyCallback, FlxObject.Separate);
        }

        public static bool PixelPerfectOverlap(FlxSprite spriteA, FlxSprite spriteB)
        {
            return FlxCollision.PixelPerfectOverlap(spriteA, spriteB);
        }
        #endregion
    }
}
