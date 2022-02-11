using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] GameObject player;

    float offset = -0.1f;

    void Update()
    {
        if (player.transform.position.y < offset)
        {
            var magnitude = Vecf.Magnitude(new float[2] { player.transform.position.x, player.transform.position.z });

            var border = MapManager.DropRate * magnitude + offset;

            if (player.transform.position.y < border)
            {
                player.transform.position = new Vector3(0.0f, 1.0f, 0.0f);
                PlayerController.Stop();
                AutoJump.Inactivate();
            }
        }
    }
}
