using Microsoft.Xna.Framework.Audio;
using System;

namespace Engine.UI.Button
{
    /// <summary>
    /// Helper function for `FlxButton` which handles its events.
    /// </summary>
    public class FlxButtonEvent : IDisposable
    {
        /// <summary>
        /// The callback function to call when this even fires.
        /// </summary>
        public Action Callback;
        /// <summary>
        /// The sound to play when this event fires.
        /// </summary>
        public SoundEffect Sound;

        /// <summary>
        /// </summary>
        /// <param name="callback">The callback function to call when this even fires.</param>
        /// <param name="sound">The sound to play when this event fires.</param>
        public FlxButtonEvent(Action callback = null, SoundEffect sound = null)
        {
            Callback = callback;
            Sound = sound;
        }

        public void Dispose()
        {
            Callback = null;
            Sound?.Dispose();
        }

        /// <summary>
        /// Fires this event (calls the callback and plays the sound)
        /// </summary>
        public void Fire()
        {
            Callback?.Invoke();

            if (Sound != null)
                Sound.Play();
        }
    }
}
