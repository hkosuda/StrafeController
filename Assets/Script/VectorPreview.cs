using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorPreview : MonoBehaviour
{
    [SerializeField] GameObject mainCamera;

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

        if (PlayerController.ZX_CurrentVector == null) { return; }
        if (PlayerController.ZX_InputVector == null) { return; }
        if (PlayerControllerSub.ZX_AddVector == null) { return; }

        SetScaleAndRotation(mainVector, PlayerController.ZX_CurrentVector, mainVectorMagnification);
        SetScaleAndRotation(inputVector, PlayerController.ZX_InputVector,inputVectorMagnification * PlayerControllerSub.MagnitudeOfProjection);
        SetScaleAndRotation(addVector, PlayerControllerSub.ZX_AddVector, addVectorMagnification);

        if (PlayerController.IsJumping)
        {
            isJumping.gameObject. SetActive(true);
            isJumping.sizeDelta = new Vector2(2.0f * lineWidth, 2.0f * lineWidth);
        }

        else
        {
            isJumping.gameObject.SetActive(false);
        }
    }

    void SetScaleAndRotation(RectTransform line, float[] zx_vector, float magnify)
    {
        var magnitude = Vecf.Magnitude(zx_vector);

        line.sizeDelta = new Vector2(lineWidth, magnitude * magnify * defaultMagnification);

        var vecDir = Mathf.Atan2(zx_vector[1], zx_vector[0]) * Mathf.Rad2Deg;
        var viewDir = mainCamera.transform.rotation.eulerAngles.y;

        var rotY = viewDir - vecDir;

        line.rotation = Quaternion.Euler(0.0f, 0.0f, rotY);
    }
}
