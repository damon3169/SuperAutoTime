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

    public Unite(string name, int health, int damages)
    {
        this.name = name;
        this.health = health;
        this.damages = damages;
    }
}


public class PlayerController : NetworkBehaviour
{
    public readonly SyncList<Unite> board = new SyncList<Unite>();
    public List<TimeUnite> boardList;

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

    // Start is called before the first frame update
    public override void OnStartClient()
    {
        board.Callback += OnBoardUpdated;
        if (isLocalPlayer)
        {
            timerDisplay = GameObject.FindGameObjectWithTag("timerUI").GetComponent<TextMeshProUGUI>();
        }
        for (int index = 0; index < board.Count; index++)
            OnBoardUpdated(SyncList<Unite>.Operation.OP_ADD, index, new Unite(), board[index]);
    }

    // Update is called once per frame
    void Update()
    {
        if (readyToBegin)
        {
            if (isLocalPlayer)
            {
                if (!this.isBattlePhase)
                {
                    StartCoroutine(shopPhase(shopPhaseDuration));
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
                    foreach (Unite unite in board)
                    {
                        Debug.Log("name=" + unite.name + " health =" + unite.health.ToString() + "damages= " + unite.damages.ToString());
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
                    setBattlePhase(true);
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

    private IEnumerator shopPhase(float shopPhaseDuration)
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
        StopCoroutine(shopPhase(shopPhaseDuration));
    }

    [Command]
    private void setShopPhase(bool newStatus)
    {
        isShopPhase = newStatus;
    }
    void OnBoardUpdated(SyncList<Unite>.Operation op, int index, Unite oldUnite, Unite newUnite)
    {
        switch (op)
        {
            case SyncList<Unite>.Operation.OP_ADD:
                break;
            case SyncList<Unite>.Operation.OP_INSERT:
                break;
            case SyncList<Unite>.Operation.OP_REMOVEAT:
                break;
            case SyncList<Unite>.Operation.OP_SET:
                break;
            case SyncList<Unite>.Operation.OP_CLEAR:
                break;
        }
    }

    [Command]
    void addNewUnite(string newName, int newHealth, int newDamages)
    {
        board.Add(new Unite(newName, newHealth, newDamages));
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
            selectedObject.GetComponent<TimeUnite>().spriteSelected.enabled = false;
        }
        selectedObject=null;
    }
}
