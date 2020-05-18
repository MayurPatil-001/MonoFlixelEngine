using System;

namespace Engine.Utils
{
    public class FlxTimer : IDisposable
    {

        private float _timeCounter = 0;
        private int _loopsCounter = 0;

        public static FlxTimerManager GlobalManager;
        public FlxTimerManager Manager { get; private set; }
        public float Time = 0;
        public int Loops = 0;
        public bool Active = false;
        public bool Finished = false;
        public Action<FlxTimer> OnComplete;

        public float TimeLeft { get => Time - _timeCounter; }
        public float ElapsedTime { get => _timeCounter; }
        public float LoopsLeft { get => Loops - _loopsCounter; }
        public int ElapsedLoops { get => _loopsCounter; }
        public float Progress { get => (Time > 0) ? (_timeCounter / Time) : 0; }

        private bool _inManager = false;

        public FlxTimer(FlxTimerManager manager = null)
        {
            Manager = manager != null ? manager : GlobalManager;
        }


        public void Dispose()
        {
            OnComplete = null;
        }

        public FlxTimer Start(float time = 1, Action<FlxTimer> onComplete = null, int loops = 1)
        {
            if (Manager != null && !_inManager)
            {
                Manager.Add(this);
                _inManager = true;
            }

            Active = true;
            Finished = false;
            Time = Math.Abs(time);

            if (loops < 0)
                loops *= -1;
            Loops = loops;
            OnComplete = onComplete;
            _timeCounter = 0;
            _loopsCounter = 0;

            return this;
        }

        public FlxTimer Reset(float newTime = -1)
        {
            if (newTime < 0)

                newTime = Time;

            Start(newTime, OnComplete, Loops);
            return this;
        }

        public void Cancel()
        {
            Finished = true;
            Active = false;

            if (Manager != null && _inManager)
            {
                Manager.Remove(this);
                _inManager = false;
            }
        }
        public void Update(float elapsed)
        {
            _timeCounter += elapsed;

            while ((_timeCounter >= Time) && Active && !Finished)
            {
                _timeCounter -= Time;
                _loopsCounter++;

                if (Loops > 0 && (_loopsCounter >= Loops))
                {
                    Finished = true;
                }
            }
        }
        public void OnLoopFinished()
        {
            if (Finished)
                Cancel();
            OnComplete?.Invoke(this);
        }

        public override string ToString()
        {
            return $"Time : {Time} Loops: {Loops} Active : {Active} Finished : {Finished} TimeLeft : {TimeLeft} LoopsLeft : {LoopsLeft} Progress : {Progress}";
        }
    }
}
