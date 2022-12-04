using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boardController : MonoBehaviour
{
    public PlayerController player;
    public TimeUnite monsterInSlot;
    public int Order;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            if (player.isShopPhaseLocal)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit hitInfo = new RaycastHit();
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
                    {
                        if (hitInfo.transform.gameObject == this.gameObject)
                        {
                            if (player.selectedObject != null)
                            {
                                // LA J'AJOUTE LES UNITE
                                //Y A PAS DE MOSNTRE DANS CE SLOT -- BUY UNIT
                                if (monsterInSlot == null)
                                {
                                    if (!player.selectedObject.GetComponent<TimeUnite>().isInShop)
                                    {
                                        player.selectedObject.transform.position = this.transform.position;
                                        monsterInSlot = player.selectedObject.GetComponent<TimeUnite>();
                                        if (monsterInSlot.boardFather != null)
                                        {
                                            monsterInSlot.boardFather.monsterInSlot = null;
                                            player.addNewUniteInEmpty(monsterInSlot.boardFather.Order);
                                        }
                                        monsterInSlot.boardFather = this;
                                        monsterInSlot.transform.parent = this.transform;
                                        monsterInSlot.isInShop = false;
                                        monsterInSlot.player = player;
                                        player.removeSelectedObject();
                                        player.addNewUniteInBoard(Order, monsterInSlot);
                                    }
                                    else if (player.moneyLeft >= player.selectedObject.GetComponent<TimeUnite>().cost)
                                    {
                                        player.selectedObject.GetComponent<TimeUnite>().isFreeze = false;
                                        player.totalTime += player.selectedObject.GetComponent<TimeUnite>().cost;
                                        player.selectedObject.transform.position = this.transform.position;
                                        monsterInSlot = player.selectedObject.GetComponent<TimeUnite>();
                                        monsterInSlot.GetComponent<Collider>().enabled = false;
                                        if (monsterInSlot.boardFather != null)
                                        {
                                            monsterInSlot.boardFather.monsterInSlot = null;
                                            player.addNewUniteInEmpty(monsterInSlot.boardFather.Order);
                                        }
                                        monsterInSlot.boardFather = this;
                                        monsterInSlot.transform.parent = this.transform;
                                        monsterInSlot.isInShop = false;
                                        monsterInSlot.player = player;
                                        player.removeSelectedObject();
                                        player.addNewUniteInBoard(Order, monsterInSlot);
                                    }
                                }
                                else
                                {
                                    //Si selected a deja un board
                                    if (player.selectedObject.GetComponent<TimeUnite>().boardFather != null)
                                    {
                                        //Si monstre est le meme, combine
                                        if (player.selectedObject.GetComponent<TimeUnite>().nameUnite == monsterInSlot.nameUnite && player.selectedObject != monsterInSlot.gameObject)
                                        {
                                            Debug.Log("combine");
                                            monsterInSlot.health += player.selectedObject.GetComponent<TimeUnite>().health;
                                            monsterInSlot.damages += player.selectedObject.GetComponent<TimeUnite>().damages;
                                            //Add unite in boardsyncList
                                            player.addNewUniteInEmpty(player.selectedObject.GetComponent<TimeUnite>().boardFather.Order);
                                            player.addNewUniteInBoard(Order, player.selectedObject.GetComponent<TimeUnite>());
                                            player.selectedObject.GetComponent<TimeUnite>().boardFather.monsterInSlot = null;
                                            GameObject.Destroy(player.selectedObject);
                                            player.removeSelectedObject();
                                        }
                                        //Sinon move swap
                                        else
                                        {
                                            Debug.Log("swap");
                                            Vector3 pos = monsterInSlot.transform.position;
                                            TimeUnite thisMonster = monsterInSlot;
                                            boardController previousBoard = player.selectedObject.GetComponent<TimeUnite>().boardFather;
                                            //Swap position
                                            monsterInSlot.transform.position = player.selectedObject.transform.position;
                                            player.selectedObject.transform.position = pos;
                                            //Add unite in boardsyncList
                                            player.addNewUniteInBoard(previousBoard.Order, monsterInSlot);
                                            player.addNewUniteInBoard(Order, player.selectedObject.GetComponent<TimeUnite>());
                                            //Swap Unite
                                            monsterInSlot = player.selectedObject.GetComponent<TimeUnite>();
                                            player.selectedObject.GetComponent<TimeUnite>().boardFather.monsterInSlot = thisMonster;
                                            //Swap boardSlot
                                            thisMonster.boardFather = previousBoard;
                                            monsterInSlot.boardFather = this;
                                            //Swap PARENT
                                            thisMonster.transform.parent = thisMonster.boardFather.transform;
                                            monsterInSlot.transform.parent = this.transform;
                                            //Unselect Unite
                                            player.removeSelectedObject();
                                        }
                                    }
                                    else if (player.selectedObject.GetComponent<TimeUnite>().nameUnite == monsterInSlot.nameUnite)
                                    {
                                        if (player.selectedObject.GetComponent<TimeUnite>().isInShop)
                                        {
                                            if (player.moneyLeft >= player.selectedObject.GetComponent<TimeUnite>().cost)
                                            {
                                                monsterInSlot.health += player.selectedObject.GetComponent<TimeUnite>().health;
                                                monsterInSlot.damages += player.selectedObject.GetComponent<TimeUnite>().damages;
                                                player.totalTime += player.selectedObject.GetComponent<TimeUnite>().cost;
                                                player.addNewUniteInBoard(Order, monsterInSlot);
                                                GameObject.Destroy(player.selectedObject);
                                                player.removeSelectedObject();
                                            }
                                        }
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


        }
        else if (GameObject.FindGameObjectWithTag("Player") && player == null)
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
