using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SelfLife : MonoBehaviour
{

    public List<Image> heartImages;
    public int lifePoint;

    private void Start()
    {
        lifePoint = 3;
    }
}
