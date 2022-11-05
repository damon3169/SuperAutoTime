using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeUnite : MonoBehaviour
{
    public string nameUnite = "Empty";

    public int health;
    public int damages;
    public string description;

    public int level = 1;
    public int cost = 3;
    public enum Triggers // your custom enumeration
    {
        nothing,
        startOfShop,
        EndOfShop,
        Every1s,
        beginOfFight,
        OnXs,
        OnDeath,
        onDamage,
        onAttack
    };


    public enum Effects // your custom enumeration
    {
        GainDamageAndHealth,
        Forward,
        GainTimerShop,
        DealDamage,
        debug

    };

    public enum Targets // your custom enumeration
    {
        Self,
        All,
        Ally,
        Ennemie,
        randomAlly,
        randomEnnemi
    };

    [Header("Select the trigger")]
    public Triggers triggerList;
    [Header("if EveryXs, add the seconds (between 0 and 9)")]
    public List<int> timeEffect;
    [Header("Select the target")]

    public Targets targetList;
    [Space(5)]
    [Header("If target enemies or ally, add the position in lists")]
    public List<int> AllyNumber;
    public List<int> EnnemiNumber;
    [Space(5)]
    [Header("Select the effect")]
    public Effects effectList;

    [Header("If damage, put damage, if  stat selected, put stats")]
    public int damageSpell;
    public int damagesBonus = 0;
    public int healthBonus = 0;
    public int forwardDuration = 0;
    public int shopDurationBonus = 0;

    [Space(20)]

    public bool InPlaceForFight = false;
    public bool isFighting = false;
    public bool isInShop = true;

    public PlayerController otherPlayer;
    bool isEnemyDead;
    public int positionInBoard;
    public IEnumerator fightCorout;
    private bool BeginEffectAlreaduLaunched = false;
    public PlayerController player;
    public SpriteRenderer spriteSelected;
    public boardController boardFather;
    public ParticleSystem damageParticle;
    public TMP_Text damagesText;
    public TMP_Text healthText;
    public BattleController battleController;
    public SpriteRenderer mainSprite;
    
    public TMP_Text nameDisplay;


    private void Start()
    {
        battleController = GameObject.FindGameObjectWithTag("BattleController").GetComponent<BattleController>();
        nameDisplay.text = this.nameUnite;
    }
    // Update is called once per frame
    void Update()
    {
        damagesText.text = damages.ToString(); ;
        healthText.text = health.ToString();
        if (player != null)
        {
            if (player.isShopPhaseLocal)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit hitInfo = new RaycastHit();
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
                    {
                        if (hitInfo.transform.gameObject == this.gameObject)
                        {
                            selectObject();
                        }
                    }
                }
            }
        }
        if (isInShop)
        {
            if (player == null)
                if (GameObject.FindGameObjectWithTag("Player") && player == null)
                {
                    foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                    {
                        if (player.GetComponent<PlayerController>().isLocalPlayer)
                        {
                            this.player = player.GetComponent<PlayerController>();
                        }
                        else
                        {
                            otherPlayer = player.GetComponent<PlayerController>();
                        }
                    }
                }
        }

        if (this.health <= 0)
        {
            player.boardSlotList[this.positionInBoard].GetComponent<boardController>().monsterInSlot = null;
            if (this.triggerList == Triggers.OnDeath)
            {
                launchEffect();
            }
            if (this == player.fightingUnite)
            {
                player.fightingUnite = null;
            }
            player.launchMoveunite = true;
            GameObject.DestroyImmediate(this.gameObject, true);
        }
    }


    public void selectObject()
    {
        player.removeSelectedObject();
        player.selectedObject = this.gameObject;
        spriteSelected.enabled = true;
    }

    public void takeDamages(int damages)
    {
        damageParticle.Play();
        health -= damages;
        if (triggerList == Triggers.onDamage && health > 0)
        {
            launchEffect();
        }
    }

    public bool launchBeginOfFightEffect()
    {
        if (BeginEffectAlreaduLaunched)
        {
            return false;
        }
        else
        {
            launchEffect();
            BeginEffectAlreaduLaunched = true;
            return true;//FAIRE TIME EFFECT ICI
        }

    }

    public void gainStat(int healthBonus, int damageBonus)
    {
        health += healthBonus;
        damages += damageBonus;
    }


    public void launchEffect()
    {
        List<TimeUnite> cible = new List<TimeUnite>();
        switch (targetList)
        {
            case Targets.Self:
                cible.Add(this);
                break;
            case Targets.All:
                foreach (GameObject slot in player.boardSlotList)
                {
                    if (slot.GetComponent<boardController>().monsterInSlot)
                    {
                        cible.Add(slot.GetComponent<boardController>().monsterInSlot);
                    }
                }
                foreach (GameObject slot in otherPlayer.boardSlotList)
                {
                    if (slot.GetComponent<boardController>().monsterInSlot)
                    {
                        cible.Add(slot.GetComponent<boardController>().monsterInSlot);
                    }
                }
                break;
            case Targets.Ally:
                foreach (GameObject slot in player.boardSlotList)
                {
                    if (slot.GetComponent<boardController>().monsterInSlot)
                    {
                        foreach (int allyPos in AllyNumber)
                            if (allyPos == slot.GetComponent<boardController>().monsterInSlot.positionInBoard)
                                cible.Add(slot.GetComponent<boardController>().monsterInSlot);
                    }
                }
                break;
            case Targets.Ennemie:
                foreach (GameObject slot in otherPlayer.boardSlotList)
                {
                    if (slot.GetComponent<boardController>().monsterInSlot)
                    {
                        foreach (int enemyPos in AllyNumber)
                            if (enemyPos == slot.GetComponent<boardController>().monsterInSlot.positionInBoard)
                                cible.Add(slot.GetComponent<boardController>().monsterInSlot);
                    }
                }
                break;
            case Targets.randomAlly:
                for (int y = player.randomSelected; y < player.listRandom.Count; y++)
                {
                    if (player.boardSlotList[player.listRandom[y]].GetComponent<boardController>().monsterInSlot)
                    {
                        cible.Add(player.boardSlotList[player.listRandom[y]].GetComponent<boardController>().monsterInSlot);
                        player.randomSelected = y + 1;
                        break;
                    }
                }
                break;
            case Targets.randomEnnemi:
                for (int y = otherPlayer.randomSelected; y < otherPlayer.listRandom.Count; y++)
                {
                    if (otherPlayer.boardSlotList[otherPlayer.listRandom[y]].GetComponent<boardController>().monsterInSlot)
                    {
                        cible.Add(otherPlayer.boardSlotList[otherPlayer.listRandom[y]].GetComponent<boardController>().monsterInSlot);
                        otherPlayer.randomSelected = y + 1;
                        break;
                    }
                }
                break;
        }
        switch (effectList)
        {
            case Effects.GainDamageAndHealth:
                foreach (TimeUnite unite in cible)
                {
                    unite.gainStat(healthBonus, damagesBonus);
                }
                break;
            case Effects.DealDamage:
                foreach (TimeUnite unite in cible)
                {
                    unite.takeDamages(damageSpell);
                }
                break;
            case Effects.Forward:
                battleController.LaunchForward(forwardDuration);
                break;
            case Effects.GainTimerShop:
                player.shopPhaseDuration += shopDurationBonus;
                break;
            case Effects.debug:
                Debug.Log("Effect launched at " + Time.time + " with Trigger=" + triggerList.ToString());
                break;
        }
    }

}
