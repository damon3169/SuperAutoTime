using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeUnite : MonoBehaviour
{
    public int health;
    public int damages;
    public PlayerController player;
    public SpriteRenderer spriteSelected;
    public boardController boardFather;
    public string nameUnite = "Empty";
    public int level = 1;
    public int cost = 3;

    public GameObject itemEquipped;

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
                        selectObject();
                    }

                }
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
}
