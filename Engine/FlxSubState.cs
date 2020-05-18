using Microsoft.Xna.Framework;
using System;

namespace Engine
{
    public class FlxSubState: FlxState
    {
        public Action OpenCallback;
        public Action CloseCallback;

        public FlxState ParentState { get; set; }
        private Color BgColor { get; set; }
        public bool Created { get; set; }
        private FlxSprite _bgSprite;

        public FlxSubState(Color? bgColor = null)
        {
            if (!bgColor.HasValue)
                bgColor = Color.Transparent;
            BgColor = bgColor.Value;
            CloseCallback = null;
            OpenCallback = null;
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
        }

        public void Close()
        {
            if (ParentState != null && ParentState.SubState == this)
                ParentState.CloseSubState();
        }
    }
}
