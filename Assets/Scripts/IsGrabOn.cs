using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsGrabOn : MonoBehaviour
{
    public bool IsGrab;
    // Start is called before the first frame update
    void Start()
    {
        IsGrab = false;
    }

    public void UpdateGrabStatus()
    {
        IsGrab = true;
    }

    public void UpdateNoGrabStatus()
    {
        IsGrab = false;
    }
}
