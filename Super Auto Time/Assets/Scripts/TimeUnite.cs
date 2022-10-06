using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeUnite : MonoBehaviour
{
    public int health;
    public int damages;
    public int positionOnBoard = 0;
    public PlayerController player;
    private float turnSpeed = 45.0f;
    public SpriteRenderer spriteSelected;
    public boardController boardFather;
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }
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
                    if (clicked_collider!=null && clicked_collider.transform.gameObject == this.gameObject)
                    {
                        selectObject();
                    }

                }
            }
        }
        else if (GameObject.FindGameObjectWithTag("Player"))
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
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
