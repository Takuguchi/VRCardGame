using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// オープニングシーンでモンスターをローテーション表示するクラス。
/// ランダムに1体を選んで3Dモデルとカード画像を表示し、
/// ChangeMonster() で順番に切り替える（ボタン等から呼ぶ想定）。
/// </summary>
public class OPManager : MonoBehaviour
{
    public List<GameObject> model3Ds;   // 表示するモンスター3Dモデルのプレハブ一覧
    public List<GameObject> cards;      // 対応するカードのプレハブ一覧
    private int selectNum;              // 現在表示中のモンスターインデックス
    private Vector3 cardPos, modelPos;  // 生成基準位置（キャッシュ）
    public Transform modelTr, cardTr;   // モデル・カードの生成位置Transformの参照
    public GameObject tmpModel, tmpCard; // 現在シーンに表示中のモデル・カードオブジェクト
    public AudioSource _audio;

    void Start()
    {
        modelPos = modelTr.position;
        cardPos = cardTr.position;

        // 起動時にランダムなモンスターを選んで表示
        selectNum = Random.Range(0, model3Ds.Count);
        tmpModel = Instantiate(model3Ds[selectNum], modelPos, Quaternion.Euler(0, 180f, 0));
        // オープニング用にモデルを縮小表示（元スケールの 4/7 倍）
        tmpModel.transform.localScale = new Vector3(
            tmpModel.transform.localScale.x * 4 / 7,
            tmpModel.transform.localScale.y * 4 / 7,
            tmpModel.transform.localScale.z * 4 / 7);
        tmpCard = Instantiate(cards[selectNum], cardPos, Quaternion.Euler(-90f, 0, 0));
    }

    /// <summary>
    /// 現在のモンスターを破棄し、次のモンスター（インデックス+1のサイクル）に切り替える。
    /// ボタンのクリックイベントから呼ばれる。
    /// </summary>
    public void ChangeMonster()
    {
        StartCoroutine(ChangeMonsterC());
    }

    IEnumerator ChangeMonsterC()
    {
        Destroy(tmpModel);
        Destroy(tmpCard);
        yield return new WaitForSeconds(0.2f); // 破棄の処理待ち

        selectNum = (selectNum + 1) % 3; // 0→1→2→0 の順でサイクル
        tmpModel = Instantiate(model3Ds[selectNum], modelPos, Quaternion.Euler(0, 180f, 0));
        tmpModel.transform.localScale = new Vector3(
            tmpModel.transform.localScale.x * 4 / 7,
            tmpModel.transform.localScale.y * 4 / 7,
            tmpModel.transform.localScale.z * 4 / 7);
        tmpCard = Instantiate(cards[selectNum], cardPos, Quaternion.Euler(-90f, 0, 0));
    }
}
