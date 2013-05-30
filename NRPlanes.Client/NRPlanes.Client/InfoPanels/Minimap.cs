using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NRPlanes.Client.Common;

namespace NRPlanes.Client.InfoPanels
{
    public class Minimap : InfoPanelItem
    {
        private readonly GameWorldXna _gameWorldXna;

        private readonly CoordinatesTransformer _coordinatesTransformer;

        private Texture2D _background;
        
        private Texture2D _visibleAreaTexture;

        public Minimap(PlanesGame game, Rectangle positionRecangle, GameWorldXna gameWorldXna)
            : base(game, positionRecangle)
        {
            _gameWorldXna = gameWorldXna;

            _coordinatesTransformer = new CoordinatesTransformer(gameWorldXna.GameWorld.Size, positionRecangle);
        }

        public override void Initialize()
        {
            _background = Game.Content.Load<Texture2D>("Minimap/minimap_background");

            _visibleAreaTexture = Game.Content.Load<Texture2D>("Minimap/visible_area");

            base.Initialize();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_background,
                             _coordinatesTransformer.PhysicalRectangle,
                             Color.White);

            spriteBatch.Draw(_visibleAreaTexture,
                             _coordinatesTransformer.Transform(
                                 _gameWorldXna.CoordinatesTransformer.VisibleLogicalRectangle),
                             Color.White);

            foreach (var drawableObject in _gameWorldXna.DrawableGameComponents)
            {
                if (drawableObject is IOnMinimapDrawable)
                {
                    var minimapVisivbleObject = drawableObject as IOnMinimapDrawable;

                    minimapVisivbleObject.DrawOnMinimap(gameTime, spriteBatch, _coordinatesTransformer);
                }
            }
        }
    }
}