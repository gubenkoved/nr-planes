using System;
using NRPlanes.Core.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NRPlanes.Client.Common;
using NRPlanes.Core.Common;
using NRPlanes.Core.Planes;
using NRPlanes.Core.Equipments.Weapons;
using NRPlanes.Client.Particles;

namespace NRPlanes.Client.GameComponents
{
    public class LaserGunXna : DrawableEquipment
    {
        public new LaserGun Equipment
        {
            get { return base.Equipment as LaserGun; }
        }
        
        private Texture2D m_texture;
        private DateTime m_lastShotDateTime;
        private SymmetricParticlesEmitter m_particlesEmitter;

        public LaserGunXna(PlanesGame game, LaserGun weapon, CoordinatesTransformer coordinatesTransformer)
            : base(game, weapon, coordinatesTransformer)
        {
            m_particlesEmitter = new SymmetricParticlesEmitter(game.GameManager.GameWorldXna)
            {
                PositionDeviationRadius = 0.4,
                VelocityDeviationRadius = 3,
                AlphaVelocityDeviationFactor = 0.3
            };
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (m_texture == null)
                m_texture = Game.Content.Load<Texture2D>("Images/weapon");

            if (m_lastShotDateTime != Equipment.LastShotDateTime)
            {
                m_lastShotDateTime = Equipment.LastShotDateTime;

                m_particlesEmitter.Emit(new Particle(Game, CoordinatesTransformer)
                {
                    Color = Color.White,
                    Position = Equipment.GetAbsolutePosition(),
                    Size = new Size(1, 1),
                    AlphaVelocity = -3.0f,
                    TimeToLive = TimeSpan.FromSeconds(2),
                    Velocity = Equipment.RelatedGameObject.Velocity + new Vector(0, 10).Rotate(Equipment.GetAbsoluteRotation()),
                    SizeFactorVelocity = new Vector(6, 6)
                }, 3);
            }

            var origin = new Vector2(m_texture.Width / 2.0f, m_texture.Height / 2.0f);

            var scaleVector = CoordinatesTransformer.CreateScaleVector(Equipment.Size,
                                                                       new Size(m_texture.Width, m_texture.Height));

            spriteBatch.Draw(m_texture,
                             CoordinatesTransformer.Transform(Equipment.GetAbsolutePosition()),
                             null,
                             Color.White,
                             MathHelper.ToRadians((float) Equipment.GetAbsoluteRotation()),
                             origin,
                             scaleVector,
                             SpriteEffects.None,
                             LayersDepths.Weapon);
        }
    }
}