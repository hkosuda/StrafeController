using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM
{
    public class PM_Camera : Controller
    {
        //static readonly float hOmegaThreshold = 1620.0f;
        //static readonly float vOmegaThreshold = 720.0f;

        static readonly float hOmegaThreshold = 1440.0f;
        static readonly float vOmegaThreshold = 360.0f;

        static Transform tr;

        static public float addRotX;
        static public float addRotY;

        static float degRotX;
        static float degRotY;

        public override void Initialize()
        {
            tr = Player.Camera.transform;
            tr.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        }

        public override void Update(float dt)
        {
            var dx = Input.GetAxis("Mouse Y") * PmMain.MouseSensitivity;
            var dy = Input.GetAxis("Mouse X") * PmMain.MouseSensitivity;

            if (Mathf.Abs(dx) / dt > vOmegaThreshold) { dx = 0.0f; }
            if (Mathf.Abs(dy) / dt > hOmegaThreshold) { dy = 0.0f; }

            degRotX -= dx;
            degRotY += dy;

            var rotX = degRotX + addRotX;

            if (rotX > 90.0f) { degRotX = 90.0f - addRotX; rotX = 90.0f; }
            if (rotX < -90.0f) { degRotX = -90.0f - addRotX; rotX = -90.0f; }

            tr.eulerAngles = new Vector3(rotX, degRotY, 0.0f);
        }

        static public Vector3 EulerAngle()
        {
            return new Vector3(degRotX, degRotY, 0.0f);
        }

        static public void SetEulerAngles(Vector3 euler)
        {
            tr.eulerAngles = euler;

            degRotX = euler.x;
            degRotY = euler.y;

            addRotX = 0.0f;
            addRotY = 0.0f;
        }
    }
}
