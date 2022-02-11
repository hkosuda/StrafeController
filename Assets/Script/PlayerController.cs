using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject mainCamera = null;

    [SerializeField] int landingFrameBuffer = 3;
    [SerializeField] int jumpingFrameBuffer = 3;

    [SerializeField] float jumpingSpeed = 3.6f;

    [SerializeField] float groundSpeed = 10.0f;
    [SerializeField] float airSpeed = 0.5f;
    [SerializeField] float groundAccel = 120.0f;
    [SerializeField] float airAccel = 100.0f;
    [SerializeField] float draggingAccel = 70.0f;

    [SerializeField] float playerHeight = 1.0f;

    static public float MaxSpeedOnTheGround { get; private set; }
    static public float MaxSpeedInTheAir { get; private set; }
    static public float AccelOnTheGround { get; private set; }
    static public float AccelInTheAir { get; private set; }
    static public float FrictionAccel { get; private set; }

    static public bool IsJumping { get; private set; }
    static public Rigidbody Rb { get; private set; }

    static public float[] ZX_CurrentVector { get; private set; }
    static public float[] ZX_InputVector { get; private set; }

    static int _landingFrameBuffer;
    static int _jumpingFrameBuffer;

    static float groundHeight;
    static float raycastOriginHeight = 1.0f;
    static float offset = 0.0001f;

    void Start()
    {
        IsJumping = false;

        ZX_CurrentVector = new float[2] { 0.0f, 0.0f };
        Rb = gameObject.GetComponent<Rigidbody>();

        _landingFrameBuffer = landingFrameBuffer;
        _jumpingFrameBuffer = jumpingFrameBuffer;
    }

    void Update()
    {
        UpdateSettings();
        UpdateJumpingFrameBuffer();

        ZX_InputVector = GetZXInputVector(mainCamera.transform.rotation.eulerAngles.y * Mathf.Deg2Rad);
        var landingIndicator = CheckLanding();

        // landing indicator : -1 -> in the air
        if (landingIndicator < 0)
        {
            IsJumping = true;
            ZX_CurrentVector = PlayerControllerSub.GetNextVector(ZX_CurrentVector, ZX_InputVector, Time.deltaTime, false);

        }

        // perfect landing, half landing
        else
        {
            IsJumping = false;
            Rb.position = new Vector3(Rb.position.x, groundHeight + playerHeight, Rb.position.z);

            if (GetJumpingInput())
            {
                ZX_CurrentVector = PlayerControllerSub.GetNextVector(ZX_CurrentVector, ZX_InputVector, Time.deltaTime, onTheGround: false);
                Rb.velocity = new Vector3(Rb.velocity.x, jumpingSpeed, Rb.velocity.z);

                landingFrameBuffer = _landingFrameBuffer;
                jumpingFrameBuffer = _jumpingFrameBuffer;
                IsJumping = true;
            }

            else
            {
                // half landing
                if (landingIndicator == 0)
                {
                    ZX_CurrentVector = PlayerControllerSub.GetNextVector(ZX_CurrentVector, ZX_InputVector, Time.deltaTime, onTheGround: false);
                }

                // perfect landing
                else
                {
                    Rb.velocity = new Vector3(Rb.velocity.x, 0.0f, Rb.velocity.z);
                    ZX_CurrentVector = PlayerControllerSub.GetNextVector(ZX_CurrentVector, ZX_InputVector, Time.deltaTime, onTheGround: true);
                }
            }
        }

        UpdateRbVelocity();
    }

    float[] GetZXInputVector(float radRotY)
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

    bool GetJumpingInput()
    {
        if (AutoJump.Active)
        {
            return true;
        }

        if (Input.mouseScrollDelta.y > 0)
        {
            return true;
        }

        return false;
    }

    void UpdateJumpingFrameBuffer()
    {
        _jumpingFrameBuffer--;

        if (_jumpingFrameBuffer < 0)
        {
            _jumpingFrameBuffer = 0;
        }
    }

    void UpdateRbVelocity()
    {
        var vz = ZX_CurrentVector[0];
        var vx = ZX_CurrentVector[1];

        Rb.velocity = new Vector3(vx, Rb.velocity.y, vz);
    }

    int CheckLanding()
    {
        // -1 : in the air, 0 : half landing, 1 : perfect landing
        if (_jumpingFrameBuffer > 0)
        {
            return -1;
        }

        var hit = GetFloorWithBoxCasting();

        if (hit.collider != null)
        {
            if (Rb.position.y - hit.point.y <= playerHeight + offset)
            {
                _landingFrameBuffer--;
                groundHeight = hit.point.y;

                // half landing
                if (_landingFrameBuffer > 0)
                {
                    return 0;
                }

                // half landing
                else
                {
                    _landingFrameBuffer = 0;
                    return 1;
                }
            }
        }

        return -1;
        
        // inner function
        RaycastHit GetFloorWithBoxCasting()
        {
            // 0.21 = 0.3 / sqrt(2) ... 0.3 : radius of player
            var boxSize = new Vector3(0.21f, raycastOriginHeight * 0.5f, 0.21f);
            var direction = new Vector3(0.0f, -1.0f, 0.0f);

            var layer = 1 << 6;
            var origin = Rb.position + new Vector3(0.0f, raycastOriginHeight, 0.0f);
            var quaternion = Quaternion.Euler(0.0f, mainCamera.transform.rotation.eulerAngles.y, 0.0f);

            Physics.BoxCast(origin, boxSize, direction, out RaycastHit hit, quaternion, Mathf.Infinity, layer, QueryTriggerInteraction.Ignore);

            return hit;
        }
    }

    void UpdateSettings()
    {
        MaxSpeedOnTheGround = groundSpeed;
        MaxSpeedInTheAir = airSpeed;
        AccelOnTheGround = groundAccel;
        AccelInTheAir = airAccel;
        FrictionAccel = draggingAccel;
    }

    static public void Stop()
    {
        Rb.velocity = Vector3.zero;
        ZX_CurrentVector = new float[2] { 0.0f, 0.0f };
    }
}

static public class PlayerControllerSub
{
    // for debug
    static public float[] ZX_AddVector { get; private set; }
    static public float MagnitudeOfProjection { get; private set; }

    static public float[] GetNextVector(float[] zx_playerVector, float[] zx_inputVector, float dt, bool onTheGround = true)
    {
        var playerVector = new Vector2(zx_playerVector[1], zx_playerVector[0]);
        var inputVector = new Vector2(zx_inputVector[1], zx_inputVector[0]);

        var nextPlayerVector = GetNextPlayerVector(playerVector, inputVector, dt, onTheGround);

        return new float[2] { nextPlayerVector.y, nextPlayerVector.x };
    }

    static Vector2 GetNextPlayerVector(Vector2 playerVector, Vector2 inputVector, float dt, bool onTheGround = true)
    {
        var normalizedInputVector = inputVector.normalized;

        // 地上でのパラメータ
        var _frictionAccel = PlayerController.FrictionAccel;
        var maxSpeed = PlayerController.MaxSpeedOnTheGround;
        var accel = PlayerController.AccelOnTheGround;

        // 空中でのパラメータ
        if (!onTheGround)
        {
            _frictionAccel = 0.0f;
            maxSpeed = PlayerController.MaxSpeedInTheAir;
            accel = PlayerController.AccelInTheAir;
        }

        // 摩擦ベクトルの大きさを計算する
        var magnitudeOfFriction = Clip(playerVector.magnitude, 0.0f, _frictionAccel * dt);

        // 摩擦ベクトルを得る（プレイヤーベクトルと逆向き）
        var frictionVector = playerVector.normalized * (-magnitudeOfFriction);

        // プレイヤーベクトルと摩擦ベクトルの和を得る．
        var playerVector_fric = playerVector + frictionVector;

        // 射影ベクトルの大きさを得る．
        var magnitudeOfProjection = Vector2.Dot(playerVector_fric, normalizedInputVector);

        // 加算ベクトルの大きさを得る．
        var magnitudeOfAddVector = Clip(maxSpeed - magnitudeOfProjection, 0.0f, accel * dt);

        // 加算ベクトルを得る．
        var addVector = normalizedInputVector * magnitudeOfAddVector;

        // <for debug>
        ZX_AddVector = new float[2] { addVector.y, addVector.x };
        MagnitudeOfProjection = magnitudeOfProjection;
        // </for debug>

        // 加算ベクトルと，プレイヤーベクトルと摩擦ベクトルを足したベクトルとの和を，次のフレームでのプレイヤーベクトルとする．
        return playerVector_fric + addVector;
    }

    static float Clip(float val, float min, float max)
    {
        if (val <= min) { return min; }

        if (val <= max) { return val; }

        return max;
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

    static public float Disp(float[] point1, float[] point2)
    {
        var dr = point1[0] - point2[0];
        var dc = point1[1] - point2[1];

        return Magnitude(new float[2] { dr, dc });
    }
}

