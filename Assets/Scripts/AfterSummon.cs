using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 召喚演出後、モンスターが地面に着地したときの反応を制御するクラス。
/// 着地時（Ground タグとの衝突）に咆哮アニメーションとSEを再生する。
/// </summary>
public class AfterSummon : MonoBehaviour
{
    Animator animator;

    public AudioSource _screamBGM; // 着地後の咆哮SE

    void Start()
    {
        animator = GetComponent<Animator>();
        _screamBGM.Stop(); // 初期化時に誤って鳴らないよう停止
    }

    void Update()
    {
    }

    /// <summary>
    /// Ground タグのオブジェクトと衝突したとき（着地したとき）に
    /// 咆哮アニメーションと咆哮SEを再生する。
    /// </summary>
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            animator.SetBool("Scream", true);
            _screamBGM.Play();
        }
    }
}
