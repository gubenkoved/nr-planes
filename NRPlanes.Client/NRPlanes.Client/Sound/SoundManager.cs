using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using NRPlanes.Core.Primitives;

namespace NRPlanes.Client.Sound
{
    /// <summary>
    /// Manages the playback of sounds (such as volume).    
    /// </summary>
    public class SoundManager
    {
        /// <summary>
        /// This PlanesGame reference used to automate sound effects creation process
        /// </summary>
        private readonly PlanesGame m_game;
        private readonly Func<Rect> m_getVisibleRectangleDelegate;

        /// <summary>
        /// Function delegate which takes visible rectangle and sound position and returns sound volume multiplier
        /// </summary>
        private Func<Rect, Vector, float> m_volumeEsimationFunction;

        public SoundManager(PlanesGame game, Func<Rect> getVisibleRectangleDelegate)
        {
            m_game = game;
            m_getVisibleRectangleDelegate = getVisibleRectangleDelegate;

            m_volumeEsimationFunction = (Rect visible, Vector position) =>
                {
                    double distanceToCenterOfVisibleRect = (visible.Center - position).Length;
                    double halfDiagonalLen = new Vector(visible.Width, visible.Height).Length / 2;

                    // in circle with radius halfDiagonalLen mult = 1
                    // when on every halfDiagonalLen alienation mult decreases in E times

                    return (float)Math.Exp(-Math.Max(0, distanceToCenterOfVisibleRect - halfDiagonalLen) / halfDiagonalLen);
                };
        }

        /// <summary>
        /// Weak references is used to allow GC to garbage collect unnecessary sound effects 
        /// </summary>
        private List<WeakReference> m_registeredSounds = new List<WeakReference>();

        /// <summary>
        /// Some of sound effects have to have GC root to prevent garbage collect because it have no other references.
        /// After playback sound will be removed from this collection
        /// </summary>
        private List<BasicSoundEffect> m_soundHardLinks = new List<BasicSoundEffect>();

        /// <summary>
        /// Only registered sounds will be controlled by Manager
        /// </summary>
        private void RegisterWeak(BasicSoundEffect effect)
        {
            m_registeredSounds.Add(new WeakReference(effect));

            AdjustEffect(effect);
        }

        /// <summary>
        /// Create GC root to prevent GC
        /// </summary>
        private void RegisterHard(BasicSoundEffect effect)
        {
            m_soundHardLinks.Add(effect);
        }

        private void AdjustEffect(BasicSoundEffect effect)
        {
            Rect visibleRectange = m_getVisibleRectangleDelegate();

            if (effect.Position.HasValue)
                effect.SoundVolumeMultiplier = m_volumeEsimationFunction(visibleRectange, effect.Position.Value);
        }

        public void Update(TimeSpan elapsed)
        {
            #region Process weak references
            for (int i = m_registeredSounds.Count - 1; i >= 0; i--)
            {
                if (m_registeredSounds[i].IsAlive)
                {
                    var effect = (BasicSoundEffect)m_registeredSounds[i].Target;

                    if (effect is INeedToUpdate)
                        ((INeedToUpdate)effect).Update(elapsed);

                    AdjustEffect(effect);
                }
                else
                {
                    m_registeredSounds.RemoveAt(i);
                }
            } 
            #endregion

            #region Process GC roots
            for (int i = m_soundHardLinks.Count - 1; i >= 0; i--)
            {
                if (m_soundHardLinks[i].IsStopped)
                    m_soundHardLinks.RemoveAt(i);
            } 
            #endregion
        }
        
        public BasicSoundEffect CreateBasicSoundEffect(string relativeSoundName, bool createGCRootToPreventGCWhilePlay = true)
        {
            string path = string.Format("Sounds/{0}", relativeSoundName);

            var effect = new BasicSoundEffect(m_game.Content.Load<SoundEffect>(path).CreateInstance());

            RegisterWeak(effect);

            if (createGCRootToPreventGCWhilePlay)
            {
                RegisterHard(effect);
            }

            return effect;
        }
        public FadeInOutSoundEffect CreateFadeInOutSoundEffect(string relativeSoundName, TimeSpan fadeIn, TimeSpan fadeOut)
        {
            string path = string.Format("Sounds/{0}", relativeSoundName);

            var effect = new FadeInOutSoundEffect(m_game.Content.Load<SoundEffect>(path).CreateInstance(), fadeIn, fadeOut);

            RegisterWeak(effect);

            return effect;
        }
    }
}
