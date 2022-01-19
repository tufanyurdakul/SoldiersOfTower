using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats : MonoBehaviour
{
    public byte UnitId { get; set; }
    public string CreateId { get; set; }
    public byte AttackDamage { get; set; }
    public float AttackSpeed { get; set; }
    public int Health { get; set; }
    public byte Armour { get; set; }
    public byte Resistance { get; set; }
    public byte AbilityPower { get; set; }
    public int MaxHealth { get; set; }
    public int AttackCount { get; set; }
    public bool ab { get; set; }
    void Awake()
    {
        //Statics statics = new Statics();
        //statics = JsonUtility.FromJson<Statics>(PlayerPrefs.GetString("Unit1"));
        //FillModel(statics);
    }
    public void FillModel(Statics statics)
    {
        UnitId = statics.UnitId;
        AttackDamage = statics.AttackDamage;
        AttackSpeed = statics.AttackSpeed;
        Health = statics.Health;
        Armour = statics.Armour;
        Resistance = statics.Resistance;
        AbilityPower = statics.AbilityPower;

    }
}
public class Statics
{
    public byte UnitId;
    public int CreateId;
    public byte AttackDamage;
    public float AttackSpeed;
    public int Health;
    public byte Armour;
    public byte Resistance;
    public byte AbilityPower;
}
