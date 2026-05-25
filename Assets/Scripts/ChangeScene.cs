using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// シーン間の遷移を管理するクラス。
/// Build Settings のシーンインデックスで遷移先を指定する。
/// 0 = オープニングシーン, 1 = メインゲームシーン
/// </summary>
public class ChangeScene : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {
    }

    // オープニングシーン(0) → メインゲームシーン(1) へ遷移
    public void ChangeOp2Main()
    {
        SceneManager.LoadScene(1);
    }

    // メインゲームシーン(1) → オープニングシーン(0) へ遷移（ゲーム終了後）
    public void ChangeMain2Op()
    {
        SceneManager.LoadScene(0);
    }
}
