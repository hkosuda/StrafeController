using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM
{
    public class PM_InputVector : Controller
    {
        static public Vector2 ML_InputVector { get; private set; }
        static public Vector2 InputVector { get; private set; }

        public override void Update(float dt)
        {
            var vm = 0.0f;
            var vl = 0.0f;

            if (Input.GetKey(KeyCode.W)) { vm += 1.0f; }
            if (Input.GetKey(KeyCode.S)) { vm -= 1.0f; }

            if (Input.GetKey(KeyCode.D)) { vl += 1.0f; }
            if (Input.GetKey(KeyCode.A)) { vl -= 1.0f; }

            ML_InputVector = new Vector2(vm, vl).normalized;

            var rotY = PM_Camera.EulerAngle().y * Mathf.Deg2Rad;

            var vz = vm * Mathf.Cos(rotY) - vl * Mathf.Sin(rotY);
            var vx = vm * Mathf.Sin(rotY) + vl * Mathf.Cos(rotY);

            InputVector = new Vector2(vx, vz).normalized;
        }
    }
}

