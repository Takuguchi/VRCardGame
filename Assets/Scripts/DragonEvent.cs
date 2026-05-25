using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ドラゴンモデルのアニメーションイベントを受け取り、SE とパーティクルを再生するクラス。
/// Animator のキーフレームにイベントを設定し、攻撃の各フェーズで対応するメソッドを呼ぶ。
/// </summary>
public class DragonEvent : MonoBehaviour
{
    private AudioSource _audio;

    // ---- 各アクションの SE とパーティクル（Inspectorで設定）----
    public AudioClip clawSound, takeoffSound, FlameAttackSound, LandSound;
    public ParticleSystem clawP, takeoffP, FlameAttackP, LandP;

    void Start()
    {
        _audio = GetComponent<AudioSource>();
    }

    // 爪攻撃：クロー攻撃SEとパーティクルを同時再生
    public void ClawAttack()
    {
        _audio.clip = clawSound;
        _audio.Play();
        clawP.Play();
    }

    // 飛び立ち：離陸SEとパーティクルを同時再生
    public void TakeOff()
    {
        _audio.clip = takeoffSound;
        _audio.Play();
        takeoffP.Play();
    }

    // 炎攻撃：炎SEとパーティクルを同時再生
    public void FlameAttack()
    {
        _audio.clip = FlameAttackSound;
        _audio.Play();
        FlameAttackP.Play();
    }

    // 着地：着地SEとパーティクルを同時再生
    public void Land()
    {
        _audio.clip = LandSound;
        _audio.Play();
        LandP.Play();
    }
}
