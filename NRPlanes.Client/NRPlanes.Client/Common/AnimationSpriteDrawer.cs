using System;
using NRPlanes.Core.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NRPlanes.Core.Common;

namespace NRPlanes.Client.Common
{
    public class AnimationSpriteDrawer
    {
        private Texture2D m_texture;
        private TimeSpan m_currentFrameElapsed;
        private bool m_movingBackward;
        private bool m_reverseLoop;

        public Size FrameSize { get; private set; }
        public TimeSpan TimePerFrame { get; private set; }
        public int TotalFrames
        {
            get { return (int) (m_texture.Width * m_texture.Height / (FrameSize.Width * FrameSize.Height)); }
        }
        public int CurrentFrame { get; private set; }
        
        public AnimationSpriteDrawer(Texture2D texture, Size frameSize, TimeSpan timePerFrame, bool randomFirstFrame = true, bool reverseLoop = false)
        {
            m_texture = texture;
            m_reverseLoop = reverseLoop;
            
            FrameSize = frameSize;
            TimePerFrame = timePerFrame;

            if (randomFirstFrame)
            {
                CurrentFrame = RandomProvider.Next() % TotalFrames;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 rotationOrigin, Vector2 scale, SpriteEffects effects, float layerDepth )
        {
            m_currentFrameElapsed += gameTime.ElapsedGameTime;
            
            if (m_currentFrameElapsed > TimePerFrame)
            {
                m_currentFrameElapsed = TimeSpan.Zero;

                NextFrame();
            }

            int framesInRow = (int) (m_texture.Width / FrameSize.Width);

            int row = CurrentFrame / framesInRow;
            int column = CurrentFrame % framesInRow;

            var sourceRect = new Rectangle((int) (column * FrameSize.Width),
                                           (int) (row * FrameSize.Height),
                                           (int) (FrameSize.Width),
                                           (int) (FrameSize.Height));

            spriteBatch.Draw(m_texture,
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
            if (!m_reverseLoop)
            {
                CurrentFrame = (CurrentFrame + 1) % TotalFrames;
            }
            else
            {
                if (CurrentFrame == TotalFrames - 1)
                    m_movingBackward = true;

                if (CurrentFrame == 0)
                    m_movingBackward = false;

                if (!m_movingBackward)
                    ++CurrentFrame;
                else
                    --CurrentFrame;
            }
        }
    }
}