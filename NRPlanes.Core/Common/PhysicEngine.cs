using System;
using System.Collections.Generic;
using System.Linq;
using NRPlanes.Core.Primitives;

namespace NRPlanes.Core.Common
{
    public static class PhysicEngine
    {
        public static void UpdateMechanicalParameters(GameObject obj, TimeSpan elapsed)
        {
            double elapsedSeconds = elapsed.TotalSeconds;

            #region Position
            obj.Position += obj.Velocity * elapsedSeconds;

            obj.Velocity += obj.Acceleration * elapsedSeconds;

            obj.Acceleration = new Vector();

            obj.Affect(CalculateAirResistanceForce(obj), obj.Position);
            #endregion

            #region Rotation
            obj.Rotation += obj.RotationVelocity * elapsedSeconds;
            obj.Rotation = Helper.NormalizeAngle(obj.Rotation);

            obj.RotationVelocity += obj.RotationAcceleration * elapsedSeconds;

            obj.RotationAcceleration = 0.0;

            obj.RotationAcceleration = CalculateAngularResistance(obj);
            #endregion
        }

        /// <summary>
        /// Affect on game object with specified force and fulcrum
        /// </summary>
        /// <param name="obj">Game object which under force action</param>
        /// <param name="force">Vector of force (absolulte value)</param>
        /// <param name="fulcrum">Point at which the applied impact</param>
        public static void Affect(GameObject obj, Vector force, Vector fulcrum)
        {
            #region Force affect
            obj.Acceleration += force / obj.Mass;
            #endregion

            #region Moment of force affect
            Vector radiusVector = fulcrum - obj.Position;
            double angle = Vector.AngleBetween(radiusVector, force);

            var momentOfForce = Math.Sin(Helper.ToRadians(angle)) * force.Length * radiusVector.Length;

            obj.RotationAcceleration += momentOfForce / obj.AngularMass;
            #endregion
        }

        public static void ImpulseAffect(GameObject obj, Vector impulse, Vector fulcrum)
        {
            #region Impusle affect
            obj.Velocity += impulse / obj.Mass;
            #endregion

            #region Moment of impusle affect
            Vector radiusVector = fulcrum - obj.Position;
            double angle = Vector.AngleBetween(radiusVector, impulse);

            var momentOfImpulse = Math.Sin(Helper.ToRadians(angle)) * impulse.Length * radiusVector.Length;

            obj.RotationVelocity += momentOfImpulse / obj.AngularMass;
            #endregion
        }

        public static Vector CalculateAirResistanceForce(GameObject obj)
        {
            const double alpha = 0.1;

            if (obj.Velocity.Length > 0.0)
            {
                var angleBetweenVelocityAndRotation = obj.Velocity.Angle() - obj.Rotation;

                var referenceArea = obj.ReferenceArea.GetValue(angleBetweenVelocityAndRotation);

                var ort = obj.Velocity.Ort();

                return -alpha * referenceArea * obj.Velocity.LengthSquared * ort;
            }
            else
            {
                return new Vector();
            }
        }

        public static double CalculateAngularResistance(GameObject obj)
        {
            const double gamma = 0.01;

            return -gamma * Math.Sign(obj.RotationVelocity) * obj.RotationVelocity * obj.RotationVelocity;
        }

        /// <summary>
        /// Sweep and Prune collision detection algorithm <see cref="http://habrahabr.ru/blogs/algorithm/135948/"/>        
        /// </summary>
        public static IEnumerable<Collision> GetCollisions(GameObject[] objects)
        {
            var absoluteGeomertyCache = new Dictionary<GameObject, Geometry>(); // absolute geometry cache

            for (int i = 0; i < objects.Length; i++)
            {
                absoluteGeomertyCache[objects[i]] = objects[i].CalculateAbsoluteGeometry();
            }
            
            var xCoordinateComparer = new Comparison<GameObject>((x, y) => absoluteGeomertyCache[x].BoundingRectangle.X.CompareTo(absoluteGeomertyCache[y].BoundingRectangle.X));

            Array.Sort(objects, xCoordinateComparer); // sort game objects by x coordinate of left bound (of bounding rectangle)

            for (int i = 0; i < objects.Length; i++)
            {
                double maxX = absoluteGeomertyCache[objects[i]].BoundingRectangle.X
                    + absoluteGeomertyCache[objects[i]].BoundingRectangle.Width;

                for (int j = i + 1; j < objects.Length && absoluteGeomertyCache[objects[j]].BoundingRectangle.X <= maxX; j++)
                {
                    if (absoluteGeomertyCache[objects[i]].IsIntersectsOrInclude(absoluteGeomertyCache[objects[j]]))
                    {
                        // NOTE: collision position can be determined exactly

                        var collisionPositon = objects[i].Position;

                       yield return new Collision(objects[i], objects[j], collisionPositon);
                    }
                }
            }
        }
    }
}