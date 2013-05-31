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

        private readonly GameWorldXna m_gameWorldXna;

        public PlaneInfo PlaneInfoPanel { get; private set; }
        public Minimap MinimapPanel { get; private set; }
        private readonly List<InfoPanelItem> m_infoPanelItems;

        private SpriteBatch m_spriteBatch;

        //note: info panel position rectangle needed??
        public InfoPanel(PlanesGame game, GameWorldXna gameWorldXna) 
            : base(game)
        {
            m_gameWorldXna = gameWorldXna;

            m_infoPanelItems = new List<InfoPanelItem>();

            const int panelsWidth = 200;
            const int indent = 10;

            var minimapPosition = new Rectangle(game.Graphics.PreferredBackBufferWidth - panelsWidth - indent, indent, panelsWidth,
                                                (int)(panelsWidth / gameWorldXna.GameWorld.Size.Aspect));

            var planeInfoPosition = new Rectangle(game.Graphics.PreferredBackBufferWidth - panelsWidth - indent,
                                                  minimapPosition.Y + minimapPosition.Height + indent, panelsWidth,
                                                  game.Graphics.PreferredBackBufferHeight - 2 * indent -
                                                  minimapPosition.Y - minimapPosition.Height);

            MinimapPanel = new Minimap(game, minimapPosition, m_gameWorldXna);
            PlaneInfoPanel = new PlaneInfo(game, planeInfoPosition);

            m_infoPanelItems.Add(MinimapPanel);
            m_infoPanelItems.Add(PlaneInfoPanel);
        }

        public override void Initialize()
        {
            m_spriteBatch = new SpriteBatch(Game.Graphics.GraphicsDevice);

            foreach (var infoPanelItem in m_infoPanelItems)
            {
                infoPanelItem.Initialize();
            }

            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            m_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            foreach (var infoPanelItem in m_infoPanelItems)
            {
                infoPanelItem.Draw(gameTime, m_spriteBatch);
            }

            m_spriteBatch.End();
        }
    }
}