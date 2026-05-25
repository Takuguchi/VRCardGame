using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using OculusSampleFramework;
using Oculus.Interaction;

/// <summary>
/// 敵フィールド側のベンチゾーン（左・中・右の3か所）の一つに対応するクラス。
/// VRコントローラーでグラブしたカードがこのコライダーに入った際、
/// カードをベンチに固定し3Dモデルを召喚する（summonPhase 専用）。
/// BattleField（プレイヤー側 selectPhase 用）と対になる設計。
/// </summary>
public class CardColliderEvent : MonoBehaviour
{
    private Color originalColor;        // ハイライト前のフィールド色
    private bool IsStay;
    private Vector3 pos;                // このベンチゾーンの座標（キャッシュ）
    public bool doneMove;               // カードの配置が完了したかどうか
    public Transform summonLocation;    // 3Dモデルを配置するベンチ上の位置
    public GameObject popCardSystem;
    public GameObject summonfieldPrefab; // ベンチフィールドのプレハブ
    public GameObject phaseController;
    private GameObject summoned;        // ベンチに生成したフィールドオブジェクト
    private GameObject summonedModel;   // ベンチに生成した3Dモデル
    private GameObject summonedEffect;
    private int triggerCount;           // 現在コライダー内にあるカードの数
    private bool summonPhase;
    private bool firstLocateDup;        // 同一カードの2重配置防止フラグ
    private Animator animator;
    public GameObject battleField;      // BattleField参照（selectNumSelf の確認用）
    public GameObject suggestion;       // このベンチゾーンへの案内オブジェクト
    public int locationNum;             // このベンチゾーンの番号（0=左, 1=中, 2=右）

    private void Start()
    {
        originalColor = GetComponent<Renderer>().material.color;
        doneMove = false;
        pos = GetComponent<Transform>().position;
        triggerCount = 0;
        firstLocateDup = false;
    }

    private void Update()
    {
        // PhaseControllerから summonPhase の状態を毎フレーム取得
        summonPhase = phaseController.GetComponent<PhaseController>().summonPhase;
    }

    /// <summary>
    /// カードが入ってきたときにベンチゾーンをハイライト。
    /// summonPhase中かつ未配置の最初の1枚のみ黄色くする。
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Card")
        {
            triggerCount++;
            if (summonPhase && firstLocateDup == false && triggerCount == 1)
            {
                GetComponent<Renderer>().material.color = Color.yellow; // ハイライト
            }
        }
    }

    /// <summary>
    /// カードが出ていったときにハイライトを解除。
    /// カードが置かれた後に引き抜かれた場合は召喚済みモデルを破棄する。
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Card")
        {
            triggerCount--;
            if (triggerCount == 0)
            {
                GetComponent<Renderer>().material.color = originalColor; // 元の色に戻す
                if (firstLocateDup)
                {
                    firstLocateDup = false;
                }
            }
            // summonPhase中に配置済みカードが引き抜かれた場合、召喚を取り消す
            if (summonPhase)
            {
                if (doneMove && triggerCount == 0)
                {
                    if (summonedModel != null && summoned != null)
                    {
                        Destroy(summonedModel);
                        Destroy(summoned);
                        doneMove = false;
                    }
                }
            }
        }
    }

    /// <summary>
    /// カードがコライダー内に留まっている間、配置条件を満たしたらカードを固定して召喚する。
    /// 条件: summonPhase中 / 未配置 / グラブを離した状態
    /// </summary>
    private void OnTriggerStay(Collider other)
    {
        if (doneMove == false && other.gameObject.tag == "Card" && summonPhase && firstLocateDup == false)
        {
            Transform card_transform = other.gameObject.GetComponent<Transform>();
            // コライダーの親を3階層遡ってカードのルートオブジェクトを取得
            Transform root = card_transform.parent.transform.parent.transform.parent;
            Transform root_transform = root.GetComponent<Transform>();
            GameObject root_obj = root.gameObject;

            if (!root_obj.GetComponent<IsGrabOn>().IsGrab) // グラブを離したタイミングで固定
            {
                DoMove(root_obj);   // カードをゾーン中央にスナップ
                doneMove = true;
                Summon(other.gameObject, root_obj); // 3Dモデルをベンチに召喚
            }
        }
        if (other.gameObject.tag == "Card" && summonPhase == false)
        {
            firstLocateDup = true; // フェーズ外のカードが来ても無視するためのフラグ
        }
    }

    /// <summary>
    /// カードをゾーン中央にスナップし、グラブを無効化する。
    /// benchCards / benchField リストへの登録も行う。
    /// </summary>
    private void DoMove(GameObject card)
    {
        GetComponent<Renderer>().material.color = originalColor;
        Transform root_transform = card.GetComponent<Transform>();
        root_transform.rotation = Quaternion.Euler(0, 0, 0);
        root_transform.position = new Vector3(pos.x, pos.y + 0.01f, pos.z); // ベンチ上に置く
        card.GetComponent<Grabbable>().CanGrab = false; // 置いたらグラブ不可
        card.GetComponent<Card>().summoned = true;       // ベンチ済みフラグを立てる

        // selectPhaseでバトルに出るカードがまだ決まっていない初回の場合は追加、
        // 決まっている場合（前のターンで負けた枠）は上書き
        if (battleField.GetComponent<BattleField>().selectNumSelf == -1)
        {
            popCardSystem.GetComponent<PopCard>().benchCards.Add(card);
            popCardSystem.GetComponent<PopCard>().benchField.Add(this.gameObject);
        }
        else
        {
            popCardSystem.GetComponent<PopCard>().benchCards[battleField.GetComponent<BattleField>().selectNumSelf] = card;
        }
        suggestion.SetActive(false); // ガイド矢印を非表示
    }

    /// <summary>
    /// ベンチゾーンにカードフィールドと3Dモンスターモデルを生成し、
    /// 召喚アニメーションと登場ボイスを再生する。
    /// PhaseController の benchSummonCnt をインクリメントしてフェーズ進行を制御する。
    /// </summary>
    private void Summon(GameObject card, GameObject rootObj)
    {
        // カードのテクスチャ名からモンスター情報を辞書で引く
        Renderer renderer = card.GetComponent<Renderer>();
        Material material = renderer.material;
        Texture texture = material.GetTexture("_MainTex2");
        GameObject model3d = popCardSystem.GetComponent<PopCard>().texture2models[texture.name];
        GameObject summonEffect = popCardSystem.GetComponent<PopCard>().texture2summonEffects[texture.name];

        // カードフィールドプレハブを召喚位置に生成し、表面テクスチャを設定
        summoned = Instantiate(summonfieldPrefab, summonLocation.position, Quaternion.Euler(90, 0, 0));
        renderer = summoned.GetComponent<Renderer>();
        material = renderer.material;
        material.SetTexture("_MainTex", texture);

        // 3Dモデルを生成して召喚アニメーションを再生
        summonedModel = Instantiate(model3d, summonLocation.position, Quaternion.Euler(0, 0, 0));
        popCardSystem.GetComponent<PopCard>().texture2audio[texture.name].Play(); // 登場ボイス

        // 待機アニメーションをランダムに設定（モンスターごとに複数パターンある）
        animator = summonedModel.GetComponent<Animator>();
        animator.SetTrigger("SyoukanRoar");
        animator.SetInteger("taiki", Random.Range(0, popCardSystem.GetComponent<PopCard>().texture2numTaiki[texture.name]));

        // フェーズ制御用カウントをインクリメント（3になると summonPhase 終了）
        phaseController.GetComponent<PhaseController>().benchSummonCnt++;

        // カードオブジェクトに生成したモデルとフィールドを紐づける
        rootObj.GetComponent<Card>().model3D = summonedModel;
        rootObj.GetComponent<Card>().cardField = summoned;
    }

    /// <summary>
    /// 召喚エフェクト再生後にモデルを生成するコルーチン（現在コメントアウト済み・未使用）。
    /// </summary>
    private IEnumerator WaitAndDoSomething(GameObject model3d)
    {
        summonedModel = Instantiate(model3d, summonLocation.position, Quaternion.Euler(0, 0, 0));
        yield return new WaitForSeconds(1.0f);
    }
}
