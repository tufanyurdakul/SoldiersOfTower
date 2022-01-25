using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData9 : MonoBehaviour
{
    private UnitStats unitStats;
    void Start()
    {
        unitStats = gameObject.GetComponent<UnitStats>();
        Statics statics = new Statics();
        statics = JsonUtility.FromJson<Statics>(PlayerPrefs.GetString("Unit9"));
        unitStats.FillModel(statics);
    }
}
