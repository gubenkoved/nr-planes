using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NRPlanes.Client.Common;

namespace NRPlanes.Client.InfoPanels
{
    public class Minimap : InfoPanelItem
    {
        private readonly GameWorldXna m_gameWorldXna;
        private readonly CoordinatesTransformer m_coordinatesTransformer;
        private Texture2D m_background;
        private Texture2D m_visibleAreaTexture;

        public Minimap(PlanesGame game, Rectangle positionRecangle, GameWorldXna gameWorldXna)
            : base(game, positionRecangle)
        {
            m_gameWorldXna = gameWorldXna;

            m_coordinatesTransformer = new CoordinatesTransformer(gameWorldXna.GameWorld.Size, positionRecangle);
        }

        public override void Initialize()
        {
            m_background = Game.Content.Load<Texture2D>("Minimap/minimap_background");

            m_visibleAreaTexture = Game.Content.Load<Texture2D>("Minimap/visible_area");

            base.Initialize();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(m_background,
                             m_coordinatesTransformer.PhysicalRectangle,
                             Color.White);

            spriteBatch.Draw(m_visibleAreaTexture,
                             m_coordinatesTransformer.Transform(
                                 m_gameWorldXna.CoordinatesTransformer.VisibleLogicalRectangle),
                             Color.White);

            using (var handle = m_gameWorldXna.DrawableGameComponentsSafeReadHandle)
            {
                foreach (var drawableObject in handle.Items)
                {
                    if (drawableObject is IOnMinimapDrawable)
                    {
                        var minimapVisivbleObject = drawableObject as IOnMinimapDrawable;

                        minimapVisivbleObject.DrawOnMinimap(gameTime, spriteBatch, m_coordinatesTransformer);
                    }
                }
            }
        }
    }
}