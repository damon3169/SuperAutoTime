using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeUnite : MonoBehaviour
{
    public string nameUnite = "Empty";

    public int health;
    public int damages;
    public PlayerController player;
    public SpriteRenderer spriteSelected;
    public boardController boardFather;
    public int level = 1;
    public int cost = 3;
    public enum Triggers // your custom enumeration
    {
        startOfShop,
        EnOfShop,
        Every1s
    };
    public Triggers triggerList;
    private bool isShopphaseBegin = false;
    private bool isShopphaseEnd = false;
    public bool InPlaceForFight = false;
    public bool isFighting = false;
    public bool isInShop = true;

    public PlayerController otherPlayer;
    bool isEnemyDead;
    public int positionInBoard;
    public IEnumerator fightCorout;

    private void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {

        if (player != null)
        {
            if (player.isShopPhase)
            {
                if (triggerList == Triggers.startOfShop && !isShopphaseBegin && boardFather != null)
                {
                    //LAUNCH EFFECT
                    isShopphaseBegin = true;
                    isShopphaseEnd = false;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    /*Collider2D clicked_collider = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    if (clicked_collider != null && clicked_collider.transform.gameObject == this.gameObject)
                    {
                        selectObject();
                    }
                    Debug.Log("Mouse is pressed down");*/

                    RaycastHit hitInfo = new RaycastHit();
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
                    {
                        if (hitInfo.transform.gameObject == this.gameObject)
                        {
                            selectObject();
                        }
                    }
                }
            }
            else
            {
                if (!isInShop)
                {
                    if (boardFather.tag == "BoardSloatE")
                    {
                        player = otherPlayer;
                    }
                    if (triggerList == Triggers.EnOfShop && !isShopphaseEnd && boardFather != null)
                    {
                        //LAUNCH EFFECT
                        isShopphaseEnd = true;
                        isShopphaseBegin = false;
                    }
                    if (player.isTimerLaunch)
                    {

                    }

                }
            }
        }
        if (isInShop)
        {
            if (player == null)
                if (GameObject.FindGameObjectWithTag("Player") && player == null)
                {
                    foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                    {
                        if (player.GetComponent<PlayerController>().isLocalPlayer)
                        {
                            this.player = player.GetComponent<PlayerController>();
                        }
                        else
                        {
                            otherPlayer = player.GetComponent<PlayerController>();
                        }
                    }
                }
        }
        else
        {
            if (GameObject.FindGameObjectWithTag("Player") && player != null)
            {
                foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                {
                    if (this.player.gameObject != player)
                    {
                        this.otherPlayer = player.GetComponent<PlayerController>();
                    }

                }
            }
        }

        if (this.health < 0)
        {
            player.boardSlotList[this.positionInBoard].GetComponent<boardController>().monsterInSlot = null;
            if(this == player.fightingUnite)
            {
                player.fightingUnite = null;
            }
            player.launchMoveunite = true;
            GameObject.DestroyImmediate(this.gameObject, true);
        }
    }


    public void onEndPurchasePhase()
    {

    }
    public void onBeginPurchasePhase()
    {

    }
    public void onBeginBattlePhase()
    {

    }

    public void selectObject()
    {
        player.removeSelectedObject();
        player.selectedObject = this.gameObject;
        spriteSelected.enabled = true;
    }

    public void takeDamages(int damages)
    {
        health -= damages;
    }



}
