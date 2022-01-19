using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class infoData : MonoBehaviour
{
    private int gold = 0;
    private int health = 0;
    private int ehealth = 0;
    public TextMeshProUGUI tmp,tmpHp,tmpEHp;
    public GameObject ud;
    private UnitData unitData;
    // Start is called before the first frame update
    void Start()
    {
        unitData = ud.GetComponent<UnitData>();
        gold = unitData.money;
        tmp.SetText(gold + "$");
    }

    // Update is called once per frame
    void Update()
    {
        if (gold != unitData.money)
        {
            gold = unitData.money;
            tmp.SetText(gold + "$");
        }
        if (health != unitData.yourHp)
        {
            health = unitData.yourHp;
            tmpHp.SetText("Hp:"+health);
        }
        if (ehealth != unitData.enemyHp)
        {
            ehealth = unitData.enemyHp;
            tmpEHp.SetText("EHp:" + ehealth);
        }
    }
}
