using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;
using System.Globalization;
using TMPro;
using UnityEngine.SceneManagement;

public class NetworkClient : SocketIOComponent
{
    // Start is called before the first frame update
    private Dictionary<string, GameObject> serverObjects;
    public string myId { get; private set; }
    public GameObject[] myObjects;
    public GameObject[] enemyObjects;
    public Transform[] transforms;
    public Transform[] Enemytransforms;
    private UnitData ud;
    private List<string> createIds;
    public TextMeshProUGUI tmpCond;
    public float x2 { get; private set; }
    public float enemyScreen { get; private set; }
    public float enemyX2 { get; private set; }
    public string roomId { get; private set; }
    private int count;
    public float time { get; private set; }
    private bool Finish = false;
    public override void Start()
    {
        roomId = Room.roomId;
        x2 = (float)Screen.width / (float)Screen.height;
        x2 = (x2 * 9) / 16;
        //float mySize = 1.775919732441472f;
        //this.x2 = z2 / mySize;
        Application.targetFrameRate = 60;
        createIds = new List<string>();
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        GameObject unitData = GameObject.Find("UnitData");
        ud = unitData.GetComponent<UnitData>();

        base.Start();
        setUpEvents();

    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

    }
    private void setUpEvents()
    {

        On("open", (E) =>
         {
             count++;
             if (count < 2)
             {
                 SendRoom sendRoom = new SendRoom();
                 sendRoom.myId = myId;
                 sendRoom.roomId = roomId;
                 Emit("Join Room", new JSONObject(JsonUtility.ToJson(sendRoom)));
                 Healths xx = new Healths();
                 xx.id = "1";
                 xx.roomId = roomId;
                 Emit("register2", new JSONObject(JsonUtility.ToJson(xx)));
             }
             Debug.Log("connection maded");

         });

        On("register", (E) =>
        {
            myId = E.data["id"].ToString().Replace("'", "").Replace("\"", "");
            Healths hp = new Healths();
            hp.id = myId;
            hp.health = ud.yourHp;
            hp.roomId = roomId;
        });
        On("healths", (E) =>
        {
            string id = E.data["id"].ToString().Replace("'", "").Replace("\"", "");
            int health = (int)(E.data["health"].f);
            if (id != myId)
            {
                ud.enemyHp = health;
            }
        });
        On("timer", (E) =>
        {
            time++;
            if (time > 90 && ud.money < 15 && time % 5 == 0)
            {
                ud.money = ud.money + 2 > 15 ? 15 : ud.money + 3;
            }
            else if (time > 60 && ud.money < 15 && time % 4 == 0 && time <= 90)
            {
                ud.money = ud.money + 2 > 15 ? 15 : ud.money + 2;
            }
            else if (ud.money < 15 && time % 4 == 0)
            {
                ud.money = ud.money + 1 > 15 ? 15 : ud.money + 1;
            }
        });
        On("size", (E) =>
         {

             int size = (int)E.data["size"].f;
             Debug.Log("size" + size);
             if (size > 1)
             {
                 Healths hp = new Healths();
                 hp.id = myId;
                 hp.health = ud.yourHp;
                 hp.roomId = roomId;
                 Emit("healths", new JSONObject(JsonUtility.ToJson(hp)));
                 hp = new Healths();
                 hp.id = myId;
                 hp.health = Screen.width;
                 hp.asp = Screen.height;
                 hp.roomId = roomId;
                 Emit("x2", new JSONObject(JsonUtility.ToJson(hp)));
             }

         });
        On("x2", (E) =>
        {
            Debug.Log(E.data);
            string id = E.data["id"].ToString().Replace("'", "").Replace("\"", "");
            if (id != myId)
            {
                float enemyWidth = (E.data["health"].f);
                float enemyHeight = (E.data["asp"].f);
                enemyX2 = (((enemyWidth / enemyHeight) * 9) / 16);
                enemyScreen = (enemyWidth / enemyHeight);
                Time.timeScale = 1;
            }
        });
        On("spawn", (E) =>
        {
            if (!Finish)
            {
                string id = E.data["id"].ToString().Replace("'", "").Replace("\"", "");
                int unitId = int.Parse(E.data["unitid"].ToString().Replace("'", "").Replace("\"", ""));
                int placeId = int.Parse(E.data["placeId"].ToString().Replace("'", "").Replace("\"", ""));
                int health = (int)(E.data["health"].f);
                byte attackDamage = (byte)E.data["attackDamage"].f;
                byte abilityPower = (byte)E.data["abilityPower"].f;
                byte armour = (byte)E.data["armour"].f;
                byte resistance = (byte)E.data["resistance"].f;
                float attackSpeed = (E.data["attackSpeed"].f) * 0.1f;
                string createId = E.data["insId"].ToString().Replace("'", "").Replace("\"", "");
                if (id.Equals(myId))
                {
                    GameObject c = Instantiate(myObjects[unitId - 1], transforms[placeId].position, myObjects[unitId - 1].transform.rotation);
                    Transform xa = c.transform;
                    xa.localScale = new Vector3(xa.localScale.x * x2, xa.localScale.y, xa.localScale.z);
                    UnitStats us = c.GetComponent<UnitStats>();
                    us.CreateId = createId;
                    us.MaxHealth = health;
                    us.AttackSpeed = attackSpeed;
                    us.UnitId = (byte)unitId;

                }
                else
                {
                    GameObject x = Instantiate(enemyObjects[unitId - 1], Enemytransforms[placeId].position, enemyObjects[unitId - 1].transform.rotation);
                    Transform xa = x.transform;
                    xa.localScale = new Vector3(xa.localScale.x * x2, xa.localScale.y, xa.localScale.z);
                    UnitStats us = x.GetComponent<UnitStats>();
                    us.CreateId = createId;
                    us.MaxHealth = health;
                    us.AttackSpeed = attackSpeed;
                    us.Health = health;
                    us.AttackDamage = attackDamage;
                    us.AbilityPower = abilityPower;
                    us.Armour = armour;
                    us.Resistance = resistance;
                }
            }
           
        });
        On("attack", (E) =>
        {
            string id = E.data["id"].ToString().Replace("'", "").Replace("\"", "");
            string CreateId = E.data["CreateId"].ToString().Replace("'", "").Replace("\"", "");
            int damage = (int)E.data["damage"].f;
            if (id == myId)
            {
                GameObject[] myObject2 = GameObject.FindGameObjectsWithTag("enemyUnit");
                foreach (var item in myObject2)
                {
                    UnitStats usx = item.GetComponent<UnitStats>();
                    UnitFunctions ufx = item.GetComponent<UnitFunctions>();
                    if (usx.CreateId == CreateId)
                    {
                        if (damage > 0)
                        {
                            usx.Health -= damage;
                            ufx.ShowDamageOnText(damage);
                            ufx.ShowHealthImage();
                        }
                        
                        if (usx.Health <= 0)
                        {
                            Kill kill = new Kill
                            {
                                id = myId,
                                CreateId = usx.CreateId,
                                roomId = roomId
                            };
                            Emit("kill", new JSONObject(JsonUtility.ToJson(kill)));
                        }
                    }
                }
            }
            else
            {
                GameObject[] myObject = GameObject.FindGameObjectsWithTag("unit");
                foreach (var item in myObject)
                {
                    UnitStats usx = item.GetComponent<UnitStats>();
                    UnitFunctions ufx = item.GetComponent<UnitFunctions>();
                    if (usx.CreateId == CreateId)
                    {
                        if (damage > 0)
                        {
                            usx.Health -= damage;
                            ufx.ShowDamageOnText(damage);
                            ufx.ShowHealthImage();
                        }
                        if (usx.Health <= 0)
                        {
                            Kill kill = new Kill
                            {
                                id = myId,
                                CreateId = usx.CreateId,
                                roomId = roomId
                            };
                            Emit("kill", new JSONObject(JsonUtility.ToJson(kill)));
                        }
                    }
                }
            }


        });
        On("kill", (E) =>
        {
            string id = E.data["id"].ToString().Replace("'", "").Replace("\"", "");
            string CreateId = E.data["CreateId"].ToString().Replace("'", "").Replace("\"", "");

            GameObject[] myObject2 = GameObject.FindGameObjectsWithTag("enemyUnit");
            foreach (var item in myObject2)
            {
                UnitStats usx = item.GetComponent<UnitStats>();
                if (usx != null)
                {
                    if (usx.CreateId == CreateId)
                    {
                        Destroy(item);
                    }
                }
                else
                {
                    Destroy(item);
                }
            }
            GameObject[] myObject = GameObject.FindGameObjectsWithTag("unit");
            foreach (var item in myObject)
            {
                UnitStats usx = item.GetComponent<UnitStats>();
                if (usx != null)
                {
                    if (usx.CreateId == CreateId)
                    {
                        Destroy(item);
                    }
                }
                else
                {
                    Destroy(item);
                }
            }
        });
        On("lifeSteal", (E) =>
        {
            string CreateId = E.data["CreateId"].ToString().Replace("'", "").Replace("\"", "");
            int value = (int)E.data["value"].f;
            GameObject[] allFriends = GameObject.FindGameObjectsWithTag("unit");
            GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("enemyUnit");
            DoIt(allFriends, CreateId, value,0);
            DoIt(allEnemies, CreateId, value,0);
        });
        On("attackCount", (E) =>
        {
            string CreateId = E.data["CreateId"].ToString().Replace("'", "").Replace("\"", "");
            int attackCount = (int)E.data["AttackCount"].f;
            GameObject[] allFriends = GameObject.FindGameObjectsWithTag("unit");
            GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("enemyUnit");
            Debug.Log(E.data);
            DoIt(allFriends, CreateId, attackCount, 1);
            DoIt(allEnemies, CreateId, attackCount, 1);
        });
        On("attackBaseSignal", (E) =>
        {
            string id = E.data["id"].ToString().Replace("'", "").Replace("\"", "");
            string cid = E.data["createId"].ToString().Replace("'", "").Replace("\"", "");
            bool baseAttack = E.data["value"].b;
            if (id != myId)
            {
                GameObject[] allEnemy = GameObject.FindGameObjectsWithTag("enemyUnit");
                foreach (var item in allEnemy)
                {
                    UnitStats us = item.GetComponent<UnitStats>();
                    if (us.CreateId == cid)
                    {
                        Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
                        rb.velocity = new Vector2(0, 0);
                        Animator anim = item.GetComponent<Animator>();
                        us.ab = !baseAttack;
                        anim.SetFloat("attackSpeed", (anim.runtimeAnimatorController.animationClips[1].length / (1 / us.AttackSpeed)));
                        anim.SetBool("isWalk", false);
                        anim.SetBool("isAtack", true);
                        anim.SetBool("isAttack", true);
                    }
                }
            }
        });
        On("attackBase", (E) =>
        {

            string id = E.data["id"].ToString().Replace("'", "").Replace("\"", "");
            int damage = (int)(E.data["damage"].f);
            int which = 0;
            if (id != myId && ud.yourHp > 0)
            {
                ud.yourHp -= damage;
                which = 1;
                if (ud.yourHp <= 0 && !Finish)
                {
                    Finish = true;
                }
            }
            else if (id == myId && ud.enemyHp > 0)
            {
                ud.enemyHp -= damage;
                which = 2;
                if (ud.enemyHp <= 0 && !Finish)
                {
                    Finish = true;
                }
            }
            if (Finish)
            {
                ud.money = 0;
                GameObject[] myUnit = GameObject.FindGameObjectsWithTag("unit");
                foreach (var item in myUnit)
                {
                    Destroy(item);
                }
                GameObject[] enemyUnit = GameObject.FindGameObjectsWithTag("enemyUnit");
                foreach (var item in enemyUnit)
                {
                    Destroy(item);
                }
                if (which == 1)
                {
                    tmpCond.SetText("You Lose");
                }
                if (which == 2)
                {
                    tmpCond.SetText("Win");
                }
                StartCoroutine(Menu());
                //Diss d = new Diss();
                //d.id = myId;
                //Emit("diss", new JSONObject(JsonUtility.ToJson(d)));
            }
        });
        //On("disconnected", (E) =>
        //{
        //    string id = E.data["id"].ToString().Replace("'", "").Replace("\"", "");
        //    if (id != myId)
        //    {
        //        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemyUnit");
        //        foreach (var item in enemies)
        //        {
        //            Destroy(item);
        //        }
        //    }
        //});
    }
    private void DoIt(GameObject[] items, string CreateId, int value,int work)
    {
        foreach (var item in items)
        {
            UnitStats us = item.GetComponent<UnitStats>();
            UnitFunctions uf = item.GetComponent<UnitFunctions>();
            if (us.CreateId == CreateId)
            {
                if (work == 0)
                {
                    if (us.Health > 0)
                    {
                        int maxHp = us.MaxHealth;
                        int hp = us.Health;
                        value = hp + value >= maxHp ? maxHp - hp :  value;
                        us.Health = us.Health + value >= us.MaxHealth ? us.MaxHealth : us.Health + value;
                        uf.ShowRegenerationOnText(value);
                        uf.ShowHealthImage();
                    }
                }
                else if (work == 1)
                {
                    value++;
                    us.AttackCount = value;
                }
               
            }
        }
    }
    IEnumerator Menu()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("another");
    }
}
public class Diss
{
    public string id;
}
public class Kill
{
    public string CreateId;
    public string id;
    public string roomId;
}
public class FinishGame
{
    public string id;
   
}
