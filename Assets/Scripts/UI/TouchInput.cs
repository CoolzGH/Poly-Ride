using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInput : MonoBehaviour
{
    [HideInInspector]
    public bool buttonPressed = false;

    public void ButtonDown()
    {
        buttonPressed = true;
    }

    public void ButtonUp()
    {
        buttonPressed = false;
    }
}