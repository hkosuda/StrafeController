using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewController : MonoBehaviour
{
    [SerializeField] float sensitivity = 1.0f;

    static public float DegRotZ { get; private set; }
    static public float DegRotX { get; private set; }
    static public float DegRotY { get; private set; }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        DegRotX -= Input.GetAxis("Mouse Y") * sensitivity;
        DegRotY += Input.GetAxis("Mouse X") * sensitivity;

        if (DegRotX > 90.0f) { DegRotX = 90.0f; }
        if (DegRotX < -90.0f) { DegRotX = -90.0f; }

        gameObject.transform.rotation = Quaternion.Euler(DegRotX, DegRotY, DegRotZ);
    }
}
