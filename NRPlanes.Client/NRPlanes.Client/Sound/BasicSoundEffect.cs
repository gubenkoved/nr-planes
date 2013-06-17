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
        private readonly SoundEffectInstance m_sound;

        /// <summary>
        /// Real sound source position. 
        /// Used by SoundManager to adjust sound playback
        /// </summary>
        public Vector? Position { get; set; }

        private float m_soundVolumeMultiplier;
        /// <summary>
        /// Usually adjusted by SoundManager.
        /// Affects on result sound volume
        /// </summary>
        public float SoundVolumeMultiplier 
        {
            get
            {
                return m_soundVolumeMultiplier;
            }
            set
            {
                m_soundVolumeMultiplier = value;

                UpdateRealVolume();
            }
        }

        private float m_volume;
        /// <summary>
        /// Value is [0; 1] which will be multiplied on SoundVolumeMultiplier to get result volume value
        /// </summary>
        public float Volume 
        {
            get
            {
                return m_volume;
            }
            set
            {
                m_volume = value;

                UpdateRealVolume();
            }
        }

        public bool IsLooped 
        {
            get
            {
                return m_sound.IsLooped;
            }
            set
            {
                m_sound.IsLooped = value;
            }
        }
        public bool IsStopped
        {
            get
            {
                return m_sound.State == SoundState.Stopped;
            }
        }

        public BasicSoundEffect(SoundEffectInstance sound)
        {
            m_sound = sound;

            m_soundVolumeMultiplier = 1.0f;
            m_volume = 1.0f;
        }

        public virtual void Play()
        {
            UpdateRealVolume();

            m_sound.Play();
        }
        public virtual void Stop()
        {            
            m_sound.Stop();
        }

        private void UpdateRealVolume()
        {
            m_sound.Volume = Volume * SoundVolumeMultiplier;
        }
    }
}
