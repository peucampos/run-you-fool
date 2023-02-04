using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput : MonoBehaviour
{
    public bool MoveRight;
    public bool MoveLeft;
    void Update()
    {
        if (Input.GetAxis("Horizontal") > 0)
        {
            MoveRight = true;
        }
        else
        {
            MoveRight = false;
        }

        if (Input.GetAxis("Horizontal") < 0)
        {
            MoveLeft = true;
        }
        else
        {
            MoveLeft = false;
        }
    }
}


