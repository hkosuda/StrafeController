using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM
{
    public class PM_Jumping : Controller
    {
        // constants
        static readonly int jumpingFrameBuffer = 10;

        // valiables
        static public int JumpingFrameBufferRemain { get; private set; }
        static public bool JumpingBegin { get; private set; }
        static public bool AutoJump { get; set; } = false;

        public override void Update(float dt)
        {
            if (Input.GetKeyDown(KeyCode.Space)) { AutoJump = !AutoJump; }

            if (PM_Landing.LandingIndicator < 0)
            {
                JumpingFrameBufferRemain--;
                if (JumpingFrameBufferRemain < 0) { JumpingFrameBufferRemain = 0; }

                return;
            }

            if (JumpingFrameBufferRemain <= 0)
            {
                if (Input.mouseScrollDelta.y > 0.0f || AutoJump)
                {
                    JumpingFrameBufferRemain = jumpingFrameBuffer;
                    //JumpingBegin = true;

                    var v = Player.Rb.velocity;
                    Player.Rb.velocity = new Vector3(v.x, PmParams.pm_jumping_velocity, v.z);
                }
            }
        }

        public override void FixedUpdate(float dt)
        {
            if (PM_Landing.LandingIndicator < 0)
            {
                JumpingFrameBufferRemain--;
                if (JumpingFrameBufferRemain < 0) { JumpingFrameBufferRemain = 0; }

                return;
            }

            if (JumpingFrameBufferRemain <= 0)
            {
                if (Input.mouseScrollDelta.y > 0.0f || Input.GetKey(KeyCode.Mouse1))
                {
                    JumpingFrameBufferRemain = jumpingFrameBuffer;
                    //JumpingBegin = true;

                    var v = Player.Rb.velocity;
                    Player.Rb.velocity = new Vector3(v.x, PmParams.pm_jumping_velocity, v.z);
                }
            }
        }

        public override void LateUpdate()
        {
            JumpingBegin = false;
        }
    }
}

