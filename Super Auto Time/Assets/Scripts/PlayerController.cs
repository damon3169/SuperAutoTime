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
    //public string item;

    public Unite(string name, int health, int damages)
    {
        this.name = name;
        this.health = health;
        this.damages = damages;
        //this.item = item;
    }
}


public class PlayerController : NetworkBehaviour
{
    bool firstTimeMoveUniteTo = true;
    public float moneyLeft = 0;
    public ShopController shop;
    private float journeyLength;
    private Vector3 startPosition;

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
    private bool isSetupDone = false;

    private GameObject boardAnimator;
    private GameObject[] boardSlotList;
    public Vector3 arenaPostion;
    public float startTimeMovement;
    private bool setupEnemyBoard = false;

    // Start is called before the first frame update
    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            boardAnimator = GameObject.FindGameObjectWithTag("Board");
            boardSlotList = GameObject.FindGameObjectsWithTag("boardSlot");
            timerDisplay = GameObject.FindGameObjectWithTag("timerUI").GetComponent<TextMeshProUGUI>();
            for (int index = 0; index < 5; index++)
            {
                addNewUnite("Empty", 0, 0);
            }
            shop = GameObject.FindGameObjectWithTag("Shop").GetComponent<ShopController>();
        }
        if (!isLocalPlayer)
        {
            boardSlotList = GameObject.FindGameObjectsWithTag("BoardSlotE");
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
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
                    //SWITCH SCENE HERE
                    boardAnimator.GetComponent<Animator>().SetBool("CombatStart", true);
                    //isSetupDone = false; do this after fight
                }
            }
            if (readyToBegin)
            {

                if (isBattlePhase)
                {
                    if (boardAnimator.GetComponent<boardAnimationController>().isInPlaceForFight)
                    {
                        boardAnimator.GetComponent<Animator>().SetBool("CombatStart", false);
                        moveUniteTo(boardSlotList[boardSlotList.Length - 1], arenaPostion);
                    }
                }
                else
                {
                    if (!this.isBattlePhase && !isSetupDone)
                    {
                        StartCoroutine(shopPhase());
                        shop.refreshShop();
                        isSetupDone = true;
                    }

                    if (board.Count > 0)
                    {
                        int i = 0;
                        foreach (Unite unite in board)
                        {

                            //Debug.Log("Slot number:" + i + "name=" + unite.name + " health =" + unite.health.ToString() + "damages= "+ unite.damages.ToString());
                            i++;
                        }
                    }
                }


            }
        }
        else if (isBattlePhase && !setupEnemyBoard)
        {
            int i = 0;
            int boardNumber = 4;
            for (boardNumber = 4; boardNumber >= 0; boardNumber--)
            {
                if (board[boardNumber].name != "Empty")
                {
                    GameObject unit = Instantiate(Resources.Load<GameObject>("Prefabs/" + board[boardNumber].name), boardSlotList[4 - boardNumber].transform.position, Quaternion.identity);
                    unit.GetComponent<TimeUnite>().health = board[boardNumber].health;
                    unit.GetComponent<TimeUnite>().damages = board[boardNumber].damages;
                }
                i++;
            }
            setupEnemyBoard = true;
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

    private IEnumerator shopPhase()
    {
        float totalTime = 0;
        while (totalTime <= shopPhaseDuration)
        {
            totalTime += Time.deltaTime;
            timerDisplay.text = (shopPhaseDuration - totalTime).ToString();
            moneyLeft = (shopPhaseDuration - totalTime);
            var integer = (int)totalTime;
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
    public void addNewUnite(string newName, int newHealth, int newDamages)
    {
        board.Add(new Unite(newName, newHealth, newDamages));
    }

    [Command]
    public void addNewUniteAtBoardSlot(int slotNumber, string newName, int newHealth, int newDamages)
    {
        board[slotNumber] = (new Unite(newName, newHealth, newDamages));
    }
    //Add stuff in board unite list from outside

    public void addNewUniteInBoard(int slot, TimeUnite unite)
    {
        addNewUniteAtBoardSlot(slot, unite.nameUnite, unite.health, unite.damages);
    }

    public void addNewUniteInEmpty(int slot)
    {
        addNewUniteAtBoardSlot(slot, "Empty", 0, 0);
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

    public void moveUniteTo(GameObject uniteToMove, Vector3 destination)
    {
        if (firstTimeMoveUniteTo)
        {
            journeyLength = Vector3.Distance(uniteToMove.transform.position, destination);
            startPosition = uniteToMove.transform.position;
            startTimeMovement = Time.time;
            firstTimeMoveUniteTo = false;
        }
        // Distance moved equals elapsed time times speed..
        float distCovered = (Time.time - startTimeMovement) * 5f;

        // Fraction of journey completed equals current distance divided by total distance.
        float fractionOfJourney = distCovered / journeyLength;

        // Set our position as a fraction of the distance between the markers.
        uniteToMove.transform.position = Vector3.Lerp(startPosition, destination, fractionOfJourney);
    }
}
