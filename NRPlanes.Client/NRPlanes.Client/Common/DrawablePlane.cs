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
        private Texture2D _minimapImage;

        public new Plane GameObject { get { return base.GameObject as Plane; } }

        protected DrawablePlane(PlanesGame game, Plane plane, CoordinatesTransformer coordinatesTransformer)
            : base(game, plane, coordinatesTransformer)
        {            
        }

        public void DrawOnMinimap(GameTime gameTime, SpriteBatch minimapSpriteBatch, CoordinatesTransformer coordinatesTransformer)
        {
            if (_minimapImage == null)
                _minimapImage = Game.Content.Load<Texture2D>("Minimap/arrow");

            var origin = new Vector2(_minimapImage.Width / 2.0f, _minimapImage.Height / 2.0f);

            minimapSpriteBatch.Draw(_minimapImage,
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