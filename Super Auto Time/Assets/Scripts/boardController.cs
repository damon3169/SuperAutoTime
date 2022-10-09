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
            if (player.isShopPhase)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit hitInfo = new RaycastHit();
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
                    {
                        if (hitInfo.transform.gameObject == this.gameObject)
                        {
                            if (player.selectedObject)
                            {
                                //LA J'AJOUTE LES ITEMS
                                if (player.selectedObject.GetComponent<ItemsStat>() != null)
                                {
                                    /*player.selectedObject.GetComponent<ItemsStat>().equippedUnite = monsterInSlot;
                                    player.selectedObject.GetComponent<ItemsStat>().addStat();
                                    player.addNewUniteInBoard(this.transform.GetSiblingIndex(), monsterInSlot);
                                    GameObject.DestroyImmediate(player.selectedObject, true);
                                    player.removeSelectedObject();*/
                                }
                                else if (player.selectedObject.GetComponent<Items>() != null)
                                {
                                    /* if (monsterInSlot != null)
                                     {
                                         GameObject.DestroyImmediate(monsterInSlot.itemEquipped, true);
                                         monsterInSlot.itemEquipped = player.selectedObject;
                                         player.selectedObject.GetComponent<Items>().equippedUnite = monsterInSlot;
                                         player.selectedObject.transform.position = this.transform.position;
                                         player.selectedObject.transform.parent = monsterInSlot.transform;
                                         player.selectedObject.transform.GetComponent<SpriteRenderer>().enabled = false;
                                         player.selectedObject.GetComponent<Collider2D>().enabled = false;
                                         player.addNewUniteInBoard(this.transform.GetSiblingIndex(), monsterInSlot);
                                         player.removeSelectedObject();
                                     }*/

                                }
                                else
                                {// LA J'AJOUTE LES UNITE
                                 //Y A PAS DE MOSNTRE DANS CE SLOT
                                    if (monsterInSlot == null)
                                    {
                                        if (player.moneyLeft > player.selectedObject.GetComponent<TimeUnite>().cost)
                                        {
                                            player.moneyLeft -= player.selectedObject.GetComponent<TimeUnite>().cost;
                                            player.selectedObject.transform.position = this.transform.position;
                                            monsterInSlot = player.selectedObject.GetComponent<TimeUnite>();
                                            monsterInSlot.GetComponent<Collider>().enabled = false;
                                            if (monsterInSlot.boardFather != null)
                                            {
                                                monsterInSlot.boardFather.monsterInSlot = null;
                                                player.addNewUniteInEmpty(monsterInSlot.boardFather.transform.GetSiblingIndex());
                                            }
                                            monsterInSlot.boardFather = this;
                                            monsterInSlot.transform.parent = this.transform;
                                            monsterInSlot.isInShop = false;
                                            monsterInSlot.player = player;
                                            player.removeSelectedObject();
                                            player.addNewUniteInBoard(this.transform.GetSiblingIndex(), monsterInSlot);
                                        }
                                    }
                                    else
                                    {
                                        //Si la cible a deja un monstre
                                        if (player.selectedObject.GetComponent<TimeUnite>().boardFather != null)
                                        {
                                            Vector3 pos = monsterInSlot.transform.position;
                                            TimeUnite thisMonster = monsterInSlot;
                                            boardController previousBoard = player.selectedObject.GetComponent<TimeUnite>().boardFather;
                                            //Swap position
                                            monsterInSlot.transform.position = player.selectedObject.transform.position;
                                            player.selectedObject.transform.position = pos;
                                            //Add unite in boardsyncList
                                            player.addNewUniteInBoard(this.transform.GetSiblingIndex(), monsterInSlot);
                                            player.addNewUniteInBoard(player.selectedObject.GetComponent<TimeUnite>().boardFather.transform.GetSiblingIndex(), player.selectedObject.GetComponent<TimeUnite>());
                                            //Swap Unite
                                            monsterInSlot = player.selectedObject.GetComponent<TimeUnite>();
                                            player.selectedObject.GetComponent<TimeUnite>().boardFather.monsterInSlot = thisMonster;
                                            //Swap boardSlot
                                            player.selectedObject.GetComponent<TimeUnite>().boardFather = previousBoard;
                                            monsterInSlot.boardFather = this;
                                            //Swap PARENT
                                            player.selectedObject.transform.parent = player.selectedObject.GetComponent<TimeUnite>().boardFather.transform;
                                            monsterInSlot.transform.parent = this.transform;
                                            //Unselect Unite
                                            player.removeSelectedObject();
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
