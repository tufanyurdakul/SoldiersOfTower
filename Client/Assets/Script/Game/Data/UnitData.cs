using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitData : MonoBehaviour
{
    // Start is called before the first frame update
    public int money { get; set; }
    public GameObject bg;
    public GameObject[] canvases;
    public NetworkClient nc;
    public int enemyHp { get; set; }
    public int yourHp { get; set; }
    public float timer { get; private set; }
    void Awake()
    {
        yourHp = 200;
        timer = 5;
        money = 1;
        enemyHp = 0;
        float z2 = (float)Screen.width / (float)Screen.height;
        float x2 = (z2 * 9) / 16;
        bg.transform.localScale = new Vector3(bg.transform.localScale.x * x2, bg.transform.localScale.y, bg.transform.localScale.z);
        foreach (var item in canvases)
        {
            item.transform.localScale = new Vector3(item.transform.localScale.x * x2, item.transform.localScale.y, item.transform.localScale.z);
        }
        PlayerPrefs.SetString("Unit1", "{\"AttackDamage\":24,\"AbilityPower\":6,\"AttackSpeed\":1,\"UnitId\":1,\"Health\":135,\"Armour\":35,\"Resistance\":20}");
        PlayerPrefs.SetString("Unit2", "{\"AttackDamage\":15,\"AbilityPower\":15,\"AttackSpeed\":1,\"UnitId\":2,\"Health\":50,\"Armour\":0,\"Resistance\":0}");
        PlayerPrefs.SetString("Unit3", "{\"AttackDamage\":15,\"AbilityPower\":15,\"AttackSpeed\":1.6,\"UnitId\":3,\"Health\":200,\"Armour\":10,\"Resistance\":40}");
        PlayerPrefs.SetString("Unit4", "{\"AttackDamage\":15,\"AbilityPower\":15,\"AttackSpeed\":1.2,\"UnitId\":4,\"Health\":150,\"Armour\":0,\"Resistance\":100}");
        PlayerPrefs.SetString("Unit5", "{\"AttackDamage\":10,\"AbilityPower\":20,\"AttackSpeed\":1,\"UnitId\":5,\"Health\":70,\"Armour\":0,\"Resistance\":0}");
        PlayerPrefs.SetString("Unit6", "{\"AttackDamage\":15,\"AbilityPower\":15,\"AttackSpeed\":0.8,\"UnitId\":6,\"Health\":300,\"Armour\":110,\"Resistance\":20}");
        PlayerPrefs.SetString("Unit7", "{\"AttackDamage\":15,\"AbilityPower\":15,\"AttackSpeed\":1.5,\"UnitId\":7,\"Health\":250,\"Armour\":70,\"Resistance\":70}");
        PlayerPrefs.SetString("Unit8", "{\"AttackDamage\":17,\"AbilityPower\":13,\"AttackSpeed\":1.6,\"UnitId\":8,\"Health\":150,\"Armour\":50,\"Resistance\":10}");
        PlayerPrefs.SetString("Unit10", "{\"AttackDamage\":25,\"AbilityPower\":5,\"AttackSpeed\":1,\"UnitId\":10,\"Health\":60,\"Armour\":0,\"Resistance\":0}");
        Time.timeScale = 0;
    }
}
public class Healths
{
    public int health;
    public string id;
    public int asp;
    public string roomId;

}
public class Signal
{
    public string id;
    public string createId;
    public bool value;
    public string roomId;
}
[SerializeField]
public class SendServerAttack2
{
    public string CreateId;
    public int UnitId;
    public string id;
    public int damage;
    public string roomId;
}
public class SendAttackCount
{
    public string CreateId;
    public int AttackCount;
    public string roomId;
}
[SerializeField]
public class Movement
{
    public int x;
    public int speed;
    public string id;
    public string createId;
    public int unitId;
}
public class LifeSteal
{
    public int value;
    public string CreateId;
    public string roomId;
}

