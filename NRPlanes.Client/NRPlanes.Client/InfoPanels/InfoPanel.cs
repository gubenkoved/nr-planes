using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NRPlanes.Client.Common;
using System.Linq;

namespace NRPlanes.Client.InfoPanels
{
    public class InfoPanel : DrawableGameComponent
    {
        public new PlanesGame Game
        {
            get { return base.Game as PlanesGame; }
        }

        private readonly GameWorldXna _gameWorldXna;

        public PlaneInfo PlaneInfoPanel { get; private set; }
        public Minimap MinimapPanel { get; private set; }
        private readonly List<InfoPanelItem> _infoPanelItems;

        private SpriteBatch _spriteBatch;

        //note: info panel position rectangle needed??
        public InfoPanel(PlanesGame game, GameWorldXna gameWorldXna) 
            : base(game)
        {
            _gameWorldXna = gameWorldXna;

            _infoPanelItems = new List<InfoPanelItem>();

            const int panelsWidth = 200;
            const int indent = 10;

            var minimapPosition = new Rectangle(game.Graphics.PreferredBackBufferWidth - panelsWidth - indent, indent, panelsWidth,
                                                (int)(panelsWidth / gameWorldXna.GameWorld.Size.Aspect));

            var planeInfoPosition = new Rectangle(game.Graphics.PreferredBackBufferWidth - panelsWidth - indent,
                                                  minimapPosition.Y + minimapPosition.Height + indent, panelsWidth,
                                                  game.Graphics.PreferredBackBufferHeight - 2 * indent -
                                                  minimapPosition.Y - minimapPosition.Height);

            MinimapPanel = new Minimap(game, minimapPosition, _gameWorldXna);
            PlaneInfoPanel = new PlaneInfo(game, planeInfoPosition);

            _infoPanelItems.Add(MinimapPanel);
            _infoPanelItems.Add(PlaneInfoPanel);
        }

        public override void Initialize()
        {
            _spriteBatch = new SpriteBatch(Game.Graphics.GraphicsDevice);

            foreach (var infoPanelItem in _infoPanelItems)
            {
                infoPanelItem.Initialize();
            }

            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            foreach (var infoPanelItem in _infoPanelItems)
            {
                infoPanelItem.Draw(gameTime, _spriteBatch);
            }

            _spriteBatch.End();
        }
    }
}