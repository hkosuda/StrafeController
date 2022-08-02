using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM
{
    public class PmUtil
    {
        // constants
        static public Vector2 AddVector { get; private set; }
        static public Vector2 NextVector { get; private set; }

        static public Vector2 CalcVector(Vector2 inputVector, Vector2 currentVector, float dt, bool onground)
        {
            // load settings 
            var draggingAccel = PmParams.pm_dragging_accel;
            var maxSpeed = PmParams.pm_max_speed_on_ground;
            var accel = PmParams.pm_accel_on_ground;

            if (!onground)
            {
                draggingAccel = 0.0f;
                maxSpeed = PmParams.pm_max_speed_in_air;
                accel = PmParams.pm_accel_in_air;
            }

            var normalizedInputVector = inputVector.normalized;

            var magnitudeOfFriction = Clip(currentVector.magnitude, 0.0f, draggingAccel * dt);

            var frictionVector = currentVector.normalized * (-magnitudeOfFriction);

            var playerVector_fric = currentVector + frictionVector;

            var magnitudeOfProjection = Vector2.Dot(playerVector_fric, normalizedInputVector);

            var magnitudeOfAddVector = Clip(maxSpeed - magnitudeOfProjection, 0.0f, accel * dt);

            var addVector = normalizedInputVector * magnitudeOfAddVector;

            var nextPlayerVector = playerVector_fric + addVector;

            AddVector = addVector;
            NextVector = nextPlayerVector;

            return nextPlayerVector;

            // - inner function
            static float Clip(float val, float minVal, float maxVal)
            {
                if (val < minVal) { return minVal; }
                if (val > maxVal) { return maxVal; }
                return val;
            }
        }
    }
}