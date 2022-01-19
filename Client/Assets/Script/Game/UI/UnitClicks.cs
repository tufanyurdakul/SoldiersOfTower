using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;
using UnityEngine.SceneManagement;

public class UnitClicks : MonoBehaviour
{
    public Button unit1, unit2, unit3,unit4,unit5,unit6,unit7,unit8,unit10,close;
    private NetworkClient nc;
    public GameObject inf;
    private UnitData ud;
    public GameObject unitData;
    public GameObject[] barracks;
    public GameObject[] chats;
    private List<GameObject> openedSprites;
    private List<float> timer;
    public int where { get; set; }
    void Start()
    {
        openedSprites = new List<GameObject>();
        timer = new List<float>();
        ud = unitData.GetComponent<UnitData>();
        nc = inf.GetComponent<NetworkClient>();
        unit1.onClick.AddListener(create1);
        unit2.onClick.AddListener(create2);
        unit3.onClick.AddListener(create3);
        unit4.onClick.AddListener(create4);
        unit5.onClick.AddListener(create5);
        unit6.onClick.AddListener(create6);
        unit7.onClick.AddListener(create7);
        unit8.onClick.AddListener(create8);
        unit10.onClick.AddListener(create10);
        close.onClick.AddListener(closePanel);
    }
    private void create1()
    {
        if (ud.money >= 1 && Time.timeScale != 0)
        {
            Statics s = new Statics();
            s = JsonUtility.FromJson<Statics>(PlayerPrefs.GetString("Unit1"));
            Position p = new Position();
            p.unitId = 1;
            p.id = nc.myId;
            p.placeId = where;
            p.health = s.Health;
            p.attackDamage = s.AttackDamage;
            p.abilityPower = s.AbilityPower;
            p.armour = s.Armour;
            p.resistance = s.Resistance;
            p.attackSpeed = (int)(s.AttackSpeed * 10);
            p.roomId = nc.roomId;

            nc.Emit("spawn", new JSONObject(JsonUtility.ToJson(p)));
            write(p.unitId, p.unitId);
        }
        else
        {
            show();
          
        }
    }
    private void create2()
    {
        if (ud.money >= 2 && Time.timeScale != 0)
        {
            Statics s = new Statics();
            s = JsonUtility.FromJson<Statics>(PlayerPrefs.GetString("Unit2"));
            Position p = new Position();
            p.unitId = 2;
            p.id = nc.myId;
            p.placeId = where;
            p.health = s.Health;
            p.attackDamage = s.AttackDamage;
            p.abilityPower = s.AbilityPower;
            p.armour = s.Armour;
            p.resistance = s.Resistance;
            p.attackSpeed = (int)(s.AttackSpeed * 10);
            p.roomId = nc.roomId;

            nc.Emit("spawn", new JSONObject(JsonUtility.ToJson(p)));
            write(p.unitId, p.unitId);
        }
        else
        {
            show();
        }
    }
    private void create3()
    {
        if (ud.money >= 4 && Time.timeScale != 0)
        {
            Statics s = new Statics();
            s = JsonUtility.FromJson<Statics>(PlayerPrefs.GetString("Unit3"));
            Position p = new Position();
            p.unitId = 3;
            p.id = nc.myId;
            p.placeId = where;
            p.health = s.Health;
            p.attackDamage = s.AttackDamage;
            p.abilityPower = s.AbilityPower;
            p.armour = s.Armour;
            p.resistance = s.Resistance;
            p.attackSpeed = (int)s.AttackSpeed * 10;
            p.roomId = nc.roomId;

            nc.Emit("spawn", new JSONObject(JsonUtility.ToJson(p)));
            write(p.unitId, 4);
        }
        else
        {
            show();
        }
    }
    private void create4()
    {
        if (ud.money >= 3 && Time.timeScale != 0)
        {
            Statics s = new Statics();
            s = JsonUtility.FromJson<Statics>(PlayerPrefs.GetString("Unit4"));
            Position p = new Position();
            p.unitId = 4;
            p.id = nc.myId;
            p.placeId = where;
            p.health = s.Health;
            p.attackDamage = s.AttackDamage;
            p.abilityPower = s.AbilityPower;
            p.armour = s.Armour;
            p.resistance = s.Resistance;
            p.attackSpeed = (int)(s.AttackSpeed * 10);
            p.roomId = nc.roomId;
            nc.Emit("spawn", new JSONObject(JsonUtility.ToJson(p)));
            write(p.unitId,3);
        }
        else
        {
            show();

        }
    }
    private void create5()
    {
        if (ud.money >= 7 && Time.timeScale != 0)
        {
            Statics s = new Statics();
            s = JsonUtility.FromJson<Statics>(PlayerPrefs.GetString("Unit5"));
            Position p = new Position();
            p.unitId = 5;
            p.id = nc.myId;
            p.placeId = where;
            p.health = s.Health;
            p.attackDamage = s.AttackDamage;
            p.abilityPower = s.AbilityPower;
            p.armour = s.Armour;
            p.resistance = s.Resistance;
            p.attackSpeed = (int)(s.AttackSpeed * 10);
            p.roomId = nc.roomId;
            nc.Emit("spawn", new JSONObject(JsonUtility.ToJson(p)));
            write(p.unitId, 7);
        }
        else
        {
            show();

        }
    }
    private void create6()
    {
        if (ud.money >= 5 && Time.timeScale != 0)
        {
            Statics s = new Statics();
            s = JsonUtility.FromJson<Statics>(PlayerPrefs.GetString("Unit6"));
            Position p = new Position();
            p.unitId = 6;
            p.id = nc.myId;
            p.placeId = where;
            p.health = s.Health;
            p.attackDamage = s.AttackDamage;
            p.abilityPower = s.AbilityPower;
            p.armour = s.Armour;
            p.resistance = s.Resistance;
            p.roomId = nc.roomId;
            p.attackSpeed = (int)(s.AttackSpeed * 10);
            nc.Emit("spawn", new JSONObject(JsonUtility.ToJson(p)));
            write(p.unitId, 5);
        }
        else
        {
            show();

        }
    }
    private void create7()
    {
        if (ud.money >= 10 && Time.timeScale != 0)
        {
            Statics s = new Statics();
            s = JsonUtility.FromJson<Statics>(PlayerPrefs.GetString("Unit7"));
            Position p = new Position();
            p.unitId = 7;
            p.id = nc.myId;
            p.placeId = where;
            p.health = s.Health;
            p.attackDamage = s.AttackDamage;
            p.abilityPower = s.AbilityPower;
            p.armour = s.Armour;
            p.resistance = s.Resistance;
            p.roomId = nc.roomId;
            p.attackSpeed = (int)(s.AttackSpeed * 10);
            nc.Emit("spawn", new JSONObject(JsonUtility.ToJson(p)));
            write(p.unitId, 10);
        }
        else
        {
            show();

        }
    }
    private void create8()
    {
        if (ud.money >= 2 && Time.timeScale != 0)
        {
            Statics s = new Statics();
            s = JsonUtility.FromJson<Statics>(PlayerPrefs.GetString("Unit8"));
            Position p = new Position();
            p.unitId = 8;
            p.id = nc.myId;
            p.placeId = where;
            p.health = s.Health;
            p.attackDamage = s.AttackDamage;
            p.abilityPower = s.AbilityPower;
            p.armour = s.Armour;
            p.resistance = s.Resistance;
            p.roomId = nc.roomId;
            p.attackSpeed = (int)(s.AttackSpeed * 10);
            nc.Emit("spawn", new JSONObject(JsonUtility.ToJson(p)));
            write(p.unitId, 2);
        }
        else
        {
            show();

        }
    }
    private void create10()
    {
        if (ud.money >= 6 && Time.timeScale != 0)
        {
            Statics s = new Statics();
            s = JsonUtility.FromJson<Statics>(PlayerPrefs.GetString("Unit10"));
            Position p = new Position();
            p.unitId = 10;
            p.id = nc.myId;
            p.placeId = where;
            p.health = s.Health;
            p.attackDamage = s.AttackDamage;
            p.abilityPower = s.AbilityPower;
            p.armour = s.Armour;
            p.resistance = s.Resistance;
            p.roomId = nc.roomId;
            p.attackSpeed = (int)(s.AttackSpeed * 10);
            nc.Emit("spawn", new JSONObject(JsonUtility.ToJson(p)));
            write(p.unitId, 6);
        }
        else
        {
            show();

        }
    }
    private void show()
    {
        chats[where].SetActive(true);
        openedSprites.Add(chats[where]);
        timer.Add(0.5f);
    }
    private void write(int uid,int price)
    {

        ud.money -= price;
        closePanel();
    }
    private void closePanel()
    {
        gameObject.SetActive(false);
        //SceneManager.LoadScene("another");
    }
    // Update is called once per frame
    void Update()
    {
        if (openedSprites.Count > 0)
        {
            for(int i = 0; i<openedSprites.Count; i++)
            {
                timer[i] -= Time.deltaTime;
                if (timer[i] <= 0)
                {
                    openedSprites[i].SetActive(false);
                    timer.RemoveAt(i);
                    openedSprites.RemoveAt(i);
                }
            }
        }

    }
    [SerializeField]
    public class Position
    {
        public string id;
        public int unitId;
        public int placeId;
        public int health;
        public int attackDamage;
        public int abilityPower;
        public int armour;
        public int resistance;
        public int attackSpeed;
        public string roomId;
    }
}
