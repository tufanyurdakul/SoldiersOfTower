using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData4 : MonoBehaviour
{
    private UnitStats unitStats;
    void Start()
    {
        unitStats = gameObject.GetComponent<UnitStats>();
        Statics statics = new Statics();
        statics = JsonUtility.FromJson<Statics>(PlayerPrefs.GetString("Unit4"));
        unitStats.FillModel(statics);
    }
}
