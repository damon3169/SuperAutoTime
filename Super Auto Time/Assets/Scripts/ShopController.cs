using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopController : MonoBehaviour
{
    public List<GameObject> ListUniteShopLevel1;
    public List<GameObject> ListUniteShopLevel2;
    public List<GameObject> ListUniteShopLevel3;
    public List<float> ListUnitePourcentageLevel1;
    public List<float> ListUnitePourcentageLevel2;
    public List<float> ListUnitePourcentageLevel3;
   // public List<List<float>> ListUnitePourcentagelevels;

    PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        Object[] units = Resources.LoadAll("Prefabs/Unit/");
        foreach(var unit in  units)
        {
            GameObject unitGameObject = (GameObject)unit;
            Debug.Log(unitGameObject);
            if(unitGameObject.GetComponent<TimeUnite>().level==1)
            {
                ListUniteShopLevel1.Add(unitGameObject);
            }
            if(unitGameObject.GetComponent<TimeUnite>().level==2)
            {
                ListUniteShopLevel2.Add(unitGameObject);
            }
            if(unitGameObject.GetComponent<TimeUnite>().level==3)
            {
                ListUniteShopLevel3.Add(unitGameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            //refreshShop();
        }
        else if (GameObject.FindGameObjectWithTag("Player"))
        {
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (player.GetComponent<PlayerController>().isLocalPlayer)
                {
                    this.player = player.GetComponent<PlayerController>();
                }
            }
        }
    }

    public void refreshShop()
    {
        GameObject unit = null;
        foreach (GameObject UniteSlot in GameObject.FindGameObjectsWithTag("UniteShopSlot"))
        {
            if (UniteSlot.transform.childCount > 1)
                GameObject.Destroy(UniteSlot.transform.GetChild(1).gameObject);
            if (player.playerCurrentLevel == 1)
            {
                unit = Instantiate(ListUniteShopLevel1[Random.Range(0, ListUniteShopLevel1.Count)], UniteSlot.transform.position, Quaternion.identity);
            }

            else if (player.playerCurrentLevel == 2)
            {
                int random = Random.Range(0, 100);
                if (random < ListUnitePourcentageLevel2[0])
                {
                    unit = Instantiate(ListUniteShopLevel1[Random.Range(0, ListUniteShopLevel1.Count)], UniteSlot.transform.position, Quaternion.identity);
                }
                else
                {
                    unit = Instantiate(ListUniteShopLevel2[Random.Range(0, ListUniteShopLevel2.Count)], UniteSlot.transform.position, Quaternion.identity);
                }
            }
            else if (player.playerCurrentLevel == 3)
            {
                int random = Random.Range(0, 100);
                if (random < ListUnitePourcentageLevel3[0])
                {
                    unit = Instantiate(ListUniteShopLevel1[Random.Range(0, ListUniteShopLevel1.Count)], UniteSlot.transform.position, Quaternion.identity);
                }
                else if (random < ListUnitePourcentageLevel3[1])
                {
                    unit = Instantiate(ListUniteShopLevel2[Random.Range(0, ListUniteShopLevel2.Count)], UniteSlot.transform.position, Quaternion.identity);
                }
                else
                {
                    unit = Instantiate(ListUniteShopLevel3[Random.Range(0, ListUniteShopLevel3.Count)], UniteSlot.transform.position, Quaternion.identity);
                }
            }
            UniteSlot.transform.GetChild(0).GetComponent<TMP_Text>().text= unit.GetComponent<TimeUnite>().description;
            unit.transform.parent = UniteSlot.transform;
            
        }
        /*foreach (GameObject UniteSlot in GameObject.FindGameObjectsWithTag("ItemShopSlot"))
        {
            if (player.round < 3)
            {
                Instantiate(ListItemShopLevel1[Random.Range(0, ListItemShopLevel1.Count)], UniteSlot.transform.position, Quaternion.identity);
            }

            else if (player.round > 3 && player.round < 6)
            {
                int random = Random.Range(0, 100);
                if (random < ListItemPourcentageLevel2[0])
                    Instantiate(ListItemShopLevel1[Random.Range(0, ListItemShopLevel1.Count)], UniteSlot.transform.position, Quaternion.identity);
                else
                    Instantiate(ListItemShopLevel2[Random.Range(0, ListItemShopLevel2.Count)], UniteSlot.transform.position, Quaternion.identity);
            }
            else
            {
                int random = Random.Range(0, 100);
                if (random < ListItemPourcentageLevel2[0])
                    Instantiate(ListItemShopLevel1[Random.Range(0, ListItemShopLevel1.Count)], UniteSlot.transform.position, Quaternion.identity);
                else if (random < ListItemPourcentageLevel2[1])
                    Instantiate(ListItemShopLevel2[Random.Range(0, ListItemShopLevel2.Count)], UniteSlot.transform.position, Quaternion.identity);
                else
                    Instantiate(ListItemShopLevel3[Random.Range(0, ListItemShopLevel3.Count)], UniteSlot.transform.position, Quaternion.identity);
            }
        }*/
    }
}
