using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NRPlanes.Client.Common
{
    public abstract class MyDrawableGameComponent : DrawableGameComponent
    {
        public new PlanesGame Game
        {
            get { return base.Game as PlanesGame; }
        }

        public CoordinatesTransformer CoordinatesTransformer { get; private set; }

        public bool IsGarbage { get; protected set; }

        public override sealed void Draw(GameTime gameTime)
        {
            throw new NotSupportedException("This operation are NOT supported");
        }

        protected MyDrawableGameComponent(PlanesGame game, CoordinatesTransformer coordinatesTransformer)
            : base(game)
        {
            CoordinatesTransformer = coordinatesTransformer;
        }

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}