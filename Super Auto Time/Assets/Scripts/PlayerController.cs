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
    public List<int> playerLevelXP;
    public int playerXP;
    public int playerMaxLevel = 3;
    public int XPPerRound = 3;
    public int playerCurrentLevel = 1;
    public float moneyLeft = 0;
    public ShopController shop;
    private float journeyLength;
    private Vector3 startPosition;
    [SyncVar]
    public int life = 10;
    public readonly SyncList<Unite> board = new SyncList<Unite>();
    public readonly SyncList<int> listRandom = new SyncList<int>();
    public List<TimeUnite> boardList;
    public int round = 0;
    //States of the game
    [SyncVar]
    public bool isBattlePhaseOnline = false;
    public bool isBattlePhaseLocal = false;
    [SyncVar]
    public bool isShopPhaseOnline = true;
    public bool isShopPhaseLocal = true;
    public float baseShopDuration = 60;
    [HideInInspector]
    public float shopPhaseDuration = 60;

    private TextMeshProUGUI timerDisplay;
    [SyncVar]
    public bool readyToBegin = false;
    public GameObject selectedObject;
    private bool isSetupDone = false;

    public GameObject boardAnimator;
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
    public bool isFirstShop = true;
    public PlayerController otherPlayer;
    private TMP_Text PVCounter;

    // Start is called before the first frame update
    public override void OnStartClient()
    {
        Time.timeScale = 0.9f;
        shopPhaseDuration = baseShopDuration;
        if (isLocalPlayer)
        {
            PVCounter = GameObject.FindGameObjectWithTag("PV").GetComponent<TMP_Text>();
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
        if (!otherPlayer)
        {
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (this != player.GetComponent<PlayerController>())
                {
                    otherPlayer = player.GetComponent<PlayerController>();
                }
            }
        }
        else
        {
            if (life == 0 || otherPlayer.life == 0)
            {
                Debug.Log("GAME IS DONE");
            }
        }
        if (Input.GetMouseButtonDown(1) && selectedObject)
        {
            this.selectedObject.GetComponent<TimeUnite>().spriteSelected.enabled = false;
            this.selectedObject = null;
        }
        //CHECK IF NEED TO LEVEL UP
        if (playerCurrentLevel < playerMaxLevel)
        {
            if (playerXP >= playerLevelXP[playerCurrentLevel - 1])
            {
                playerCurrentLevel += 1;
                playerXP = 0;
            }
        }

        if (isLocalPlayer)
        {
            PVCounter.text = this.life.ToString() + " PV";
            //IF TWO PLAYERS BEGIN
            if (!readyToBegin && GameObject.FindGameObjectsWithTag("Player").Length > 1)
            {
                setReadyToBegin();
            }
            //place unit for fight if someone dies
            if (isBattlePhaseLocal)
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
            //check if both player are ready to begin shop phase
            if (!isBattlePhaseLocal && readyToBegin)
            {
                bool isReadyToBattle = true;
                foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                {
                    if (player == this.gameObject)
                    {
                        if (player.GetComponent<PlayerController>().isShopPhaseLocal)
                        {
                            isReadyToBattle = false;
                        }
                    }
                    else
                    if (player.GetComponent<PlayerController>().isShopPhaseOnline)
                    {
                        isReadyToBattle = false;
                    }
                }
                if (isReadyToBattle)
                {
                    Debug.Log("GOD C'ETAIT TROP CHIANT MAIS CA MARCHE ENFIN PUTAIN");
                    removeSelectedObject();
                    setBattlePhaseFromOutside(true);
                    //SWITCH SCENE HERE
                    boardAnimator.GetComponent<Animator>().SetBool("CombatStart", true);
                    boardAnimator.GetComponent<Animator>().SetBool("ShopStart", false);
                    //isSetupDone = false; do this after fight
                }

            }
            if (readyToBegin)
            {
                if (isBattlePhaseLocal)
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
                            boardAnimator.GetComponent<boardAnimationController>().isInPlaceForShop = false;
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
                    //Begin shop phase
                    if (!this.isBattlePhaseLocal && !isSetupDone)
                    {
                        StartCoroutine(shopPhase());
                        shop.refreshShop();
                        isSetupDone = true;
                    }
                }
            }
        }
        else
        {

            if (otherPlayer.isBattlePhaseLocal)
            {
                //place unit for fight if someone dies
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
            if (readyToBegin && otherPlayer.boardAnimator.GetComponent<Animator>().GetBool("CombatStart") && !setupEnemyBoard)
            {
                int boardNumber = 0;
                //Create ennemies before fight
                foreach (Unite unite in board)
                {
                    if (unite.name != "Empty")
                    {
                        GameObject unit = Instantiate(Resources.Load<GameObject>("Prefabs/Unit/" + unite.name), boardSlotList[boardNumber].transform.position, Quaternion.identity);
                        unit.GetComponent<TimeUnite>().health = unite.health;
                        unit.GetComponent<TimeUnite>().damages = unite.damages;
                        unit.GetComponent<TimeUnite>().boardFather = boardSlotList[boardNumber].GetComponent<boardController>();
                        unit.GetComponent<TimeUnite>().boardFather.monsterInSlot = unit.GetComponent<TimeUnite>();
                        unit.transform.parent = this.boardSlotList[boardNumber].transform;
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
            if (otherPlayer.isBattlePhaseLocal && !canLaunchTimerFight && setupEnemyBoard && !launchMoveunite)
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
    private void setBattlePhaseOnline(bool newStatus)
    {
        isBattlePhaseOnline = newStatus;
    }

    public void setBattlePhaseFromOutside(bool newStatus)
    {
        setBattlePhaseOnline(newStatus);
        isBattlePhaseLocal = newStatus;
    }

    private IEnumerator shopPhase()
    {
        while (totalTime <= shopPhaseDuration)
        {
            totalTime += Time.deltaTime;
            timerDisplay.text = ((int)shopPhaseDuration - (int)totalTime).ToString();
            moneyLeft = (shopPhaseDuration - totalTime);
            var integer = (int)totalTime;
            yield return null;
        }

        setShopPhaseLocal(false);
        StopCoroutine(shopPhase());
    }

    [Command]
    private void setShopPhaseOnline(bool newStatus)
    {
        isShopPhaseOnline = newStatus;
    }

    private void setShopPhaseLocal(bool newStatus)
    {
        isShopPhaseLocal = newStatus;
        setShopPhaseOnline(newStatus);
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
        float fracComplete = (Time.time - timeBeginMoving) / 3f;

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
            if (uniteList[i])
                if (!uniteList[i].InPlaceForFight)
                {
                    asChange = false;
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
        foreach (GameObject slot in boardSlotList)
        {
            if (slot.GetComponent<boardController>().monsterInSlot)
            {
                i++;
            }
        }
        return i;
    }
    [Command]
    public void removeLife(int lifeRemoved)
    {
        life-=lifeRemoved;
    }

    public void resetShop()
    {
        //Set battlephase to false locally and online
        setBattlePhaseFromOutside(false);
        shopPhaseDuration = baseShopDuration;
        if (getNumberUnits() == 0)
        {
            removeLife(1);
        }
        // efface les unite
        foreach (GameObject slot in GameObject.FindGameObjectsWithTag("Unit"))
        {
            GameObject.Destroy(slot);
        }
        //Recree les unite au bonne endroit
        int boardNumber = 0;
        foreach (Unite unite in board)
        {
            if (unite.name != "Empty")
            {
                GameObject unit = Instantiate(Resources.Load<GameObject>("Prefabs/Unit/" + unite.name), boardSlotList[boardNumber].transform.position, Quaternion.identity);
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
                unit.transform.parent = unit.GetComponent<TimeUnite>().boardFather.transform;
                if (unit.GetComponent<TimeUnite>().triggerList == TimeUnite.Triggers.startOfShop)
                    unit.GetComponent<TimeUnite>().launchEffect();
            }
            boardNumber++;
        }
        beginMoving = true;
        setupEnemyBoard = false;
        canLaunchTimerFight = false;
        isSetupDone = false;
        totalTime = 0;
        isTimerLaunch = false;
        round += 1;
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (this != player.GetComponent<PlayerController>())
            {
                player.GetComponent<PlayerController>().beginMoving = true;
                player.GetComponent<PlayerController>().setupEnemyBoard = false;
                player.GetComponent<PlayerController>().canLaunchTimerFight = false;
                player.GetComponent<PlayerController>().isSetupDone = false;
                player.GetComponent<PlayerController>().totalTime = 0;
                player.GetComponent<PlayerController>().isTimerLaunch = false;
            }
        }
        setShopPhaseLocal(true);
        playerXP += XPPerRound;
        this.boardAnimator.GetComponent<Animator>().SetBool("ShopStart", false);
    }

    public void addXPToPlayer(int XP)
    {
        playerXP += XP;
    }
}
