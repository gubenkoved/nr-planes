using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NRPlanes.Core.Common;
using System;
using NRPlanes.Core.Equipments;

namespace NRPlanes.Client.InfoPanels
{
    public class IndicatorsDrawer
    {
        private readonly Texture2D m_backgroundTexture;
        private readonly Texture2D m_valueTexture;
        private readonly Texture2D m_lowChargeTexture;
        private readonly SpriteFont m_font;

        private int m_drawCount;

        public IndicatorsDrawer(Texture2D backgroundTexture, Texture2D valueTexture, Texture2D lowChargeTexture, SpriteFont font)
        {
            m_backgroundTexture = backgroundTexture;
            m_valueTexture = valueTexture;
            m_lowChargeTexture = lowChargeTexture;
            m_font = font;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle position, Color backgroundColor, Color color, Color textColor, PlaneEquipment equipment)
        {
            ++m_drawCount;

            Draw(
                spriteBatch,
                position,
                equipment.Charge / equipment.MaximumCharge,
                backgroundColor,
                color,
                equipment.Info,
                textColor);

            if (equipment.IsLowCharge)
            {
                const float lowChargeMarkIndention = 4;
                float lowChargreMarkHeight = position.Height - 2 * lowChargeMarkIndention;
                float scale = lowChargreMarkHeight / m_lowChargeTexture.Height;                

                Vector2 center = new Vector2(m_lowChargeTexture.Width / 2.0f, m_lowChargeTexture.Height / 2.0f);

                spriteBatch.Draw(
                    m_lowChargeTexture,
                    new Vector2(lowChargeMarkIndention + position.X + center.X * scale, lowChargeMarkIndention + position.Y + +center.Y * scale),
                    null,
                    Color.FromNonPremultiplied(255, 255, 255, 200),
                    0,
                    center,
                    scale * (float)(1f + Math.Sin(m_drawCount / 50.0) / 10f),
                    SpriteEffects.None,
                    0);

            }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle position, double normalizedValue, Color backgroundColor, Color color, string text, Color textColor)
        {
            spriteBatch.Draw(m_backgroundTexture,
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