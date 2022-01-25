using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class UnitsMovement : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D rgbUnit;
    private Vector2 firstVelocity;
    private Animator anim;
    private float atackSpeedTimer, atackSpeedFull;
    private UnitStats stats, enemyStats;
    private UnitFunctions unitFunctions, enemyUnitFunctions;
    private int enemyState;
    //private int  speed,firstSpeed;
    private NetworkClient nc;
    public string EnemyId, roomId;
    private bool attackBarracs = false;
    private GameObject[] Barracks;
    private float x2;
    private float range = 0.25f;
    private GameObject attackAnimation;
    private void Awake()
    {
        roomId = Room.roomId;
        GameObject network = GameObject.Find("Network");
        nc = network.GetComponent<NetworkClient>();
    }
    void Start()
    {
        x2 = (float)Screen.width / (float)Screen.height;
        Barracks = GameObject.FindGameObjectsWithTag("Finish");
        rgbUnit = gameObject.GetComponent<Rigidbody2D>();
        switch (gameObject.transform.rotation.y)
        {
            case 1:
                rgbUnit.velocity = new Vector2(-1, 0) * x2;
                enemyState = 1;
                attackAnimation = Resources.Load("Effects/AttackEffectEnemy") as GameObject;
                break;
            case 0:
                rgbUnit.velocity = new Vector2(1, 0) * x2;
                enemyState = 0;
                attackAnimation = Resources.Load("Effects/AttackEffect") as GameObject;
                break;
        }
        firstVelocity = rgbUnit.velocity;
        anim = gameObject.GetComponent<Animator>();
        stats = gameObject.GetComponent<UnitStats>();
        unitFunctions = gameObject.GetComponent<UnitFunctions>();
        atackSpeedTimer = (1 / stats.AttackSpeed);
        atackSpeedFull = atackSpeedTimer;
        anim.SetFloat("attackSpeed", (anim.runtimeAnimatorController.animationClips[1].length / (1 / stats.AttackSpeed)));
    }
    void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Unit1Attack"))
        {
            atackSpeedTimer -= Time.deltaTime;

            if (enemyState == 0)
            {
                if (atackSpeedTimer <= 0)
                {
                    if (enemyUnitFunctions != null && enemyStats != null && enemyState == 0)
                    {
                        SendServerAttack2 ssa = new SendServerAttack2();
                        int physicalDamage = unitFunctions.HitPhysicalDamage(stats.AttackDamage);
                        ssa.damage = physicalDamage;
                        ssa.CreateId = enemyStats.CreateId;
                        ssa.UnitId = 1;
                        ssa.id = nc.myId;
                        ssa.roomId = roomId;
                        nc.Emit("attack", new JSONObject(JsonUtility.ToJson(ssa)));
                        int lifeSteal = 1 + (int)Mathf.Ceil((float)physicalDamage * ((float)stats.AbilityPower * 0.02f));
                        LifeSteal ls = new LifeSteal()
                        {
                            CreateId = stats.CreateId,
                            value = lifeSteal,
                            roomId = roomId
                        };
                        GameObject effect = Instantiate(attackAnimation, gameObject.transform.position, gameObject.transform.rotation);
                        effect.transform.SetParent(gameObject.transform);
                        nc.Emit("lifeSteal", new JSONObject(JsonUtility.ToJson(ls)));
                    }
                    else if (enemyUnitFunctions == null && attackBarracs)
                    {
                        Debug.Log("b");
                        if (enemyState == 0)
                        {
                            SendServerAttack2 ssa = new SendServerAttack2();
                            ssa.damage = stats.AttackDamage;
                            ssa.CreateId = stats.CreateId;
                            ssa.UnitId = 1;
                            ssa.id = nc.myId;
                            ssa.roomId = roomId;
                            nc.Emit("attackBase", new JSONObject(JsonUtility.ToJson(ssa)));
                        }
                    }


                    atackSpeedTimer = atackSpeedFull;
                }
            }
            else if (enemyState == 1)
            {
                if (atackSpeedTimer <= 0)
                {
                    GameObject effect = Instantiate(attackAnimation, gameObject.transform.position, gameObject.transform.rotation);
                    effect.transform.SetParent(gameObject.transform);
                    atackSpeedTimer = atackSpeedFull;
                }
            }
        }
        if (enemyState == 0 && !attackBarracs)
        {
            GameObject[] all = GameObject.FindGameObjectsWithTag("unit");
            GameObject[] allenemy = GameObject.FindGameObjectsWithTag("enemyUnit");
            if (all.Length > 0)
            {
                bool next = true;

                foreach (var item in all)
                {
                    if (gameObject.transform.position.y == item.transform.position.y && item != gameObject)
                    {
                        if (gameObject.transform.position.x > item.transform.position.x)
                        {
                            next = true;

                        }
                        else if (gameObject.transform.position.x < item.transform.position.x && Mathf.Abs(gameObject.transform.position.x - item.transform.position.x) * x2 < 2)
                        {
                            //speed = 0;
                            rgbUnit.velocity = new Vector2(0, 0);

                            next = false;
                            break;
                        }
                        else if (gameObject.transform.position.x == item.transform.position.x)
                        {
                            //speed = 0;
                            rgbUnit.velocity = new Vector2(0, 0);
                            gameObject.transform.position = new Vector3(gameObject.transform.position.x - 0.04f, gameObject.transform.position.y, gameObject.transform.position.z);
                            next = false;
                            break;
                        }

                    }
                }
                if (allenemy.Length > 0)
                {
                    float lenX = range * x2;
                    GameObject choosen = null;
                    foreach (var item in allenemy)
                    {
                        if (gameObject.transform.position.y == item.transform.position.y && item != gameObject)
                        {
                            if (Mathf.Abs(gameObject.transform.position.x - item.transform.position.x) < range * x2)
                            {
                                if (lenX > Mathf.Abs(gameObject.transform.position.x - item.transform.position.x))
                                {
                                    lenX = Mathf.Abs(gameObject.transform.position.x - item.transform.position.x);
                                    choosen = item;
                                }
                                next = false;
                            }
                        }
                    }
                    if (!next)
                    {
                        if (choosen != null)
                        {
                            rgbUnit.velocity = new Vector2(0, 0);
                            //speed = 0;
                            anim.SetBool("isAtack", true);
                            anim.SetBool("isWalk", false);
                            enemyUnitFunctions = choosen.GetComponent<UnitFunctions>();
                            enemyStats = choosen.GetComponent<UnitStats>();
                        }
                    }
                }
                if (next)
                {
                    bool isThere = false;
                    foreach (var item in Barracks)
                    {
                        if (item.transform.position.y == gameObject.transform.position.y)
                        {
                            if (item.transform.position.x < gameObject.transform.position.x || Mathf.Abs(item.transform.position.x - gameObject.transform.position.x) < range * x2)
                            {
                                isThere = true;
                                break;
                            }
                        }
                    }
                    if (isThere)
                    {
                        enemyUnitFunctions = null;
                        anim.SetBool("isAtack", true);
                        anim.SetBool("isWalk", false);
                        Signal s = new Signal();
                        s.id = nc.myId;
                        s.createId = stats.CreateId;
                        s.roomId = roomId;
                        nc.Emit("attackBaseSignal", new JSONObject(JsonUtility.ToJson(s)));
                        attackBarracs = true;
                        //speed = 0;
                        rgbUnit.velocity = new Vector2(0, 0);

                    }
                    else
                    {
                        rgbUnit.velocity = firstVelocity;

                        //speed = firstSpeed;
                        enemyUnitFunctions = null;
                        anim.SetBool("isAtack", false);
                        anim.SetBool("isWalk", true);
                    }

                }
            }
        }
        if (enemyState == 0 && attackBarracs)
        {
            GameObject[] allenemy = GameObject.FindGameObjectsWithTag("enemyUnit");
            if (allenemy.Length > 0)
            {
                foreach (var item in allenemy)
                {
                    if (item.transform.position.y == gameObject.transform.position.y)
                    {
                        if (Mathf.Abs(item.transform.position.x - gameObject.transform.position.x) < range * x2)
                        {
                            attackBarracs = false;
                            enemyUnitFunctions = item.GetComponent<UnitFunctions>();
                            enemyStats = item.GetComponent<UnitStats>();
                            Signal s = new Signal();
                            s.id = nc.myId;
                            s.createId = stats.CreateId;
                            s.value = true;
                            s.roomId = roomId;
                            nc.Emit("attackBaseSignal", new JSONObject(JsonUtility.ToJson(s)));
                            anim.SetBool("isAtack", true);
                            anim.SetBool("isWalk", false);
                        }
                    }
                }
            }
        }
        if (enemyState == 1)
        {

        }
        if (enemyState == 1 && !stats.ab)
        {
            GameObject[] all = GameObject.FindGameObjectsWithTag("enemyUnit");
            GameObject[] allenemy = GameObject.FindGameObjectsWithTag("unit");
            if (all.Length > 0)
            {
                bool next = true;
                foreach (var item in all)
                {
                    if (gameObject.transform.position.y == item.transform.position.y && item != gameObject)
                    {
                        if (gameObject.transform.position.x < item.transform.position.x)
                        {
                            next = true;

                        }
                        else if (gameObject.transform.position.x > item.transform.position.x && Mathf.Abs(gameObject.transform.position.x - item.transform.position.x) * x2 < 2)
                        {
                            rgbUnit.velocity = new Vector2(0, 0);
                            next = false;
                            break;
                        }
                        else if (gameObject.transform.position.x == item.transform.position.x)
                        {
                            rgbUnit.velocity = new Vector2(0, 0);
                            gameObject.transform.position = new Vector3(gameObject.transform.position.x + 0.04f, gameObject.transform.position.y, gameObject.transform.position.z);
                            next = false;
                            break;
                        }

                    }
                }
                if (allenemy.Length > 0)
                {
                    float lenX = range * x2;
                    bool choosen = false;
                    foreach (var item in allenemy)
                    {
                        if (gameObject.transform.position.y == item.transform.position.y && item != gameObject)
                        {
                            if (Mathf.Abs(gameObject.transform.position.x - item.transform.position.x) < range * x2)
                            {
                                if (lenX > Mathf.Abs(gameObject.transform.position.x - item.transform.position.x))
                                {
                                    lenX = Mathf.Abs(gameObject.transform.position.x - item.transform.position.x);
                                    choosen = true;
                                }
                                next = false;
                            }
                        }
                    }
                    if (!next)
                    {
                        if (choosen)
                        {
                            rgbUnit.velocity = new Vector2(0, 0);
                            anim.SetBool("isAtack", true);
                            anim.SetBool("isWalk", false);

                        }
                    }
                }
                if (next)
                {
                    bool isThere = false;
                    GameObject[] barracks = GameObject.FindGameObjectsWithTag("Respawn");
                    foreach (var item in barracks)
                    {
                        if (item.transform.position.y == gameObject.transform.position.y)
                        {
                            if (item.transform.position.x > gameObject.transform.position.x || Mathf.Abs(item.transform.position.x - gameObject.transform.position.x) < range * x2)
                            {
                                isThere = true;
                                break;
                            }
                        }
                    }
                    if (isThere)
                    {
                        rgbUnit.velocity = new Vector2(0, 0);
                        anim.SetBool("isAtack", true);
                        anim.SetBool("isWalk", false);
                    }
                    else
                    {
                        rgbUnit.velocity = firstVelocity;
                        anim.SetBool("isAtack", false);
                        anim.SetBool("isWalk", true);
                    }
                }
            }
        }
    }
}
