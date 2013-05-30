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
    /// This class designed as singletone to grant access from any part of client and there is no need to have multiply instance
    /// </summary>
    public class SoundManager
    {
        public static SoundManager Instance {get; private set;}

        public static SoundManager CreateInstance(PlanesGame game, Func<Rect> getVisibleRectangleDelegate)
        {
            Instance = new SoundManager(game, getVisibleRectangleDelegate);

            return Instance;
        }

        /// <summary>
        /// This PlanesGame reference used to automate sound effects creation process
        /// </summary>
        private readonly PlanesGame _game;

        private readonly Func<Rect> _getVisibleRectangleDelegate;

        /// <summary>
        /// Function delegate which takes visible rectangle and sound position and returns sound volume multiplier
        /// </summary>
        private Func<Rect, Vector, float> _volumeEsimationFunction;

        private SoundManager(PlanesGame game, Func<Rect> getVisibleRectangleDelegate)
        {
            _game = game;
            _getVisibleRectangleDelegate = getVisibleRectangleDelegate;

            _volumeEsimationFunction = (Rect visible, Vector position) =>
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
        private List<WeakReference> _registeredSounds = new List<WeakReference>();

        /// <summary>
        /// Some of sound effects have to have GC root to prevent garbage collect because it have no other references.
        /// After playback sound will be removed from this collection
        /// </summary>
        private List<BasicSoundEffect> _soundHardLinks = new List<BasicSoundEffect>();

        /// <summary>
        /// Only registered sounds will be controlled by Manager
        /// </summary>
        private void RegisterWeak(BasicSoundEffect effect)
        {
            _registeredSounds.Add(new WeakReference(effect));

            AdjustEffect(effect);
        }

        /// <summary>
        /// Create GC root to prevent GC
        /// </summary>
        private void RegisterHard(BasicSoundEffect effect)
        {
            _soundHardLinks.Add(effect);
        }

        private void AdjustEffect(BasicSoundEffect effect)
        {
            Rect visibleRectange = _getVisibleRectangleDelegate();

            if (effect.Position.HasValue)
                effect.SoundVolumeMultiplier = _volumeEsimationFunction(visibleRectange, effect.Position.Value);
        }

        public void Update(TimeSpan elapsed)
        {
            #region Process weak references
            for (int i = _registeredSounds.Count - 1; i >= 0; i--)
            {
                if (_registeredSounds[i].IsAlive)
                {
                    var effect = (BasicSoundEffect)_registeredSounds[i].Target;

                    if (effect is INeedToUpdate)
                        ((INeedToUpdate)effect).Update(elapsed);

                    AdjustEffect(effect);
                }
                else
                {
                    _registeredSounds.RemoveAt(i);
                }
            } 
            #endregion

            #region Process GC roots
            for (int i = _soundHardLinks.Count - 1; i >= 0; i--)
            {
                if (_soundHardLinks[i].IsStopped)
                    _soundHardLinks.RemoveAt(i);
            } 
            #endregion
        }
        
        public BasicSoundEffect CreateBasicSoundEffect(string relativeSoundName, bool createGCRootToPreventGCWhilePlay = true)
        {
            string path = string.Format("Sounds/{0}", relativeSoundName);

            var effect = new BasicSoundEffect(_game.Content.Load<SoundEffect>(path).CreateInstance());

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

            var effect = new FadeInOutSoundEffect(_game.Content.Load<SoundEffect>(path).CreateInstance(), fadeIn, fadeOut);

            RegisterWeak(effect);

            return effect;
        }
    }
}
