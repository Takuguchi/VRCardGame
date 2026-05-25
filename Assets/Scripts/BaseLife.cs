using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤー・敵共通のライフポイント管理基底クラス。
/// SelfLife / EnemyLife の共通インターフェースを定義する。
/// 現在は UI 更新を子クラスに委ねており、このクラスは値の保持と変更のみ担当する。
/// </summary>
public class BaseLife : MonoBehaviour
{
    public int lifePoint; // 現在のライフ（初期値 3）

    private void Start()
    {
        lifePoint = 3;
    }

    // ライフを指定値に設定する
    public void SetVal(int val)
    {
        lifePoint = val;
    }

    // ライフを1減らす
    public void Reduce()
    {
        lifePoint--;
    }

    // 現在のライフを返す
    public int GetVal()
    {
        return lifePoint;
    }
}
