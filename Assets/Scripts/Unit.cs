using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;
    public int damage;
    public int maxHP;
    public int currentHP;
    public int xp;
    public int xpToLevel;
    public int bp;

    public bool TakeDamage(int dmg)
    {
        currentHP -= dmg;
        if(currentHP <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
