using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    public PlayerController player;
    public SpriteRenderer spriteSelected;
    public TimeUnite equippedUnite;
    public string nameItem = "Empty_item";
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
                    Collider2D clicked_collider = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    if (clicked_collider != null && clicked_collider.transform.gameObject == this.gameObject)
                    {
                        selectObject();
                    }

                }
            }
        }
        else if (GameObject.FindGameObjectWithTag("Player"))
        {
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (this.transform.tag == "BoardSlotE")
                {
                    if (!player.GetComponent<PlayerController>().isLocalPlayer)
                    {
                        this.player = player.GetComponent<PlayerController>();
                    }
                }
                else if (player.GetComponent<PlayerController>().isLocalPlayer)
                {
                    this.player = player.GetComponent<PlayerController>();
                }
                
            }
        }
    }

    public void selectObject()
    {
        player.removeSelectedObject();
        player.selectedObject = this.gameObject;
        spriteSelected.enabled = true;
    }
}
