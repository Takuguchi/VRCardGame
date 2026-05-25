using UnityEngine;

/// <summary>
/// アニメーションイベントからSEとパーティクルエフェクトを再生する汎用フック。
/// Animatorのキーフレームにイベントを設定し、このクラスの PlayEffects を呼ぶことで
/// アニメーションと音・エフェクトを同期させる。
/// </summary>
public class BaseAnimationEventHook : MonoBehaviour
{
    private AudioSource audioSource;
    public ParticleSystem effect; // 再生するパーティクルエフェクト

    /// <summary>
    /// アニメーションイベントから呼ばれる。クリップが設定されていればSEを、
    /// エフェクトが設定されていればパーティクルを再生する。
    /// </summary>
    public void PlayEffects(AnimationEvent animationEvent)
    {
        if (audioSource.clip != null) {
            audioSource.Play();
        };

        if (effect != null) {
            effect.Play();
        }
    }
}
