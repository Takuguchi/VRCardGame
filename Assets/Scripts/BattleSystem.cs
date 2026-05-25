using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using OVR.OpenVR;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{

    public PopCard popCard;
    public GameObject rootEnemyCard;
    public GameObject rootMyCard;
    public GameObject phaseController;
    public GameObject battleField;
    public GameObject enemyManager, popCardSystem;
    private Animator animatorSelf, animatorEnemy;
    private int i;
    public AudioSource DW, DL, RW, RL, SW, SL, DT;
    public GameObject Smoke;

    //public List<ParticleSystem> robotWins, robotLoses, daragonWins, dragonLoses, slimeWins, slimeLoses;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DeleteCard(int result)
    {
        if (result == 0)
        {
            Destroy(rootEnemyCard.GetComponent<Card>().model3D);
            Destroy(rootEnemyCard.GetComponent<Card>().cardField);
            Destroy(rootEnemyCard);
        }
        if (result == 1)
        {
            Destroy(rootMyCard.GetComponent<Card>().model3D);
            Destroy(rootMyCard.GetComponent<Card>().cardField);
            Destroy(rootMyCard);
            battleField.GetComponent<BattleField>().triggerCount--;
        }
        if(result == 2)
        {
            Destroy(rootMyCard.GetComponent<Card>().model3D);
            Destroy(rootMyCard.GetComponent<Card>().cardField);
            Destroy(rootMyCard);
            battleField.GetComponent<BattleField>().triggerCount--;
            Destroy(rootEnemyCard.GetComponent<Card>().model3D);
            Destroy(rootEnemyCard.GetComponent<Card>().cardField);
            Destroy(rootEnemyCard);
        }
    }

    public int Battle()
    {
        Texture enemyTex = GetTexture(rootEnemyCard);
        Texture myTex = GetTexture(rootMyCard);
        string enemyHand = popCard.GetComponent<PopCard>().texture2hands[enemyTex.name];
        string myHand = popCard.GetComponent<PopCard>().texture2hands[myTex.name];
        int yourResult = GetResult(enemyHand, myHand);
        i = battleField.GetComponent<BattleField>().selectNumSelf;
        animatorSelf = popCardSystem.GetComponent<PopCard>().benchCards[i].GetComponent<Card>().model3D.GetComponent<Animator>();
        i = enemyManager.GetComponent<EnemyManager>().selectNum;
        animatorEnemy = enemyManager.GetComponent<EnemyManager>().enemySummoneds[i].GetComponent<Animator>();
        if (yourResult == 0)
        {
            animatorSelf.SetTrigger("WinAttack");
            animatorEnemy.SetTrigger("LoseAttack");
        }
        else if (yourResult == 2)
        {
            animatorSelf.SetTrigger("LoseAttack");
            animatorEnemy.SetTrigger("LoseAttack");
        }
        else if (yourResult == 1)
        {
            animatorSelf.SetTrigger("LoseAttack");
            animatorEnemy.SetTrigger("WinAttack");
        }
        else
        {
            throw new System.Exception("omission of requirements in GetResult method.");
        }
        return yourResult;
    }

    private Texture GetTexture(GameObject rootCard)
    {
        Transform tr2 = rootCard.transform;
        Transform targetTransform = tr2.Find("Visuals/Root/Black_PlayingCards_Spade10_00");
        GameObject targetObject = targetTransform.gameObject;
        Renderer renderer = targetObject.GetComponent<Renderer>();
        Material material = renderer.material;
        return material.GetTexture("_MainTex2");
    }

    private int GetResult(string enemyHand, string myHand)
    {
        //0: you win, 1: you lose, 2: draw
        //Paper, Rock, Scissors
        if(myHand == "Paper")
        {
            if (enemyHand == "Rock")
            {
                StartCoroutine(MotionDragonWin());
                StartCoroutine(MotionSlimeLose());
                return 0;
            }
            if (enemyHand == "Scissors")
            {
                StartCoroutine(MotionDragonLose());
                StartCoroutine(MotionRobotWin());
                return 1;
            }
            if (enemyHand == "Paper")
            {
                StartCoroutine(MotionDragonLose());
                return 2;
            }
        }
        if (myHand == "Rock")
        {
            if (enemyHand == "Rock")
            {
                StartCoroutine(MotionSlimeLose());
                return 2;
            }
            if (enemyHand == "Scissors")
            {
                StartCoroutine(MotionSlimeWin());
                StartCoroutine(MotionRobotLose());
                return 0;
            }
            if (enemyHand == "Paper")
            {
                StartCoroutine(MotionSlimeLose());
                StartCoroutine(MotionDragonWin());
                return 1;
            }
        }
        if (myHand == "Scissors")
        {
            if (enemyHand == "Rock")
            {
                StartCoroutine(MotionSlimeWin());
                StartCoroutine(MotionRobotLose());
                return 1;
            }
            if (enemyHand == "Scissors")
            {
                StartCoroutine(MotionRobotLose());
                return 2;
            }
            if (enemyHand == "Paper")
            {
                StartCoroutine(MotionDragonLose());
                StartCoroutine(MotionRobotWin());
                return 0;
            }
        }
        return -1;
    }

    IEnumerator MotionDragonWin()
    {
        DW.Play();
        yield return new WaitForSeconds(3f);
        Smoke.SetActive(true);
        yield return new WaitForSeconds(1f);
        yield return new WaitForSeconds(4f);
        Smoke.SetActive(false);
        yield return new WaitForSeconds(1f);
        DW.Stop();
    }

    IEnumerator MotionDragonLose()
    {
        DL.Play();
        yield return new WaitForSeconds(9f);
        DL.Stop();
        DT.Play();
    }

    IEnumerator MotionRobotWin()
    {
        RW.Play();
        yield return new WaitForSeconds(9f);
        RW.Stop();
    }

    IEnumerator MotionRobotLose()
    {
        RL.Play();
        yield return new WaitForSeconds(9f);
        RL.Stop();
    }

    IEnumerator MotionSlimeWin()
    {
        SW.Play();
        yield return new WaitForSeconds(9f);
        SW.Stop();
    }

    IEnumerator MotionSlimeLose()
    {
        SL.Play();
        yield return new WaitForSeconds(9f);
        SL.Stop();
    }
}
