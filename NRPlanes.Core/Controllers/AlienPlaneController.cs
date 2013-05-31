using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Core.Common;
using NRPlanes.Core.Primitives;

namespace NRPlanes.Core.Controllers
{
    public class AlienPlaneController : PlaneControllerBase
    {
        private const double MAX_ANGLE_DEVIATION_TO_SHOOT = 30;
        private const double MAX_DISTANCE_TO_SHOOT_IN_PLANE_HEIGHT = 7;

        private GameWorld m_world;

        public AlienPlaneController(GameWorld world, Plane controlled)
            : base(controlled)
        {
            m_world = world;
        }

        public override void Update(TimeSpan elapsed)
        {
            // disable previous motions
            ControlledPlane.EndMotion(MotionType.All);

            Plane nearestPlayer = FindNearestPlayersPlane();

            if (nearestPlayer == null)
                return;

            double angleToPlayer = Helper.RelativeAngleBetweenPositions(ControlledPlane.Position, nearestPlayer.Position);
            double rotationDelta = Helper.NormalizeAngle(angleToPlayer - ControlledPlane.Rotation);

            if (rotationDelta > 180) // rotate through left side
            {
                ControlledPlane.StartMotion(MotionType.Left);                
            }
            else
            {
                ControlledPlane.StartMotion(MotionType.Right);
            }

            // when player too close and angle deviation too small
            if ((ControlledPlane.Position - nearestPlayer.Position).Length < ControlledPlane.RelativeGeometry.BoundingRectangle.Height * MAX_DISTANCE_TO_SHOOT_IN_PLANE_HEIGHT
                && (rotationDelta < MAX_ANGLE_DEVIATION_TO_SHOOT || rotationDelta > 360 - MAX_ANGLE_DEVIATION_TO_SHOOT))
            {
                ControlledPlane.Fire(WeaponPosition.CenterFront);
                ControlledPlane.Fire(WeaponPosition.LeftFront);
                ControlledPlane.Fire(WeaponPosition.RightFront);                
            }
            else
            {
                ControlledPlane.StartMotion(MotionType.Forward);
            }
        }

        private Plane FindNearestPlayersPlane()
        {
            using (var handle = m_world.GameObjectsSafeReadHandle)
            {
                IEnumerable<Plane> playersPlanes = handle.Items.Where(o => o is Plane && !m_world.PlaneControllers.Any(c => c.ControlledPlane == o)).Cast<Plane>().ToList();
                
                Vector curPosition = ControlledPlane.Position;

                Plane nearestPlayer = playersPlanes.OrderBy(plane => (plane.Position - curPosition).LengthSquared).FirstOrDefault();

                return nearestPlayer;
            }
        }
    }
}
