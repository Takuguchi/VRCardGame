using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

/// <summary>
/// 個々のカードオブジェクトの状態を管理するクラス。
/// VRコントローラーでカードを掴んで動かしたとき（位置変化を検出）に
/// 次のカードを自動生成し、フェーズ終了を PhaseController に通知する。
/// </summary>
public class Card : MonoBehaviour
{
    private Vector3 popPosition;    // 前フレームのカード位置（移動検出用）
    private bool hasExecuted = false; // 同一カードで複数回ドロー処理が走らないようにするフラグ
    public int _popCnt;             // このカードが生成された時点の popCnt（drawPhase 判定に使用）

    public PopCard popCard;
    public GameObject popCardSystem;
    public GameObject phaseController;

    public bool summoned;           // このカードがベンチに召喚済みかどうか
    public GameObject model3D;      // このカードに対応する3Dモンスターモデル
    public GameObject cardField;    // このカードに対応するベンチフィールドオブジェクト

    void Start()
    {
        popPosition = transform.position;
        _popCnt = popCardSystem.GetComponent<PopCard>().popCnt; // 生成時の枚数を記録
        summoned = false;
        GetComponent<Grabbable>().CanGrab = true;
    }

    void Update()
    {
        // ---- startPhase: カードを引いたら次を自動生成 ----
        // カードが動いた（引かれた）かつ未実行かつ5枚未満のとき、1秒後に次のカードをデッキに積む
        if (transform.position != popPosition && !hasExecuted && popCardSystem.GetComponent<PopCard>().popCnt < 5 && phaseController.GetComponent<PhaseController>().startPhase)
        {
            StartCoroutine(Pop(1f));
            hasExecuted = true;
        }

        // startPhaseで5枚目を引いたとき、2秒後にstartPhaseを終了させる
        if (phaseController.GetComponent<PhaseController>().startPhase == true && popCardSystem.GetComponent<PopCard>().popCnt == 5 && transform.position != popPosition)
        {
            StartCoroutine(FinishStartPhase());
        }

        // ---- drawPhase: 1枚ドローフェーズでカードを引いたときの処理 ----
        // _popCnt で「このカードが drawPhase 用に生成されたもの」かを識別し、
        // 引かれたら drawPhase を終了させる
        if (phaseController.GetComponent<PhaseController>().drawPhase == true && popCardSystem.GetComponent<PopCard>().popCnt >= 6 && transform.position != popPosition && _popCnt == popCardSystem.GetComponent<PopCard>().popCnt)
        {
            StartCoroutine(FinishDrawPhase());
        }

        popPosition = transform.position; // 次フレームの比較用に位置を更新
    }

    // 1秒後にデッキ先頭に次のカードを生成する（startPhase用連続ドロー）
    IEnumerator Pop(float delay)
    {
        yield return new WaitForSeconds(delay);
        popCard.PopCardToDeckHead();
    }

    // 2秒後に startPhase を false にしてフェーズを進める
    IEnumerator FinishStartPhase()
    {
        yield return new WaitForSeconds(2f);
        phaseController.GetComponent<PhaseController>().startPhase = false;
    }

    // 2秒後に drawPhase を false にしてフェーズを進める
    IEnumerator FinishDrawPhase()
    {
        yield return new WaitForSeconds(2f);
        phaseController.GetComponent<PhaseController>().drawPhase = false;
    }
}
