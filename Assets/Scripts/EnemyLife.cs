using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyLife : MonoBehaviour
{

    public List<Image> heartImages;
    public int lifePoint;

    private void Start()
    {
        lifePoint = 3;
    }
}
