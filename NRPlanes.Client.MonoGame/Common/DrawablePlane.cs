using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NRPlanes.Client.InfoPanels;
using NRPlanes.Core.Common;
using NRPlanes.Core.Primitives;
using Plane = NRPlanes.Core.Common.Plane;

namespace NRPlanes.Client.Common
{
    public abstract class DrawablePlane : DrawableGameObject, IOnMinimapDrawable
    {
        private Texture2D m_minimapTexture;

        public new Plane GameObject { get { return base.GameObject as Plane; } }

        protected DrawablePlane(PlanesGame game, Plane plane, CoordinatesTransformer coordinatesTransformer)
            : base(game, plane, coordinatesTransformer)
        {            
        }

        public void DrawOnMinimap(GameTime gameTime, SpriteBatch minimapSpriteBatch, CoordinatesTransformer coordinatesTransformer)
        {
            if (m_minimapTexture == null)
                m_minimapTexture = Game.Content.Load<Texture2D>("Minimap/arrow");

            var origin = new Vector2(m_minimapTexture.Width / 2.0f, m_minimapTexture.Height / 2.0f);

            minimapSpriteBatch.Draw(m_minimapTexture,
                                    coordinatesTransformer.Transform(GameObject.Position),
                                    null,
                                    Color.White,
                                    (float)Helper.ToRadians(GameObject.Rotation),
                                    origin,
                                    1.0f,
                                    SpriteEffects.None,
                                    0.0f);
        }
    }
}