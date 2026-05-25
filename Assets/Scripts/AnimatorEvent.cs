using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEvent : MonoBehaviour
{
    public Animator animator;
    private AnimatorStateInfo stateInfo;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(HandleDragon());
        //StartCoroutine(HandleSlime());
        //StartCoroutine(HandleRobot());
    }

    // Update is called once per frame
    IEnumerator HandleDragon()
    {
        if (animator != null)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Fly Flame Attack"))
            {
                //flame
            }
        }
        yield return null;
    }
}
