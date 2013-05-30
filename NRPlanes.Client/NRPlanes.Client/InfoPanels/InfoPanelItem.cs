using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NRPlanes.Client.InfoPanels
{
    public abstract class InfoPanelItem : DrawableGameComponent
    {
        public new PlanesGame Game
        {
            get { return base.Game as PlanesGame; }
        }

        public Rectangle PositionRectangle { get; private set; }

        protected InfoPanelItem(PlanesGame game, Rectangle positionRectangle) 
            : base(game)
        {
            PositionRectangle = positionRectangle;
        }

        public override sealed void Draw(GameTime gameTime)
        {
            throw new NotSupportedException("Use draw function with specified sprite batch");
        }

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}