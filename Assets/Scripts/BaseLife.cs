using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseLife : MonoBehaviour
{
    public int lifePoint;

    private void Start()
    {
        lifePoint = 3;
    }

    public void SetVal(int val)
    {
        lifePoint = val;
    }

    public void Reduce()
    {
        lifePoint--;
    }

    public int GetVal()
    {
        return lifePoint;
    }
}
