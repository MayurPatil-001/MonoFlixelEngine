using Microsoft.Xna.Framework;
using System;

namespace Engine
{
    public class FlxSubState: FlxState
    {
        public Action OpenCallback;
        public Action CloseCallback;

        public FlxState ParentState { get; set; }
        private Color _bgColor;
        private Color BgColor { get => _bgColor; set { _bgColor = value; _bgSprite.Color = _bgColor; } }
        public bool Created { get; set; }
        private FlxSprite _bgSprite;

        public FlxSubState(Color? bgColor = null) : base()
        {
            if (!bgColor.HasValue)
                bgColor = Color.Transparent;
            _bgColor = bgColor.Value;
            CloseCallback = null;
            OpenCallback = null;
            Initialize();
#if DEBUG
            FlxG.Log.Info("FlxSubState instance Created : " + this + "(" + GetHashCode() + ")");
#endif
        }

        protected override void LoadContent()
        {
            //Just to make sure this is added in background before any other sprites/Objects
            _bgSprite = new FlxSprite();
            _bgSprite.MakeGraphic(FlxG.Width, FlxG.Height, BgColor);
            Add(_bgSprite);
            base.LoadContent();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            CloseCallback = null;
            OpenCallback = null;
            ParentState = null;
#if DEBUG
            FlxG.Log.Info("FlxSubState instance Discposed : " + this + "(" + GetHashCode() + ")");
#endif
        }

        public void Close()
        {
            if (ParentState != null && ParentState.SubState == this)
                ParentState.CloseSubState();
        }
    }
}
