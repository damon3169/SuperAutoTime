using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boardAnimationController : MonoBehaviour
{
          public bool isInPlaceForShop= false;

    public bool isInPlaceForFight= false;
    public void boardInPlaceToFight()
    {
        isInPlaceForFight=true;
        isInPlaceForShop=false;
        
    }

    public void boardInPlaceToShop()
    {
        isInPlaceForFight=false;
        isInPlaceForShop=true;
    }

    public void boardSendRandomList()
    {
        foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if(player.GetComponent<PlayerController>().isLocalPlayer)
            {
                player.GetComponent<PlayerController>().sendRandomList();
            }
        }
    }
}
