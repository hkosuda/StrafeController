using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PM;

public class VelocityPreview : MonoBehaviour
{
    static Text text;

    private void Awake()
    {
        text = gameObject.GetComponent<Text>();
    }

    private void Update()
    {
        var v = Player.Rb.velocity;
        var pv = new Vector2(v.x, v.z);

        text.text = pv.magnitude.ToString("F2") + " [m/s]";
    }
}
