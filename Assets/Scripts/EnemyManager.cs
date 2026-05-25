using System.Collections;
using System.Collections.Generic;
using OVR.OpenVR;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject popCardSystem;
    public List<GameObject> HandCards;
    public PopCard popCard;
    public GameObject phaseController;
    public Transform leftLocation;
    public Transform midLocation;
    public Transform rightLocation;
    public GameObject leftField;
    public GameObject rightField;
    public GameObject midField;
    public Texture uramen;
    public GameObject battleSystem;
    public Transform battleLocation;
    public GameObject battleField;
    public bool looseFlag;

    private GameObject leftSummoned, rightSummoned, midSummoned;
    private GameObject leftModel, midModel, rightModel, leftHandModel, midHandModel, rightHandModel;
    public int selectNum = -1;
    public List<GameObject> enemySummoneds;
    public List<GameObject> handModels;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        selectNum = -1;
        looseFlag = false;
        enemySummoneds = new List<GameObject>();
        HandCards = popCard.ChooseThreeCards();
    }

    // Update is called once per frame
    public void PlayBench()
    {
        if (selectNum == -1)
        {
            //Renderer renderer = HandCards[0].GetComponent<Renderer>();
            //Material material = renderer.material;
            //Texture texture = material.GetTexture("_MainTex2");
            leftSummoned = Instantiate(leftField, leftLocation.position, Quaternion.Euler(90, 0, 0));
            Renderer renderer = leftSummoned.GetComponent<Renderer>();
            Material material = renderer.material;
            material.SetTexture("_MainTex", uramen);
            leftSummoned.SetActive(true);
            //renderer = HandCards[1].GetComponent<Renderer>();
            //material = renderer.material;
            //texture = material.GetTexture("_MainTex2");
            midSummoned = Instantiate(midField, midLocation.position, Quaternion.Euler(90, 0, 0));
            renderer = midSummoned.GetComponent<Renderer>();
            material = renderer.material;
            material.SetTexture("_MainTex", uramen);
            midSummoned.SetActive(true);
            //renderer = HandCards[2].GetComponent<Renderer>();
            //material = renderer.material;
            //texture = material.GetTexture("_MainTex2");
            rightSummoned = Instantiate(rightField, rightLocation.position, Quaternion.Euler(90, 0, 0));
            renderer = rightSummoned.GetComponent<Renderer>();
            material = renderer.material;
            material.SetTexture("_MainTex", uramen);
            rightSummoned.SetActive(true);
        }
        else if(selectNum == 0 && looseFlag == true)
        {
            leftSummoned = Instantiate(leftField, leftLocation.position, Quaternion.Euler(90, 0, 0));
            Renderer renderer = leftSummoned.GetComponent<Renderer>();
            Material material = renderer.material;
            material.SetTexture("_MainTex", uramen);
            leftSummoned.SetActive(true);
        }
        else if(selectNum == 1 && looseFlag == true)
        {
            midSummoned = Instantiate(midField, midLocation.position, Quaternion.Euler(90, 0, 0));
            Renderer renderer = midSummoned.GetComponent<Renderer>();
            Material material = renderer.material;
            material.SetTexture("_MainTex", uramen);
            midSummoned.SetActive(true);
        }
        else if(selectNum == 2 && looseFlag == true)
        {
            rightSummoned = Instantiate(rightField, rightLocation.position, Quaternion.Euler(90, 0, 0));
            Renderer renderer = rightSummoned.GetComponent<Renderer>();
            Material material = renderer.material;
            material.SetTexture("_MainTex", uramen);
            rightSummoned.SetActive(true);
        }

    }

    public void OpenBench()
    {
        if (selectNum == -1)
        {
            //Left
            Transform tr2 = HandCards[0].transform;
            Transform targetTransform = tr2.Find("Visuals/Root/Black_PlayingCards_Spade10_00");
            GameObject targetObject = targetTransform.gameObject;
            Renderer renderer = targetObject.GetComponent<Renderer>();
            Material material = renderer.material;
            Texture texture = material.GetTexture("_MainTex2");
            GameObject model3d = popCardSystem.GetComponent<PopCard>().texture2models[texture.name];
            //GameObject summonEffect = popCardSystem.GetComponent<PopCard>().texture2summonEffects[texture.name];
            renderer = leftSummoned.GetComponent<Renderer>();
            material = renderer.material;
            material.SetTexture("_MainTex", texture);
            leftHandModel = Instantiate(popCardSystem.GetComponent<PopCard>().texture2handmodels[texture.name], new Vector3(leftLocation.position.x, popCardSystem.GetComponent<PopCard>().texture2HandPosZ[texture.name], leftLocation.position.z),  Quaternion.Euler(0, 0, 0));
            leftModel = Instantiate(model3d, leftLocation.position, Quaternion.Euler(0, 180f, 0));
            animator = leftModel.GetComponent<Animator>();
            animator.SetTrigger("SyoukanRoar");
            animator.SetInteger("taiki", Random.Range(0, popCardSystem.GetComponent<PopCard>().texture2numTaiki[texture.name]));
            enemySummoneds.Add(leftModel);
            handModels.Add(leftHandModel);

            //Mid
            tr2 = HandCards[1].transform;
            targetTransform = tr2.Find("Visuals/Root/Black_PlayingCards_Spade10_00");
            targetObject = targetTransform.gameObject;
            renderer = targetObject.GetComponent<Renderer>();
            material = renderer.material;
            texture = material.GetTexture("_MainTex2");
            model3d = popCardSystem.GetComponent<PopCard>().texture2models[texture.name];
            //summonEffect = popCardSystem.GetComponent<PopCard>().texture2summonEffects[texture.name];
            renderer = midSummoned.GetComponent<Renderer>();
            material = renderer.material;
            material.SetTexture("_MainTex", texture);
            midHandModel = Instantiate(popCardSystem.GetComponent<PopCard>().texture2handmodels[texture.name], new Vector3(midLocation.position.x, popCardSystem.GetComponent<PopCard>().texture2HandPosZ[texture.name], midLocation.position.z), Quaternion.Euler(0, 0, 0));
            midModel = Instantiate(model3d, midLocation.position, Quaternion.Euler(0, 180f, 0));
            animator = midModel.GetComponent<Animator>();
            animator.SetTrigger("SyoukanRoar");
            animator.SetInteger("taiki", Random.Range(0, popCardSystem.GetComponent<PopCard>().texture2numTaiki[texture.name]));
            enemySummoneds.Add(midModel);
            handModels.Add(midHandModel);

            //Right
            tr2 = HandCards[2].transform;
            targetTransform = tr2.Find("Visuals/Root/Black_PlayingCards_Spade10_00");
            targetObject = targetTransform.gameObject;
            renderer = targetObject.GetComponent<Renderer>();
            material = renderer.material;
            texture = material.GetTexture("_MainTex2");
            model3d = popCardSystem.GetComponent<PopCard>().texture2models[texture.name];
            //summonEffect = popCardSystem.GetComponent<PopCard>().texture2summonEffects[texture.name];
            renderer = rightSummoned.GetComponent<Renderer>();
            material = renderer.material;
            material.SetTexture("_MainTex", texture);
            rightHandModel = Instantiate(popCardSystem.GetComponent<PopCard>().texture2handmodels[texture.name], new Vector3(rightLocation.position.x, popCardSystem.GetComponent<PopCard>().texture2HandPosZ[texture.name], rightLocation.position.z), Quaternion.Euler(0, 0, 0));
            rightModel = Instantiate(model3d, rightLocation.position, Quaternion.Euler(0, 180f, 0));
            animator = rightModel.GetComponent<Animator>();
            animator.SetTrigger("SyoukanRoar");
            animator.SetInteger("taiki", Random.Range(0, popCardSystem.GetComponent<PopCard>().texture2numTaiki[texture.name]));
            enemySummoneds.Add(rightModel);
            handModels.Add(rightHandModel);
        }
        else if(selectNum == 0 && looseFlag == true)
        {
            Transform tr2 = HandCards[0].transform;
            Transform targetTransform = tr2.Find("Visuals/Root/Black_PlayingCards_Spade10_00");
            GameObject targetObject = targetTransform.gameObject;
            Renderer renderer = targetObject.GetComponent<Renderer>();
            Material material = renderer.material;
            Texture texture = material.GetTexture("_MainTex2");
            GameObject model3d = popCardSystem.GetComponent<PopCard>().texture2models[texture.name];
            //GameObject summonEffect = popCardSystem.GetComponent<PopCard>().texture2summonEffects[texture.name];
            renderer = leftSummoned.GetComponent<Renderer>();
            material = renderer.material;
            material.SetTexture("_MainTex", texture);
            leftHandModel = Instantiate(popCardSystem.GetComponent<PopCard>().texture2handmodels[texture.name], new Vector3(leftLocation.position.x, popCardSystem.GetComponent<PopCard>().texture2HandPosZ[texture.name], leftLocation.position.z), Quaternion.Euler(0, 0, 0));
            leftModel = Instantiate(model3d, leftLocation.position, Quaternion.Euler(0, 180f, 0));
            animator = leftModel.GetComponent<Animator>();
            animator.SetTrigger("SyoukanRoar");
            animator.SetInteger("taiki", Random.Range(0, popCardSystem.GetComponent<PopCard>().texture2numTaiki[texture.name]));
            enemySummoneds[0] = leftModel;
            handModels[0] = leftHandModel;
            handModels[1].SetActive(true);
            handModels[2].SetActive(true);
        }
        else if(selectNum == 1 && looseFlag == true)
        {
            Transform tr2 = HandCards[1].transform;
            Transform targetTransform = tr2.Find("Visuals/Root/Black_PlayingCards_Spade10_00");
            GameObject targetObject = targetTransform.gameObject;
            Renderer renderer = targetObject.GetComponent<Renderer>();
            Material material = renderer.material;
            Texture texture = material.GetTexture("_MainTex2");
            GameObject model3d = popCardSystem.GetComponent<PopCard>().texture2models[texture.name];
            //GameObject summonEffect = popCardSystem.GetComponent<PopCard>().texture2summonEffects[texture.name];
            renderer = midSummoned.GetComponent<Renderer>();
            material = renderer.material;
            material.SetTexture("_MainTex", texture);
            midHandModel = Instantiate(popCardSystem.GetComponent<PopCard>().texture2handmodels[texture.name], new Vector3(midLocation.position.x, popCardSystem.GetComponent<PopCard>().texture2HandPosZ[texture.name], midLocation.position.z), Quaternion.Euler(0, 0, 0));
            midModel = Instantiate(model3d, midLocation.position, Quaternion.Euler(0, 180f, 0));
            animator = midModel.GetComponent<Animator>();
            animator.SetTrigger("SyoukanRoar");
            animator.SetInteger("taiki", Random.Range(0, popCardSystem.GetComponent<PopCard>().texture2numTaiki[texture.name]));
            enemySummoneds[1] = midModel;
            handModels[1] = midHandModel;
            handModels[0].SetActive(true);
            handModels[2].SetActive(true);
        }
        else if (selectNum == 2 && looseFlag == true)
        {
            Transform tr2 = HandCards[2].transform;
            Transform targetTransform = tr2.Find("Visuals/Root/Black_PlayingCards_Spade10_00");
            GameObject targetObject = targetTransform.gameObject;
            Renderer renderer = targetObject.GetComponent<Renderer>();
            Material material = renderer.material;
            Texture texture = material.GetTexture("_MainTex2");
            GameObject model3d = popCardSystem.GetComponent<PopCard>().texture2models[texture.name];
            //GameObject summonEffect = popCardSystem.GetComponent<PopCard>().texture2summonEffects[texture.name];
            renderer = rightSummoned.GetComponent<Renderer>();
            material = renderer.material;
            material.SetTexture("_MainTex", texture);
            rightHandModel = Instantiate(popCardSystem.GetComponent<PopCard>().texture2handmodels[texture.name], new Vector3(rightLocation.position.x, popCardSystem.GetComponent<PopCard>().texture2HandPosZ[texture.name], rightLocation.position.z), Quaternion.Euler(0, 0, 0));
            rightModel = Instantiate(model3d, rightLocation.position, Quaternion.Euler(0, 180f, 0));
            animator = rightModel.GetComponent<Animator>();
            animator.SetTrigger("SyoukanRoar");
            animator.SetInteger("taiki", Random.Range(0, popCardSystem.GetComponent<PopCard>().texture2numTaiki[texture.name]));
            enemySummoneds[2] = rightModel;
            handModels[2] = rightHandModel;
            handModels[0].SetActive(true);
            handModels[1].SetActive(true);
        }
        if (looseFlag == false)
        {
            for (int i=0; i<3; i++)
            {
                handModels[i].SetActive(true);
            }
        }
    }

    public void SelectCard ()
    {
        selectNum = Random.Range(0, 3);
        GameObject chooseCard = HandCards[selectNum];
        battleSystem.GetComponent<BattleSystem>().rootEnemyCard = chooseCard;
        if (selectNum == 0)
        {
            //left
            leftSummoned.transform.position = battleField.transform.position;
            leftModel.transform.position = battleField.transform.position;
            chooseCard.GetComponent<Card>().model3D = leftModel;
            chooseCard.GetComponent<Card>().cardField = leftSummoned;
        }
        else if (selectNum == 1)
        {
            //mid
            midSummoned.transform.position = battleField.transform.position;
            midModel.transform.position = battleField.transform.position;
            chooseCard.GetComponent<Card>().model3D = midModel;
            chooseCard.GetComponent<Card>().cardField = midSummoned;
        }
        else if (selectNum == 2)
        {
            //right
            rightSummoned.transform.position = battleField.transform.position;
            rightModel.transform.position = battleField.transform.position;
            chooseCard.GetComponent<Card>().model3D = rightModel;
            chooseCard.GetComponent<Card>().cardField = rightSummoned;
        }

        for (int i=0; i<3; i++)
        {
            handModels[i].SetActive(false);
            if (i == selectNum)
            {
                continue;
            }

            if (i == 0)
            {
                leftModel.SetActive(false);
            }

            if (i == 1)
            {
                midModel.SetActive(false);
            }

            if (i == 2)
            {
                rightModel.SetActive(false);
            }

        }
    }

    public void RefullCard()
    {
        GameObject card = popCard.GetRandomCard();
        HandCards[selectNum] = card;
        for (int i = 0; i < 3; i++)
        {
            if (i == selectNum)
            {
                continue;
            }

            if (i == 0)
            {
                leftModel.SetActive(true);
                handModels[i].SetActive(true);
            }

            if (i == 1)
            {
                midModel.SetActive(true);
                handModels[i].SetActive(true);
            }

            if (i == 2)
            {
                rightModel.SetActive(true);
                handModels[i].SetActive(true);
            }
        }
    }

    public void returnPosition()
    {
        if (selectNum == 0)
        {
            //left
            leftSummoned.transform.position = leftField.transform.position;
            leftModel.transform.position = leftField.transform.position;
        }
        else if (selectNum == 1)
        {
            //mid
            midSummoned.transform.position = midField.transform.position;
            midModel.transform.position = midField.transform.position;
        }
        else if (selectNum == 2)
        {
            //right
            rightSummoned.transform.position = rightField.transform.position;
            rightModel.transform.position = rightField.transform.position;
        }
        for (int i = 0; i < 3; i++)
        {
            handModels[i].SetActive(true);
            if (i == selectNum)
            {
                continue;
            }

            if (i == 0)
            {
                leftModel.SetActive(true);
                //handModels[i].SetActive(true);
            }

            if (i == 1)
            {
                midModel.SetActive(true);
                //handModels[i].SetActive(true);
            }

            if (i == 2)
            {
                rightModel.SetActive(true);
                //handModels[i].SetActive(true);
            }
        }
    }
}