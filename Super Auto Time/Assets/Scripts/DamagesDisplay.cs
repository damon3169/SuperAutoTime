using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagesDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    public float aliveTime = 5f;
    public TMP_Text damageText;
    void Start()
    {
        StartCoroutine(goUp(Time.time));
    }


    public IEnumerator goUp(float startTime)
    {
        while (Time.time - startTime < aliveTime)
        {
            transform.Translate(transform.up * Time.deltaTime, Space.World);
            yield return null;
        }
        GameObject.Destroy(this.gameObject);
        yield break;
    }

    public void setDamageText(int damage)
    {
        damageText.text ="-"+ damage.ToString();
    }
}
