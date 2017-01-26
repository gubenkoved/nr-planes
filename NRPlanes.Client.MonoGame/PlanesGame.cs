using System;
using NRPlanes.Core.Primitives;
using Microsoft.Xna.Framework;
using NRPlanes.Client.Common;
using NRPlanes.Core.Common;
using NRPlanes.Core.Planes;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace NRPlanes.Client
{
    public class PlanesGame : Game
    {
        public readonly GameManager GameManager;
        public GraphicsDeviceManager Graphics { get; private set; }

        public PlanesGame()
        {
            Graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";

            if (Config.Default.IsFullScreen)
            {
                Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                Graphics.IsFullScreen = true;
            }
            else
            {
                Graphics.PreferredBackBufferWidth = Config.Default.BufferWidth;
                Graphics.PreferredBackBufferHeight = Config.Default.BufferHeight;
                Graphics.IsFullScreen = false;
            }

            GameManager = new GameManager(this);

            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromMilliseconds(20);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            GameManager.Initialize();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            GameManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            GameManager.Draw(gameTime);

            //base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            Process.GetCurrentProcess().Kill();
        }
    }
}
