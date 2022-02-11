using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VelocityPreview2 : MonoBehaviour
{
    [SerializeField] bool showPreview = true;

    Text text;

    void Start()
    {
        text = gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();
    }

    void Update()
    {
        text.text = "";

        if (PlayerController.Rb == null) { return; }
        if (!showPreview) { return; }

        var mag = Vecf.Magnitude(new float[2] { PlayerController.Rb.velocity.x, PlayerController.Rb.velocity.z });
        text.text = mag.ToString("F2") + " [m/s]";
    }
}
