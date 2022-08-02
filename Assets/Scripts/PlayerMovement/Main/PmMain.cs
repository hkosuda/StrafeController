using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM
{
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public class PmMain : MonoBehaviour
    {
        [SerializeField] float mouseSensitivity = 1.0f;

        static public bool Interrupt { get; set; }
        static public float MouseSensitivity { get; private set; }

        static readonly List<Controller> controllerList = new List<Controller>()
        {
            new PM_Camera(),
            new PM_InputVector(),

            new PM_Landing(),
            new PM_Jumping(),

            new PM_PlaneVector(),
            new PM_PostProcessor(),
        };

        void Awake()
        {
            MouseSensitivity = mouseSensitivity;

            foreach(var controller in controllerList)
            {
                controller.Initialize();
            }
        }

        private void OnDestroy()
        {
            foreach(var controller in controllerList)
            {
                controller.Shutdown();
            }
        }

        void Update()
        {
            var dt = Time.deltaTime;

            foreach (var controller in controllerList)
            {
                if (Interrupt) { break; }
                controller.Update(Time.deltaTime);
            }

            Interrupt = false;
        }

        void LateUpdate()
        {
            foreach(var controller in controllerList)
            {
                if (Interrupt) { break; }
                controller.LateUpdate();
            }

            Interrupt = false;
        }

        void FixedUpdate()
        {
            var dt = Time.fixedDeltaTime;

            foreach(var controller in controllerList)
            {
                if (Interrupt) { break; }
                controller.FixedUpdate(dt);
            }

            Interrupt = false;
        }
    }
}

