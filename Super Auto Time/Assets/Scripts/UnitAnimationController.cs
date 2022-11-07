using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimationController : MonoBehaviour
{
    public void onAnimationHit()
    {
        transform.parent.GetComponent<TimeUnite>().dealDamageToFirstInBoard();
    }

    public void onAnimationKill()
    {
        transform.parent.GetComponent<TimeUnite>().killUnit();
    }
}
