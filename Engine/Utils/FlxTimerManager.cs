using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Engine.Utils
{
    public class FlxTimerManager : FlxBasic
    {
        private List<FlxTimer> _timers = new List<FlxTimer>();

        public FlxTimerManager()
        {
            Visible = false;
        }

        protected override void Dispose(bool disposing)
        {
            Clear();
            _timers = null;
            base.Dispose(disposing);
        }

        public override void Update(GameTime gameTime)
        {
            List<FlxTimer> loopedTimers = null;
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach (FlxTimer timer in _timers)
            {
                if (timer.Active && !timer.Finished && timer.Time >= 0)
                {
                    int timerLoops = timer.ElapsedLoops;
                    timer.Update(elapsed);

                    if (timerLoops != timer.ElapsedLoops)
                    {
                        if (loopedTimers == null)
                            loopedTimers = new List<FlxTimer>();

                        loopedTimers.Add(timer);
                    }
                }
            }

            if (loopedTimers != null)
            {
                while (loopedTimers.Count > 0)
                {
                    FlxTimer timer = loopedTimers[0];
                    loopedTimers.Remove(timer);
                    timer.OnLoopFinished();
                }
            }
            base.Update(gameTime);
        }

        public void Add(FlxTimer timer)
        {
            _timers.Add(timer);
        }

        public void Remove(FlxTimer timer)
        {
            _timers.Remove(timer);
        }

        public void CompleteAll()
        {
            List<FlxTimer> timersToFinish = new List<FlxTimer>();
            foreach (FlxTimer timer in _timers)
                if (timer.Loops > 0 && timer.Active)
                    timersToFinish.Add(timer);

            foreach (FlxTimer timer in timersToFinish)
            {
                while (!timer.Finished)
                {
                    timer.Update(timer.TimeLeft);
                    timer.OnLoopFinished();
                }
            }
        }

        public void Clear()
        {
            _timers.Clear();
        }

        public void ForEach(Action<FlxTimer> function)
        {
            foreach (FlxTimer timer in _timers)
                function(timer);
        }
    }
}
