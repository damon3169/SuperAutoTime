using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsStat : Items
{
    // Start is called before the first frame update
    public int statHealth;
    public int statDamages;

    void Start()
    {
        
    }

    public void addStat()
    {
        equippedUnite.health += statHealth;
        equippedUnite.damages += statDamages;
    }
}
