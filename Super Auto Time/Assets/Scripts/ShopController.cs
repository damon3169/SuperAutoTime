using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    public List<GameObject> ListUniteShopLevel1;
    public List<GameObject> ListUniteShopLevel2;
    public List<GameObject> ListUniteShopLevel3;
    public List<float> ListUnitePourcentageLevel1;
    public List<float> ListUnitePourcentageLevel2;
    public List<float> ListUnitePourcentageLevel3;

    public List<GameObject> ListItemShopLevel1;
    public List<GameObject> ListItemShopLevel2;
    public List<GameObject> ListItemShopLevel3;
    public List<float> ListItemPourcentageLevel1;
    public List<float> ListItemPourcentageLevel2;
    public List<float> ListItemPourcentageLevel3;
    PlayerController player;
    // Start is called before the first frame update
    void Start()
    {

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
        foreach (GameObject UniteSlot in GameObject.FindGameObjectsWithTag("UniteShopSlot"))
        {
            if (player.round < 3)
            {
                Instantiate(ListUniteShopLevel1[Random.Range(0, ListUniteShopLevel1.Count)], UniteSlot.transform.position, Quaternion.identity);
            }

            else if (player.round > 3 && player.round < 6)
            {
                int random = Random.Range(0, 100);
                if (random < ListUnitePourcentageLevel2[0])
                    Instantiate(ListUniteShopLevel1[Random.Range(0, ListUniteShopLevel1.Count)], UniteSlot.transform.position, Quaternion.identity);
                else
                    Instantiate(ListUniteShopLevel2[Random.Range(0, ListUniteShopLevel2.Count)], UniteSlot.transform.position, Quaternion.identity);
            }
            else
            {
                int random = Random.Range(0, 100);
                if (random < ListUnitePourcentageLevel3[0])
                    Instantiate(ListUniteShopLevel1[Random.Range(0, ListUniteShopLevel1.Count)], UniteSlot.transform.position, Quaternion.identity);
                else if (random < ListUnitePourcentageLevel3[1])
                    Instantiate(ListUniteShopLevel2[Random.Range(0, ListUniteShopLevel2.Count)], UniteSlot.transform.position, Quaternion.identity);
                else
                    Instantiate(ListUniteShopLevel3[Random.Range(0, ListUniteShopLevel3.Count)], UniteSlot.transform.position, Quaternion.identity);
            }
        }
        foreach (GameObject UniteSlot in GameObject.FindGameObjectsWithTag("ItemShopSlot"))
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
        }
    }
}
