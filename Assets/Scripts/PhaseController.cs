using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
using UnityEngine;
using TMPro;

/// <summary>
/// ゲーム全体のフェーズ（局面）を管理するメインコントローラー。
/// startPhase → summonPhase → selectPhase → battlePhase → drawPhase の順で進み、
/// どちらかのライフが0になるまでターンをループさせる。
/// </summary>
public class PhaseController : MonoBehaviour
{
    // ---- フェーズフラグ ----
    // 各フェーズがアクティブかどうかを表すフラグ。
    // Coroutine内でこれらを監視してフェーズ遷移を制御する。
    public bool startPhase;   // 初回カードドローフェーズ
    public bool summonPhase;  // ベンチへのカード召喚フェーズ
    public bool selectPhase;  // バトルに出すカードの選択フェーズ
    public bool battlePhase;  // 勝敗判定フェーズ
    public bool drawPhase;    // 追加ドローフェーズ

    // ベンチに置かれたカードの枚数（3枚でsummonPhase終了）
    public int benchSummonCnt;

    // ---- 外部コンポーネント参照 ----
    public EnemyManager enemyManager;
    public PopCard popCard;
    public BattleSystem battleSystem;
    public GameObject popCardSystem;
    public BattleField batlleFeild;
    public GameObject _battleField;
    public GameObject _enemyManager;
    public GameObject enemyLife, selfLife;          // ライフUIオブジェクト
    public TMP_Text turnText;                        // ターン数テキスト
    public ParticleSystem summonP, battleP, selectP, drawP, WinP, LoseP; // 各フェーズ演出パーティクル
    public GameObject enemyHuman;                   // 敵キャラのAnimatorを持つGameObject
    private Animator enemyAnime;                    // 敵キャラのアニメーター
    public ChangeScene chScene;                     // シーン遷移コンポーネント
    public AudioSource winSound;                    // 勝利BGM
    public AudioSource mainSound, LifeDis;          // メインBGM、ライフ消失SE
    // 各フェーズのガイド表示用矢印オブジェクト
    public GameObject DrawS, MidBenchS, LeftBenchS, RightBenshS, BattleFieldS;
    public TMP_Text tmpGuide;                       // プレイヤー向けガイドテキスト

    private int turnNum;       // 現在のターン番号
    private Color tempColor;   // ライフハートUIのアルファ操作に使う一時カラー
    private int yourResult;    // 直前のバトル結果（0:勝ち, 1:負け, 2:引き分け）

    void Start()
    {
        enemyAnime = enemyHuman.GetComponent<Animator>();
        popCard.PopCardToDeckHead(); // 最初の1枚をデッキ先頭に生成
        benchSummonCnt = 0;
        turnNum = 1;
        turnText.text = "Turn " + turnNum.ToString();

        // 全フェーズをリセットし、startPhaseから開始
        startPhase = true;
        summonPhase = false;
        selectPhase = false;
        battlePhase = false;
        drawPhase = false;

        yourResult = -1; // 未決定状態

        // 両者のライフを3に初期化
        enemyLife.GetComponent<EnemyLife>().lifePoint = 3;
        selfLife.GetComponent<SelfLife>().lifePoint = 3;

        StartCoroutine(ProceedPhase());
    }

    void Update()
    {

    }

    /// <summary>
    /// ゲームの1ターンを制御するメインコルーチン。
    /// 各フェーズの開始演出→待機ループ→後処理→次フェーズへ、という流れで進む。
    /// ターン終了後、両者ライフが残っていれば再帰的に自身を呼び出してループする。
    /// </summary>
    private IEnumerator ProceedPhase()
    {
        turnText.text = "TURN " + turnNum.ToString();

        // ---- startPhase: 手札を5枚引くフェーズ ----
        if (startPhase == true)
        {
            drawP.Play();
            tmpGuide.text = "右の山から5枚カードを引いてください";
            DrawS.SetActive(true); // ドロー場所を示す矢印を表示
        }

        // 5枚引き終わるまで待機（Card.csのFinishStartPhaseがstartPhaseをfalseにする）
        while (startPhase)
        {
            yield return null;
        }
        DrawS.SetActive(false);

        // ---- summonPhase: ベンチへカードを3枚置くフェーズ ----
        summonPhase = true;
        enemyManager.PlayBench(); // 敵もベンチにカードを裏向きで配置

        // 前のターンでバトルに使ったカードのフラグをリセット
        if (_battleField.GetComponent<BattleField>().selectNumSelf != -1)
        {
            _battleField.GetComponent<BattleField>().doneMove = false;
            _battleField.GetComponent<BattleField>().firstLocateDup = false;
            popCardSystem.GetComponent<PopCard>().benchField[_battleField.GetComponent<BattleField>().selectNumSelf].GetComponent<CardColliderEvent>().doneMove = false;
        }

        if (yourResult != 0)
        {
            summonP.Play(); // 召喚エフェクト再生（勝利後は非表示）
        }

        // ガイドテキストと矢印を更新：
        // 初回 or 全枠空きなら3枠すべてを示し、勝利後の枠補充なら空いた1枠のみ示す
        if (_battleField.GetComponent<BattleField>().selectNumSelf == -1)
        {
            MidBenchS.SetActive(true);
            LeftBenchS.SetActive(true);
            RightBenshS.SetActive(true);
            tmpGuide.text = "右の山から3枚のカードをベンチに置いてください";
        }
        else if (popCardSystem.GetComponent<PopCard>().benchField[_battleField.GetComponent<BattleField>().selectNumSelf].GetComponent<CardColliderEvent>().locationNum == 0 && yourResult != 0)
        {
            LeftBenchS.SetActive(true);
            tmpGuide.text = "右の山から1枚選び空いているベンチに置いてください";
        }
        else if (popCardSystem.GetComponent<PopCard>().benchField[_battleField.GetComponent<BattleField>().selectNumSelf].GetComponent<CardColliderEvent>().locationNum == 1 && yourResult != 0)
        {
            MidBenchS.SetActive(true);
            tmpGuide.text = "右の山から1枚選び空いているベンチに置いてください";
        }
        else if (popCardSystem.GetComponent<PopCard>().benchField[_battleField.GetComponent<BattleField>().selectNumSelf].GetComponent<CardColliderEvent>().locationNum == 2 && yourResult != 0)
        {
            RightBenshS.SetActive(true);
            tmpGuide.text = "右の山から1枚選び空いているベンチに置いてください";
        }

        // benchSummonCntが3になるまで待機（CardColliderEventがカウントアップする）
        while (summonPhase)
        {
            yield return null;
            if (benchSummonCnt == 3)
            {
                summonPhase = false;
            }
        }

        // ベンチ矢印を非表示にして2秒待機
        MidBenchS.SetActive(false);
        LeftBenchS.SetActive(false);
        RightBenshS.SetActive(false);
        yield return new WaitForSeconds(2f);

        // ---- selectPhase: バトルに出すカードを選択するフェーズ ----
        selectP.Play();
        tmpGuide.text = "ベンチから1枚選び、バトル場に置いてください";
        BattleFieldS.SetActive(true);
        selectPhase = true;
        enemyAnime.SetTrigger("PlayCard"); // 敵がカードを選ぶアニメーション
        enemyManager.OpenBench();          // 敵ベンチを表向きに公開
        popCard.changeCardsCanGrab();      // プレイヤーのカードをグラブ可能に

        // 選択前のベンチカード位置を記録（選択後に他のカードを元の位置に戻すため）
        List<Vector3> benchpos = new List<Vector3>();
        for (int i = 0; i < 3; i++)
        {
            benchpos.Add(popCardSystem.GetComponent<PopCard>().benchCards[i].transform.position);
        }

        // selectPhaseが終わるまで5秒ごとにカードを元の位置に戻し続ける
        // （VRコントローラーで誤って動かされた場合のリセット処理）
        while (selectPhase)
        {
            for (int i = 0; i < 3; i++)
            {
                popCardSystem.GetComponent<PopCard>().benchCards[i].transform.position = benchpos[i];
                popCardSystem.GetComponent<PopCard>().benchCards[i].transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            yield return new WaitForSeconds(5f);
        }
        tmpGuide.text = "";

        // バトルに出さなかったカードの3Dモデルを非表示にしてフィールドを整理
        for (int i = 0;i < 3; i++)
        {
            if (i == batlleFeild.GetComponent<BattleField>().selectNumSelf)
            {
                continue; // バトルに出したカードはそのまま
            }
            popCardSystem.GetComponent<PopCard>().benchCards[i].transform.position = benchpos[i];
            popCardSystem.GetComponent<PopCard>().benchCards[i].transform.rotation = Quaternion.Euler(0, 0, 0);
            popCardSystem.GetComponent<PopCard>().benchCards[i].GetComponent<Card>().model3D.SetActive(false);
        }

        // ---- battlePhase: じゃんけん判定フェーズ ----
        popCard.changeCardsCannotGrab(); // グラブ不可に戻す
        enemyAnime.SetTrigger("Batlle"); // 敵の攻撃アニメーション
        battlePhase = true;
        enemyManager.SelectCard();       // 敵がランダムにカードを1枚選んでバトル場へ移動
        yield return new WaitForSeconds(1f);

        battleP.Play();
        // バトル結果を取得: 0=勝ち, 1=負け, 2=引き分け
        yourResult = battleSystem.Battle();

        // バトルアニメーションが終わるのを待ってからカードを削除
        yield return new WaitForSeconds(12f);
        battleSystem.DeleteCard(yourResult);

        // 結果に応じて敵キャラのリアクションアニメーション
        if (yourResult == 1)
        {
            enemyAnime.SetTrigger("Win");
        }
        if (yourResult == 0)
        {
            enemyAnime.SetTrigger("Damaged");
        }
        if (yourResult == 2)
        {
            enemyAnime.SetTrigger("Draw");
        }

        battlePhase = false;
        yield return new WaitForSeconds(3f);

        // バトル結果によってライフを更新
        if (yourResult == 0 || yourResult == 2)
        {
            // プレイヤー勝利 or 引き分け：敵が負けたカードを補充
            enemyManager.RefullCard();
            _enemyManager.GetComponent<EnemyManager>().looseFlag = true;
        }
        else
        {
            // プレイヤー敗北：自分のライフを1減らしてハートUIを非表示に
            _enemyManager.GetComponent<EnemyManager>().looseFlag = false;
            enemyManager.returnPosition(); // 敵カードを元の位置に戻す
            selfLife.GetComponent<SelfLife>().lifePoint--;
            tempColor = selfLife.GetComponent<SelfLife>().heartImages[selfLife.GetComponent<SelfLife>().lifePoint].color;
            tempColor.a = 0.0f; // アルファを0にして非表示化
            selfLife.GetComponent<SelfLife>().heartImages[selfLife.GetComponent<SelfLife>().lifePoint].color = tempColor;
            LifeDis.Play(); // ライフ消失SE
        }

        // 引き分け or 敗北時はベンチカード数を減らす（消えたカードの分）
        if (yourResult == 1 || yourResult == 2)
        {
            benchSummonCnt--;
        }
        else
        {
            // プレイヤー勝利：バトルに出したカードを元のベンチ位置に戻し、敵のライフを減らす
            batlleFeild.returnPosition(benchpos[batlleFeild.GetComponent<BattleField>().selectNumSelf]);
            enemyLife.GetComponent<EnemyLife>().lifePoint--;
            tempColor = enemyLife.GetComponent<EnemyLife>().heartImages[enemyLife.GetComponent<EnemyLife>().lifePoint].color;
            tempColor.a = 0.0f;
            enemyLife.GetComponent<EnemyLife>().heartImages[enemyLife.GetComponent<EnemyLife>().lifePoint].color = tempColor;
            LifeDis.Play();
        }

        // バトルに出さなかったカードの3Dモデルを再表示
        for (int i = 0; i < 3; i++)
        {
            if (i == batlleFeild.GetComponent<BattleField>().selectNumSelf)
            {
                continue;
            }
            popCardSystem.GetComponent<PopCard>().benchCards[i].GetComponent<Card>().model3D.SetActive(true);
        }

        drawPhase = false;
        yield return new WaitForSeconds(1f);

        // ---- drawPhase: カードを1枚引くフェーズ（負け or 引き分け時のみ） ----
        // 両者ライフが残っている場合のみドローを行う
        if ((yourResult == 1 || yourResult == 2) && selfLife.GetComponent<SelfLife>().lifePoint > 0 && enemyLife.GetComponent<EnemyLife>().lifePoint > 0)
        {
            drawP.Play();
            DrawS.SetActive(true);
            tmpGuide.text = "右の山から1枚カードを引いてください";
            popCardSystem.GetComponent<PopCard>().popCnt++;
            drawPhase = true;
            popCard.PopCardToDeckHead(); // 次のカードをデッキ上に生成
        }
        else
        {
            drawPhase = false;
        }

        // カードを引き終わるまで待機（Card.csのFinishDrawPhaseがdrawPhaseをfalseにする）
        while (drawPhase)
        {
            yield return null;
        }
        DrawS.SetActive(false);
        tmpGuide.text = "";

        // 全フェーズフラグをリセット
        selectPhase = false;
        summonPhase = false;
        battlePhase = false;
        drawPhase = false;

        // 敵ライフが0なら敗北アニメーション再生
        if (enemyLife.GetComponent<EnemyLife>().lifePoint == 0)
        {
            enemyAnime.SetTrigger("Lose");
        }

        // ---- ゲーム継続チェック ----
        // 両者ライフが残っていれば次のターンへ
        if (selfLife.GetComponent<SelfLife>().lifePoint > 0 && enemyLife.GetComponent<EnemyLife>().lifePoint > 0)
        {
            turnNum++;
            StartCoroutine(ProceedPhase()); // 再帰呼び出しで次ターン開始
        }

        yield return new WaitForSeconds(1f);

        // ゲームオーバーまたはゲームクリア時の演出とシーン遷移
        if (selfLife.GetComponent<SelfLife>().lifePoint == 0)
        {
            LoseP.Play();
            yield return new WaitForSeconds(3f);
            chScene.ChangeMain2Op(); // オープニングシーンに戻る
        }
        else if (enemyLife.GetComponent<EnemyLife>().lifePoint == 0)
        {
            WinP.Play();
            mainSound.Stop();
            winSound.Play();
            yield return new WaitForSeconds(7f);
            chScene.ChangeMain2Op(); // オープニングシーンに戻る
        }
    }
}
