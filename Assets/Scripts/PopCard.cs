using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// カードの生成・管理と、テクスチャ名をキーにしたモンスター情報の辞書を保持するクラス。
/// カードプレハブを指定位置にInstantiateし、テクスチャを差し替えることで
/// 見た目の異なるカードを動的に生成する。
/// </summary>
public class PopCard : MonoBehaviour
{
    // ---- Inspectorから設定するアセット参照 ----
    public GameObject OriginalCardPrefab;      // カードの基底プレハブ
    public Transform tr;                        // カード生成基準位置
    public List<Texture2D> textures;           // 各モンスターのカードテクスチャ一覧
    public List<GameObject> models3d;          // 各モンスターの3Dモデルプレハブ一覧
    public List<GameObject> summonEffects;     // 召喚エフェクトプレハブ一覧（現在未使用）
    public List<GameObject> handModels;        // じゃんけんの手（グー・チョキ・パー）モデル一覧
    public List<int> numTaiki;                 // 各モンスターの待機アニメーション数（ランダム再生に使用）
    public List<string> hands;                 // 各モンスターの持ちて文字列（"Rock","Scissors","Paper"）
    public List<AudioSource> toujouVoices;     // 各モンスターの登場ボイス

    // ---- テクスチャ名をキーにしたルックアップ辞書 ----
    // テクスチャ名でモンスター情報を O(1) で引けるように Start() でビルドする
    public Dictionary<string, GameObject> texture2models;       // テクスチャ名 → 3Dモデル
    public Dictionary<string, float> texture2HandPosZ;          // テクスチャ名 → 手モデルのY座標
    public Dictionary<string, GameObject> texture2handmodels;   // テクスチャ名 → 手モデル
    public Dictionary<string, int> texture2numTaiki;            // テクスチャ名 → 待機アニメ数
    public Dictionary<string, GameObject> texture2summonEffects;// テクスチャ名 → 召喚エフェクト
    public Dictionary<string, AudioSource> texture2audio;       // テクスチャ名 → 登場ボイス
    public Dictionary<string, string> texture2hands;            // テクスチャ名 → じゃんけんの手

    public int popCnt;                  // デッキから引いた累計枚数（フェーズ判定に使用）
    public GameObject phaseController;  // フェーズ制御オブジェクト
    public List<GameObject> benchCards; // ベンチに置かれたカードオブジェクト一覧
    public List<GameObject> benchField; // ベンチフィールドオブジェクト一覧（CardColliderEvent）

    [SerializeField]
    private List<float> handlPosZ;      // 手モデルのY座標リスト（Inspectorで設定）

    private float posx, posy, posz;     // カード生成基準座標（キャッシュ）

    public void Start()
    {
        // 各辞書を初期化し、テクスチャリストのインデックスを対応付けてデータを格納
        texture2models = new Dictionary<string, GameObject>();
        texture2summonEffects = new Dictionary<string, GameObject>();
        texture2hands = new Dictionary<string, string>();
        texture2numTaiki = new Dictionary<string, int>();
        texture2HandPosZ = new Dictionary<string, float>();
        texture2handmodels = new Dictionary<string, GameObject>();
        texture2audio = new Dictionary<string, AudioSource>();
        for (int i = 0; i < textures.Count; i++)
        {
            texture2models[textures[i].name] = models3d[i];
            texture2summonEffects[textures[i].name] = summonEffects[i];
            texture2hands[textures[i].name] = hands[i];
            texture2numTaiki[textures[i].name] = numTaiki[i];
            texture2handmodels[textures[i].name] = handModels[i];
            texture2HandPosZ[textures[i].name] = handlPosZ[i];
            texture2audio[textures[i].name] = toujouVoices[i];
        }

        // 生成基準座標をキャッシュ
        posx = tr.position.x;
        posy = tr.position.y;
        posz = tr.position.z;

        popCnt = 1; // 初回は既に1枚生成済みとして扱う
    }

    /// <summary>
    /// デッキの先頭（固定座標）にランダムなカードを1枚生成する。
    /// startPhase/drawPhase での手札補充に使用。
    /// </summary>
    public void PopCardToDeckHead()
    {
        Texture2D texture = GetRandomTexture(textures);
        // デッキの先頭座標に裏向き（Euler 0,0,180）でカードプレハブを生成
        GameObject card = Instantiate(OriginalCardPrefab, new Vector3(0.591f, 0.956f, 0.2787f) , Quaternion.Euler(0, 0, 180));
        Transform tr2 = card.transform;
        Transform targetTransform = tr2.Find("Visuals/Root/Black_PlayingCards_Spade10_00");
        if (targetTransform != null)
        {
            GameObject targetObject = targetTransform.gameObject;
            Renderer renderer = targetObject.GetComponent<Renderer>();
            Material material = renderer.material;
            // _MainTex2 スロットにモンスターのテクスチャを差し替えてカードの見た目を設定
            material.SetTexture("_MainTex2", texture);
        }
        card.SetActive(true);
        popCnt++;
    }

    /// <summary>
    /// テクスチャリストの先頭3種をそれぞれ使ったカードを3枚生成して返す。
    /// 敵の手札初期化（EnemyManager.Start）で使用。
    /// </summary>
    public List<GameObject> ChooseThreeCards()
    {
        List<GameObject> threeCards = new List<GameObject>();
        for (int i=0; i<3; i++)
        {
            Texture2D texture = textures[i]; // 固定インデックスで3種のモンスターカードを選択
            GameObject card = Instantiate(OriginalCardPrefab, new Vector3(posx, posy, posz), Quaternion.Euler(0, 0, 180));
            Transform tr2 = card.transform;
            Transform targetTransform = tr2.Find("Visuals/Root/Black_PlayingCards_Spade10_00");
            if (targetTransform != null)
            {
                GameObject targetObject = targetTransform.gameObject;
                Renderer renderer = targetObject.GetComponent<Renderer>();
                Material material = renderer.material;
                // _MainTex2 スロットにモンスターのテクスチャを差し替えてカードの見た目を設定
                material.SetTexture("_MainTex2", texture);
            }
            threeCards.Add(card);
        }
        return threeCards;
    }

    /// <summary>
    /// ランダムなテクスチャを持つカードを1枚生成して返す。
    /// バトル後の敵手札補充（EnemyManager.RefullCard）で使用。
    /// </summary>
    public GameObject GetRandomCard()
    {
        Texture2D texture = GetRandomTexture(textures);
        GameObject card = Instantiate(OriginalCardPrefab, new Vector3(posx, posy, posz), Quaternion.Euler(0, 0, 180));
        Transform tr2 = card.transform;
        Transform targetTransform = tr2.Find("Visuals/Root/Black_PlayingCards_Spade10_00");
        if (targetTransform != null)
        {
            GameObject targetObject = targetTransform.gameObject;
            Renderer renderer = targetObject.GetComponent<Renderer>();
            Material material = renderer.material;
            // _MainTex2 スロットにモンスターのテクスチャを差し替えてカードの見た目を設定
            material.SetTexture("_MainTex2", texture);
        }
        return card;
    }

    /// <summary>
    /// リストからランダムに1つのTexture2Dを返すヘルパー。
    /// </summary>
    private Texture2D GetRandomTexture(List<Texture2D> list)
    {
        if (list == null || list.Count == 0) return null;
        int randomIndex = Random.Range(0, list.Count);
        return list[randomIndex];
    }

    /// <summary>
    /// ベンチに置かれた全カードのグラブを有効化する。
    /// selectPhase 開始時にプレイヤーがカードを持てるようにするために呼ぶ。
    /// </summary>
    public void changeCardsCanGrab()
    {
        for (int i = 0; i < benchCards.Count; i++)
        {
            benchCards[i].GetComponent<Grabbable>().CanGrab = true;
        }
    }

    /// <summary>
    /// ベンチに置かれた全カードのグラブを無効化する。
    /// battlePhase 開始時に誤操作を防ぐために呼ぶ。
    /// </summary>
    public void changeCardsCannotGrab()
    {
        for (int i = 0; i < benchCards.Count; i++)
        {
            benchCards[i].GetComponent<Grabbable>().CanGrab = false;
        }
    }
}
