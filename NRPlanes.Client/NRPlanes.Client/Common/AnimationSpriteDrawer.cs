using System;
using NRPlanes.Core.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NRPlanes.Client.Common
{
    public class AnimationSpriteDrawer
    {
        private static Random _random = new Random();

        private Texture2D _texture;

        private TimeSpan _currentFrameElapsed;

        private bool movingBackward;
        private bool _reverseLoop;

        public Size FrameSize { get; private set; }

        public TimeSpan TimePerFrame { get; private set; }

        public int TotalFrames
        {
            get { return (int) (_texture.Width * _texture.Height / (FrameSize.Width * FrameSize.Height)); }
        }

        public int CurrentFrame { get; private set; }
        
        public AnimationSpriteDrawer(Texture2D texture, Size frameSize, TimeSpan timePerFrame, bool randomFirstFrame = true, bool reverseLoop = false)
        {
            _texture = texture;

            _reverseLoop = reverseLoop;

            FrameSize = frameSize;
            
            TimePerFrame = timePerFrame;

            if (randomFirstFrame)
            {
                CurrentFrame = _random.Next(TotalFrames);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 rotationOrigin, Vector2 scale, SpriteEffects effects, float layerDepth )
        {
            _currentFrameElapsed += gameTime.ElapsedGameTime;
            
            if (_currentFrameElapsed > TimePerFrame)
            {
                _currentFrameElapsed = TimeSpan.Zero;

                NextFrame();
            }

            int framesInRow = (int) (_texture.Width / FrameSize.Width);

            int row = CurrentFrame / framesInRow;
            int column = CurrentFrame % framesInRow;

            var sourceRect = new Rectangle((int) (column * FrameSize.Width),
                                           (int) (row * FrameSize.Height),
                                           (int) (FrameSize.Width),
                                           (int) (FrameSize.Height));

            spriteBatch.Draw(_texture,
                             position,
                             sourceRect,
                             color,
                             rotation,
                             rotationOrigin,
                             scale,
                             effects,
                             layerDepth);
        }

        private void NextFrame()
        {
            if (!_reverseLoop)
            {
                CurrentFrame = (CurrentFrame + 1) % TotalFrames;
            }
            else
            {
                if (CurrentFrame == TotalFrames - 1)
                    movingBackward = true;

                if (CurrentFrame == 0)
                    movingBackward = false;

                if (!movingBackward)
                    ++CurrentFrame;
                else
                    --CurrentFrame;
            }
        }
    }
}