using System;
using Microsoft.Xna.Framework;
using NRPlanes.Core.Common;
using NRPlanes.Core.Planes;
using NRPlanes.Core.Primitives;
using NRPlanes.Client.InfoPanels;
using NRPlanes.Client.NRPlanesServerReference;
using NRPlanes.ServerData.OperationResults;
using NRPlanes.Core.Controllers;

namespace NRPlanes.Client.Common
{
    public class GameManager
    {
        private readonly PlanesGame _game;
        private readonly GameServiceClient _client;
        
        private GameWorld _gameWorld;
        private GameWorldXna _gameWorldXna;
        private InfoPanel _infoPanel;

        private NRPlanes.Core.Common.Plane _ownPlane;        
        private Guid _ownGuid;

        private ObjectsSynchronizer _synchronizer;

        public GameManager(PlanesGame game)
        {
            _game = game;
            
            _client = new GameServiceClient();
            _client.Open();
        }

        public void Initialize()
        {
            // getting game world from server
            JoinResult initInfo = _client.Join();
            
            _ownGuid = initInfo.PlayerGuid; // assigned by server unique own indetifier to identificate player
            _gameWorld = new GameWorld(initInfo.LogicalSize, initInfo.StaticObjects);
            
            _gameWorldXna = new GameWorldXna(_game, _gameWorld, new Rectangle(0, 0, _game.Graphics.PreferredBackBufferWidth, _game.Graphics.PreferredBackBufferHeight));
            _infoPanel = new InfoPanel(_game, _gameWorldXna);
            
            #region Create plane, controller and set camera
            _ownPlane = XWingPlane.BasicConfiguration(new Vector(_gameWorld.Size.Width / 2.0, _gameWorld.Size.Height / 2.0));

            _gameWorld.AddPlaneController(
                new LocalPlaneController(_ownPlane));

            _gameWorldXna.CenterOfView = _ownPlane;
            _gameWorldXna.ForceSetCameraOnCenterOfView();
            #endregion

            _synchronizer = new ObjectsSynchronizer(_client, _gameWorld, _ownGuid, _ownPlane);

            _gameWorld.AddGameObject(_ownPlane); // Add after subscribing of GameWorld events

            _infoPanel.Initialize();
            _infoPanel.PlaneInfoPanel.Plane = _ownPlane;

            _gameWorldXna.Initialize();
        }

        public void Update(GameTime gameTime)
        {
            // ONLY FOR DEBUG
            // TIME multiplier
            //TimeSpan elapsed = TimeSpan.FromSeconds(0.10 * gameTime.ElapsedGameTime.TotalSeconds);

            TimeSpan elapsed = gameTime.ElapsedGameTime;

            _synchronizer.Update();
            _gameWorld.Update(elapsed);            
            _infoPanel.Update(gameTime);
            _gameWorldXna.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            _gameWorldXna.Draw(gameTime);

            _infoPanel.Draw(gameTime);
        }
    }
}