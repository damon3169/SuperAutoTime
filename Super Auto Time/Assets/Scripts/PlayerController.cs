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
    public float moneyLeft = 0;
    public ShopController shop;
    private float journeyLength;
    private Vector3 startPosition;
    public int life = 10;
    public readonly SyncList<Unite> board = new SyncList<Unite>();
    public readonly SyncList<int> listRandom = new SyncList<int>();
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
    public GameObject[] boardSlotList;
    public GameObject[] boardSlotListTemp;

    private Transform arenaPostion;
    public float startTimeMovement;
    private bool setupEnemyBoard = false;
    bool unitMoving = false;
    Vector3 nextPos;
    public bool canLaunchTimerFight = false;
    public bool isTimerLaunch = false;
    public float timeFight = 0;
    public float beginTimeFight;
    public TimeUnite fightingUnite;
    public float timeBeginMoving;
    public bool beginMoving = true;
    public bool launchMoveunite = false;
    public List<TimeUnite> uniteList;
    public float totalTime = 0;
    public int randomSelected = 0;


    // Start is called before the first frame update
    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            // arenaPostion = GameObject.FindGameObjectWithTag("ArenaLocalPos").transform;
            boardAnimator = GameObject.FindGameObjectWithTag("Board");
            boardSlotList = new GameObject[6];
            boardSlotListTemp = GameObject.FindGameObjectsWithTag("boardSlot");
            for (int i = 0; i < boardSlotListTemp.Length; i++)
            {
                foreach (GameObject slot in boardSlotListTemp)
                {
                    if (slot.GetComponent<boardController>().Order == i)
                    {
                        boardSlotList[i] = slot;
                    }
                }
            }
            timerDisplay = GameObject.FindGameObjectWithTag("timerUI").GetComponent<TextMeshProUGUI>();
            for (int index = 0; index < 6; index++)
            {
                addNewUnite("Empty", 0, 0);
            }
            shop = GameObject.FindGameObjectWithTag("Shop").GetComponent<ShopController>();
        }
        if (!isLocalPlayer)
        {
            boardAnimator = GameObject.FindGameObjectWithTag("Board");
            //arenaPostion = GameObject.FindGameObjectWithTag("ArenaLocalPosE").transform;
            boardSlotListTemp = GameObject.FindGameObjectsWithTag("BoardSlotE");
            boardSlotList = new GameObject[6];
            for (int i = 0; i < boardSlotListTemp.Length; i++)
            {
                foreach (GameObject slot in boardSlotListTemp)
                {
                    if (slot.GetComponent<boardController>().Order == i)
                    {
                        boardSlotList[i] = slot;
                    }
                }
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (isBattlePhase)
        {
            if (launchMoveunite)
            {
                if (beginMoving)
                {
                    timeBeginMoving = Time.time;
                    beginMoving = false;
                    uniteList.Clear();
                    foreach (GameObject slot in boardSlotList)
                    {
                        if (slot.GetComponent<boardController>().monsterInSlot)
                        {
                            slot.GetComponent<boardController>().monsterInSlot.InPlaceForFight = false;
                            uniteList.Add(slot.GetComponent<boardController>().monsterInSlot);
                        }
                    }
                }
                moveUniteToEmptySlot();
            }
        }
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
                    if (!canLaunchTimerFight && !launchMoveunite)
                    {
                        if (boardAnimator.GetComponent<boardAnimationController>().isInPlaceForFight)
                        {
                            boardAnimator.GetComponent<Animator>().SetBool("CombatStart", false);

                            if (beginMoving)
                            {
                                uniteList.Clear();
                                foreach (GameObject slot in boardSlotList)
                                {
                                    if (slot.GetComponent<boardController>().monsterInSlot)
                                    {
                                        slot.GetComponent<boardController>().monsterInSlot.InPlaceForFight = false;
                                        uniteList.Add(slot.GetComponent<boardController>().monsterInSlot);


                                    }
                                }
                                timeBeginMoving = Time.time;
                                beginMoving = false;
                            }
                            moveUniteToEmptySlot();
                        }
                    }
                    else
                    {
                        if (!isTimerLaunch)
                        {
                            beginTimeFight = Time.time;
                            isTimerLaunch = true;
                        }
                        timeFight = Time.time - beginTimeFight;
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
        else
        {
            if (isBattlePhase && !setupEnemyBoard)
            {
                int boardNumber = 0;
                foreach (Unite unite in board)
                {
                    if (unite.name != "Empty")
                    {
                        GameObject unit = Instantiate(Resources.Load<GameObject>("Prefabs/" + unite.name), boardSlotList[boardNumber].transform.position, Quaternion.identity);
                        unit.GetComponent<TimeUnite>().health = unite.health;
                        unit.GetComponent<TimeUnite>().damages = unite.damages;
                        unit.GetComponent<TimeUnite>().boardFather = boardSlotList[boardNumber].GetComponent<boardController>();
                        unit.GetComponent<TimeUnite>().boardFather.monsterInSlot = unit.GetComponent<TimeUnite>();
                        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                        {
                            if (this != player.GetComponent<PlayerController>())
                            {
                                unit.GetComponent<TimeUnite>().otherPlayer = player.GetComponent<PlayerController>();
                            }
                        }
                        unit.GetComponent<TimeUnite>().player = this;
                        unit.GetComponent<TimeUnite>().boardFather.monsterInSlot.isInShop = false;
                    }
                    boardNumber++;
                }

                setupEnemyBoard = true;
            }
            if (isBattlePhase && !canLaunchTimerFight && setupEnemyBoard && !launchMoveunite)
            {
                if (beginMoving)
                {
                    timeBeginMoving = Time.time;
                    beginMoving = false;
                    uniteList.Clear();
                    foreach (GameObject slot in boardSlotList)
                    {
                        if (slot.GetComponent<boardController>().monsterInSlot)
                        {
                            slot.GetComponent<boardController>().monsterInSlot.InPlaceForFight = false;
                            uniteList.Add(slot.GetComponent<boardController>().monsterInSlot);
                        }
                    }
                }
                moveUniteToEmptySlot();
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

    public void setBattlePhaseFromOutside(bool newStatus)
    {
        setBattlePhase( newStatus);
    }

    private IEnumerator shopPhase()
    {
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

    public bool moveUniteTo(GameObject uniteToMove, Vector3 destination)
    {
        // The center of the arc
        Vector3 center = (uniteToMove.transform.position + destination) * 0.5F;

        // move the center a bit downwards to make the arc vertical
        center -= new Vector3(0, 1, 0);

        // Interpolate over the arc relative to center
        Vector3 riseRelCenter = uniteToMove.transform.position - center;
        Vector3 setRelCenter = destination - center;

        // The fraction of the animation that has happened so far is
        // equal to the elapsed time divided by the desired time for
        // the total journey.
        float fracComplete = (Time.time - timeBeginMoving) / 1f;

        uniteToMove.transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, fracComplete);
        uniteToMove.transform.position += center;
        // Do something when we reach the target
        if (uniteToMove.transform.position == destination)
        {
            uniteToMove.GetComponent<TimeUnite>().InPlaceForFight = true;
            return true;
        }
        else return false;

    }

    public void moveUniteToEmptySlot()
    {
        bool asChange = false;
        bool ready = true;

        for (int i = 0; i < uniteList.Count; i++)
        {
            if (!uniteList[i].InPlaceForFight)
            {
                asChange = false;
                uniteList[i].positionInBoard = i;
                unitMoving = true;
                asChange = moveUniteTo(uniteList[i].gameObject, boardSlotList[i].transform.position);
                if (asChange)
                {
                    ready = false;
                    if (i == 0)
                    {
                        fightingUnite = uniteList[i];
                        uniteList[i].isFighting = true;
                    }
                    boardSlotList[i].GetComponent<boardController>().monsterInSlot = uniteList[i];
                    uniteList[i].boardFather = boardSlotList[i].GetComponent<boardController>();
                    uniteList[i].positionInBoard = i;
                    uniteList[i].InPlaceForFight = true;
                }
                else
                {
                    ready = false;
                }
            }
        }
        int popo = 0;

        if (ready)
        {
            unitMoving = false;
            foreach (GameObject slot in boardSlotList)
            {
                if (popo > uniteList.Count - 1)
                {
                    slot.GetComponent<boardController>().monsterInSlot = null;
                }

                popo++;
            }
        }

        if (!unitMoving)
        {
            if (launchMoveunite == true) launchMoveunite = false;
            if (canLaunchTimerFight == false) canLaunchTimerFight = true;
            beginMoving = true;
        }
    }
    public List<TimeUnite> lookForSpellEffect(string effectName)
    {
        List<TimeUnite> unitesWithEffect = new List<TimeUnite>();
        foreach (GameObject slot in boardSlotList)
        {
            if (slot.GetComponent<boardController>().monsterInSlot)
            {
                if (slot.GetComponent<boardController>().monsterInSlot.triggerList.ToString() == effectName)
                {
                    unitesWithEffect.Add(slot.GetComponent<boardController>().monsterInSlot);
                }
            }
        }
        return unitesWithEffect;
    }

    [Command]
    public void sendRandomList()
    {
        for (int i = 0; i < 120; i++)
        {
            listRandom.Add(Random.Range(0, 5));
        }
    }
    [Command]
    public void RemoveAtRandomList(int number)
    {
        this.listRandom.RemoveAt(number);
    }

    public int getNumberUnits()
    {
        int i = 0;
        foreach(GameObject slot in boardSlotList)
        {
            if (slot.GetComponent<boardController>().monsterInSlot)
            {
                i++;
            }
        }
        return i;
    }
}
