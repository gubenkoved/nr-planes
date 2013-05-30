using System;
using Microsoft.Xna.Framework.Audio;

namespace NRPlanes.Client.Sound
{
    /// <summary>
    /// Provides smooth fade in/out sound play.
    /// </summary>
    public class FadeInOutSoundEffect : BasicSoundEffect, INeedToUpdate
    {
        private enum State
        {
            FadeIn,
            FadeOut,
            Play,
            Stop
        }
        private State _state;

        private TimeSpan _elapsedOnState;
        private float _startStateVolume;

        private readonly TimeSpan _fadeInTime;
        private readonly TimeSpan _fadeOutTime;

        public FadeInOutSoundEffect(SoundEffectInstance sound, TimeSpan fadeIn, TimeSpan fadeOut)
            :base(sound)
        {
            IsLooped = true;

            _fadeInTime = fadeIn;

            _fadeOutTime = fadeOut;

            _state = State.Stop;
        }

        public override void Play()
        {
            if (_state != State.FadeIn && _state != State.Play)
            {
                _state = State.FadeIn;

                _startStateVolume = Volume;

                base.Play();

                _elapsedOnState = TimeSpan.Zero;
            }
        }
        public override void Stop()
        {
            if (_state != State.FadeOut && _state != State.Stop)
            {
                _state = State.FadeOut;

                _startStateVolume = Volume;

                _elapsedOnState = TimeSpan.Zero;
            }
        }

        void INeedToUpdate.Update(TimeSpan elapsed)
        {
            _elapsedOnState += elapsed;

            var stateCompleteness = (float)Math.Min(1.0, _elapsedOnState.TotalSeconds / _fadeInTime.TotalSeconds);

            if (_state == State.FadeIn)
            {
                Volume = _startStateVolume + stateCompleteness * (1.0f - _startStateVolume);

                if (stateCompleteness == 1.0f)
                {
                    _state = State.Play;

                    _elapsedOnState = TimeSpan.Zero;
                }
            }

            if (_state == State.FadeOut)
            {
                Volume = (float)Math.Max(0.0, 1.0 - _elapsedOnState.TotalSeconds / _fadeOutTime.TotalSeconds);
                Volume = _startStateVolume - stateCompleteness * _startStateVolume;

                if (stateCompleteness == 1.0f)
                {
                    _state = State.Stop;

                    _elapsedOnState = TimeSpan.Zero;

                    Stop();
                }
            }
        }
    }
}