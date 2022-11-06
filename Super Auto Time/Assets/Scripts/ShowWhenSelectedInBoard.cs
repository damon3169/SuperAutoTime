using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowWhenSelectedInBoard : MonoBehaviour
{
    private PlayerController player;

    void Update()
    {
        if (player==null)
        {
            foreach (GameObject players in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (players.GetComponent<PlayerController>().isLocalPlayer)
                {
                    player = players.GetComponent<PlayerController>();
                }
            }
        }
        else
        {
            if (player.selectedObject)
                if (player.selectedObject.GetComponent<TimeUnite>().boardFather != null)
                {
                    gameObject.GetComponent<Image>().enabled = true;
                }
                else
                {
                    gameObject.GetComponent<Image>().enabled = false;
                }

        }
    }
}
