using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using static Engine.Group.FlxTypedGroup<Engine.FlxSprite>;

namespace Engine.Group
{
    public class FlxTypedSpriteGroup<T> : FlxSprite where T : FlxSprite
    {
        private List<T> _sprites;
        public FlxTypedGroup<T> Group { get; }
        public List<T> Members => Group.Members;
        public int Length { get => Members.Count; }
        public int MaxSize { get; set; }

        #region override Properties
        public override float Width { get { return GetWidth(); } }
        public override float Height { get { return GetHeight(); } }
        #endregion


        public FlxTypedSpriteGroup(float x = 0, float y = 0, int maxSize = 0):base(x, y)
        {
            MaxSize = maxSize;
            Group = new FlxTypedGroup<T>(MaxSize);
            _sprites = Group.Members;
        }
        #region Overrides
        protected override void InitVars()
        {
            FlixelType = FlxType.SPRITEGROUP;
            InitMotionVars();
        }

        public override void Update(GameTime gameTime)
        {
            Group.Update(gameTime);
            if (Moves)
                UpdateMotion(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Group.SpriteBatch = SpriteBatch;
            Group.Draw(gameTime);
        }

#if DEBUG
        public override void DebugDrawBoundigbox(GameTime gameTime)
        {
            Group.SpriteBatch = SpriteBatch;
            Group.DebugDrawBoundigbox(gameTime);
        }
        public override void DebugDrawHitBox(GameTime gameTime)
        {
            Group.SpriteBatch = SpriteBatch;
            Group.DebugDrawHitBox(gameTime);
        }
#endif

        protected override void Dispose(bool disposing)
        {
            _sprites = null;
            Group.Dispose();
            base.Dispose(disposing);

#if DEBUG
            FlxG.Log.Info("TypedSpriteGroup Dispose for" + this);
#endif
        }

        public override bool IsOnScreen(FlxCamera camera = null)
        {
            foreach (FlxSprite sprite in _sprites as List<FlxSprite>)
                if (sprite != null && sprite.Exists && sprite.Visible && sprite.IsOnScreen(camera))
                    return true;
            return false;
        }
        public override void Kill()
        {
            base.Kill();
            Group.Kill();
        }

        public override void Revive()
        {
            base.Revive();
            Group.Revive();
        }
        public override void Reset(float x, float y)
        {
            Revive();
            SetPosition(x, y);
            foreach (FlxSprite sprite in _sprites as List<FlxSprite>)
                if (sprite != null)
                    sprite.Reset(x, y);
        }

        public override void SetPosition(float x, float y)
        {
            base.SetPosition(x, y);
        }
        public override FlxSprite LoadGraphic(Texture2D texture, bool isAnimated = false, int frameWidth = 0, int frameHeight = 0)
        {
#if DEBUG
            throw new NotSupportedException();
#endif
            return this;
        }

        public override FlxSprite LoadGraphic(string graphicAssetPath, bool isAnimated = false, int frameWidth = 0, int frameHeight = 0)
        {
#if DEBUG
            throw new NotSupportedException();
#endif
            return this;
        }

        public override FlxSprite MakeGraphic(int width, int height, Color color)
        {
#if DEBUG
            throw new NotSupportedException();
#endif
            return this;
        }
        #endregion

        #region Public methods
        public T Add(T sprite, bool force =  false)
        {
            PreAdd(sprite);
            return Group.Add(sprite, force);
        }


        private void PreAdd(T sprite)
        {
            FlxSprite flxSprite = sprite as FlxSprite;
            flxSprite.X += X;
            flxSprite.Y += Y;
            flxSprite.Alpha *= Alpha;
            flxSprite.ScrollFactor = ScrollFactor;
            flxSprite.Cameras = Cameras;
        }

        public T Recycle(Type objectClass = default, ObjectFactory objectFactory = null, bool force = false)
        {
            if (objectFactory != null)
                throw new NotImplementedException();
            return Group.Recycle(objectClass, null, force);
        }

        public T Remove(T sprite)
        {
            return Group.Remove(sprite);
        }

        #endregion

        #region Utils
        private float GetWidth()
        {
            if (Length == 0)
                return 0;

            float minX = float.MaxValue;
            float maxX = float.MinValue;

            foreach (FlxSprite member in _sprites as List<FlxSprite>)
            {
                if (member == null)
                    continue;
                float minMemberX = member.X;
                float maxMemberX = minMemberX + member.Width;

                if (maxMemberX > maxX)
                    maxX = maxMemberX;
                if (minMemberX < minX)
                    minX = minMemberX;
            }
            return maxX - minX;
        }
        private float GetHeight()
        {
            if (Length == 0)
                return 0;

            float minY = float.MaxValue;
            float maxY = float.MinValue;

            foreach (FlxSprite member in _sprites as List<FlxSprite>)
            {
                if (member == null)
                    continue;
                float minMemberY = member.Y;
                float maxMemberY = minMemberY + member.Height;

                if (maxMemberY > maxY)
                    maxY = maxMemberY;
                if (minMemberY < minY)
                    minY = minMemberY;
            }
            return maxY - minY;
        }
        #endregion
    }
}
