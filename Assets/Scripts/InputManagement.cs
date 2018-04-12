using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine;

public class InputManagement : MonoBehaviour
{
    void Start()
    {
        InputManager.OnDeviceAttached += inputDevice => Debug.Log( "Attached: " + inputDevice.Name );
        InputManager.OnDeviceDetached += inputDevice => Debug.Log( "Detached: " + inputDevice.Name );
        InputManager.OnActiveDeviceChanged += inputDevice => Debug.Log( "Switched: " + inputDevice.Name );    
    }
    
}
