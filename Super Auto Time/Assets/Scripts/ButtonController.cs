using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ButtonController : MonoBehaviour
{
    private PlayerController player;
    public int xpUpValue = 3;
    private ShopController shop;
    public int valueRefreshSop = 10;
    private void Start()
    {
        shop = GameObject.FindGameObjectWithTag("Shop").GetComponent<ShopController>();
    }
    void Update()
    {
        if (player == null)
        {
            foreach (GameObject players in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (players.GetComponent<PlayerController>().isLocalPlayer)
                {
                    player = players.GetComponent<PlayerController>();
                }
            }
        }
        else{
             if (this.name == "End Turn Button")
        {
            this.transform.GetChild(0).GetComponent<TMP_Text>().text = "end turn and add "+(int)player.moneyLeft+" XP";
        }
        }
       
    }


    public void OnClickRefreshButton()
    {
        if (shop)
        {
            if (player.moneyLeft >= valueRefreshSop)
            {
                shop.refreshShop();
                player.totalTime += valueRefreshSop;
            }

        }
    }

    public void onSelectedInBoard()
    {

    }

    public void onFreezeButton()
    {
        if (!player.isFreeze)
        {
            foreach (GameObject UniteSlot in GameObject.FindGameObjectsWithTag("UniteShopSlot"))
            {
                if (UniteSlot.transform.childCount > 2)
                    UniteSlot.transform.GetChild(2).GetComponent<TimeUnite>().isFreeze = true;
            }
            player.isFreeze = true;
            player.totalTime += 1;
        }
        else
        {
            foreach (GameObject UniteSlot in GameObject.FindGameObjectsWithTag("UniteShopSlot"))
            {
                if (UniteSlot.transform.childCount > 2)
                    UniteSlot.transform.GetChild(2).GetComponent<TimeUnite>().isFreeze = false;
            }
            player.isFreeze = false;
        }
    }

    public void clickOnEndButton()
    {
        if (player)
        {
            player.totalTime = player.shopPhaseDuration;
            player.addXPToPlayer(((int)player.moneyLeft));
            player.totalTime = player.shopPhaseDuration;
        }
    }
}
