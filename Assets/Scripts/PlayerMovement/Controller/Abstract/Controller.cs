using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM
{
    public abstract class Controller
    {
        public virtual void Initialize()
        {
            return;
        }

        public virtual void Shutdown()
        {
            return;
        }

        public virtual void Update(float dt)
        {
            return;
        }

        public virtual void LateUpdate()
        {
            return;
        }

        public virtual void FixedUpdate(float dt)
        {
            return;
        }
    }
}

