using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharCtrl : MonoBehaviour
{
    static public bool IsJumping { get; private set; }

    static public Vector3 MainVector { get; private set; }

    static public Rigidbody Rb { get; private set; }

    static public float[] ZX_CurrentVector { get; private set; }

    static public float radRotYc;
    static public float radRotYp;

    static int frameBuffer;

    void Start()
    {
        IsJumping = false;

        ZX_CurrentVector = new float[2] { 0.0f, 0.0f };

        Rb = gameObject.GetComponent<Rigidbody>();

        radRotYp = ViewController.DegRotY * Mathf.Deg2Rad;
    }

    void Update()
    {
        radRotYp = ViewController.DegRotY * Mathf.Deg2Rad;
        var zx_inputVector = GetZXInputVector(radRotYp);

        // ground
        if (CheckLanding())
        {
            IsJumping = false;

            if (JumpingInput())
            {
                ZX_CurrentVector = GroundCharCtrl.GetNextVector(ZX_CurrentVector, zx_inputVector, Time.deltaTime, false);
                Rb.velocity = new Vector3(Rb.velocity.x, 4.8f, Rb.velocity.z);

                frameBuffer = 2;
                IsJumping = true;
            }

            else
            {
                ZX_CurrentVector = GroundCharCtrl.GetNextVector(ZX_CurrentVector, zx_inputVector, Time.deltaTime, true);
            }
        }

        // air
        else
        {
            IsJumping = true;

            ZX_CurrentVector = GroundCharCtrl.GetNextVector(ZX_CurrentVector, zx_inputVector, Time.deltaTime, false);

            frameBuffer--;
            if (frameBuffer < 0) { frameBuffer = 0; }
        }

        UpdateRbVelocity();

        radRotYp = ViewController.DegRotY * Mathf.Deg2Rad;
    }

    static float[] GetZXInputVector(float radRotY)
    {
        var vm = 0.0f;
        var vl = 0.0f;

        if (Input.GetKey(KeyCode.W)) { vm += 1.0f; }
        if (Input.GetKey(KeyCode.S)) { vm -= 1.0f; }

        if (Input.GetKey(KeyCode.D)) { vl += 1.0f; }
        if (Input.GetKey(KeyCode.A)) { vl -= 1.0f; }

        var ml_inputVector = new float[2] { vm, vl };
        var zx_inputVector = Vecf.Rot(ml_inputVector, -radRotY);

        return zx_inputVector;
    }

    static bool JumpingInput()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            return true;
        }

        return false;
    }

    static void UpdateRbVelocity()
    {
        var vz = ZX_CurrentVector[0];
        var vx = ZX_CurrentVector[1];

        Rb.velocity = new Vector3(vx, Rb.velocity.y, vz);
    }

    bool CheckLanding()
    {
        if (frameBuffer < 0) { return false; }

        if (gameObject.transform.position.y < 0.001f)
        {
            return true;
        }

        return false;
    }
}

public class AirCharCtrl
{

}

public class GroundCharCtrl
{
    // settings
    static public float MaxGroundSpeed { get; private set; } = 10.0f;
    static public float MaxAirSpeed { get; private set; } = 1.0f;
    static public float MaxGroundAccel { get; private set; } = 20.0f * MaxGroundSpeed; // reaching to max speed in 0.1 sec
    static public float MaxAirAccel { get; private set; } = 50.0f * MaxAirSpeed;
    static public float DraggingAccel { get; private set; } = MaxGroundAccel * 0.2f;

    // vectors
    static public float[] ZX_MainVector { get; private set; } = new float[2] { 0.0f, 0.0f };
    static public float[] ZX_InputVector { get; private set; } = new float[2] { 0.0f, 0.0f };
    static public float[] ZX_AddVector { get; private set; } = new float[2] { 0.0f, 0.0f };
    static public float[] ZX_NextMainVector { get; private set; } = new float[2] { 0.0f, 0.0f };
    static public float CurrentSpeed { get; private set; }

    static public float[] GetNextVector(float[] zx_mainVector, float[] zx_inputVector, float dt, bool onTheGround)
    {
        ZX_MainVector = zx_mainVector;
        ZX_InputVector = Vecf.Normalize(zx_inputVector);

        ZX_AddVector = new float[2] { 0.0f, 0.0f };
        ZX_NextMainVector = new float[2] { 0.0f, 0.0f };

        CurrentSpeed = 0.0f;

        if (onTheGround)
        {
            zx_mainVector = AddFriction(zx_mainVector, DraggingAccel, dt);
        }

        if (zx_inputVector[0] == 0.0f || zx_inputVector[1] == 0.0f) { return zx_mainVector; }

        CurrentSpeed = Vecf.Dot(zx_inputVector, zx_mainVector);

        float addSpeed;

        if (onTheGround)
        {
            addSpeed = Clip(MaxGroundSpeed - CurrentSpeed, 0.0f, MaxGroundAccel * dt);
        }

        else
        {
            addSpeed = Clip(MaxAirSpeed - CurrentSpeed, 0.0f, MaxAirAccel * dt);
        }

        ZX_AddVector = Vecf.Magnify(Vecf.Normalize(zx_inputVector), addSpeed);

        ZX_NextMainVector = Vecf.Sum(zx_mainVector, ZX_AddVector);

        return ZX_NextMainVector;
    }

    static float[] AddFriction(float[] vector, float friction, float dt)
    {
        var currentSpeed = Vecf.Magnitude(vector);
        if (currentSpeed == 0.0f) { return new float[2] { 0.0f, 0.0f }; }

        var nextSpeed = currentSpeed - friction * dt;
        if (nextSpeed < 0.0f) { nextSpeed = 0.0f; }

        var coeff = nextSpeed / currentSpeed;

        return Vecf.Magnify(vector, coeff);
    }

    static float Clip(float val, float minVal, float maxVal)
    {
        if (val < minVal)
        {
            return minVal;
        }

        if (val > maxVal)
        {
            return maxVal;
        }

        return val;
    }
}

public class Vecf
{
    static public float Magnitude(float[] vector)
    {
        return Mathf.Sqrt(Mathf.Pow(vector[0], 2.0f) + Mathf.Pow(vector[1], 2.0f));
    }

    static public float[] Normalize(float[] vector)
    {
        var magnitude = Magnitude(vector);
        if (magnitude == 0.0f) { return new float[2] { 0.0f, 0.0f }; }


        var v0 = vector[0] / magnitude;
        var v1 = vector[1] / magnitude;

        return new float[2] { v0, v1 };
    }

    static public float Dot(float[] zx_baseVector, float[] zx_vector)
    {
        zx_baseVector = Normalize(zx_baseVector);

        var theta1 = Mathf.Atan2(zx_baseVector[1], zx_baseVector[0]);
        var theta2 = Mathf.Atan2(zx_vector[1], zx_vector[0]);

        var dTheta = theta2 - theta1;

        var magnitude = Magnitude(zx_vector);
        return magnitude * Mathf.Cos(dTheta);
    }

    static public float[] Magnify(float[] vector, float coeff)
    {
        return new float[2] { vector[0] * coeff, vector[1] * coeff };
    }

    static public float[] Sum(float[] vector1, float[] vector2)
    {
        return new float[2] { vector1[0] + vector2[0], vector1[1] + vector2[1] };
    }

    static public float[] Rot(float[] vector, float rad)
    {
        var vz = vector[0];
        var vx = vector[1];

        var vm = vz * Mathf.Cos(rad) + vx * Mathf.Sin(rad);
        var vl = -vz * Mathf.Sin(rad) + vx * Mathf.Cos(rad);

        return new float[2] { vm, vl };
    }
}

