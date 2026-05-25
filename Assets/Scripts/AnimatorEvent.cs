using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アニメーターの状態を監視してイベント処理を行うクラス（実装途中）。
/// 現在はドラゴンの「Fly Flame Attack」ステートの検出のみ記述されており、
/// 実際の処理はコメントアウトされている。
/// 将来的には各モンスターの特定アニメーション状態で追加処理を行う予定。
/// </summary>
public class AnimatorEvent : MonoBehaviour
{
    public Animator animator;
    private AnimatorStateInfo stateInfo;

    void Start()
    {
        StartCoroutine(HandleDragon());
        // StartCoroutine(HandleSlime());
        // StartCoroutine(HandleRobot());
    }

    // ドラゴンの「Fly Flame Attack」ステートを検出するコルーチン（処理は未実装）
    IEnumerator HandleDragon()
    {
        if (animator != null)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Fly Flame Attack"))
            {
                // 炎攻撃エフェクトなどの追加処理をここに記述予定
            }
        }
        yield return null;
    }
}
