using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public struct Unite
{
    public string name;
    public int health;
    public int damages;
    public string item;

    public Unite(string name, int health, int damages, string item)
    {
        this.name = name;
        this.health = health;
        this.damages = damages;
        this.item = item;
    }
}


public class PlayerController : NetworkBehaviour
{
    public ShopController shop;

    public readonly SyncList<Unite> board = new SyncList<Unite>();
    public List<TimeUnite> boardList;
    public int round;
    //States of the game
    [SyncVar]
    public bool isBattlePhase = false;
    [SyncVar]
    public bool isShopPhase = true;
    public float shopPhaseDuration = 30;

    private TextMeshProUGUI timerDisplay;
    [SyncVar]
    public bool readyToBegin = false;
    public GameObject selectedObject;
    private bool isSetupDone=false;

    // Start is called before the first frame update
    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            timerDisplay = GameObject.FindGameObjectWithTag("timerUI").GetComponent<TextMeshProUGUI>();
            for (int index = 0; index < 5; index++)
            {
                addNewUnite("Empty", 0, 0, "Empty_item");
            }
            shop = GameObject.FindGameObjectWithTag("Shop").GetComponent<ShopController>();
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (readyToBegin)
        {
            if (isLocalPlayer)
            {
                if (!this.isBattlePhase && !isSetupDone)
                {
                    StartCoroutine(shopPhase());
                    shop.refreshShop();
                    isSetupDone = true;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    /* foreach (TimeUnite unite in boardList)
                     {
                         addNewUnite(unite.name, unite.health, unite.damages);
                     }*/
                }
                if (Input.GetMouseButtonDown(1))
                {
                    removeSelectedObject();
                }
                if (board.Count > 0)
                {
                     int i = 0;
                     foreach (Unite unite in board)
                     {

                         Debug.Log("Slot number:" + i + "name=" + unite.name + " health =" + unite.health.ToString() + "damages= " 
                         + unite.damages.ToString()+ "item= " + unite.item);
                         i++;
                     }
                }
                if (isBattlePhase)
                {
                    //Switch scene et lancer le fight
                }
            }
        }
        if (isLocalPlayer)
        {
            if (isServer && shopPhaseDuration < 6)
            {
                shopPhaseDuration += 6;
            }
            if (GameObject.FindGameObjectsWithTag("Player").Length > 1)
            {
                setReadyToBegin();
            }
            if (!isBattlePhase && readyToBegin)
            {
                bool isReadyToBattle = true;
                foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                {
                    if (player.GetComponent<PlayerController>().isShopPhase)
                    {
                        isReadyToBattle = false;
                    }
                }
                if (isReadyToBattle)
                {
                    Debug.Log("GOD C'ETAIT TROP CHIANT MAIS CA MARCHE ENFIN PUTAIN");
                    removeSelectedObject();
                    isSetupDone = false;
                    setBattlePhase(true);
                    //SWITCH SCENE HERE
                }
            }
        }

    }

    [Command]
    private void setReadyToBegin()
    {
        readyToBegin = true;
    }

    [Command]
    private void setBattlePhase(bool newStatus)
    {
        isBattlePhase = newStatus;
    }

    private IEnumerator shopPhase( )
    {
        float totalTime = 0;
        while (totalTime <= shopPhaseDuration)
        {
            totalTime += Time.deltaTime;
            timerDisplay.text = (shopPhaseDuration - totalTime).ToString();

            var integer = (int)totalTime; /* choose how to quantize this */
            /* convert integer to string and assign to text */
            yield return null;
        }
        //changeShopPhaseStatus(myPLayerID, false);
        setShopPhase(false);
        StopCoroutine(shopPhase());
    }

    [Command]
    private void setShopPhase(bool newStatus)
    {
        isShopPhase = newStatus;
    }

    //Add stuff in board unite list
    [Command]
    public void addNewUnite(string newName, int newHealth, int newDamages, string item)
    {
        board.Add(new Unite(newName, newHealth, newDamages, item));
    }

    [Command]
    public void addNewUniteAtBoardSlot(int slotNumber, string newName, int newHealth, int newDamages, string item)
    {
        board[slotNumber] = (new Unite(newName, newHealth, newDamages, item));
    }
    //Add stuff in board unite list from outside

    public void addNewUniteInBoard(int slot, TimeUnite unite)
    {
        addNewUniteAtBoardSlot(slot, unite.nameUnite, unite.health, unite.damages, unite.itemEquipped.GetComponent<Items>().nameItem);
    }

    public void addNewUniteInEmpty(int slot)
    {
        addNewUniteAtBoardSlot(slot, "Empty", 0, 0, "Empty_item");
    }

    [Command]
    void RemoveUnite(int unitePlacement)
    {
        board.RemoveAt(unitePlacement);
    }

    public void removeSelectedObject()
    {
        if (selectedObject != null)
        {
            if (selectedObject.GetComponent<TimeUnite>())
                selectedObject.GetComponent<TimeUnite>().spriteSelected.enabled = false;
            else
                selectedObject.GetComponent<Items>().spriteSelected.enabled = false;
        }
        selectedObject = null;
    }
}
