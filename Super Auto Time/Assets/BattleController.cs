using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    public PlayerController playerLocal;
    public PlayerController playerDist;
    IEnumerator fightCorout;
    public bool isHiting = false;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerLocal != null && playerDist != null)
        {
            if (playerLocal.isBattlePhase && playerDist.isBattlePhase)
                if (playerDist.canLaunchTimerFight && playerLocal.canLaunchTimerFight)
                    if (playerLocal.fightingUnite && playerDist.fightingUnite)
                    {
                        if (isHiting == false)
                        {
                            isHiting = true;
                            fightCorout = onCoroutine();
                            StartCoroutine(fightCorout);
                        }

                    }
                    else
                    {
                        if (isHiting)
                        {
                            StopCoroutine(fightCorout);
                        }
                        isHiting = false;
                    }
        }
        else
        {

            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (player.GetComponent<PlayerController>().isLocalPlayer)
                {
                    this.playerLocal = player.GetComponent<PlayerController>();
                }
                else
                {
                    playerDist = player.GetComponent<PlayerController>();
                }
            }

        }
    }
    IEnumerator onCoroutine()
    {
        while (isHiting)
        {
            int damage1 = playerLocal.fightingUnite.damages;
            int damage2 = playerDist.fightingUnite.damages;
            playerDist.fightingUnite.takeDamages(damage1);
            playerLocal.fightingUnite.takeDamages(damage2);
            yield return new WaitForSeconds(1f);
        }
    }
}
