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
        /// <summary>
        /// Current Game instnace
        /// </summary>
        public static FlxGame Game { get; private set; }
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
        /// <summary>
        /// 1x1 White Pixel Texture, Used to Draw Colored Rectangles
        /// </summary>
        public static Texture2D PixelTexture { get; private set; }
        /// <summary>
        /// Default Texture for Sprites
        /// </summary>
        public static Texture2D DefaultGraphic { get; private set; }
        /// <summary>
        /// Logger
        /// </summary>
        public static LogFrontEnd Log { get; private set; } = new LogFrontEnd();
        /// <summary>
        /// Default Game Camera
        /// </summary>
        public static FlxCamera Camera { get; private set; }
        public static CameraFrontEnd Cameras { get; private set; } = new CameraFrontEnd();
        /// <summary>
        /// Game Input Manager
        /// </summary>
        public static InputManager InputManager { get; private set; }
        /// <summary>
        /// Keyboard Input Manager, usage - Keyboard.Pressed(Key); etc.
        /// </summary>
        public static KeyboardManager Keyboard { get { return InputManager.Keyboard; } }
        /// <summary>
        /// Mouse Input Manager, Usage - Mouse.Pressed(); etc
        /// </summary>
        public static MouseManager Mouse { get { return InputManager.Mouse; } }
        /// <summary>
        /// GamePad Manager
        /// </summary>
        public static GamePadManager GamePads { get { return InputManager.GamePads; } }
        /// <summary>
        /// Game World Bouds, update this to change collision boudry of Game, Defaults to (0, 0, GameWidth, GameHeight)
        /// </summary>
        public static Rectangle WorldBounds { get; private set; }
        //public static TouchesManager Touches { get { return InputManager.Touches; } }

        /// <summary>
        /// Random Object for Integers, Floats, Objects etc.
        /// </summary>
        public static Random Random { get; private set; }

        public static PluginFrontEnd Plugins { get; private set; }

        /// <summary>
        /// Initialize Globales & Default Values
        /// </summary>
        /// <param name="game"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="zoom"></param>
        internal static void Initialize(FlxGame game, int width, int height, float zoom)
        {
            Game = game;
            Width = width;
            Height = height;

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

        /// <summary>
        /// Load Default contents
        /// </summary>
        /// <param name="graphicsDevice"></param>
        internal static void LoadContent(GraphicsDevice graphicsDevice)
        {
            PixelTexture = new Texture2D(graphicsDevice, 1, 1);
            PixelTexture.SetData(new Color[] { Color.White });
            Camera.SpriteBatch = new SpriteBatch(graphicsDevice);

            DefaultGraphic = Game.Content.Load<Texture2D>(FlxAssets.GRAPHIC_DEFAULT);
        }


        #region State Management
        /// <summary>
        /// Chamge current Game State
        /// </summary>
        /// <param name="nextState">Next state to be changed</param>
        public static void SwitchState(FlxState nextState)
        {
            Game.RequestedState = nextState;
        }

        /// <summary>
        /// Reset current Game State
        /// </summary>
        public static void ResetState()
        {
            FlxState state = (FlxState)Activator.CreateInstance(Game.CurrentState.GetType());
            SwitchState(state);
        }
        #endregion

        /// <summary>
        /// Update Inputs, Plugins & Cameras
        /// </summary>
        /// <param name="gameTime"></param>
        public static void Update(GameTime gameTime)
        {
            InputManager.Update(gameTime);
            Plugins.Update(gameTime);
            Cameras.Update(gameTime);
        }

        /// <summary>
        /// Dispose Content
        /// </summary>
        public static void Dispose()
        {
            PixelTexture.Dispose();
        }

        #region Collisions
        /// <summary>
        /// Call this function to see if one `FlxObject` overlaps another within `FlxG.worldBounds`.
        /// Can be called with one object and one group, or two groups, or two objects,
        /// whatever floats your boat! For maximum performance try bundling a lot of objects
        /// together using a `FlxGroup` (or even bundling groups together!).
        /// 
        /// NOTE: does NOT take objects' `scrollFactor` into account, all overlaps are checked in world space.
        /// 
        /// NOTE: this takes the entire area of `FlxTilemap`s into account (including "empty" tiles).
        ///       Use `FlxTilemap#overlaps()` if you don't want that.
        /// </summary>
        /// <param name="objectOrGroup1">The first object or group you want to check.</param>
        /// <param name="objectOrGroup2">The second object or group you want to check. If it is the same as the first,Flixel knows to just do a comparison within that group.</param>
        /// <param name="notifyCallback">A function with two `FlxObject` parameters -e.g. `onOverlap(object1:FlxObject, object2:FlxObject)` -that is called if those two objects overlap.</param>
        /// <param name="processingCallback">A function with two `FlxObject` parameters -e.g. `onOverlap(object1:FlxObject, object2:FlxObject)` -that is called if those two objects overlap.If a `ProcessCallback` is provided, then `NotifyCallback`will only be called if `ProcessCallback` returns true for those objects!</param>
        /// <returns>Whether any overlaps were detected.</returns>
        public static bool Overlap(FlxBasic objectOrGroup1 = null, FlxBasic objectOrGroup2 = null, NotifyCallback notifyCallback = null, ProcessingCallback processingCallback = null)
        {
            if (objectOrGroup1 == null)
                objectOrGroup1 = Game.CurrentState;
            if (objectOrGroup1 == objectOrGroup2)
                objectOrGroup2 = null;
            FlxQuadTree.Divisions = WorldDivisions;
            FlxQuadTree quadTree = FlxQuadTree.Recycle(WorldBounds.X, WorldBounds.Y, WorldBounds.Width, WorldBounds.Height);
            quadTree.Load(objectOrGroup1, objectOrGroup2, notifyCallback, processingCallback);
            bool result = quadTree.Execute();
            quadTree.Dispose();
            return result;
        }

        /// <summary>
        /// Call this function to see if one `FlxObject` collides with another within `FlxG.worldBounds`.
        /// Can be called with one object and one group, or two groups, or two objects,
        /// whatever floats your boat! For maximum performance try bundling a lot of objects
        /// together using a FlxGroup (or even bundling groups together!).
        /// 
        /// This function just calls `FlxG.overlap` and presets the `ProcessCallback` parameter to `FlxObject.separate`.
        ///  To create your own collision logic, write your own `ProcessCallback` and use `FlxG.overlap` to set it up.
        ///  NOTE: does NOT take objects' `scrollFactor` into account, all overlaps are checked in world space.
        /// </summary>
        /// <param name="objectOrGroup1">The first object or group you want to check.</param>
        /// <param name="objectOrGroup2">The second object or group you want to check. If it is the same as the first,Flixel knows to just do a comparison within that group.</param>
        /// <param name="notifyCallback"> A function with two `FlxObject` parameters -e.g. `onOverlap(object1:FlxObject, object2:FlxObject)` -that is called if those two objects overlap.</param>
        /// <returns>Whether any objects were successfully collided/separated.</returns>
        public static bool Collide(FlxBasic objectOrGroup1 = null, FlxBasic objectOrGroup2 = null, NotifyCallback notifyCallback = null)
        {
            return Overlap(objectOrGroup1, objectOrGroup2, notifyCallback, FlxObject.Separate);
        }

        /// <summary>
        /// A pixel perfect collision check between two `FlxSprite` objects.
        /// It will do a bounds check first, and if that passes it will run a
        /// pixel perfect match on the intersecting area. Works with rotated and animated sprites.
        /// May be slow, so use it sparingly.
        /// </summary>
        /// <param name="spriteA">The first `FlxSprite` to test against.</param>
        /// <param name="spriteB">The second `FlxSprite` to test again, sprite order is irrelevant.</param>
        /// <returns>Whether the sprites collide</returns>
        public static bool PixelPerfectOverlap(FlxSprite spriteA, FlxSprite spriteB)
        {
            return FlxCollision.PixelPerfectOverlap(spriteA, spriteB);
        }
        #endregion
    }
}
