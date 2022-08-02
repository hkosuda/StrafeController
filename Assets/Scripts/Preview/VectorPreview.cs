using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PM;

public class VectorPreview : MonoBehaviour
{
    [SerializeField] bool showPreview = true;

    [SerializeField] float lineWidth = 2.0f;

    [SerializeField] float defaultMagnification = 10.0f;

    [SerializeField] float inputVectorMagnification = 1.0f;
    [SerializeField] float addVectorMagnification = 1.0f;
    [SerializeField] float mainVectorMagnification = 1.0f;

    GameObject container;

    RectTransform isJumping;
    RectTransform mainVector;
    RectTransform inputVector;
    RectTransform addVector;

    void Start()
    {
        container = gameObject.transform.GetChild(0).gameObject;

        GetRectTransforms();
    }

    void GetRectTransforms()
    {
        isJumping = GetRect(0);
        mainVector = GetRect(1);
        inputVector = GetRect(2);
        addVector = GetRect(3);

        // inner function
        RectTransform GetRect(int n)
        {
            return gameObject.transform.GetChild(0).GetChild(n).gameObject.GetComponent<RectTransform>();
        }
    }

    void Update()
    {
        if (!showPreview)
        {
            container.SetActive(false);
            return;
        }

        container.SetActive(true);

        if (PM_PlaneVector.PlaneVector == null) { return; }

        SetScaleAndRotation(mainVector, PmUtil.NextVector, mainVectorMagnification);
        SetScaleAndRotation(inputVector, PM_InputVector.InputVector, inputVectorMagnification);
        SetScaleAndRotation(addVector, PmUtil.AddVector, addVectorMagnification);

        if (PM_Landing.LandingIndicator <= 0)
        {
            isJumping.gameObject. SetActive(true);
            isJumping.sizeDelta = new Vector2(2.0f * lineWidth, 2.0f * lineWidth);
        }

        else
        {
            isJumping.gameObject.SetActive(false);
        }
    }

    void SetScaleAndRotation(RectTransform line, Vector2 vector, float magnify)
    {
        var magnitude = vector.magnitude;

        line.sizeDelta = new Vector2(lineWidth, magnitude * magnify * defaultMagnification);

        var vecDir = Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;
        var viewDir = Player.Camera.transform.rotation.eulerAngles.y;

        var rotY = viewDir - vecDir;

        line.rotation = Quaternion.Euler(0.0f, 0.0f, rotY);
    }
}
