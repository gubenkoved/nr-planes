using System;
using Microsoft.Xna.Framework;
using NRPlanes.Core.Common;
using NRPlanes.Core.Planes;
using NRPlanes.Core.Primitives;
using NRPlanes.Client.InfoPanels;
using NRPlanes.ServerData.OperationResults;
using NRPlanes.Core.Controllers;
using NRPlanes.Core.Common.Client;
using NRPlanes.Client.MonoGame.ServiceReference;

namespace NRPlanes.Client.Common
{
    public class GameManager
    {
        private readonly PlanesGame m_game;
        private readonly GameServiceClient m_client;

        private ClientGameWorld m_gameWorld;
        public ClientGameWorld GameWorld
        {
            get
            {
                return m_gameWorld;
            }
        }

        private GameWorldXna m_gameWorldXna;
        public GameWorldXna GameWorldXna
        {
            get
            {
                return m_gameWorldXna;
            }
        }

        private InfoPanel m_infoPanel;

        private NRPlanes.Core.Common.Plane m_ownPlane;        
        private Guid m_ownGuid;

        private ObjectsSynchronizer m_synchronizer;

        public GameManager(PlanesGame game)
        {
            m_game = game;
            
            m_client = new GameServiceClient();
            m_client.Open();
        }

        public void Initialize()
        {
            // getting game world from server
            JoinResult initInfo = m_client.Join();
            
            m_ownGuid = initInfo.PlayerGuid; // assigned by server unique own identifier to identify player
            m_gameWorld = new ClientGameWorld(initInfo.LogicalSize);

            foreach (var staticObject in initInfo.StaticObjects)
            {
                m_gameWorld.AddStaticObject(staticObject);
            }
            
            m_gameWorldXna = new GameWorldXna(m_game, m_gameWorld, new Rectangle(0, 0, m_game.Graphics.PreferredBackBufferWidth, m_game.Graphics.PreferredBackBufferHeight));
            m_gameWorldXna.Initialize();

            m_infoPanel = new InfoPanel(m_game, m_gameWorldXna);
            
            #region Create plane, controller and set camera
            m_ownPlane = XWingPlane.BasicConfiguration(new Vector(m_gameWorld.Size.Width / 2.0, m_gameWorld.Size.Height / 2.0));
            m_ownPlane.PlayerGuid = m_ownGuid;

            m_gameWorld.AddPlaneController(new LocalPlaneController(m_ownPlane));

            m_gameWorldXna.CenterOfViewGameObject = m_ownPlane;
            m_gameWorldXna.ForceSetCameraOnCenterOfView();
            #endregion

            m_synchronizer = new ObjectsSynchronizer(m_client, m_gameWorld, m_ownGuid, m_ownPlane);

            m_gameWorld.AddGameObject(m_ownPlane); // Add after subscribing of GameWorld events

            m_infoPanel.PlaneInfoPanel.Plane = m_ownPlane;
            m_infoPanel.Initialize();
        }

        public void Update(GameTime gameTime)
        {
            // ONLY FOR DEBUG
            // TIME multiplier
            //TimeSpan elapsed = TimeSpan.FromSeconds(0.10 * gameTime.ElapsedGameTime.TotalSeconds);

            TimeSpan elapsed = gameTime.ElapsedGameTime;
            
            m_gameWorld.Update(elapsed);            
            m_infoPanel.Update(gameTime);
            m_gameWorldXna.Update(gameTime);
            m_synchronizer.Update();
        }

        public void Draw(GameTime gameTime)
        {
            m_gameWorldXna.Draw(gameTime);

            m_infoPanel.Draw(gameTime);
        }
    }
}