using Engine.Inputs;
using Engine.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Engine.UI.Button
{
    /// <summary>
    /// A simple button class that calls a function when clicked by the mouse.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FlxTypedButton<T> : FlxSprite, IFlxInput where T : FlxSprite
    {
        #region Label
        /// <summary>
        /// The label that appears on the button. Can be any `FlxSprite`.
        /// </summary>
        private T _label;
        public T Label 
        { 
            get => _label; 
            set 
            {
                value.ScrollFactor = ScrollFactor;
                _label = value;
                UpdateLabelPosition(); 
            }
        }
        public override float X 
        { 
            get => base.X;
            set
            {
                base.X = value;
                UpdateLabelPosition();
            }
        }
        public override float Y 
        {
            get => base.Y;
            set
            {
                base.Y = value;
                UpdateLabelPosition();
            }
        }
        public override float Alpha { get => base.Alpha; set { base.Alpha = value; UpdateLabelAlpha(); } }
        
        /// <summary>
        /// What offsets the `label` should have for each status.
        /// </summary>
        public Vector2[] LabelOffsets = { Vector2.Zero, Vector2.Zero, new Vector2(0, 1) };
        /// <summary>
        /// What alpha value the label should have for each status. Default is `[0.8, 1.0, 0.5]`.
        /// Multiplied with the button's `alpha`.
        /// </summary>
        public float[] LabelAlphas = { 0.8f, 1.0f, 0.5f};
        #endregion

        #region States
        /// <summary>
        /// What animation should be played for each status.
        /// Default is ["normal", "highlight", "pressed"].
        /// </summary>
        public string[] StatusAnimations = { "normal", "highlight", "pressed" };
        /// <summary>
        /// Whether you can press the button simply by releasing the touch / mouse button over it (default).
        /// If false, the input has to be pressed while hovering over the button.
        /// </summary>
        public bool AllowSwiping = true;
        //TODO: Mouse Buttons
        /// <summary>
        /// Maximum distance a pointer can move to still trigger event handlers.
        /// If it moves beyond this limit, onOut is triggered.
        /// Defaults to `float.MaxValue` (i.e. no limit).
        /// </summary>
        public float MaxInputMovement = float.MaxValue;
        /// <summary>
        /// Shows the current state of the button, either `FlxButton.NORMAL`,
        /// `FlxButton.HIGHLIGHT` or `FlxButton.PRESSED`.
        /// </summary>
        private int _status;
        public int Status { get => _status; set { _status = value; UpdateLabelAlpha(); } }
        private int _lastStaus = -1;
        /// <summary>
        /// The properties of this button's `onUp` event (callback function, sound).
        /// </summary>
        public FlxButtonEvent OnUp { get; private set; }
        /// <summary>
        /// The properties of this button's `onDown` event (callback function, sound).
        /// </summary>
        public FlxButtonEvent OnDown { get; private set; }
        /// <summary>
        /// The properties of this button's `onOver` event (callback function, sound).
        /// </summary>
        public FlxButtonEvent OnOver { get; private set; }
        /// <summary>
        /// The properties of this button's `onOut` event (callback function, sound).
        /// </summary>
        public FlxButtonEvent OnOut { get; private set; }
        #endregion

        #region Input Handling
        public bool JustReleased { get; private set; }
        public bool Released { get; private set; }
        public bool Pressed { get; private set; }
        public bool JustPressed { get; private set; }
        #endregion


        #region Constructor
        /// <summary>
        /// Creates a new `FlxTypedButton` object with a gray background.
        /// </summary>
        /// <param name="x">The x position of the button.</param>
        /// <param name="y">The y position of the button.</param>
        /// <param name="onClick">The function to call whenever the button is clicked.</param>
        public FlxTypedButton(float x = 0, float y = 0, Action onClick = null):base()
        {
            X = x;
            Y = y;
            OnUp = new FlxButtonEvent(onClick);
            OnDown = new FlxButtonEvent();
            OnOver = new FlxButtonEvent();
            OnOut = new FlxButtonEvent();

            Initialize();
        }

        public override void Initialize()
        {
            Status = FlxButton.NORMAL;

            // Since this is a UI element, the default scrollFactor is (0, 0)
            ScrollFactor = Vector2.Zero;

            //TODO: Mouse Input Management
            base.Initialize();
        }

        protected override void LoadContent()
        {
            LoadDefaultGraphic();
        }
        #endregion


        #region Overrides
        public override void GraphicLoaded()
        {
            SetupAnimation("normal", FlxButton.NORMAL);
            SetupAnimation("highlight", FlxButton.HIGHLIGHT);
            SetupAnimation("pressed", FlxButton.PRESSED);
        }

        public override void Update(GameTime gameTime)
        {
            if (Visible)
            {
                UpdateButton();

                if (_lastStaus != Status)
                {
                    UpdateStatusAnimation();
                    _lastStaus = Status;
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            if(Label != null && Label.Visible)
            {
                Label.Cameras = Cameras;
                Label.SpriteBatch = SpriteBatch;
                Label.Draw(gameTime);
            }
        }

        public override void DebugDrawHitBox(GameTime gameTime)
        {
            base.DebugDrawHitBox(gameTime);
            if (Label != null && Label.Visible)
            {
                Label.Cameras = Cameras;
                Label.DebugDrawHitBox(gameTime);
            }
        }

        public override void DebugDrawBoundigbox(GameTime gameTime)
        {
            base.DebugDrawHitBox(gameTime);
            if (Label != null && Label.Visible)
            {
                Label.Cameras = Cameras;
                Label.DebugDrawBoundigbox(gameTime);
            }
        }

        /// <summary>
        /// Called by the game state when state is changed (if this object belongs to the state)
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            Label.Dispose();
            OnUp?.Dispose();
            OnDown?.Dispose();
            OnOver?.Dispose();
            OnOut?.Dispose();
            LabelOffsets = null;
            LabelAlphas = null;

            base.Dispose(disposing);
        }
        #endregion

        #region Utils
        void LoadDefaultGraphic()
        {
            LoadGraphic(FlxAssets.GRAPHIC_BUTTON, true, 80, 20);
        }

        void SetupAnimation(string animationName, int frameIndex)
        {
            frameIndex = Math.Min(frameIndex, Animation.AllAvailableFrames - 1);
            Animation.Add(animationName, new int[]{ frameIndex });
        }

        void UpdateStatusAnimation()
        {
            Animation.Play(StatusAnimations[Status]);
        }

        /// <summary>
        /// Basic button update logic - searches for overlaps with touches and
        /// the mouse cursor and calls `updateStatus()`.
        /// </summary>
        void UpdateButton()
        {
            bool overlapFound = CheckMouseOverlap();
            if (overlapFound)
            {
                if(Status == FlxButton.NORMAL)
                {
                    OnOverHandler();
                }

                bool justPressed = FlxG.Mouse.JustPressed;
                bool justReleased = FlxG.Mouse.JustReleased;
                if (Status == FlxButton.HIGHLIGHT && justPressed)
                {
                    OnDownHandler();
                }
                else if (Status == FlxButton.PRESSED && justReleased)
                {
                    OnUpHandler();
                }
            }
            else
            {
                if(Status == FlxButton.HIGHLIGHT || Status == FlxButton.PRESSED)
                {
                    OnOutHandler();
                }
            }
        }

        void UpdateLabelPosition()
        {
            if(Label != null)
            {
                Label.X = (PixelPerfectPosition ? (float)Math.Floor(X) : X) + LabelOffsets[Status].X;
                Label.Y = (PixelPerfectPosition ? (float)Math.Floor(Y) : Y) + LabelOffsets[Status].Y;
            }
        }
        void UpdateLabelAlpha()
        {
            if (Label != null && LabelAlphas.Length > Status)
            {
                Label.Alpha = Alpha * LabelAlphas[Status];
            }
        }
        /// <summary>
        /// Updates the button status by calling the respective event handler function.
        /// </summary>
        //void UpdateStatus()
        //{

        //}
        #endregion


        #region Internal Event Handlers
        //void OnUpEventListener()
        //{
        //    if (FlxG.Mouse.Enabled)
        //    {
        //        if(Visible && Exists && Active && Status == FlxButton.PRESSED)
        //        {
        //            OnUpHandler();
        //        }
        //    }
        //}
        void OnUpHandler() 
        {
            Status = FlxButton.NORMAL;
            //TODO: Intput.Release()
            //CurrentInput = null;
            FlxG.Log.Info("OnUpHandler Status " + Status + " " + GetHashCode());
            // Order matters here, because onUp.fire() could cause a state change and destroy this object.
            OnUp.Fire();
        }
        void OnDownHandler() 
        {
            Status = FlxButton.PRESSED;
            FlxG.Log.Info("OnDownHandler Status " + Status + " " + GetHashCode());
            //TODO: Intput.Press()
            // Order matters here, because onDown.fire() could cause a state change and destroy this object.
            OnDown.Fire();
        }
        void OnOverHandler() 
        {
            //if (!FlxG.Mouse.Enabled)
            //{
            //    Status = FlxButton.NORMAL;
            //    return;
            //}
            Status = FlxButton.HIGHLIGHT;
            FlxG.Log.Info("OnOverHandler Status "+ Status + " " + GetHashCode());
            // Order matters here, because onOver.fire() could cause a state change and destroy this object.
            OnOver.Fire();
        }
        void OnOutHandler() 
        {
            Status = FlxButton.NORMAL;

            FlxG.Log.Info("OnOutHandler Status " + Status + " " + GetHashCode());
            //TODO: Inputrelease
            // Order matters here, because onOut.fire() could cause a state change and destroy this object.
            OnOut.Fire();
        }
        #endregion

        #region Input Management
        bool CheckMouseOverlap()
        {
            foreach (FlxCamera camera in Cameras)
            {
                Vector2 mousePosition = FlxG.Mouse.GetPositionInCameraView(camera);
                if (HitBox.Contains(mousePosition.X, mousePosition.Y)) 
                {
                    return true;
                }   
            }
                
            return false;
        }

        //bool CheckTouchOverlap()
        //{
        //    //TODO Touch implement
        //    return false;
        //}

        //bool CheckInput(FlxPointer pointer, IFlxInput input, Vector2 justPressedPosition, FlxCamera camera)
        //{
        //    //if (MaxInputMovement != float.MaxValue
        //    //&& justPressedPosition.DistanceTo(pointer.getScreenPosition(FlxPoint.weak())) > MaxInputMovement
        //    //&& input == currentInput)
        //    //{
        //    //    currentInput == null;
        //    //}
        //    //else if (OverlapsPoint(pointer.getWorldPosition(camera, _point), true, camera))
        //    //{
        //    //    updateStatus(input);
        //    //    return true;
        //    //}
        //    Vector2 mousePosition = FlxG.Mouse.GetPositionInCameraView(camera);

        //    return false;
        //}
        #endregion
    }
}
