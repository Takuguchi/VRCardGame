using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Unity.VisualScripting;
using UnityEngine;

public class PopCard : MonoBehaviour
{
    public GameObject OriginalCardPrefab;
    public Transform tr;
    public List<Texture2D> textures;
    public List<GameObject> models3d;
    public List<GameObject> summonEffects;
    public List<GameObject> handModels;
    public List<int> numTaiki;
    public List<string> hands;
    public List<AudioSource> toujouVoices;
    public Dictionary<string, GameObject> texture2models;
    public Dictionary<string, float> texture2HandPosZ;
    public Dictionary<string, GameObject> texture2handmodels;
    public Dictionary<string, int> texture2numTaiki;
    public Dictionary<string, GameObject> texture2summonEffects;
    public Dictionary<string, AudioSource> texture2audio;
    public Dictionary<string, string> texture2hands;
    public int popCnt;
    public GameObject phaseController;
    public List<GameObject> benchCards;
    public List<GameObject> benchField;

    [SerializeField]
    private List<float> handlPosZ;

    private float posx, posy, posz;

    public void Start()
    {
        texture2models = new Dictionary<string, GameObject>();
        texture2summonEffects = new Dictionary<string, GameObject>();
        texture2hands = new Dictionary<string, string>();
        texture2numTaiki = new Dictionary<string, int>();
        texture2HandPosZ = new Dictionary<string, float>();
        texture2handmodels = new Dictionary<string, GameObject>();
        texture2audio = new Dictionary<string, AudioSource>();
        for (int i = 0; i < textures.Count; i++)
        {
            texture2models[textures[i].name] = models3d[i];
            texture2summonEffects[textures[i].name] = summonEffects[i];
            texture2hands[textures[i].name] = hands[i];
            texture2numTaiki[textures[i].name] = numTaiki[i];
            texture2handmodels[textures[i].name] = handModels[i];
            texture2HandPosZ[textures[i].name] = handlPosZ[i];
            texture2audio[textures[i].name] = toujouVoices[i];
        }
        posx = tr.position.x;
        posy = tr.position.y;
        posz = tr.position.z;

        popCnt = 1; //In the first, a card has poped.

    }

    // Start is called before the first frame update
    public void PopCardToDeckHead()
    {
        Texture2D texture = GetRandomTexture(textures);
        GameObject card = Instantiate(OriginalCardPrefab, new Vector3(0.591f, 0.956f, 0.2787f) , Quaternion.Euler(0, 0, 180));
        Transform tr2 = card.transform;
        Transform targetTransform = tr2.Find("Visuals/Root/Black_PlayingCards_Spade10_00");
        if (targetTransform != null)
        {
            GameObject targetObject = targetTransform.gameObject;
            Renderer renderer = targetObject.GetComponent<Renderer>();
            Material material = renderer.material;
            // cardのTexture(表面)を差し替える
            material.SetTexture("_MainTex2", texture);
        }
        card.SetActive(true);
        popCnt++;
    }

    public List<GameObject> ChooseThreeCards()
    {

        List<GameObject> threeCards = new List<GameObject>();
        for (int i=0; i<3; i++)
        {
            Texture2D texture = textures[i]; //GetRandomTexture(textures);
            GameObject card = Instantiate(OriginalCardPrefab, new Vector3(posx, posy, posz), Quaternion.Euler(0, 0, 180));
            //card.transform.localScale = Vector3.zero;
            Transform tr2 = card.transform;
            Transform targetTransform = tr2.Find("Visuals/Root/Black_PlayingCards_Spade10_00");
            if (targetTransform != null)
            {
                GameObject targetObject = targetTransform.gameObject;
                Renderer renderer = targetObject.GetComponent<Renderer>();
                Material material = renderer.material;
                // cardのTexture(表面)を差し替える
                material.SetTexture("_MainTex2", texture);
            }
            threeCards.Add(card);
        }
        return threeCards;
    }

    public GameObject GetRandomCard()
    {
        Texture2D texture = GetRandomTexture(textures);
        GameObject card = Instantiate(OriginalCardPrefab, new Vector3(posx, posy, posz), Quaternion.Euler(0, 0, 180));
        //card.transform.localScale = Vector3.zero;
        Transform tr2 = card.transform;
        Transform targetTransform = tr2.Find("Visuals/Root/Black_PlayingCards_Spade10_00");
        if (targetTransform != null)
        {
            GameObject targetObject = targetTransform.gameObject;
            Renderer renderer = targetObject.GetComponent<Renderer>();
            Material material = renderer.material;
            // cardのTexture(表面)を差し替える
            material.SetTexture("_MainTex2", texture);
        }
        return card;
    }

    private Texture2D GetRandomTexture(List<Texture2D> list)
    {
        //Textureリストからランダムに取り出す
        if (list == null || list.Count == 0) return null;
        int randomIndex = Random.Range(0, list.Count);
        return list[randomIndex];
    }
    public void changeCardsCanGrab()
    {
        for (int i = 0; i < benchCards.Count; i++)
        {
            benchCards[i].GetComponent<Grabbable>().CanGrab = true;
        }
    }

    public void changeCardsCannotGrab()
    {
        for (int i = 0; i < benchCards.Count; i++)
        {
            benchCards[i].GetComponent<Grabbable>().CanGrab = false;
        }
    }

}
