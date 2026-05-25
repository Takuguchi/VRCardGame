using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterSummon : MonoBehaviour
{
    Animator animator;

    public AudioSource _screamBGM;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        _screamBGM.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            animator.SetBool("Scream", true);

            _screamBGM.Play();
        }
    }
}
