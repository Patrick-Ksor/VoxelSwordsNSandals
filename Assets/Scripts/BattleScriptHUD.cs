using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleScriptHUD : MonoBehaviour
{
    public Text nameText;
    public Text levelText;
    public Slider hpSlider;
    public Slider xpSlider;
    public Slider bpSlider;


    public void SetHUD(Unit unit)
    {
        nameText.text = unit.unitName;
        levelText.text = "Lvl " + unit.unitLevel;
        hpSlider.maxValue = unit.maxHP;
        hpSlider.value = unit.currentHP;
        xpSlider.value = unit.xp;
        bpSlider.value = unit.bp;
    }

    public void SetHP(int hp)
    {
        hpSlider.value = hp;
    }
    public void SetXP(int xp)
    {
        xpSlider.value = xp;
    }
    public void SetBP(int bp)
    {
        bpSlider.value = bp;
    }
    public void SetLvl(int lvl)
    {
        levelText.text = "Lvl " +lvl;
    }
}
