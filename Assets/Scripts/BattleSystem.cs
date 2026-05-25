using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using OVR.OpenVR;
using UnityEngine;

/// <summary>
/// バトルフェーズのじゃんけん判定と、勝敗に応じたアニメーション・SE・エフェクトを制御するクラス。
/// カードに設定されたテクスチャ名から手（グー・チョキ・パー）を取得し勝敗を算出する。
/// 結果は PhaseController に返され、ライフ更新などの後処理が行われる。
/// </summary>
public class BattleSystem : MonoBehaviour
{
    public PopCard popCard;               // テクスチャ→手の辞書参照用
    public GameObject rootEnemyCard;      // バトルに出た敵カードのルートオブジェクト
    public GameObject rootMyCard;         // バトルに出たプレイヤーカードのルートオブジェクト
    public GameObject phaseController;
    public GameObject battleField;        // プレイヤー側バトルフィールド（selectNumSelf の参照元）
    public GameObject enemyManager, popCardSystem;
    private Animator animatorSelf, animatorEnemy; // バトル中の両モンスターアニメーター
    private int i;

    // ---- モンスター別の勝敗SE（DW=DragonWin, DL=DragonLose, ... DT=DrawSE）----
    public AudioSource DW, DL, RW, RL, SW, SL, DT;

    public GameObject Smoke; // ドラゴンの炎攻撃時に表示するスモークエフェクト

    void Start()
    {
    }

    void Update()
    {
    }

    /// <summary>
    /// バトル結果に応じて敗北したカード（と3Dモデル・フィールド）をシーンから削除する。
    /// result: 0=プレイヤー勝利, 1=プレイヤー敗北, 2=引き分け
    /// </summary>
    public void DeleteCard(int result)
    {
        if (result == 0)
        {
            // プレイヤー勝利：敵カードを削除
            Destroy(rootEnemyCard.GetComponent<Card>().model3D);
            Destroy(rootEnemyCard.GetComponent<Card>().cardField);
            Destroy(rootEnemyCard);
        }
        if (result == 1)
        {
            // プレイヤー敗北：自分のカードを削除
            Destroy(rootMyCard.GetComponent<Card>().model3D);
            Destroy(rootMyCard.GetComponent<Card>().cardField);
            Destroy(rootMyCard);
            battleField.GetComponent<BattleField>().triggerCount--;
        }
        if(result == 2)
        {
            // 引き分け：両方のカードを削除
            Destroy(rootMyCard.GetComponent<Card>().model3D);
            Destroy(rootMyCard.GetComponent<Card>().cardField);
            Destroy(rootMyCard);
            battleField.GetComponent<BattleField>().triggerCount--;
            Destroy(rootEnemyCard.GetComponent<Card>().model3D);
            Destroy(rootEnemyCard.GetComponent<Card>().cardField);
            Destroy(rootEnemyCard);
        }
    }

    /// <summary>
    /// じゃんけん判定を行いバトル結果を返す。
    /// 各カードのテクスチャ名から手を取得し、GetResult() で勝敗を決定した上で
    /// 両モンスターの攻撃アニメーションを再生する。
    /// 戻り値: 0=プレイヤー勝利, 1=プレイヤー敗北, 2=引き分け
    /// </summary>
    public int Battle()
    {
        // カードのマテリアルからテクスチャを取得し、辞書で手文字列に変換
        Texture enemyTex = GetTexture(rootEnemyCard);
        Texture myTex = GetTexture(rootMyCard);
        string enemyHand = popCard.GetComponent<PopCard>().texture2hands[enemyTex.name];
        string myHand = popCard.GetComponent<PopCard>().texture2hands[myTex.name];

        int yourResult = GetResult(enemyHand, myHand);

        // バトルに出したカードのアニメーターを取得
        i = battleField.GetComponent<BattleField>().selectNumSelf;
        animatorSelf = popCardSystem.GetComponent<PopCard>().benchCards[i].GetComponent<Card>().model3D.GetComponent<Animator>();
        i = enemyManager.GetComponent<EnemyManager>().selectNum;
        animatorEnemy = enemyManager.GetComponent<EnemyManager>().enemySummoneds[i].GetComponent<Animator>();

        // 結果に応じて攻撃アニメーションのトリガーをセット
        if (yourResult == 0)
        {
            animatorSelf.SetTrigger("WinAttack");
            animatorEnemy.SetTrigger("LoseAttack");
        }
        else if (yourResult == 2)
        {
            animatorSelf.SetTrigger("LoseAttack");
            animatorEnemy.SetTrigger("LoseAttack");
        }
        else if (yourResult == 1)
        {
            animatorSelf.SetTrigger("LoseAttack");
            animatorEnemy.SetTrigger("WinAttack");
        }
        else
        {
            throw new System.Exception("omission of requirements in GetResult method.");
        }
        return yourResult;
    }

    /// <summary>
    /// カードルートオブジェクトから "_MainTex2" スロットのテクスチャを取得するヘルパー。
    /// カードの3Dモデル階層を辿ってRendererにアクセスする。
    /// </summary>
    private Texture GetTexture(GameObject rootCard)
    {
        Transform tr2 = rootCard.transform;
        Transform targetTransform = tr2.Find("Visuals/Root/Black_PlayingCards_Spade10_00");
        GameObject targetObject = targetTransform.gameObject;
        Renderer renderer = targetObject.GetComponent<Renderer>();
        Material material = renderer.material;
        return material.GetTexture("_MainTex2");
    }

    /// <summary>
    /// じゃんけんのルールに基づき勝敗を返す。
    /// 同時に対応するモンスターの勝ち/負けSE・エフェクトコルーチンを起動する。
    /// 戻り値: 0=プレイヤー勝利, 1=プレイヤー敗北, 2=引き分け
    /// </summary>
    private int GetResult(string enemyHand, string myHand)
    {
        // Paper(パー) の場合
        if(myHand == "Paper")
        {
            if (enemyHand == "Rock")   { StartCoroutine(MotionDragonWin()); StartCoroutine(MotionSlimeLose()); return 0; }
            if (enemyHand == "Scissors"){ StartCoroutine(MotionDragonLose()); StartCoroutine(MotionRobotWin()); return 1; }
            if (enemyHand == "Paper")  { StartCoroutine(MotionDragonLose()); return 2; }
        }
        // Rock(グー) の場合
        if (myHand == "Rock")
        {
            if (enemyHand == "Rock")   { StartCoroutine(MotionSlimeLose()); return 2; }
            if (enemyHand == "Scissors"){ StartCoroutine(MotionSlimeWin()); StartCoroutine(MotionRobotLose()); return 0; }
            if (enemyHand == "Paper")  { StartCoroutine(MotionSlimeLose()); StartCoroutine(MotionDragonWin()); return 1; }
        }
        // Scissors(チョキ) の場合
        if (myHand == "Scissors")
        {
            if (enemyHand == "Rock")   { StartCoroutine(MotionSlimeWin()); StartCoroutine(MotionRobotLose()); return 1; }
            if (enemyHand == "Scissors"){ StartCoroutine(MotionRobotLose()); return 2; }
            if (enemyHand == "Paper")  { StartCoroutine(MotionDragonLose()); StartCoroutine(MotionRobotWin()); return 0; }
        }
        return -1;
    }

    // ---- 各モンスターの勝ち/負け演出コルーチン ----
    // SE再生 → エフェクト表示 → 一定時間後にSEを停止するパターンで統一

    IEnumerator MotionDragonWin()
    {
        DW.Play();
        yield return new WaitForSeconds(3f);
        Smoke.SetActive(true);  // 炎攻撃のスモークエフェクト表示
        yield return new WaitForSeconds(1f);
        yield return new WaitForSeconds(4f);
        Smoke.SetActive(false);
        yield return new WaitForSeconds(1f);
        DW.Stop();
    }

    IEnumerator MotionDragonLose()
    {
        DL.Play();
        yield return new WaitForSeconds(9f);
        DL.Stop();
        DT.Play(); // ドラゴン敗北後にドロー音を再生
    }

    IEnumerator MotionRobotWin()
    {
        RW.Play();
        yield return new WaitForSeconds(9f);
        RW.Stop();
    }

    IEnumerator MotionRobotLose()
    {
        RL.Play();
        yield return new WaitForSeconds(9f);
        RL.Stop();
    }

    IEnumerator MotionSlimeWin()
    {
        SW.Play();
        yield return new WaitForSeconds(9f);
        SW.Stop();
    }

    IEnumerator MotionSlimeLose()
    {
        SL.Play();
        yield return new WaitForSeconds(9f);
        SL.Stop();
    }
}
