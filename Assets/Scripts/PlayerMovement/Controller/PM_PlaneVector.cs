using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM
{
    public class PM_PlaneVector : Controller
    {
        static public Vector2 PlaneVector { get; private set; }

        public override void FixedUpdate(float dt)
        {
            var landingIndicator = PM_Landing.LandingIndicator;

            var currentVector = CurrentVector();
            var inputVector = PM_InputVector.InputVector;

            // in the air
            if (landingIndicator < 0 || PM_Jumping.JumpingBegin)
            {
                PlaneVector = PmUtil.CalcVector(inputVector, currentVector, dt, false);
                return;
            }

            // half landing
            if (landingIndicator == 0)
            {
                PlaneVector = PmUtil.CalcVector(inputVector, currentVector, dt, false);
                return;
            }

            // perfect landing
            else
            {
                PlaneVector = PmUtil.CalcVector(inputVector, currentVector, dt, true);
            }

            // - inner function
            static Vector2 CurrentVector()
            {
                var v = Player.Rb.velocity;
                return new Vector2(v.x, v.z);
            }
        }
    }
}

