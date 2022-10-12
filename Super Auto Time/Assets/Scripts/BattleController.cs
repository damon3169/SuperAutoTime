using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleController : MonoBehaviour
{
    public PlayerController playerLocal;
    public PlayerController playerDist;
    IEnumerator fightCorout;
    public bool isHiting = false;
    public bool BeginOfFightEffectLaunched = false;
    public List<TimeUnite> uniteWithBeginEffect;
    public List<TimeUnite> uniteWith1sEffect;
    public List<TimeUnite> unitWithEveryXsEffect;
    int beginEffectUniteCounter = 0;
    bool is1sTimer = false;
    IEnumerator spell1SCorout;
    private float start10STimer = 0;
    public TMP_Text timerDisplay;
    int totalTime = 0;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerLocal != null && playerDist != null)
        {
            if (playerLocal.isBattlePhase && playerDist.isBattlePhase &&
            playerDist.canLaunchTimerFight && playerLocal.canLaunchTimerFight &&
            playerLocal.fightingUnite && playerDist.fightingUnite &&
                    !playerDist.launchMoveunite && !playerLocal.launchMoveunite)
            {
                if (!BeginOfFightEffectLaunched)
                {
                    //LOOK FOR BEGIN OF FIGHT UNITE
                    uniteWithBeginEffect.Clear();
                    foreach (TimeUnite unite in playerDist.lookForSpellEffect("beginOfFight"))
                    {
                        uniteWithBeginEffect.Add(unite);
                    }
                    foreach (TimeUnite unite in playerLocal.lookForSpellEffect("beginOfFight"))
                    {
                        uniteWithBeginEffect.Add(unite);
                    }
                    if (uniteWithBeginEffect.Count > 0 && beginEffectUniteCounter < uniteWithBeginEffect.Count)
                    {
                        if (uniteWithBeginEffect[beginEffectUniteCounter].launchBeginOfFightEffect())
                        {
                            beginEffectUniteCounter++;
                        }
                    }
                    else
                    {
                        BeginOfFightEffectLaunched = true;
                    }
                }
                else
                {
                    //END OF BEGIN EFFECT
                    //GET UNITE 
                    uniteWith1sEffect.Clear();
                    foreach (TimeUnite unite in playerDist.lookForSpellEffect("Every1s"))
                    {
                        uniteWith1sEffect.Add(unite);
                    }
                    foreach (TimeUnite unite in playerLocal.lookForSpellEffect("Every1s"))
                    {
                        uniteWith1sEffect.Add(unite);
                    }
                    unitWithEveryXsEffect.Clear();
                    foreach (TimeUnite unite in playerDist.lookForSpellEffect("EveryXs"))
                    {
                        unitWithEveryXsEffect.Add(unite);
                    }
                    foreach (TimeUnite unite in playerLocal.lookForSpellEffect("EveryXs"))
                    {
                        unitWithEveryXsEffect.Add(unite);
                    }
                    if (is1sTimer == false)
                    {
                        start10STimer = Time.time - totalTime;
                        is1sTimer = true;
                        spell1SCorout = on1sSpellTimer();
                        StartCoroutine(spell1SCorout);
                    }
                    //BEGIN TIMER FIGHT
                    if (isHiting == false)
                    {
                        isHiting = true;
                        fightCorout = onCoroutine();
                        StartCoroutine(fightCorout);
                    }
                }
            }

            else
            {
                if (isHiting)
                {
                    StopCoroutine(fightCorout);
                }
                isHiting = false;
                if (is1sTimer)
                {
                    StopCoroutine(spell1SCorout);
                }
                is1sTimer = false;
            }
        }
        else
        {

            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (player.GetComponent<PlayerController>().isLocalPlayer)
                {
                    this.playerLocal = player.GetComponent<PlayerController>();
                }
                else
                {
                    playerDist = player.GetComponent<PlayerController>();
                }
            }

        }
    }
    IEnumerator onCoroutine()
    {
        while (isHiting)
        {
            int damage1 = playerLocal.fightingUnite.damages;
            int damage2 = playerDist.fightingUnite.damages;
            playerDist.fightingUnite.takeDamages(damage1);
            playerLocal.fightingUnite.takeDamages(damage2);
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator on1sSpellTimer()
    {
        while (is1sTimer)
        {
            if (uniteWith1sEffect.Count > 0)
                foreach (TimeUnite unite in uniteWith1sEffect)
                {
                    unite.launchEffect();
                }

            if (Time.time - start10STimer < 10)
                timerDisplay.text = (Time.time - start10STimer).ToString().Substring(0, 1);
            totalTime += 1;
            if (totalTime > 9)
            {
                start10STimer -= 10;
                totalTime = 0;
            }
            foreach (TimeUnite unite in unitWithEveryXsEffect)
            {
                foreach(int timeForEffect in unite.timeEffect)
                if (((int)(Time.time - start10STimer)) == timeForEffect)
                {
                    unite.launchEffect();
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
