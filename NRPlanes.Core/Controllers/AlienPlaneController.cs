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
        private GameWorld m_world;

        public AlienPlaneController(GameWorld world, Plane controlled)
            : base(controlled)
        {
            m_world = world;
        }

        public override void Update(TimeSpan elapsed)
        {
            Plane nearestPlayer = FindNearestPlayersPlane();

            if (nearestPlayer == null)
                return;

            double needAngleToPlayer = Helper.RelativeAngleBetweenPositions(ControlledPlane.Position, nearestPlayer.Position);
            double needRotationDelta = Helper.NormalizeAngle((ControlledPlane.Rotation - needAngleToPlayer));

            if (needRotationDelta < 180) // rotate through left side
            {
                ControlledPlane.StartMotion(MotionType.Left);
            }
            else
            {
                ControlledPlane.StartMotion(MotionType.Right);
            }

            // when player too close
            if ((ControlledPlane.Position - nearestPlayer.Position).Length < ControlledPlane.RelativeGeometry.BoundingRectangle.Height * 7)
            {
                ControlledPlane.Fire(WeaponPosition.CenterFront);
                ControlledPlane.Fire(WeaponPosition.LeftFront);
                ControlledPlane.Fire(WeaponPosition.RightFront);
            }
        }

        private Plane FindNearestPlayersPlane()
        {
            IEnumerable<Plane> playersPlanes = m_world.GameObjects
                .Where(o => o is Plane && !m_world.PlaneControllers.Any(c => c.ControlledPlane == o)).Cast<Plane>().ToList();

            Vector curPosition = ControlledPlane.Position;

            Plane nearestPlayer = playersPlanes.OrderBy(plane => (plane.Position - curPosition).LengthSquared).FirstOrDefault();

            return nearestPlayer;
        }
    }
}
