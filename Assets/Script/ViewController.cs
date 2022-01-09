using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewController : MonoBehaviour
{
    [SerializeField] float sensitibity = 1.0f;

    static public float RotYp { get; private set; }

    static public float DegRotZ { get; set; }
    static public float DegRotX { get; set; }
    static public float DegRotY { get; set; }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        RotYp = DegRotY;

        DegRotX -= Input.GetAxis("Mouse Y") * sensitibity;
        DegRotY += Input.GetAxis("Mouse X") * sensitibity;

        if (DegRotX > 90.0f) { DegRotX = 90.0f; }
        if (DegRotX < -90.0f) { DegRotX = -90.0f; }

        gameObject.transform.rotation = Quaternion.Euler(DegRotX, DegRotY, DegRotZ);
    }
}
