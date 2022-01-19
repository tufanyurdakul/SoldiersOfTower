using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovement10 : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D rgbUnit;
    private Vector2 firstVelocity;
    private Animator anim;
    private float atackSpeedTimer, atackSpeedFull;
    private UnitStats stats, enemyStats;
    private UnitFunctions unitFunctions, enemyUnitFunctions;
    private int enemyState;
    private NetworkClient nc;
    public string EnemyId;
    private bool attackBarracs;
    private GameObject[] Barracks;
    private float x2;
    private string roomId;
    private float range = 2;
    private GameObject unit10Arrow;
    private List<string> ColUnitId;
    private List<int> ColCount;
    private void Awake()
    {
        ColCount = new List<int>();
        ColUnitId = new List<string>();
        GameObject network = GameObject.Find("Network");
        nc = network.GetComponent<NetworkClient>();
        anim = gameObject.GetComponent<Animator>();
    }
    void Start()
    {
        roomId = Room.roomId;
        Barracks = GameObject.FindGameObjectsWithTag("Finish");
        unit10Arrow = Resources.Load("Unit10Arrow") as GameObject;
        stats = gameObject.GetComponent<UnitStats>();
        rgbUnit = gameObject.GetComponent<Rigidbody2D>();
        x2 = (float)Screen.width / (float)Screen.height;
        switch (gameObject.transform.rotation.y)
        {
            case 1:
                rgbUnit.velocity = new Vector2(-1, 0) * x2;
                enemyState = 1;
                break;
            case 0:
                rgbUnit.velocity = new Vector2(1, 0) * x2;
                enemyState = 0;
                break;
        }
        firstVelocity = rgbUnit.velocity;
        unitFunctions = gameObject.GetComponent<UnitFunctions>();
        atackSpeedFull = (1 / stats.AttackSpeed);
        atackSpeedTimer = (int)(1.0f / (float)stats.AttackSpeed);
        anim.SetFloat("attackSpeed", (anim.runtimeAnimatorController.animationClips[1].length / (1 / stats.AttackSpeed)));
    }


    void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Unit10Attack"))
        {
            atackSpeedTimer -= Time.deltaTime;
            if (atackSpeedTimer <= 0)
            {
                if (enemyUnitFunctions != null && enemyStats != null)
                {
                    if (enemyState == 0)
                    {
                        int count = 0;
                        bool isThere = false;
                        if (ColCount.Count > 0)
                        {
                            for(int i = 0; i<ColCount.Count; i++)
                            {
                                if (ColUnitId[i] == enemyStats.CreateId)
                                {
                                    isThere = true;
                                    ColCount[i]++;
                                    count = ColCount[i];
                                    break;
                                }
                            }
                            if (!isThere)
                            {
                                ColCount.Add(0);
                                ColUnitId.Add(enemyStats.CreateId);
                                count = 0;
                            }
                        }
                        else
                        {
                            ColCount.Add(0);
                            ColUnitId.Add(enemyStats.CreateId);
                            count = 0;
                        }
                        GameObject u10Arrow = Instantiate(unit10Arrow, gameObject.transform.position, Quaternion.identity);
                        Unit10Arrow arrow = u10Arrow.GetComponent<Unit10Arrow>();
                        arrow.enemy = 0;
                        arrow.AbilityPower = stats.AbilityPower;
                        arrow.AttackDamage = stats.AttackDamage;
                        arrow.Count = count;
                        arrow.EnemyCreateId = enemyStats.CreateId;
                        arrow.AttackBarracks = false;
                        u10Arrow.transform.rotation = gameObject.transform.rotation;
                        Rigidbody2D u10Rb = u10Arrow.GetComponent<Rigidbody2D>();
                        u10Rb.velocity = new Vector2(6,0);
                        Destroy(u10Arrow, 2);
                    }
                }
                if (attackBarracs && enemyState == 0)
                {
                    GameObject u10Arrow = Instantiate(unit10Arrow, gameObject.transform.position, Quaternion.identity);
                    Unit10Arrow arrow = u10Arrow.GetComponent<Unit10Arrow>();
                    arrow.enemy = 0;
                    arrow.AttackDamage = stats.AttackDamage;
                    arrow.CreateId = stats.CreateId;
                    arrow.AttackBarracks = true;
                    u10Arrow.transform.rotation = gameObject.transform.rotation;
                    Rigidbody2D u10Rb = u10Arrow.GetComponent<Rigidbody2D>();
                    u10Rb.velocity = new Vector2(6, 0);
                    Destroy(u10Arrow, 2);
                }
                if (enemyState == 1 && enemyUnitFunctions != null)
                {
                    GameObject u10Arrow = Instantiate(unit10Arrow, gameObject.transform.position, Quaternion.identity);
                    Unit10Arrow arrow = u10Arrow.GetComponent<Unit10Arrow>();
                    arrow.enemy = 1;
                    u10Arrow.transform.rotation = gameObject.transform.rotation;
                    Rigidbody2D u10Rb = u10Arrow.GetComponent<Rigidbody2D>();
                    u10Rb.velocity = new Vector2(-6,0);
                    Destroy(u10Arrow, 2);
                }
                atackSpeedTimer = atackSpeedFull;
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
                            rgbUnit.velocity = new Vector2(0, 0);
                            next = false;
                            break;
                        }
                        else if (gameObject.transform.position.x == item.transform.position.x)
                        {
                            rgbUnit.velocity = new Vector2(0, 0);
                            gameObject.transform.position = new Vector3(gameObject.transform.position.x - 0.01f, gameObject.transform.position.y, gameObject.transform.position.z);
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
                            if (lenX < 0.75f * x2)
                            {
                                rgbUnit.velocity = new Vector2(0, 0);
                            }
                           
                            anim.SetBool("isAttack", true);
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
                        bool otherControl = false;
                        GameObject[] enemyUnit = GameObject.FindGameObjectsWithTag("enemyUnit");
                        if (enemyUnit.Length > 0)
                        {
                            foreach (var item in enemyUnit)
                            {
                                if (item.transform.position.y == gameObject.transform.position.y)
                                {
                                    otherControl = true;
                                    break;
                                }
                            }
                        }
                        if (!otherControl)
                        {
                            enemyUnitFunctions = null;
                            anim.SetBool("isAttack", true);
                            anim.SetBool("isWalk", false);
                            Signal s = new Signal();
                            s.id = nc.myId;
                            s.createId = stats.CreateId;
                            s.roomId = roomId;
                            nc.Emit("attackBaseSignal", new JSONObject(JsonUtility.ToJson(s)));
                            attackBarracs = true;
                            rgbUnit.velocity = new Vector2(0, 0);
                        }
                    }
                    else
                    {
                        rgbUnit.velocity = firstVelocity;
                        enemyUnitFunctions = null;
                        anim.SetBool("isAttack", false);
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
                            anim.SetBool("isAttack", true);
                            anim.SetBool("isWalk", false);
                        }
                    }
                }
            }
        }
        else if (enemyState == 1 && !stats.ab)
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
                            gameObject.transform.position = new Vector3(gameObject.transform.position.x - 0.01f, gameObject.transform.position.y, gameObject.transform.position.z);
                            next = false;
                            break;
                        }

                    }
                }
                if (allenemy.Length > 0)
                {
                    float lenX = range * x2;
                    bool choosen = false;
                    GameObject enemyItem = null;
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
                                    enemyItem = item;
                                }
                                next = false;
                            }
                        }
                    }
                    if (!next)
                    {
                        if (choosen)
                        {
                            enemyUnitFunctions = enemyItem.GetComponent<UnitFunctions>();
                            if (lenX < 0.75f * x2)
                            {
                                rgbUnit.velocity = new Vector2(0, 0);
                            }
                            anim.SetBool("isAttack", true);
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
                        anim.SetBool("isAttack", true);
                        anim.SetBool("isWalk", false);
                    }
                    else
                    {
                        rgbUnit.velocity = firstVelocity;
                        anim.SetBool("isAttack", false);
                        anim.SetBool("isWalk", true);
                    }
                }
            }
        }

    }
}

