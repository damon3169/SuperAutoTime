using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayLevel : MonoBehaviour
{
    private TMP_Text displayText;
    private PlayerController player;

    private void Start()
    {
        displayText = this.GetComponent<TMP_Text>();
    }
    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            if (player.playerCurrentLevel < player.playerMaxLevel)
            {
                displayText.text = "Level " + player.playerCurrentLevel + " " + player.playerXP + "/" + player.playerLevelXP[player.playerCurrentLevel - 1];
            }
            else
            {
                displayText.text = "Level " + player.playerCurrentLevel;
            }
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
}
