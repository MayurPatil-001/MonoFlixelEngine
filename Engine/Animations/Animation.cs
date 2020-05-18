using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Engine.Animations
{
    public class Animation: FlxBasic
    {
        private List<AnimationFrame> frames = new List<AnimationFrame>();
        private TimeSpan timeIntoAnimation;

        public Animation()
        {
        }

        private TimeSpan Duration
        {
            get
            {
                double totalSeconds = 0;
                foreach (var frame in frames)
                    totalSeconds += frame.Duration.TotalSeconds;
                return TimeSpan.FromSeconds(totalSeconds);
            }
        }

        public bool Looped { get; set; }
        public bool Finished { get; private set; } = false;
        public bool Paused { get; private set; } = false;

        //TODO : Add FrameIndex Property?

        public AnimationFrame CurrentFrame
        {
            get
            {
                AnimationFrame currentFrame = null;

                TimeSpan accumulatedTime = new TimeSpan();
                foreach(var frame in frames)
                {
                    if(accumulatedTime + frame.Duration >= timeIntoAnimation)
                    {
                        currentFrame = frame;
                        break;
                    }
                    else
                    {
                        accumulatedTime += frame.Duration;
                    }
                }
                if (currentFrame == null)
                    currentFrame = frames[frames.Count - 1];
                return currentFrame;
            }
        }
        public int TotalFrames { get { return frames.Count; } }

        public Animation(bool looped = true)
        {
            Looped = looped;
        }

        public override void Update(GameTime gameTime)
        {
            if (Finished || Paused)
                return;

            timeIntoAnimation += gameTime.ElapsedGameTime;

            if (timeIntoAnimation >= Duration)
            {
                if (Looped)
                {
                    double secondsIntoAnimation = timeIntoAnimation.TotalSeconds;
                    double remainder = secondsIntoAnimation % Duration.TotalSeconds;
                    timeIntoAnimation = TimeSpan.FromSeconds(remainder);
                }
                else
                {
                    Finished = true;
                }
            }
        }

        public void AddFrame(Texture2D texture, Rectangle rectangle, TimeSpan duration)
        {
            AnimationFrame newFrame = new AnimationFrame(
                texture,
                rectangle,
                duration
            );

            frames.Add(newFrame);
        }

        public void Play(bool forceRestart = false)
        {
            if (forceRestart || Finished)
                timeIntoAnimation = TimeSpan.Zero;

            Paused = false;
            Finished = false;
        }

        public void Pause()
        {
            Paused = true;
        }

        public void Resume()
        {
            Paused = false;
        }

        public void Stop()
        {
            Finished = true;
            Paused = true;
        }

        public void Restart()
        {
            Reset();
        }

        public void Reset()
        {
            timeIntoAnimation = TimeSpan.Zero;
            Paused = false;
            Finished = false;
        }

        protected override void Dispose(bool disposing)
        {
            frames.Clear();
            frames = null;
            base.Dispose(disposing);
        }

        public override string ToString()
        {
            return $"frames {frames.Count} , Finished {Finished}, Paused {Paused}, Looped {Looped}";
        }
    }
}
