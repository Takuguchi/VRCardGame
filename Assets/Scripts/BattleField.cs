using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using OculusSampleFramework;
using Oculus.Interaction;

/// <summary>
/// プレイヤー側のバトルフィールドを管理するクラス。
/// VRコントローラーでグラブしたカードがこのコライダーに入ったとき、
/// そのカードをバトルフィールドに固定し selectPhase を終了させる。
/// カードがコライダー内に留まっている間フィールドを黄色くハイライトする。
/// </summary>
public class BattleField : MonoBehaviour
{
    private Color originalColor;        // ハイライト前のフィールド色（元に戻す用）
    private bool IsStay;
    private Vector3 pos;                // このフィールドオブジェクトの座標（キャッシュ）
    public bool doneMove;               // カードの配置が完了したかどうか
    public Transform summonLocation;    // バトルフィールド上のモデル配置位置
    public GameObject popCardSystem;
    public GameObject summonfieldPrefab;
    public GameObject phaseController;
    public GameObject battleSystem;
    private GameObject summoned;        // バトルフィールドに置かれたカードフィールドオブジェクト（未使用）
    private GameObject summonedModel;   // バトルフィールドに置かれた3Dモデル（未使用）
    private GameObject summonedEffect;
    public int triggerCount;            // 現在コライダー内にあるカードの数
    private bool selectPhase;
    public bool firstLocateDup;         // 同一カードの2重配置を防ぐフラグ
    private int summonCnt;
    private Vector3 originalLoation, originalModelLocation, originalFieldLocation; // バトル後の復元用座標
    public int selectNumSelf;           // バトルに出したカードのベンチインデックス（-1=未選択）
    public GameObject suggestion;       // バトルフィールドへの案内オブジェクト

    private void Start()
    {
        originalColor = GetComponent<Renderer>().material.color;
        doneMove = false;
        pos = GetComponent<Transform>().position;
        triggerCount = 0;
        firstLocateDup = false;
        selectNumSelf = -1;
    }

    private void Update()
    {
        // PhaseControllerから現在のフェーズ情報を毎フレーム取得
        summonCnt = phaseController.GetComponent<PhaseController>().benchSummonCnt;
        selectPhase = phaseController.GetComponent<PhaseController>().selectPhase;
    }

    /// <summary>
    /// カードが入ってきたときにフィールドをハイライト。
    /// selectPhase中かつ未配置の場合のみ黄色くする。
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Card")
        {
            triggerCount++;
            if (selectPhase && firstLocateDup == false && triggerCount == 1)
            {
                GameObject parentObj = other.gameObject.transform.parent.transform.parent.transform.parent.gameObject;
                if (parentObj.GetComponent<Card>().summoned)
                {
                    GetComponent<Renderer>().material.color = Color.yellow; // フィールドをハイライト
                }
            }
        }
    }

    /// <summary>
    /// カードが出ていったときにフィールドの色を元に戻す。
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Card")
        {
            triggerCount--;
            if (triggerCount == 0)
            {
                GetComponent<Renderer>().material.color = originalColor;
                if (firstLocateDup)
                {
                    firstLocateDup = false;
                }
            }
        }
    }

    /// <summary>
    /// カードがコライダー内に留まっている間、配置条件を満たしたらカードを固定する。
    /// 条件: selectPhase中 / 未配置 / ベンチ済みカード / グラブを離した状態
    /// 配置完了後は selectPhase を false にしてフェーズを進める。
    /// </summary>
    private void OnTriggerStay(Collider other)
    {
        if (doneMove == false && other.gameObject.tag == "Card" && selectPhase && firstLocateDup == false)
        {
            Transform card_transform = other.gameObject.GetComponent<Transform>();
            // コライダーの親を3階層遡ってカードのルートオブジェクトを取得
            Transform root = card_transform.parent.transform.parent.transform.parent;
            Transform root_transform = root.GetComponent<Transform>();
            GameObject root_obj = root.gameObject;
            battleSystem.GetComponent<BattleSystem>().rootMyCard = root_obj; // BattleSystemに登録

            if (root_obj.GetComponent<Card>().summoned)
            {
                if (!root_obj.GetComponent<IsGrabOn>().IsGrab) // グラブを離したタイミングで固定
                {
                    DoMove(root_obj); // カードをフィールド中央にスナップ

                    // ベンチカードリストからこのカードのインデックスを取得
                    selectNumSelf = popCardSystem.GetComponent<PopCard>().benchCards.IndexOf(root_obj);
                    if (selectNumSelf == -1)
                    {
                        throw new System.Exception("missing GameObject serch.");
                    }

                    doneMove = true;

                    // カードに紐づく3Dモデルとフィールドをバトルフィールド位置へ移動
                    GameObject _field = root_obj.gameObject.GetComponent<Card>().cardField;
                    GameObject _model = root_obj.gameObject.GetComponent<Card>().model3D;
                    originalFieldLocation = _field.transform.position;
                    originalModelLocation = _model.transform.position;
                    _model.transform.position = summonLocation.position;
                    _field.transform.position = summonLocation.position;

                    // カードテクスチャから登場ボイスを取得して再生
                    Transform tr2 = root_obj.transform;
                    Transform targetTransform = tr2.Find("Visuals/Root/Black_PlayingCards_Spade10_00");
                    GameObject targetObject = targetTransform.gameObject;
                    Renderer renderer = targetObject.GetComponent<Renderer>();
                    Material material = renderer.material;
                    Texture texture = material.GetTexture("_MainTex2");
                    popCardSystem.GetComponent<PopCard>().texture2audio[texture.name].Play();

                    phaseController.GetComponent<PhaseController>().selectPhase = false; // selectPhase終了
                }
            }
        }
        if (other.gameObject.tag == "Card" && selectPhase == false)
        {
            firstLocateDup = true; // フェーズ外のカードが来ても無視するためのフラグ
        }
    }

    /// <summary>
    /// カードをフィールドの中央にスナップし、グラブを無効化してガイドを非表示にする。
    /// </summary>
    private void DoMove(GameObject card)
    {
        GetComponent<Renderer>().material.color = originalColor;
        Transform root_transform = card.GetComponent<Transform>();
        root_transform.rotation = Quaternion.Euler(0, 0, 0);
        root_transform.position = new Vector3(pos.x, pos.y + 0.01f, pos.z); // フィールド上に置く
        card.GetComponent<Grabbable>().CanGrab = false; // 置いたらグラブ不可に
        suggestion.SetActive(false); // ガイド矢印を非表示
    }

    /// <summary>
    /// summonPhaseで使用していたカード召喚処理（現在は CardColliderEvent に移管のため未使用）。
    /// </summary>
    private void Summon(GameObject card)
    {
        Renderer renderer = card.GetComponent<Renderer>();
        Material material = renderer.material;
        Texture texture = material.GetTexture("_MainTex2");
        GameObject model3d = popCardSystem.GetComponent<PopCard>().texture2models[texture.name];
        GameObject summonEffect = popCardSystem.GetComponent<PopCard>().texture2summonEffects[texture.name];
        summoned = Instantiate(summonfieldPrefab, summonLocation.position, Quaternion.Euler(90, 0, 0));
        renderer = summoned.GetComponent<Renderer>();
        material = renderer.material;
        material.SetTexture("_MainTex", texture);
        summonedModel = Instantiate(model3d, summonLocation.position, Quaternion.Euler(0, 0, 0));
        summonCnt++;
    }

    /// <summary>
    /// 召喚エフェクト再生後にモデルを生成するコルーチン（現在コメントアウト済み・未使用）。
    /// </summary>
    private IEnumerator WaitAndDoSomething(GameObject model3d)
    {
        summonedModel = Instantiate(model3d, summonLocation.position, Quaternion.Euler(0, 0, 0));
        yield return new WaitForSeconds(1.0f);
    }

    /// <summary>
    /// プレイヤー勝利後にバトルに出したカード・3Dモデル・フィールドをベンチ位置に戻す。
    /// PhaseController から呼ばれる。
    /// </summary>
    public void returnPosition(Vector3 _pos)
    {
        GameObject card = battleSystem.GetComponent<BattleSystem>().rootMyCard;
        card.transform.position = _pos;
        GameObject _field = card.GetComponent<Card>().cardField;
        GameObject _model = card.GetComponent<Card>().model3D;
        _model.transform.position = originalModelLocation;
        _field.transform.position = originalFieldLocation;
    }

    public void resetStatus()
    {
        doneMove = false;
    }
}
