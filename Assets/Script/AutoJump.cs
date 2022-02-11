using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoJump : MonoBehaviour
{
    [SerializeField] bool autoJump;

    static bool _autoJump;

    static public bool Active { get; private set; }

    void Start()
    {
        _autoJump = autoJump;
        Active = false;

    }

    void Update()
    {
        if (autoJump)
        {
            if (!_autoJump)
            {
                _autoJump = true;
                Active = false;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Active = !Active;
            }
        }

        else
        {
            _autoJump = false;
            Active = false;
        }
    }

    static public void Inactivate()
    {
        Active = false;
    }
}
