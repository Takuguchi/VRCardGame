using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キーボードのEnterキーでモンスターをデバッグ召喚するクラス。
/// VR環境ではなくエディタ上でモンスターの表示・アニメーションを
/// 単体テストするための開発用スクリプト。本番では使用しない。
/// </summary>
public class Summon : MonoBehaviour
{
    public Transform SummonPlace;   // 召喚する座標のTransform
    public GameObject dragonPref;   // テスト召喚するドラゴンのプレハブ
    private GameObject dragonObj;   // 召喚されたドラゴンのインスタンス

    private bool _isGenerated = false; // 2重召喚を防ぐフラグ

    void Start()
    {
    }

    void Update()
    {
        MonsterSummon();
    }

    // Enterキーを押したとき、まだ召喚していなければドラゴンを指定位置に生成する
    void MonsterSummon()
    {
        if (Input.GetKeyDown(KeyCode.Return) && _isGenerated == false)
        {
            dragonObj = Instantiate(dragonPref);
            dragonObj.SetActive(true);
            dragonObj.transform.position = SummonPlace.transform.position;
            _isGenerated = true; // 2回目以降は召喚しない
        }
    }
}
