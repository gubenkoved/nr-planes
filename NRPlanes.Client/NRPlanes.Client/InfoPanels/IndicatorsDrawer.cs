using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NRPlanes.Client.InfoPanels
{
    public class IndicatorsDrawer
    {
        private readonly Texture2D _background;

        private readonly Texture2D _valueTexture;

        private readonly SpriteFont _font;

        public IndicatorsDrawer(Texture2D background, Texture2D valueTexture, SpriteFont font = null)
        {
            _background = background;

            _valueTexture = valueTexture;

            _font = font;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle position, double normalizedValue, Color backgroundColor, Color color, string text, Color textColor)
        {
            spriteBatch.Draw(_background,
                             position,
                             backgroundColor);

            var valueRectangle = new Rectangle(position.X, position.Y, (int)(position.Width * normalizedValue), position.Height);

            var sourceRectangle = new Rectangle(0, 0, (int) (_valueTexture.Width * normalizedValue), _valueTexture.Height);
            
            spriteBatch.Draw(_valueTexture,
                             valueRectangle,
                             sourceRectangle,
                             color);

            var textSize = _font.MeasureString(text);

            spriteBatch.DrawString(_font,
                                   text,
                                   new Vector2((int)(position.X + (position.Width - textSize.X) / 2.0f),
                                               (int)(position.Y + (position.Height - textSize.Y) / 2.0f)),
                                   textColor);
        }
    }
}