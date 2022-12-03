using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    }

    public void OnclickXPButton()
    {
        if (player)
        {
            if (player.moneyLeft >= xpUpValue)
            {
                player.addXPToPlayer(xpUpValue);
                player.totalTime += xpUpValue;
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
        player.totalTime -= player.selectedObject.GetComponent<TimeUnite>().cost / 2;
        player.selectedObject.GetComponent<TimeUnite>().boardFather.monsterInSlot = null;
        player.addNewUniteInEmpty(player.selectedObject.GetComponent<TimeUnite>().boardFather.Order);
        Destroy(player.selectedObject);
        player.selectedObject = null;

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
        player.totalTime = player.shopPhaseDuration;
    }
}
