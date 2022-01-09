using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorPreview : MonoBehaviour
{
    [SerializeField] float lineWidth = 0.2f;

    [SerializeField] float magnify_mainVector = 1.0f;
    [SerializeField] float magnify_inputVector = 1.0f;
    [SerializeField] float magnify_addVector = 1.0f;
    [SerializeField] float magnify_nextMainVector = 1.0f;

    GameObject zx_mainVector;
    GameObject zx_inputVector;
    GameObject zx_addVector;
    GameObject zx_nextMainVector;
    GameObject isJumping;

    void Start()
    {
        zx_mainVector = gameObject.transform.GetChild(0).gameObject;
        zx_inputVector = gameObject.transform.GetChild(1).gameObject;
        zx_addVector = gameObject.transform.GetChild(2).gameObject;
        zx_nextMainVector = gameObject.transform.GetChild(3).gameObject;
        isJumping = gameObject.transform.GetChild(4).gameObject;
    }

    void Update()
    {
        SetScaleAndRotation(zx_mainVector, GroundCharCtrl.ZX_MainVector, magnify_mainVector);
        SetScaleAndRotation(zx_inputVector, GroundCharCtrl.ZX_InputVector, magnify_inputVector * GroundCharCtrl.CurrentSpeed);
        SetScaleAndRotation(zx_addVector, GroundCharCtrl.ZX_AddVector, magnify_addVector);
        SetScaleAndRotation(zx_nextMainVector, GroundCharCtrl.ZX_MainVector, magnify_nextMainVector);

        if (CharCtrl.IsJumping)
        {
            isJumping.SetActive(true);
        }

        else
        {
            isJumping.SetActive(false);
        }
    }

    void SetScaleAndRotation(GameObject line, float[] zx_vector, float magnify)
    {
        var magnitude = Vecf.Magnitude(zx_vector);

        line.transform.localScale = new Vector3(lineWidth, lineWidth, magnitude * magnify);

        var vecDir = Mathf.Atan2(zx_vector[1], zx_vector[0]) * Mathf.Rad2Deg;
        var viewDir = Correct(ViewController.DegRotY);

        var theta = vecDir - viewDir;

        line.transform.rotation = Quaternion.Euler(0.0f, theta, 0.0f);
    }

    float Correct(float deg)
    {
        return deg % 360.0f;
    }
}
