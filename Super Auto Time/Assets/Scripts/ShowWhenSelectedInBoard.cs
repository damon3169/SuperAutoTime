using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowWhenSelectedInBoard : MonoBehaviour
{
    private PlayerController player;

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
        else
        {
            if (player.selectedObject)
                if (player.selectedObject.GetComponent<TimeUnite>().boardFather != null)
                {
                    gameObject.GetComponent<SpriteRenderer>().enabled = true;
                    gameObject.GetComponent<Collider>().enabled = true;

                    if (Input.GetMouseButtonUp(0))
                    {
                        RaycastHit hitInfo = new RaycastHit();
                        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
                        {
                            if (hitInfo.transform.gameObject == this.gameObject)
                            {
                                player.totalTime -= player.selectedObject.GetComponent<TimeUnite>().cost / 2;
                                player.selectedObject.GetComponent<TimeUnite>().boardFather.monsterInSlot = null;
                                player.addNewUniteInEmpty(player.selectedObject.GetComponent<TimeUnite>().boardFather.Order);
                                Destroy(player.selectedObject);
                                player.removeSelectedObject();
                            }
                        }
                    }
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    gameObject.GetComponent<Collider>().enabled = false;
                }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                gameObject.GetComponent<Collider>().enabled = false;
            }

        }
    }
}
