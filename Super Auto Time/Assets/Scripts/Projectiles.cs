using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectiles : MonoBehaviour
{
    public TimeUnite target;
    public int damages;
    public float timeBeginMoving;
    public Vector3 beginPos;

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            // The center of the arc
            Vector3 center = (beginPos + target.transform.position) * 0.5F;

            // move the center a bit downwards to make the arc vertical
            center -= new Vector3(0, 2, 0);

            // Interpolate over the arc relative to center
            Vector3 riseRelCenter = beginPos - center;
            Vector3 setRelCenter = target.transform.position - center;

            // The fraction of the animation that has happened so far is
            // equal to the elapsed time divided by the desired time for
            // the total journey.
            float fracComplete = (Time.time - timeBeginMoving) / 0.4f;

            this.transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, fracComplete);
            this.transform.position += center;
            // Do something when we reach the target
            Debug.Log(fracComplete);
            if (this.transform.position == target.transform.position)
            {
                target.takeDamages(damages);
                DestroyImmediate(this.gameObject,true);
            }
        }
        else
        {
            DestroyImmediate(this.gameObject,true);
        }
    }
}
