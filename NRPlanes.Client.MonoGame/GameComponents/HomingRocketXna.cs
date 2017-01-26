using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Client.Common;
using NRPlanes.Client.Particles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using NRPlanes.Core.Bullets;
using NRPlanes.Core.Primitives;

namespace NRPlanes.Client.GameComponents
{
    public class HomingRocketXna : DrawableGameObject
    {
        public new HomingRocket GameObject
        {
            get { return base.GameObject as HomingRocket; }
        }

        private Texture2D m_texture;
        private TrailDrawer m_trailDrawer;

        public HomingRocketXna(PlanesGame game, HomingRocket bullet, CoordinatesTransformer coordinatesTransformer)
            : base(game, bullet, coordinatesTransformer)
        {
            var sound = game.GameManager.GameWorldXna.SoundManager.CreateBasicSoundEffect("bullet_sound");
            sound.Position = bullet.Position;
            sound.Play();

            m_trailDrawer = new TrailDrawer(game.Content.Load<Texture2D>("Other/line_3px"), coordinatesTransformer, Color.White, 0.3, 0.2f, 0.05f, 15, 3);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (m_texture == null)
                m_texture = Game.Content.Load<Texture2D>("Images/rocket");

            if (GameObject.TimeToLive > TimeSpan.Zero)
            {
                m_trailDrawer.PathPoints.Add(GameObject.Position);

                m_trailDrawer.DrawTrail(spriteBatch);

                var origin = new Vector2(m_texture.Width / 2.0f, m_texture.Height / 2.0f);

                var scaleVector = CoordinatesTransformer.CreateScaleVector(GameObject.RelativeGeometry.BoundingRectangle.Size,
                                                                           new Size(m_texture.Width, m_texture.Height));

                spriteBatch.Draw(m_texture,
                                 CoordinatesTransformer.Transform(GameObject.Position),
                                 null,
                                 Color.White,
                                 MathHelper.ToRadians((float)GameObject.Rotation),
                                 origin,
                                 scaleVector,
                                 SpriteEffects.None,
                                 LayersDepths.Bullet);
            }
        }
    }
}
