using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Client.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NRPlanes.Client.InfoPanels;
using NRPlanes.Core.Bonuses;
using NRPlanes.Core.Primitives;

namespace NRPlanes.Client.GameComponents
{
    public class BonusXna : DrawableGameObject, IOnMinimapDrawable
    {
        private Texture2D m_minimapTexture;
        protected Texture2D m_texture;

        public Color Color { get; private set; }
        public Bonus Bonus { get; private set; }

        public BonusXna(PlanesGame game, Bonus bonus, CoordinatesTransformer coordinatesTransformer, Color color, Texture2D texture)
            :base(game, bonus, coordinatesTransformer)
        {
            Bonus = bonus;
            Color = color;

            m_texture = texture;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(m_texture.Width / 2.0f, m_texture.Height / 2.0f);
            Vector2 scaleVector = CoordinatesTransformer.CreateScaleVector(Bonus.RelativeGeometry.BoundingRectangle.Size, new Size(m_texture.Width, m_texture.Height));

            spriteBatch.Draw(
                m_texture,
                CoordinatesTransformer.Transform(Bonus.Position),
                null,
                Color,
                (float)Bonus.Rotation,
                origin,
                scaleVector,
                SpriteEffects.None,
                LayersDepths.Bonuses);
        }

        public void DrawOnMinimap(GameTime gameTime, SpriteBatch minimapSpriteBatch, CoordinatesTransformer coordinatesTransformer)
        {
            if (m_minimapTexture == null)
                m_minimapTexture = Game.Content.Load<Texture2D>("Minimap/point");

            var origin = new Vector2(m_minimapTexture.Width / 2.0f, m_minimapTexture.Height / 2.0f);

            minimapSpriteBatch.Draw(m_minimapTexture,
                                    coordinatesTransformer.Transform(GameObject.Position),
                                    null,
                                    Color,
                                    (float)Helper.ToRadians(GameObject.Rotation),
                                    origin,
                                    1.0f,
                                    SpriteEffects.None,
                                    1.0f);
        }
    }
}
