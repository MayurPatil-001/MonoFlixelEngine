using Engine.Systems;
using Engine.Texts;
using Engine.UI.Button;
using Microsoft.Xna.Framework;
using System;

namespace Engine.UI
{
    /// <summary>
    /// A simple button class that calls a function when clicked by the mouse.
    /// </summary>
    public class FlxButton:FlxTypedButton<FlxText>
    {
        private string _fontPath;

        /// <summary>
        /// Used with public variable status, means not highlighted or pressed.
        /// </summary>
        public static int NORMAL = 0;
        /// <summary>
        /// Used with public variable status, means highlighted (usually from mouse over).
        /// </summary>
        public static int HIGHLIGHT = 1;
        /// <summary>
        /// Used with public variable status, means pressed (usually from mouse click).
        /// </summary>
        public static int PRESSED = 2;

        /// <summary>
        /// Shortcut to setting label.text
        /// </summary>
        public string Text 
        { 
            get => Label.Text;
            set 
            {
                if (Label == null)
                    InitLabel(value);
                else
                    Label.Text = value;
            }
        }

        /// <summary>
        /// Creates a new `FlxButton` object with a gray background
        /// and a callback function on the UI thread.
        /// </summary>
        /// <param name="fontPath"></param>
        /// <param name="x">The x position of the button.</param>
        /// <param name="y">The y position of the button.</param>
        /// <param name="text">The text that you want to appear on the button.</param>
        /// <param name="onClick">The function to call whenever the button is clicked.</param>
        public FlxButton(string fontPath, float x = 0, float y = 0, string text = null, Action onClick = null):base(x, y, onClick)
        {
            _fontPath = fontPath;
            for (int i = 0; i < LabelOffsets.Length; i++)
            {
                LabelOffsets[i].X -= 1;
                LabelOffsets[i].Y += 3;
            }

            InitLabel(text);
        }

        public FlxButton(float x = 0, float y = 0, string text = null, Action onClick = null) : this(FlxAssets.FONT_DEFAULT,x, y,text, onClick)
        {
        }

        void InitLabel(string text)
        {
            if(text != null)
            {
                Label = new FlxText(_fontPath, X + LabelOffsets[NORMAL].X, Y + LabelOffsets[NORMAL].Y, 80, text);
                Label.SetFormat(null, Color.Black, FlxTextAlign.CENTER);
                Label.Alpha = LabelAlphas[Status];
                //TODO DrawFrame???
            }
        }


        #region Overrides
        /// <summary>
        /// Updates the size of the text field to match the button.
        /// </summary>
        protected override void ResetHelpers()
        {
            base.ResetHelpers();
            if(Label != null)
            {
                Label.FieldWidth = Label.FieldWidth = (int)Width;
            }
        }
        #endregion
    }
}
