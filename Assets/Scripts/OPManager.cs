using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OPManager : MonoBehaviour
{
    public List<GameObject> model3Ds;
    public List<GameObject> cards;
    private int selectNum;
    private Vector3 cardPos, modelPos;
    public Transform modelTr, cardTr;
    public GameObject tmpCard, tmpModel;
    public AudioSource _audio;

    // Start is called before the first frame update
    void Start()
    {
        modelPos = modelTr.position;
        cardPos = cardTr.position;
        selectNum = Random.Range(0, model3Ds.Count);
        tmpModel = Instantiate(model3Ds[selectNum], modelPos, Quaternion.Euler(0, 180f, 0));
        tmpModel.transform.localScale = new Vector3(tmpModel.transform.localScale.x * 4 / 7, tmpModel.transform.localScale.y * 4 / 7, tmpModel.transform.localScale.z * 4 / 7);
        tmpCard = Instantiate(cards[selectNum], cardPos, Quaternion.Euler(-90f, 0, 0));
    }

    // Update is called once per frame
    
    public void ChangeMonster()
    {
        StartCoroutine(ChangeMonsterC());
    }

    IEnumerator ChangeMonsterC()
    {
        Destroy(tmpModel);
        Destroy(tmpCard);
        yield return new WaitForSeconds(0.2f);
        selectNum = (selectNum + 1) % 3;
        tmpModel = Instantiate(model3Ds[selectNum], modelPos, Quaternion.Euler(0, 180f, 0));
        tmpModel.transform.localScale = new Vector3(tmpModel.transform.localScale.x * 4 / 7, tmpModel.transform.localScale.y * 4 / 7, tmpModel.transform.localScale.z * 4 / 7);
        tmpCard = Instantiate(cards[selectNum], cardPos, Quaternion.Euler(-90f, 0, 0));
    }
}
