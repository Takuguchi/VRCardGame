using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// プレイヤー自身のライフUIを管理するクラス。
/// heartImages リストの各ハートアイコンのアルファを操作してライフを視覚的に表示する。
/// lifePoint の変更と heartImages の非表示化は PhaseController が直接行う。
/// </summary>
public class SelfLife : MonoBehaviour
{
    public List<Image> heartImages; // ハートアイコンのUIイメージ一覧（インデックス 0〜2）
    public int lifePoint;           // 現在のプレイヤーライフ（0〜3）

    private void Start()
    {
        lifePoint = 3; // 初期ライフは3
    }
}
