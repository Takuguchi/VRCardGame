using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summon : MonoBehaviour
{
    public Transform SummonPlace;
    public GameObject dragonPref;
    private GameObject dragonObj;

    private bool _isGenerated = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MonsterSummon();
    }

    void MonsterSummon()
    {
        //Instantiate(dragonPref, new Vector3(-8f, 2.5f, 8f), Quaternion.identity);
        if (Input.GetKeyDown(KeyCode.Return) && _isGenerated == false)
        {
            dragonObj = Instantiate(dragonPref);
            dragonObj.SetActive(true);
            dragonObj.transform.position = SummonPlace.transform.position;
            // ê∂ê¨çœÇ›Ç≈Ç†ÇÈÇ±Ç∆ÇãLò^
            _isGenerated = true;
        }
    }
}
