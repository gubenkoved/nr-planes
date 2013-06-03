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
        Star,
        Diamond,
        Cross
    }

    public class Particle : MyDrawableGameComponent, ICloneable
    {
        private Texture2D m_texture;

        public TimeSpan TimeToLive = TimeSpan.FromSeconds(10);
        private TimeSpan m_liveTime;

        public readonly ParticleType Type = ParticleType.BluredCircle;
        public double Alpha = 1.0;
        public double AlphaVelocity;
        public Color Color = Color.White;

        /// <summary>
        /// Particle engine never updates static partilce's mechanical params (size factor, position, velocity, rotation and rotation velocity)
        /// </summary>
        public bool IsStatic = false;

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

        public Particle(PlanesGame game, CoordinatesTransformer transformer, ParticleType type = ParticleType.BluredCircle)
            :base(game, transformer)
        {
            Type = type;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            m_liveTime += gameTime.ElapsedGameTime;

            if (m_liveTime <= TimeToLive)
            {
                Alpha += AlphaVelocity * gameTime.ElapsedGameTime.TotalSeconds;
                Alpha = Math.Max(0.0f, Math.Min(1.0f, Alpha));

                if (Alpha == 0)
                {
                    IsGarbage = true;
                }

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
            {
                switch (Type)
                {
                    case ParticleType.Circle:
                        m_texture = Game.Content.Load<Texture2D>("Particles/circle");
                        break;
                    case ParticleType.BluredCircle:
                        m_texture = Game.Content.Load<Texture2D>("Particles/circle_blured");
                        break;                    
                    case ParticleType.Star:
                        m_texture = Game.Content.Load<Texture2D>("Particles/star");
                        break;
                    case ParticleType.Diamond:
                        m_texture = Game.Content.Load<Texture2D>("Particles/diamond");
                        break;
                    case ParticleType.Cross:
                        m_texture = Game.Content.Load<Texture2D>("Particles/cross");
                        break;
                    default:
                        throw new Exception("Unknown particle type");
                }
            }

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
