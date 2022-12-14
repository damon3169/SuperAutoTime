using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boardAnimationController : MonoBehaviour
{
    public bool isInPlaceForShop = true;

    public bool isInPlaceForFight = false;
    public void boardInPlaceToFight()
    {
        GameObject.FindGameObjectWithTag("BattleController").GetComponent<BattleController>().totalTime = 0;
        GameObject.FindGameObjectWithTag("BattleController").GetComponent<BattleController>().arrow.transform.eulerAngles = new Vector3(0,0,80);
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<PlayerController>().isLocalPlayer)
            {
                player.GetComponent<PlayerController>().setBattlePhaseFromOutside(true);
            }
        }
        isInPlaceForFight = true;
        isInPlaceForShop = false;
    }

    public void boardInPlaceToShop()
    {
        isInPlaceForFight = false;
        isInPlaceForShop = true;
    }

    public void boardSendRandomList()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<PlayerController>().isLocalPlayer)
            {
                player.GetComponent<PlayerController>().isFirstShop = false;
                player.GetComponent<PlayerController>().sendRandomList();
            }
        }
    }

    public void boardResetShop()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<PlayerController>().isLocalPlayer)
            {
                player.GetComponent<PlayerController>().resetShop();
            }
        }
        isInPlaceForFight = false;
        isInPlaceForShop = true;
    }
}
