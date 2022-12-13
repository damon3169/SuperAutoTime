using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleController : MonoBehaviour
{
    public PlayerController playerLocal;
    public PlayerController playerDist;
    IEnumerator fightCorout;
    public bool BeginOfFightEffectLaunched = false;
    public List<TimeUnite> uniteWithBeginEffect;
    public List<TimeUnite> uniteWith1sEffect;
    public List<TimeUnite> unitWithEveryXsEffect;
    int beginEffectUniteCounter = 0;
    bool is1sTimer = false;
    IEnumerator spell1SCorout;
    private float start10STimer = 0;
    public TMP_Text timerDisplay;
    public int totalTime = 0;
    IEnumerator forwardCorout;
    bool forwardTime = false;


    // Update is called once per frame
    void Update()
    {
        if (playerLocal != null && playerDist != null)
        {
            if (!playerLocal.boardAnimator.GetComponent<boardAnimationController>().isInPlaceForShop)
            {
                if (playerDist.getNumberUnits() == 0 || playerLocal.getNumberUnits() == 0)
                {
                    playerLocal.boardAnimator.GetComponent<Animator>().SetBool("ShopStart", true);
                }
            }
            if (
            playerDist.canLaunchTimerFight && playerLocal.canLaunchTimerFight &&
            playerLocal.fightingUnite && playerDist.fightingUnite &&
                    !playerDist.launchMoveunite && !playerLocal.launchMoveunite)
            {
                if (playerLocal.boardAnimator.GetComponent<boardAnimationController>().isInPlaceForFight && !BeginOfFightEffectLaunched)
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
                    foreach (TimeUnite unite in playerDist.lookForSpellEffect("OnXs"))
                    {
                        unitWithEveryXsEffect.Add(unite);
                    }
                    foreach (TimeUnite unite in playerLocal.lookForSpellEffect("OnXs"))
                    {
                        unitWithEveryXsEffect.Add(unite);
                    }
                    if (is1sTimer == false)
                    {
                        //start10STimer = Time.time - totalTime;
                        is1sTimer = true;
                        spell1SCorout = on1sSpellTimer();
                        StartCoroutine(spell1SCorout);
                    }

                }
            }

            else
            {

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
    /*IEnumerator onCoroutine()
    {
        while (isHiting)
        {

            yield return new WaitForSeconds(1f);
        }
    }*/

    IEnumerator on1sSpellTimer()
    {
        while (is1sTimer)
        {
            if (uniteWith1sEffect.Count > 0)
                foreach (TimeUnite unite in uniteWith1sEffect)
                {
                    unite.launchEffect();
                }
            foreach (TimeUnite unite in unitWithEveryXsEffect)
            {
                foreach (int timeForEffect in unite.timeEffect)
                    if (totalTime == timeForEffect)
                    {
                        unite.launchEffect();
                    }
            }
            if (playerDist.fightingUnite && playerDist.fightingUnite.health > 0 && playerLocal.fightingUnite && playerLocal.fightingUnite.health > 0 && !forwardTime)
            {
                if (playerDist.fightingUnite.triggerList == TimeUnite.Triggers.onAttack)
                {
                    playerDist.fightingUnite.launchEffect();

                }
                if (playerLocal.fightingUnite.triggerList == TimeUnite.Triggers.onAttack)
                {
                    playerLocal.fightingUnite.launchEffect();

                }
                playerDist.fightingUnite.launchHitAnimation();
                playerLocal.fightingUnite.launchHitAnimation();
            }

            timerDisplay.text = totalTime.ToString();
            totalTime += 1;
            if (totalTime > 9)
            {
                //start10STimer += 10;
                totalTime = 0;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public IEnumerator onForward(float Timeforward, float startTime)
    {
        while (Time.time - startTime < Timeforward)
        {
            forwardTime = true;
            //StopCoroutine(fightCorout);
            Time.timeScale = 5f;
            yield return new WaitForSeconds(Timeforward);
        }
        Time.timeScale = 1f;
        forwardTime = false;
        yield break;
    }

    public void LaunchForward(float timeForward)
    {
        StartCoroutine(onForward(timeForward, Time.time));
    }
}
