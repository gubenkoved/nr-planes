using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using NRPlanes.Client.Common;
using NRPlanes.Core.Primitives;
using Microsoft.Xna.Framework.Graphics;

namespace NRPlanes.Client.Particles
{
    public enum ParticleType
    {
        Circle,
        BluredCircle,
        Square,
        Star,
        Diamond
    }

    public class Particle : MyDrawableGameComponent, ICloneable
    {
        private Texture2D m_texture;

        public TimeSpan TimeToLive = TimeSpan.FromSeconds(10);
        private TimeSpan m_liveTime;

        public ParticleType Type = ParticleType.BluredCircle;
        public double Alpha = 1.0;
        public double AlphaVelocity;
        public Color Color = Color.White;
        
        public Size Size = new Size(1, 1);
        public Vector SizeFactor = new Vector(1, 1);
        public Vector SizeFactorVelocity;

        public Vector Position;
        public Vector Velocity;
        public Vector Acceleration;
        public double Rotation;
        public double RotationVelocity;
        public double RotationAcceleration;

        public float Depth = LayersDepths.ParticlesDefault;

        public Particle(PlanesGame game, CoordinatesTransformer transformer)
            :base(game, transformer)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            m_liveTime += gameTime.ElapsedGameTime;

            if (m_liveTime <= TimeToLive)
            {
                Alpha += AlphaVelocity;
                Alpha = Math.Max(0.0f, Math.Min(1.0f, Alpha));

                ParticlesEngine.Update(this, gameTime.ElapsedGameTime);
            }
            else
            {
                IsGarbage = true;
            }
        }
        public override void Draw(GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            if (m_texture == null)
                m_texture = Game.Content.Load<Texture2D>("Particles/circle_blured");

            if (!IsGarbage)
            {
                var origin = new Vector2(m_texture.Width / 2.0f, m_texture.Height / 2.0f);

                var scaleVector = CoordinatesTransformer.CreateScaleVector(
                    new Size(Size.Width * SizeFactor.X, Size.Height * SizeFactor.Y), 
                    new Size(m_texture.Width, m_texture.Height));

                spriteBatch.Draw(m_texture,
                                 CoordinatesTransformer.Transform(Position),
                                 null,
                                 new Color(this.Color.R/ 255f, this.Color.G/ 255f, this.Color.B / 255f, (float)Alpha),
                                 MathHelper.ToRadians((float)Rotation),
                                 origin,
                                 scaleVector,
                                 SpriteEffects.None,
                                 Depth);
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
