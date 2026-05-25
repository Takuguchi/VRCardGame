using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
using UnityEngine;
using TMPro;

public class PhaseController : MonoBehaviour
{
    public bool startPhase;
    public bool summonPhase;
    public bool selectPhase;
    public bool battlePhase;
    public bool drawPhase;

    public int benchSummonCnt;
    public EnemyManager enemyManager;
    public PopCard popCard;
    public BattleSystem battleSystem;
    public GameObject popCardSystem;
    public BattleField batlleFeild;
    public GameObject _battleField;
    public GameObject _enemyManager;
    public GameObject enemyLife, selfLife;
    public TMP_Text turnText;
    public ParticleSystem summonP, battleP, selectP, drawP, WinP, LoseP;
    public GameObject enemyHuman;
    private Animator enemyAnime;
    public ChangeScene chScene;
    public AudioSource winSound;
    public AudioSource mainSound, LifeDis;
    public GameObject DrawS, MidBenchS, LeftBenchS, RightBenshS, BattleFieldS;
    public TMP_Text tmpGuide;

    private int turnNum;
    private Color tempColor;
    private int yourResult;

    // Start is called before the first frame update
    void Start()
    {
        enemyAnime = enemyHuman.GetComponent<Animator>();
        popCard.PopCardToDeckHead();
        benchSummonCnt = 0;
        turnNum = 1;
        turnText.text = "Turn " + turnNum.ToString();
        startPhase = true;
        summonPhase = false;
        selectPhase = false;
        battlePhase = false;
        drawPhase = false;
        yourResult = -1;
        enemyLife.GetComponent<EnemyLife>().lifePoint = 3;
        selfLife.GetComponent<SelfLife>().lifePoint = 3;
        StartCoroutine(ProceedPhase());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator ProceedPhase()
    {
        turnText.text = "TURN " + turnNum.ToString();

        if (startPhase == true)
        {
            drawP.Play();
            tmpGuide.text = "山札から5枚カードを引いてください";
            DrawS.SetActive(true);
        }
        

        while (startPhase)
        {
            yield return null;
        }
        DrawS.SetActive(false);
        summonPhase = true;
        enemyManager.PlayBench();
        Debug.Log("start phase done");
        if (_battleField.GetComponent<BattleField>().selectNumSelf != -1)
        {
            _battleField.GetComponent<BattleField>().doneMove = false;
            _battleField.GetComponent<BattleField>().firstLocateDup = false;
            popCardSystem.GetComponent<PopCard>().benchField[_battleField.GetComponent<BattleField>().selectNumSelf].GetComponent<CardColliderEvent>().doneMove = false;
        }


        if (yourResult != 0)
        {
            summonP.Play();
        }

        if (_battleField.GetComponent<BattleField>().selectNumSelf == -1)
        {
            MidBenchS.SetActive(true);
            LeftBenchS.SetActive(true);
            RightBenshS.SetActive(true);
            tmpGuide.text = "手札から3枚のカードをベンチにおいてください";
        }
        else if (popCardSystem.GetComponent<PopCard>().benchField[_battleField.GetComponent<BattleField>().selectNumSelf].GetComponent<CardColliderEvent>().locationNum == 0 && yourResult != 0)
        {
            LeftBenchS.SetActive(true);
            tmpGuide.text = "手札から1枚選び空いているベンチにおいてください";
        }
        else if (popCardSystem.GetComponent<PopCard>().benchField[_battleField.GetComponent<BattleField>().selectNumSelf].GetComponent<CardColliderEvent>().locationNum == 1 && yourResult != 0)
        {
            MidBenchS.SetActive(true);
            tmpGuide.text = "手札から1枚選び空いているベンチにおいてください";
        }
        else if (popCardSystem.GetComponent<PopCard>().benchField[_battleField.GetComponent<BattleField>().selectNumSelf].GetComponent<CardColliderEvent>().locationNum == 2 && yourResult != 0)
        {
            RightBenshS.SetActive(true);
            tmpGuide.text = "手札から1枚選び空いているベンチにおいてください";
        }

        while (summonPhase)
        {
            yield return null;
            if (benchSummonCnt == 3)
            {
                summonPhase = false;
            }
        }
        Debug.Log("summon phase done");
        MidBenchS.SetActive(false);
        LeftBenchS.SetActive(false);
        RightBenshS.SetActive(false);
        yield return new WaitForSeconds(2f);

        selectP.Play();
        tmpGuide.text = "ベンチから1枚選び、バトル場においてください";
        BattleFieldS.SetActive(true);
        selectPhase = true;
        enemyAnime.SetTrigger("PlayCard");
        enemyManager.OpenBench();
        popCard.changeCardsCanGrab();
        Debug.Log("select phase done");

        List<Vector3> benchpos = new List<Vector3>();
        for (int i = 0; i < 3; i++)
        {
            benchpos.Add(popCardSystem.GetComponent<PopCard>().benchCards[i].transform.position);
        }
        while (selectPhase)
        {
            for (int i = 0; i < 3; i++)
            {
                popCardSystem.GetComponent<PopCard>().benchCards[i].transform.position = benchpos[i];
                popCardSystem.GetComponent<PopCard>().benchCards[i].transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            yield return new WaitForSeconds(5f);
            //yield return null;
        }
        tmpGuide.text = "";
        for (int i = 0;i < 3; i++)
        {
            if (i == batlleFeild.GetComponent<BattleField>().selectNumSelf)
            {
                continue;
            }
            popCardSystem.GetComponent<PopCard>().benchCards[i].transform.position = benchpos[i];
            popCardSystem.GetComponent<PopCard>().benchCards[i].transform.rotation = Quaternion.Euler(0, 0, 0);
            popCardSystem.GetComponent<PopCard>().benchCards[i].GetComponent<Card>().model3D.SetActive(false);
        }

        popCard.changeCardsCannotGrab();
        enemyAnime.SetTrigger("Batlle");
        battlePhase = true;
        enemyManager.SelectCard();
        yield return new WaitForSeconds(1f);

        battleP.Play();
        //0: you win, 1: you lose, 2: draw
        yourResult = battleSystem.Battle();

        yield return new WaitForSeconds(12f);
        battleSystem.DeleteCard(yourResult);
        if (yourResult == 1)
        {
            enemyAnime.SetTrigger("Win");
        }
        if (yourResult == 0)
        {
            enemyAnime.SetTrigger("Damaged");
        }
        if (yourResult == 2)
        {
            enemyAnime.SetTrigger("Draw");
        }
        //popCardSystem.GetComponent<PopCard>().benchField[_battleField.GetComponent<BattleField>().selectNumSelf].GetComponent<CardColliderEvent>().triggerCount--;
        battlePhase = false;
        Debug.Log("battle phase done");
        while (battlePhase)
        {
            yield return null;
        }
        yield return new WaitForSeconds(3f);
        if (yourResult == 0 || yourResult == 2)
        {
            enemyManager.RefullCard();
            _enemyManager.GetComponent<EnemyManager>().looseFlag = true;
        }
        else
        {
            _enemyManager.GetComponent<EnemyManager>().looseFlag = false;
            enemyManager.returnPosition();
            selfLife.GetComponent<SelfLife>().lifePoint--;
            tempColor = selfLife.GetComponent<SelfLife>().heartImages[selfLife.GetComponent<SelfLife>().lifePoint].color;
            tempColor.a = 0.0f;
            selfLife.GetComponent<SelfLife>().heartImages[selfLife.GetComponent<SelfLife>().lifePoint].color = tempColor;
            LifeDis.Play();

        }
        if (yourResult == 1 || yourResult == 2)
        {
            benchSummonCnt--;
            //popCardSystem.GetComponent<PopCard>().popCnt++;
            //popCard.PopCardToDeckHead();
        }
        else
        {
            batlleFeild.returnPosition(benchpos[batlleFeild.GetComponent<BattleField>().selectNumSelf]);
            enemyLife.GetComponent<EnemyLife>().lifePoint--;
            tempColor = enemyLife.GetComponent<EnemyLife>().heartImages[enemyLife.GetComponent<EnemyLife>().lifePoint].color;
            tempColor.a = 0.0f;
            enemyLife.GetComponent<EnemyLife>().heartImages[enemyLife.GetComponent<EnemyLife>().lifePoint].color = tempColor;
            LifeDis.Play();
        }

        for (int i = 0; i < 3; i++)
        {
            if (i == batlleFeild.GetComponent<BattleField>().selectNumSelf)
            {
                continue;
            }
            popCardSystem.GetComponent<PopCard>().benchCards[i].GetComponent<Card>().model3D.SetActive(true);
        }

        drawPhase = false;
        yield return new WaitForSeconds(1f);

        if ((yourResult == 1 || yourResult == 2) && selfLife.GetComponent<SelfLife>().lifePoint > 0 && enemyLife.GetComponent<EnemyLife>().lifePoint > 0)
        {
            drawP.Play();
            DrawS.SetActive(true);
            tmpGuide.text = "山札から1枚カードを引いてください";
            popCardSystem.GetComponent<PopCard>().popCnt++;
            drawPhase = true;
            popCard.PopCardToDeckHead();
        }
        else
        {
            drawPhase = false;
        }

        
        while (drawPhase)
        {
            yield return null;
        }
        
        DrawS.SetActive(false);
        tmpGuide.text = "";

        selectPhase = false;
        summonPhase = false;
        battlePhase = false;
        drawPhase = false;

        if (enemyLife.GetComponent<EnemyLife>().lifePoint == 0)
        {
            enemyAnime.SetTrigger("Lose");
        }


        if (selfLife.GetComponent<SelfLife>().lifePoint > 0 && enemyLife.GetComponent<EnemyLife>().lifePoint > 0)
        {
            turnNum++;
            StartCoroutine(ProceedPhase());
        }


        yield return new WaitForSeconds(1f);

        if (selfLife.GetComponent<SelfLife>().lifePoint == 0)
        {
            LoseP.Play();
            yield return new WaitForSeconds(3f);
            chScene.ChangeMain2Op();
        }
        else if (enemyLife.GetComponent<EnemyLife>().lifePoint == 0)
        {
            WinP.Play();
            mainSound.Stop();
            winSound.Play();
            yield return new WaitForSeconds(7f);
            chScene.ChangeMain2Op();
        }


    }

}
