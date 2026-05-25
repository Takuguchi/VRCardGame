using UnityEngine;

public class BaseAnimationEventHook : MonoBehaviour
{

    private AudioSource audioSource;
    public ParticleSystem effect;

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