using System.Collections;
using System.Collections.Generic;
using OVR.OpenVR;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 敵のカード手札・ベンチ・バトル選択を管理するクラス。
/// プレイヤーと同様に3枚のカードをベンチに持ち、selectPhaseでランダムに1枚を選んでバトルに出す。
/// バトル結果に応じてカードの補充・位置の復元も行う。
/// </summary>
public class EnemyManager : MonoBehaviour
{
    public GameObject popCardSystem;
    public List<GameObject> HandCards;      // 敵の手札（カードオブジェクト3枚）
    public PopCard popCard;
    public GameObject phaseController;

    // ベンチの3か所の位置（Transformで定義）
    public Transform leftLocation;
    public Transform midLocation;
    public Transform rightLocation;

    // ベンチに置くカードフィールドプレハブ（裏向きカード表示用）
    public GameObject leftField;
    public GameObject rightField;
    public GameObject midField;

    public Texture uramen;                  // ベンチ裏面テクスチャ（summonPhase中は伏せる）
    public GameObject battleSystem;
    public Transform battleLocation;        // バトルフィールドの位置
    public GameObject battleField;          // プレイヤーのバトルフィールド参照
    public bool looseFlag;                  // 前ターンで敵が負けたかどうか（補充判定に使用）

    // ベンチに生成したカードフィールドオブジェクト（3か所）
    private GameObject leftSummoned, rightSummoned, midSummoned;
    // ベンチに生成した3Dモデルと手モデル（各3体）
    private GameObject leftModel, midModel, rightModel, leftHandModel, midHandModel, rightHandModel;

    public int selectNum = -1;              // バトルに選んだカードのインデックス（-1=未選択）
    public List<GameObject> enemySummoneds; // ベンチの3Dモデル一覧（BattleSystemから参照）
    public List<GameObject> handModels;     // ベンチの手モデル一覧
    private Animator animator;

    void Start()
    {
        selectNum = -1;
        looseFlag = false;
        enemySummoneds = new List<GameObject>();
        // ゲーム開始時に敵の手札として3枚のカードを生成
        HandCards = popCard.ChooseThreeCards();
    }

    /// <summary>
    /// summonPhaseで敵のベンチにカードを配置する。
    /// 初回は3枚すべてを裏向きで配置。2ターン目以降は前ターンで負けた枠のみ補充する。
    /// </summary>
    public void PlayBench()
    {
        if (selectNum == -1)
        {
            // 初回：左・中・右すべてに裏向きカードを配置
            leftSummoned = Instantiate(leftField, leftLocation.position, Quaternion.Euler(90, 0, 0));
            Renderer renderer = leftSummoned.GetComponent<Renderer>();
            Material material = renderer.material;
            material.SetTexture("_MainTex", uramen); // 裏面テクスチャを適用
            leftSummoned.SetActive(true);

            midSummoned = Instantiate(midField, midLocation.position, Quaternion.Euler(90, 0, 0));
            renderer = midSummoned.GetComponent<Renderer>();
            material = renderer.material;
            material.SetTexture("_MainTex", uramen);
            midSummoned.SetActive(true);

            rightSummoned = Instantiate(rightField, rightLocation.position, Quaternion.Euler(90, 0, 0));
            renderer = rightSummoned.GetComponent<Renderer>();
            material = renderer.material;
            material.SetTexture("_MainTex", uramen);
            rightSummoned.SetActive(true);
        }
        // 2ターン目以降、前回バトルで負けた（looseFlag==true）枠のみ補充
        else if(selectNum == 0 && looseFlag == true)
        {
            leftSummoned = Instantiate(leftField, leftLocation.position, Quaternion.Euler(90, 0, 0));
            Renderer renderer = leftSummoned.GetComponent<Renderer>();
            Material material = renderer.material;
            material.SetTexture("_MainTex", uramen);
            leftSummoned.SetActive(true);
        }
        else if(selectNum == 1 && looseFlag == true)
        {
            midSummoned = Instantiate(midField, midLocation.position, Quaternion.Euler(90, 0, 0));
            Renderer renderer = midSummoned.GetComponent<Renderer>();
            Material material = renderer.material;
            material.SetTexture("_MainTex", uramen);
            midSummoned.SetActive(true);
        }
        else if(selectNum == 2 && looseFlag == true)
        {
            rightSummoned = Instantiate(rightField, rightLocation.position, Quaternion.Euler(90, 0, 0));
            Renderer renderer = rightSummoned.GetComponent<Renderer>();
            Material material = renderer.material;
            material.SetTexture("_MainTex", uramen);
            rightSummoned.SetActive(true);
        }
    }

    /// <summary>
    /// selectPhaseで敵ベンチを表向きに公開し、3Dモンスターモデルと手モデルを生成する。
    /// 初回は3体すべてを生成し、2ターン目以降は負けた枠のみ差し替える。
    /// </summary>
    public void OpenBench()
    {
        if (selectNum == -1)
        {
            // 初回：左・中・右すべてにモデルを生成

            // --- 左枠 ---
            Transform tr2 = HandCards[0].transform;
            Transform targetTransform = tr2.Find("Visuals/Root/Black_PlayingCards_Spade10_00");
            GameObject targetObject = targetTransform.gameObject;
            Renderer renderer = targetObject.GetComponent<Renderer>();
            Material material = renderer.material;
            Texture texture = material.GetTexture("_MainTex2"); // カードのテクスチャを取得

            // カードフィールドの表面テクスチャを更新（裏→表）
            renderer = leftSummoned.GetComponent<Renderer>();
            material = renderer.material;
            material.SetTexture("_MainTex", texture);

            // 手モデルと3Dモデルをベンチ座標に生成
            leftHandModel = Instantiate(popCardSystem.GetComponent<PopCard>().texture2handmodels[texture.name], new Vector3(leftLocation.position.x, popCardSystem.GetComponent<PopCard>().texture2HandPosZ[texture.name], leftLocation.position.z), Quaternion.Euler(0, 0, 0));
            leftModel = Instantiate(popCardSystem.GetComponent<PopCard>().texture2models[texture.name], leftLocation.position, Quaternion.Euler(0, 180f, 0));
            animator = leftModel.GetComponent<Animator>();
            animator.SetTrigger("SyoukanRoar"); // 召喚時の咆哮アニメーション
            animator.SetInteger("taiki", Random.Range(0, popCardSystem.GetComponent<PopCard>().texture2numTaiki[texture.name])); // 待機アニメをランダム選択
            enemySummoneds.Add(leftModel);
            handModels.Add(leftHandModel);

            // --- 中枠 ---
            tr2 = HandCards[1].transform;
            targetTransform = tr2.Find("Visuals/Root/Black_PlayingCards_Spade10_00");
            targetObject = targetTransform.gameObject;
            renderer = targetObject.GetComponent<Renderer>();
            material = renderer.material;
            texture = material.GetTexture("_MainTex2");

            renderer = midSummoned.GetComponent<Renderer>();
            material = renderer.material;
            material.SetTexture("_MainTex", texture);
            midHandModel = Instantiate(popCardSystem.GetComponent<PopCard>().texture2handmodels[texture.name], new Vector3(midLocation.position.x, popCardSystem.GetComponent<PopCard>().texture2HandPosZ[texture.name], midLocation.position.z), Quaternion.Euler(0, 0, 0));
            midModel = Instantiate(popCardSystem.GetComponent<PopCard>().texture2models[texture.name], midLocation.position, Quaternion.Euler(0, 180f, 0));
            animator = midModel.GetComponent<Animator>();
            animator.SetTrigger("SyoukanRoar");
            animator.SetInteger("taiki", Random.Range(0, popCardSystem.GetComponent<PopCard>().texture2numTaiki[texture.name]));
            enemySummoneds.Add(midModel);
            handModels.Add(midHandModel);

            // --- 右枠 ---
            tr2 = HandCards[2].transform;
            targetTransform = tr2.Find("Visuals/Root/Black_PlayingCards_Spade10_00");
            targetObject = targetTransform.gameObject;
            renderer = targetObject.GetComponent<Renderer>();
            material = renderer.material;
            texture = material.GetTexture("_MainTex2");

            renderer = rightSummoned.GetComponent<Renderer>();
            material = renderer.material;
            material.SetTexture("_MainTex", texture);
            rightHandModel = Instantiate(popCardSystem.GetComponent<PopCard>().texture2handmodels[texture.name], new Vector3(rightLocation.position.x, popCardSystem.GetComponent<PopCard>().texture2HandPosZ[texture.name], rightLocation.position.z), Quaternion.Euler(0, 0, 0));
            rightModel = Instantiate(popCardSystem.GetComponent<PopCard>().texture2models[texture.name], rightLocation.position, Quaternion.Euler(0, 180f, 0));
            animator = rightModel.GetComponent<Animator>();
            animator.SetTrigger("SyoukanRoar");
            animator.SetInteger("taiki", Random.Range(0, popCardSystem.GetComponent<PopCard>().texture2numTaiki[texture.name]));
            enemySummoneds.Add(rightModel);
            handModels.Add(rightHandModel);
        }
        // 2ターン目以降：前回負けた枠のモデルだけ差し替え、残りの手モデルを再表示
        else if(selectNum == 0 && looseFlag == true)
        {
            Transform tr2 = HandCards[0].transform;
            Transform targetTransform = tr2.Find("Visuals/Root/Black_PlayingCards_Spade10_00");
            GameObject targetObject = targetTransform.gameObject;
            Renderer renderer = targetObject.GetComponent<Renderer>();
            Material material = renderer.material;
            Texture texture = material.GetTexture("_MainTex2");
            renderer = leftSummoned.GetComponent<Renderer>();
            material = renderer.material;
            material.SetTexture("_MainTex", texture);
            leftHandModel = Instantiate(popCardSystem.GetComponent<PopCard>().texture2handmodels[texture.name], new Vector3(leftLocation.position.x, popCardSystem.GetComponent<PopCard>().texture2HandPosZ[texture.name], leftLocation.position.z), Quaternion.Euler(0, 0, 0));
            leftModel = Instantiate(popCardSystem.GetComponent<PopCard>().texture2models[texture.name], leftLocation.position, Quaternion.Euler(0, 180f, 0));
            animator = leftModel.GetComponent<Animator>();
            animator.SetTrigger("SyoukanRoar");
            animator.SetInteger("taiki", Random.Range(0, popCardSystem.GetComponent<PopCard>().texture2numTaiki[texture.name]));
            enemySummoneds[0] = leftModel;
            handModels[0] = leftHandModel;
            handModels[1].SetActive(true);
            handModels[2].SetActive(true);
        }
        else if(selectNum == 1 && looseFlag == true)
        {
            Transform tr2 = HandCards[1].transform;
            Transform targetTransform = tr2.Find("Visuals/Root/Black_PlayingCards_Spade10_00");
            GameObject targetObject = targetTransform.gameObject;
            Renderer renderer = targetObject.GetComponent<Renderer>();
            Material material = renderer.material;
            Texture texture = material.GetTexture("_MainTex2");
            renderer = midSummoned.GetComponent<Renderer>();
            material = renderer.material;
            material.SetTexture("_MainTex", texture);
            midHandModel = Instantiate(popCardSystem.GetComponent<PopCard>().texture2handmodels[texture.name], new Vector3(midLocation.position.x, popCardSystem.GetComponent<PopCard>().texture2HandPosZ[texture.name], midLocation.position.z), Quaternion.Euler(0, 0, 0));
            midModel = Instantiate(popCardSystem.GetComponent<PopCard>().texture2models[texture.name], midLocation.position, Quaternion.Euler(0, 180f, 0));
            animator = midModel.GetComponent<Animator>();
            animator.SetTrigger("SyoukanRoar");
            animator.SetInteger("taiki", Random.Range(0, popCardSystem.GetComponent<PopCard>().texture2numTaiki[texture.name]));
            enemySummoneds[1] = midModel;
            handModels[1] = midHandModel;
            handModels[0].SetActive(true);
            handModels[2].SetActive(true);
        }
        else if (selectNum == 2 && looseFlag == true)
        {
            Transform tr2 = HandCards[2].transform;
            Transform targetTransform = tr2.Find("Visuals/Root/Black_PlayingCards_Spade10_00");
            GameObject targetObject = targetTransform.gameObject;
            Renderer renderer = targetObject.GetComponent<Renderer>();
            Material material = renderer.material;
            Texture texture = material.GetTexture("_MainTex2");
            renderer = rightSummoned.GetComponent<Renderer>();
            material = renderer.material;
            material.SetTexture("_MainTex", texture);
            rightHandModel = Instantiate(popCardSystem.GetComponent<PopCard>().texture2handmodels[texture.name], new Vector3(rightLocation.position.x, popCardSystem.GetComponent<PopCard>().texture2HandPosZ[texture.name], rightLocation.position.z), Quaternion.Euler(0, 0, 0));
            rightModel = Instantiate(popCardSystem.GetComponent<PopCard>().texture2models[texture.name], rightLocation.position, Quaternion.Euler(0, 180f, 0));
            animator = rightModel.GetComponent<Animator>();
            animator.SetTrigger("SyoukanRoar");
            animator.SetInteger("taiki", Random.Range(0, popCardSystem.GetComponent<PopCard>().texture2numTaiki[texture.name]));
            enemySummoneds[2] = rightModel;
            handModels[2] = rightHandModel;
            handModels[0].SetActive(true);
            handModels[1].SetActive(true);
        }

        // 初回は全ての手モデルを表示
        if (looseFlag == false)
        {
            for (int i=0; i<3; i++)
            {
                handModels[i].SetActive(true);
            }
        }
    }

    /// <summary>
    /// battlePhaseで敵がランダムにカードを1枚選び、バトルフィールドへ移動させる。
    /// BattleSystem に rootEnemyCard として登録し、選ばれなかったモデルは非表示にする。
    /// </summary>
    public void SelectCard ()
    {
        selectNum = Random.Range(0, 3); // 0〜2のランダムでカードを選択
        GameObject chooseCard = HandCards[selectNum];
        battleSystem.GetComponent<BattleSystem>().rootEnemyCard = chooseCard;

        // 選んだカードのフィールドと3Dモデルをバトルフィールド座標へ移動
        if (selectNum == 0)
        {
            leftSummoned.transform.position = battleField.transform.position;
            leftModel.transform.position = battleField.transform.position;
            chooseCard.GetComponent<Card>().model3D = leftModel;
            chooseCard.GetComponent<Card>().cardField = leftSummoned;
        }
        else if (selectNum == 1)
        {
            midSummoned.transform.position = battleField.transform.position;
            midModel.transform.position = battleField.transform.position;
            chooseCard.GetComponent<Card>().model3D = midModel;
            chooseCard.GetComponent<Card>().cardField = midSummoned;
        }
        else if (selectNum == 2)
        {
            rightSummoned.transform.position = battleField.transform.position;
            rightModel.transform.position = battleField.transform.position;
            chooseCard.GetComponent<Card>().model3D = rightModel;
            chooseCard.GetComponent<Card>().cardField = rightSummoned;
        }

        // 手モデルをすべて非表示にし、バトルに出なかったモデルも非表示にする
        for (int i=0; i<3; i++)
        {
            handModels[i].SetActive(false);
            if (i == selectNum) continue;

            if (i == 0) leftModel.SetActive(false);
            if (i == 1) midModel.SetActive(false);
            if (i == 2) rightModel.SetActive(false);
        }
    }

    /// <summary>
    /// プレイヤー勝利後に敵の手札を補充する（バトルで負けた枠に新しいランダムカードを設定）。
    /// バトルに出なかったモデルと手モデルを再表示する。
    /// </summary>
    public void RefullCard()
    {
        GameObject card = popCard.GetRandomCard(); // ランダムな新カードを生成
        HandCards[selectNum] = card;

        // バトルに出なかったカードのモデルと手モデルを再表示
        for (int i = 0; i < 3; i++)
        {
            if (i == selectNum) continue;

            if (i == 0) { leftModel.SetActive(true); handModels[i].SetActive(true); }
            if (i == 1) { midModel.SetActive(true); handModels[i].SetActive(true); }
            if (i == 2) { rightModel.SetActive(true); handModels[i].SetActive(true); }
        }
    }

    /// <summary>
    /// プレイヤー敗北後に敵カードをベンチの元の位置に戻す。
    /// バトルに出なかったモデルと手モデルも再表示する。
    /// </summary>
    public void returnPosition()
    {
        if (selectNum == 0)
        {
            leftSummoned.transform.position = leftField.transform.position;
            leftModel.transform.position = leftField.transform.position;
        }
        else if (selectNum == 1)
        {
            midSummoned.transform.position = midField.transform.position;
            midModel.transform.position = midField.transform.position;
        }
        else if (selectNum == 2)
        {
            rightSummoned.transform.position = rightField.transform.position;
            rightModel.transform.position = rightField.transform.position;
        }

        // 全モデルと手モデルを再表示
        for (int i = 0; i < 3; i++)
        {
            handModels[i].SetActive(true);
            if (i == selectNum) continue;

            if (i == 0) leftModel.SetActive(true);
            if (i == 1) midModel.SetActive(true);
            if (i == 2) rightModel.SetActive(true);
        }
    }
}
