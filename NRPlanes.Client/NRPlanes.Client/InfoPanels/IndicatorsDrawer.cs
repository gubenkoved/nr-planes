using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NRPlanes.Client.InfoPanels
{
    public class IndicatorsDrawer
    {
        private readonly Texture2D m_background;
        private readonly Texture2D m_valueTexture;
        private readonly SpriteFont m_font;

        public IndicatorsDrawer(Texture2D background, Texture2D valueTexture, SpriteFont font = null)
        {
            m_background = background;
            m_valueTexture = valueTexture;
            m_font = font;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle position, double normalizedValue, Color backgroundColor, Color color, string text, Color textColor)
        {
            spriteBatch.Draw(m_background,
                             position,
                             backgroundColor);

            Rectangle valueRectangle = new Rectangle(position.X, position.Y, (int)(position.Width * normalizedValue), position.Height);
            Rectangle sourceRectangle = new Rectangle(0, 0, (int)(m_valueTexture.Width * normalizedValue), m_valueTexture.Height);
            
            spriteBatch.Draw(m_valueTexture,
                             valueRectangle,
                             sourceRectangle,
                             color);

            Vector2 textSize = m_font.MeasureString(text);

            spriteBatch.DrawString(m_font,
                                   text,
                                   new Vector2((int)(position.X + (position.Width - textSize.X) / 2.0f),
                                               (int)(position.Y + (position.Height - textSize.Y) / 2.0f)),
                                   textColor);
        }
    }
}