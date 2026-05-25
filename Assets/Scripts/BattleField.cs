using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using OculusSampleFramework;
using Oculus.Interaction;

public class BattleField : MonoBehaviour
{
    private Color originalColor;
    private bool IsStay;
    private Vector3 pos;
    //public GameObject GrabCardObj;
    public bool doneMove;
    public Transform summonLocation;
    public GameObject popCardSystem;
    public GameObject summonfieldPrefab;
    public GameObject phaseController;
    public GameObject battleSystem;
    private GameObject summoned;
    private GameObject summonedModel;
    private GameObject summonedEffect;
    public int triggerCount;
    private bool selectPhase;
    public bool firstLocateDup;
    private int summonCnt;
    private Vector3 originalLoation, originalModelLocation, originalFieldLocation;
    public int selectNumSelf;
    public GameObject suggestion;

    private void Start()
    {
        originalColor = GetComponent<Renderer>().material.color;
        doneMove = false;
        pos = GetComponent<Transform>().position;
        triggerCount = 0;
        firstLocateDup = false;
        selectNumSelf = -1;
        //IsStay = GrabCardObj.GetComponent<IsColliderOn>().IsCollision;
    }

    private void Update()
    {
        summonCnt = phaseController.GetComponent<PhaseController>().benchSummonCnt;
        selectPhase = phaseController.GetComponent<PhaseController>().selectPhase;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Card")
        {
            triggerCount++;
            if (selectPhase && firstLocateDup == false && triggerCount== 1)
            {
                GameObject parentObj = other.gameObject.transform.parent.transform.parent.transform.parent.gameObject;
                if (parentObj.GetComponent<Card>().summoned)
                {
                    GetComponent<Renderer>().material.color = Color.yellow;
                }
            }
            //GrabCardObj.GetComponent<IsColliderOn>().IsCollision = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Card")
        {
            triggerCount--;
            if (triggerCount == 0)
            {  
                GetComponent<Renderer>().material.color = originalColor;
                if (firstLocateDup)
                {
                    firstLocateDup = false;
                }

            }
            /*
            if (selectPhase)
            {
                if (doneMove && triggerCount == 0)
                {
                    if (summonedModel != null && summoned != null)
                    {
                        Destroy(summonedModel);
                        Destroy(summoned);
                        doneMove = false;
                    }
                }
            }
            */
            //GrabCardObj.GetComponent<IsColliderOn>().IsCollision = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (doneMove == false && other.gameObject.tag == "Card" && selectPhase && firstLocateDup == false)
        {
            Transform card_transform = other.gameObject.GetComponent<Transform>();
            Transform root = card_transform.parent.transform.parent.transform.parent;
            Transform root_transform = root.GetComponent<Transform>();
            GameObject root_obj = root.gameObject;
            battleSystem.GetComponent<BattleSystem>().rootMyCard = root_obj;
            if (root_obj.GetComponent<Card>().summoned)
            {
                if (!root_obj.GetComponent<IsGrabOn>().IsGrab)
                {
                    DoMove(root_obj);
                    selectNumSelf = popCardSystem.GetComponent<PopCard>().benchCards.IndexOf(root_obj);
                    if (selectNumSelf == -1)
                    {
                        throw new System.Exception("missing GameObject serch.");
                    }
                    doneMove = true;
                    //Summon(other.gameObject);
                    GameObject _field = root_obj.gameObject.GetComponent<Card>().cardField;
                    GameObject _model = root_obj.gameObject.GetComponent<Card>().model3D;
                    originalFieldLocation = _field.transform.position;
                    originalModelLocation = _model.transform.position; 
                    _model.transform.position = summonLocation.position;
                    _field.transform.position = summonLocation.position;
                    Transform tr2 = root_obj.transform;
                    Transform targetTransform = tr2.Find("Visuals/Root/Black_PlayingCards_Spade10_00");
                    GameObject targetObject = targetTransform.gameObject;
                    Renderer renderer = targetObject.GetComponent<Renderer>();
                    Material material = renderer.material;
                    // cardÇÃTexture(ï\ñ )Çç∑Çµë÷Ç¶ÇÈ
                    Texture texture = material.GetTexture("_MainTex2");
                    popCardSystem.GetComponent<PopCard>().texture2audio[texture.name].Play();
                    phaseController.GetComponent<PhaseController>().selectPhase = false;
                }
            }
        }
        if (other.gameObject.tag == "Card" && selectPhase == false)
        {
            firstLocateDup = true;
        }
    }

    private void DoMove(GameObject card)
    {
        GetComponent<Renderer>().material.color = originalColor;
        Transform root_transform = card.GetComponent<Transform>();
        root_transform.rotation = Quaternion.Euler(0, 0, 0);
        root_transform.position = new Vector3(pos.x, pos.y + 0.01f, pos.z);
        //Sequence seq = DOTween.Sequence();
        //seq.Append(root_transform.DOMove(new Vector3(pos.x, pos.y+0.01f, pos.z), 1f));
        //seq.Join(root_transform.DORotate(new Vector3(0, 0, 0), 1f, RotateMode.FastBeyond360));
        card.GetComponent<Grabbable>().CanGrab = false;
        suggestion.SetActive(false);
    }

    private void Summon(GameObject card)
    {
        Renderer renderer = card.GetComponent<Renderer>();
        Material material = renderer.material;
        Texture texture = material.GetTexture("_MainTex2");
        GameObject model3d = popCardSystem.GetComponent<PopCard>().texture2models[texture.name];
        GameObject summonEffect = popCardSystem.GetComponent<PopCard>().texture2summonEffects[texture.name];
        summoned = Instantiate(summonfieldPrefab, summonLocation.position, Quaternion.Euler(90, 0, 0));
        renderer = summoned.GetComponent<Renderer>();
        material = renderer.material;
        material.SetTexture("_MainTex", texture);
        //summonedEffect = Instantiate(summonEffect, summonLocation.position, Quaternion.Euler(90, 0, 0));
        //StartCoroutine(WaitAndDoSomething(model3d));
        summonedModel = Instantiate(model3d, summonLocation.position, Quaternion.Euler(0, 0, 0));
        //Destroy(summonedEffect);
        summonCnt++;
    }

    private IEnumerator WaitAndDoSomething(GameObject model3d)
    {
        //yield return new WaitForSeconds(2.5f);
        summonedModel = Instantiate(model3d, summonLocation.position, Quaternion.Euler(0, 0, 0));
        yield return new WaitForSeconds(1.0f);
        //Destroy(summonedEffect);
    }

    public void returnPosition(Vector3 _pos)
    {
        GameObject card = battleSystem.GetComponent<BattleSystem>().rootMyCard;
        card.transform.position = _pos;
        GameObject _field = card.GetComponent<Card>().cardField;
        GameObject _model = card.GetComponent<Card>().model3D;
        _model.transform.position = originalModelLocation;
        _field.transform.position = originalFieldLocation;
    }

    public void resetStatus()
    {
        //firstLocateDup = false;
        doneMove = false;
    }

}
