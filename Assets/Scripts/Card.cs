using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class Card : MonoBehaviour
{

    private Vector3 popPosition;
    private bool hasExecuted = false;
    public int _popCnt;

    public PopCard popCard;
    public GameObject popCardSystem;
    public GameObject phaseController;

    public bool summoned;
    public GameObject model3D;
    public GameObject cardField;

    void Start()
    {
        popPosition = transform.position;
        _popCnt = popCardSystem.GetComponent<PopCard>().popCnt;
        summoned = false;
        GetComponent<Grabbable>().CanGrab = true;
    }


    void Update()
    {
        if (transform.position != popPosition && !hasExecuted && popCardSystem.GetComponent<PopCard>().popCnt < 5 && phaseController.GetComponent<PhaseController>().startPhase)
        {
            StartCoroutine(Pop(1f)); // 1ïbå„Ç…é¿çs
            hasExecuted = true;
        }

        if (phaseController.GetComponent<PhaseController>().startPhase  == true && popCardSystem.GetComponent<PopCard>().popCnt == 5 && transform.position != popPosition)
        {

            StartCoroutine(FinishStartPhase());
        }

        if (phaseController.GetComponent<PhaseController>().drawPhase == true && popCardSystem.GetComponent<PopCard>().popCnt >= 6 && transform.position != popPosition && _popCnt == popCardSystem.GetComponent<PopCard>().popCnt)
        {

            StartCoroutine(FinishDrawPhase());
        }

        popPosition = transform.position;

    }

    IEnumerator Pop(float delay)
    {
        yield return new WaitForSeconds(delay);
        popCard.PopCardToDeckHead();
    }

    IEnumerator FinishStartPhase()
    {
        yield return new WaitForSeconds(2f);
        phaseController.GetComponent<PhaseController>().startPhase = false;
    }

    IEnumerator FinishDrawPhase()
    {
        yield return new WaitForSeconds(2f);
        phaseController.GetComponent<PhaseController>().drawPhase = false;
    }

}
