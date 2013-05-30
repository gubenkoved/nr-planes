using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using NRPlanes.Core.Primitives;

namespace NRPlanes.Client.Sound
{
    public class BasicSoundEffect
    {
        private readonly SoundEffectInstance _sound;

        /// <summary>
        /// Real sound source position. 
        /// Used by SoundManager to adjust sound playback
        /// </summary>
        public Vector? Position { get; set; }

        private float _soundVolumeMultiplier;
        /// <summary>
        /// Usually adjusted by SoundManager.
        /// Affects on result sound volume
        /// </summary>
        public float SoundVolumeMultiplier 
        {
            get
            {
                return _soundVolumeMultiplier;
            }
            set
            {
                _soundVolumeMultiplier = value;

                UpdateRealVolume();
            }
        }

        private float _volume;
        /// <summary>
        /// Value is [0; 1] which will be multiplied on SoundVolumeMultiplier to get result volume value
        /// </summary>
        public float Volume 
        {
            get
            {
                return _volume;
            }
            set
            {
                _volume = value;

                UpdateRealVolume();
            }
        }

        public bool IsLooped 
        {
            get
            {
                return _sound.IsLooped;
            }
            set
            {
                _sound.IsLooped = value;
            }
        }
        public bool IsStopped
        {
            get
            {
                return _sound.State == SoundState.Stopped;
            }
        }

        public BasicSoundEffect(SoundEffectInstance sound)
        {
            _sound = sound;

            _soundVolumeMultiplier = 1.0f;
            _volume = 1.0f;
        }

        public virtual void Play()
        {
            UpdateRealVolume();

            _sound.Play();
        }
        public virtual void Stop()
        {            
            _sound.Stop();
        }

        private void UpdateRealVolume()
        {
            _sound.Volume = Volume * SoundVolumeMultiplier;
        }
    }
}
