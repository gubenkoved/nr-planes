using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NRPlanes.Core.Common;
using Plane = NRPlanes.Core.Common.Plane;
using NRPlanes.Core.Controllers;

namespace NRPlanes.Client.Common
{
    public class LocalPlaneController : PlaneControllerBase
    {
        private KeyboardState _oldState;

        public LocalPlaneController(Plane plane)
            :base(plane)
        {

        }

        public override void Update()
        {
            var newState = Keyboard.GetState();

            #region Mooving
            #region Forward

            if (newState.IsKeyDown(Keys.Up) && _oldState.IsKeyUp(Keys.Up))
            {
                ControlledPlane.StartMotion(MotionType.Forward);
            }

            if (newState.IsKeyUp(Keys.Up) && _oldState.IsKeyDown(Keys.Up))
            {
                ControlledPlane.EndMotion(MotionType.Forward);
            }

            #endregion

            #region Left

            if (newState.IsKeyDown(Keys.Left) && _oldState.IsKeyUp(Keys.Left))
            {
                ControlledPlane.StartMotion(MotionType.Left);
            }

            if (newState.IsKeyUp(Keys.Left) && _oldState.IsKeyDown(Keys.Left))
            {
                ControlledPlane.EndMotion(MotionType.Left);
            }

            #endregion

            #region Right

            if (newState.IsKeyDown(Keys.Right) && _oldState.IsKeyUp(Keys.Right))
            {
                ControlledPlane.StartMotion(MotionType.Right);
            }

            if (newState.IsKeyUp(Keys.Right) && _oldState.IsKeyDown(Keys.Right))
            {
                ControlledPlane.EndMotion(MotionType.Right);
            }

            #endregion

            #region Fire

            if (newState.IsKeyDown(Keys.Space))
            {
                ControlledPlane.Fire();
            }

            if (newState.IsKeyDown(Keys.Z))
            {
                ControlledPlane.Fire(WeaponPosition.LeftFront);
            }

            if (newState.IsKeyDown(Keys.X))
            {
                ControlledPlane.Fire(WeaponPosition.RightFront);
            }

            #endregion 
            #endregion

            #region Shield
            if (newState.IsKeyDown(Keys.LeftShift) && _oldState.IsKeyUp(Keys.LeftShift))
            {
                ControlledPlane.ActivateShield();
            }

            if (newState.IsKeyUp(Keys.LeftShift) && _oldState.IsKeyDown(Keys.LeftShift))
            {
                ControlledPlane.DeactivateShield();
            }
            #endregion

            _oldState = newState;
        }
    }
}