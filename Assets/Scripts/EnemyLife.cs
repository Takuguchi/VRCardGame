using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 敵のライフUIを管理するクラス。
/// SelfLife と同じ構造で、敵側のハートアイコンを管理する。
/// lifePoint の変更と heartImages の非表示化は PhaseController が直接行う。
/// </summary>
public class EnemyLife : MonoBehaviour
{
    public List<Image> heartImages; // 敵のハートアイコンのUIイメージ一覧（インデックス 0〜2）
    public int lifePoint;           // 現在の敵ライフ（0〜3）

    private void Start()
    {
        lifePoint = 3; // 初期ライフは3
    }
}
