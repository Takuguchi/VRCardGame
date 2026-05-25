using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonEvent : MonoBehaviour
{
    private AudioSource _audio;
    public AudioClip clawSound, takeoffSound, FlameAttackSound, LandSound;
    public ParticleSystem clawP, takeoffP, FlameAttackP, LandP;

    // Start is called before the first frame update
    void Start()
    {
        _audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    
    public void ClawAttack()
    {
        _audio.clip = clawSound;
        _audio.Play();
        clawP.Play();
    }

    public void TakeOff()
    {
        _audio.clip = takeoffSound;
        _audio.Play();
        takeoffP.Play();
    }

    public void FlameAttack()
    {
        _audio.clip = FlameAttackSound;
        _audio.Play();
        FlameAttackP.Play();
    }

    public void Land()
    {
        _audio.clip = LandSound;
        _audio.Play();
        LandP.Play();
    }

}
