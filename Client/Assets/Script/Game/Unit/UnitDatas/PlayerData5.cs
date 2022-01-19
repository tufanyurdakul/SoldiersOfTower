using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData5 : MonoBehaviour
{
    private UnitStats unitStats;
    void Start()
    {
        unitStats = gameObject.GetComponent<UnitStats>();
        Statics statics = new Statics();
        statics = JsonUtility.FromJson<Statics>(PlayerPrefs.GetString("Unit5"));
        unitStats.FillModel(statics);
    }
}
