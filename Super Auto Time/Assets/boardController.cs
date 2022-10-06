using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boardController : MonoBehaviour
{
    public PlayerController player;
    public TimeUnite monsterInSlot;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            if (player.isShopPhase)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Collider2D clicked_collider = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    if (clicked_collider != null && clicked_collider.transform.gameObject == this.gameObject)
                    {
                        if (player.selectedObject)
                        {
                            //IL EST DANS UN BOARD
                            if (monsterInSlot == null)
                            {
                                player.selectedObject.transform.position = this.transform.position;
                                monsterInSlot = player.selectedObject.GetComponent<TimeUnite>();
                                monsterInSlot.GetComponent<Collider2D>().enabled = false;
                                if (monsterInSlot.boardFather != null)
                                {
                                    monsterInSlot.boardFather.monsterInSlot = null;
                                }
                                monsterInSlot.boardFather = this;
                                player.removeSelectedObject();

                            }
                            else
                            {
                                //IL EST DANS UN BOARD
                                if (player.selectedObject.GetComponent<TimeUnite>().boardFather != null)
                                {
                                    Vector3 pos = monsterInSlot.transform.position;
                                    TimeUnite thisMonster = monsterInSlot;
                                    boardController previousBoard = player.selectedObject.GetComponent<TimeUnite>().boardFather;
                                    //Swap position
                                    monsterInSlot.transform.position = player.selectedObject.transform.position;
                                    player.selectedObject.transform.position = pos;
                                    //Swap Unite
                                    monsterInSlot = player.selectedObject.GetComponent<TimeUnite>();
                                    player.selectedObject.GetComponent<TimeUnite>().boardFather.monsterInSlot = thisMonster;
                                    //Swap boardSlot
                                    player.selectedObject.GetComponent<TimeUnite>().boardFather = previousBoard;
                                    monsterInSlot.boardFather = this;
                                    //Unselect Unite
                                    player.removeSelectedObject();
                                }

                            }

                        }
                        else if (monsterInSlot != null)
                        {
                            monsterInSlot.selectObject();
                        }

                    }

                }
            }
        }
        else if (GameObject.FindGameObjectWithTag("Player"))
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }
    }
}
