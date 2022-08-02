using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM
{
    public class Player : MonoBehaviour
    {
        static public readonly float playerRadius = 0.5f;
        static public readonly float centerY = 0.9f;

        static public GameObject Myself { get; private set; }
        static public GameObject Camera { get; private set; }

        static public Rigidbody Rb { get; private set; }
        static public MeshCollider Collider { get; private set; }

        private void Awake()
        {
            Myself = gameObject;
            Camera = GameObject.FindWithTag("MainCamera");

            Rb = gameObject.GetComponent<Rigidbody>();
            Collider = gameObject.GetComponent<MeshCollider>();
        }
    }
}
