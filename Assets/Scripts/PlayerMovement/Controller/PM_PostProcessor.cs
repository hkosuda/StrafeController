using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM
{
    public class PM_PostProcessor : Controller
    {
        public override void FixedUpdate(float dt)
        {
            var pv = PM_PlaneVector.PlaneVector;

            if (PM_Jumping.JumpingBegin)
            {
                Player.Rb.velocity = new Vector3(pv.x, PmParams.pm_jumping_velocity, pv.y);
            }

            else
            {
                Player.Rb.velocity = new Vector3(pv.x, Player.Rb.velocity.y, pv.y);
            }
        }
    }
}

