using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Animations
{
    public class AnimationController : CoreGameComponent
    {
        Texture2D _texture;
        int _rows;
        int _columns;
        Rectangle[] _rects;

        private Dictionary<string, Animation> _animations = new Dictionary<string, Animation>();
        private Animation _currentAnimation = null;
        private Animation _defaultAnimation = null;
        private int _frameIndex = 0;
        public int FrameIndex
        {
            get { return _frameIndex; }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > _rects.Length - 1)
                    value = _rects.Length - 1;
                _frameIndex = value;
                if (_defaultAnimation != null)
                    _defaultAnimation.CurrentFrame.SourceRectangle = _rects[_frameIndex];
            }
        }
        public Animation CurrentAnimation
        {
            get
            {
                if (_currentAnimation == null)
                {
                    if (_defaultAnimation == null && _rects.Length > 0)
                    {
                        _defaultAnimation = new Animation(false);
                        _defaultAnimation.AddFrame(_texture, _rects[FrameIndex], TimeSpan.Zero);
                    }
                    return _defaultAnimation;
                }
                else
                    return _currentAnimation;
            }
            private set { _currentAnimation = value; }
        }

        /// <summary>
        /// Total Usable Frames Count of All Animations
        /// </summary>
        public int AllAvailableFrames => _rects.Length;
        /// <summary>
        /// Current Annimations Total Frames
        /// </summary>
        public int TotalFrames 
        {
            get
            {
                if (CurrentAnimation == null)
                    return _rects.Length;
                else
                    return CurrentAnimation.TotalFrames;
            }
        }

        public AnimationController(Texture2D texture, int frameWidth = 0, int frameHeight = 0)
        {
            _texture = texture;
            _columns = _texture.Width / frameWidth;
            _rows = _texture.Height / frameHeight;

            _rects = new Rectangle[_rows * _columns];
            int index = 0;
            for(int y = 0; y < _rows; y++)
            {
                for(int x= 0; x < _columns; x++)
                {
                    _rects[index] = new Rectangle(x * frameWidth, y * frameHeight, frameWidth, frameHeight);
                    index++;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (CurrentAnimation != null)
                CurrentAnimation.Update(gameTime);
        }


        public void Add(string name, int[] frames, int frameRate = 30, bool looped = true)
        {
            Animation newAnimation = new Animation(looped);
            TimeSpan perFrameDuration = frameRate == 0? TimeSpan.Zero : TimeSpan.FromSeconds(1f / (frameRate / frames.Length));
            foreach(int frame in frames)
                newAnimation.AddFrame(_texture, _rects[frame], perFrameDuration);
            _animations.Add(name, newAnimation);
        }

        public void Play(string name, bool forceRestart = false)
        {
            if(name == null || !_animations.ContainsKey(name))
            {
                //TODO: Logger
                return;
            }

            Animation newAnimation = _animations[name];
            if (CurrentAnimation != newAnimation)
                newAnimation.Reset();

            CurrentAnimation = newAnimation;
            CurrentAnimation.Play(forceRestart);
        }

        public void Pause()
        {
            CurrentAnimation.Pause();
        }

        public void Resume()
        {
            CurrentAnimation.Resume();
        }

        public void Stop()
        {
            CurrentAnimation.Stop();
        }

        public void Destroy()
        {
            foreach (Animation animation in _animations.Values)
                animation.Dispose();
            _animations.Clear();
            _animations = null;
            Array.Clear(_rects, 0, _rects.Length);
            _rects = null;
            _texture = null;
        }
    }
}
