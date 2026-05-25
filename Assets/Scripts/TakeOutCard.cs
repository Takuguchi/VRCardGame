using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// カードをデッキから取り出すアニメーションを制御するクラス（実装途中）。
/// DOTween を使って指定コライダーまでカードを移動させる予定だったが、
/// DoMove 内の移動先座標の実装が未完成のため現在は動作しない。
/// </summary>
public class TakeOutCard : MonoBehaviour
{
    public Collider targetCollider; // 移動先のコライダー
    public GameObject Card;         // 移動させるカードオブジェクト

    private void Start()
    {
        Transform transform = GetComponent<Transform>();
    }

    // DOTween のシーケンスでカードをアニメーション移動させる（目的地が未設定のため未完成）
    private void DoMove()
    {
        Sequence seq = DOTween.Sequence();
        // seq.Append(transform.DOMove(new Vector3(tra), 1f)); // 移動先座標を要実装
    }
}
