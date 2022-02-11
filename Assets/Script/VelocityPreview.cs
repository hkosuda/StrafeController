using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VelocityPreview : MonoBehaviour
{
    [SerializeField] GameObject mainCamera;
    [SerializeField] bool showPreview = true;

    Text text;

    void Start()
    {
        text = gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();   
    }

    // Update is called once per frame
    void Update()
    {
        if (showPreview)
        {
            text.gameObject.SetActive(true);
            UpdateText();
        }

        else
        {
            text.gameObject.SetActive(false);
        }
    }

    void UpdateText()
    {
        if (PlayerController.ZX_CurrentVector == null) { return; }
        if (PlayerControllerSub.ZX_AddVector == null) { return; }

        var cr_mag = Vecf.Magnitude(PlayerController.ZX_CurrentVector);
        var ad_mag = Vecf.Magnitude(PlayerControllerSub.ZX_AddVector);

        var rotY = mainCamera.transform.rotation.eulerAngles.y * Mathf.Deg2Rad;

        var ml_currentVector = Vecf.Rot(PlayerController.ZX_CurrentVector, rotY);
        var ml_addVector = Vecf.Rot(PlayerControllerSub.ZX_AddVector, rotY);

        var format = "F2";

        var info = "";
        info += "<color=#FF0000FF>ADD VECTOR ... \t\tM : " + ml_addVector[0].ToString(format) + ", \t\tL : " + ml_addVector[1].ToString(format) + ", \t\t|V| : " + ad_mag.ToString(format) + "</color>\n";
        info += "<color=#00FF00FF>MAIN VECTOR ... \tM : " + ml_currentVector[0].ToString(format) + ", \t\tL : " + ml_currentVector[1].ToString(format) + ", \t\t|V| : " + cr_mag.ToString(format) + "</color>";

        text.text = info;
    }
}
