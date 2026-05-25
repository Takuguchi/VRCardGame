using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TakeOutCard : MonoBehaviour
{
    public Collider targetCollider;
    public GameObject Card;

    private void Start()
    {
        Transform transform = GetComponent<Transform>();
    }

    private void DoMove()
    {
        Sequence seq = DOTween.Sequence();
        //seq.Append(transform.DOMove(new Vector3(tra), 1f));
    }

}
