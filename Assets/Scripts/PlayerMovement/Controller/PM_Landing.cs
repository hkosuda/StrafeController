using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM
{
    public class PM_Landing : Controller
    {
        static public EventHandler<RaycastHit> Landed { get; set; }

        static public int LandingIndicator { get; private set; }
        static public RaycastHit HitInfo { get; private set; }
        static public float DeltaY { get; private set; }
        static public float FloorY { get; private set; }

        // constants
        static public readonly int landingFrameBuffer = 1;
        static public readonly float landingHeightEpsilon = 0.001f;

        // valiables
        static int landingFrameBufferRemain;

        public override void Update(float dt)
        {
            LandingIndicator = CheckLanding();
        }

        // main function
        static int CheckLanding()
        {
            if (PM_Jumping.JumpingFrameBufferRemain > 0)
            {
                return -1;
            }

            if (SingleSphereCastCheck())
            {
                return LandingOrHalf();
            }

            InitializeLandingFrameBuffer();
            return -1;
        }

        // sub functions
        static void InitializeLandingFrameBuffer()
        {
            landingFrameBufferRemain = landingFrameBuffer;
        }

        static int LandingOrHalf()
        {
            var previousLandingIndicator = LandingIndicator;

            if (previousLandingIndicator < 0)
            {
                Landed?.Invoke(null, HitInfo);
            }

            landingFrameBufferRemain--;

            if (landingFrameBufferRemain > 0)
            {
                return 0;
            }

            landingFrameBufferRemain = 0;
            return 1;
        }

        static bool SingleSphereCastCheck()
        {
            var radius = Player.playerRadius - 0.02f;
            var rbPosition = Player.Rb.transform.position;
            var origin = rbPosition + new Vector3(0.0f, 0.1f, 0.0f);

            Physics.SphereCast(origin, radius, Vector3.down, out RaycastHit hitInfo, Mathf.Infinity);
            HitInfo = hitInfo;

            if (hitInfo.collider != null)
            {
                FloorY = hitInfo.point.y;
                DeltaY = rbPosition.y - FloorY;

                if (DeltaY <= Player.centerY + landingHeightEpsilon)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

