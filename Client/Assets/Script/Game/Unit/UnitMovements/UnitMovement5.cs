using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitMovement5 : MonoBehaviour
{
    // Start is called before the first frame update

    private Animator anim;
    private float atackSpeedTimer, atackSpeedFull;
    private UnitStats stats, enemyStats;
    private UnitFunctions unitFunctions, enemyUnitFunctions;
    private int enemyState;
    private NetworkClient nc;
    public string EnemyId;
    private bool attackBarracs = false;
    private GameObject[] Barracks;
    private float x2;
    private GameObject ammo, ad;
    private Rigidbody2D rgbUnit;
    private Vector2 firstVelocity;
    private string roomId;
    private float range = 4;
    private void Awake()
    {
        GameObject network = GameObject.Find("Network");
        nc = network.GetComponent<NetworkClient>();
    }
    void Start()
    {
        rgbUnit = gameObject.GetComponent<Rigidbody2D>();
        Barracks = GameObject.FindGameObjectsWithTag("Finish");
        x2 = (float)Screen.width / (float)Screen.height;
        roomId = Room.roomId;
        switch (gameObject.transform.rotation.y)
        {
            case 1:
                rgbUnit.velocity = new Vector3(-1, 0) * x2;
                enemyState = 1;
                break;
            case 0:
                rgbUnit.velocity = new Vector3(1, 0) * x2;
                enemyState = 0;
                break;
        }
        firstVelocity = rgbUnit.velocity;
        anim = gameObject.GetComponent<Animator>();
        stats = gameObject.GetComponent<UnitStats>();
        unitFunctions = gameObject.GetComponent<UnitFunctions>();
        atackSpeedTimer = (1 / stats.AttackSpeed);
        atackSpeedFull = (1 / stats.AttackSpeed);
        anim.SetFloat("attackSpeed", (anim.runtimeAnimatorController.animationClips[1].length / (1 / stats.AttackSpeed)));
        ammo = Resources.Load("FlameAttack") as GameObject;
    }



    void Update()
    {

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Unit5Attack"))
        {
            atackSpeedTimer -= Time.deltaTime;
            if (atackSpeedTimer <= 0)
            {
                if (enemyUnitFunctions != null && enemyStats != null)
                {
                    if (enemyState == 0)
                    {
                        int damage = enemyUnitFunctions.HitPhysicalDamage(stats.AttackDamage);
                        SendServerAttack2 ssa = new SendServerAttack2();
                        ssa.CreateId = enemyStats.CreateId;
                        ssa.roomId = roomId;
                        ssa.id = nc.myId;
                        ssa.UnitId = 5;
                        ssa.damage = damage;
                        nc.Emit("attack", new JSONObject(JsonUtility.ToJson(ssa)));
                        GameObject copyAmmo = Instantiate(ammo, enemyUnitFunctions.transform.position, enemyUnitFunctions.transform.rotation);
                        FlameEffect fe = copyAmmo.GetComponent<FlameEffect>();
                        fe.AbilityPower = stats.AbilityPower;
                        fe.FromWho = -1;
                        fe.id = nc.myId;
                    }
                }
                if (enemyUnitFunctions == null && attackBarracs)
                {
                    if (enemyState == 0)
                    {
                        SendServerAttack2 ssa = new SendServerAttack2();
                        ssa.damage = stats.AttackDamage;
                        ssa.CreateId = stats.CreateId;
                        ssa.UnitId = 5;
                        ssa.id = nc.myId;
                        ssa.roomId = roomId;
                        nc.Emit("attackBase", new JSONObject(JsonUtility.ToJson(ssa)));
                    }
                }
                if (ad != null && enemyState == 1)
                {
                    GameObject CopyAmmo = Instantiate(ammo, ad.transform.position, ad.transform.rotation);
                    CopyAmmo.transform.Rotate(0, 180, 0);
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
                            rgbUnit.velocity = new Vector3(0, 0);
                            next = false;
                            break;
                        }
                        else if (gameObject.transform.position.x == item.transform.position.x)
                        {
                            rgbUnit.velocity = new Vector3(0, 0);
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
                            rgbUnit.velocity = new Vector3(0, 0);
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
                        enemyUnitFunctions = null;
                        anim.SetBool("isAttack", true);
                        anim.SetBool("isWalk", false);
                        Signal s = new Signal();
                        s.id = nc.myId;
                        s.createId = stats.CreateId;
                        s.roomId = roomId;
                        nc.Emit("attackBaseSignal", new JSONObject(JsonUtility.ToJson(s)));
                        attackBarracs = true;
                        rgbUnit.velocity = new Vector3(0, 0);
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
                            anim.SetBool("isAttack", true);
                            anim.SetBool("isWalk", false);
                            Signal s = new Signal();
                            s.id = nc.myId;
                            s.createId = stats.CreateId;
                            s.value = true;
                            s.roomId = roomId;
                            nc.Emit("attackBaseSignal", new JSONObject(JsonUtility.ToJson(s)));
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
                                    ad = item;
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
                            anim.SetBool("isAttack", true);
                            anim.SetBool("isWalk", false);

                        }
                    }
                }
                if (next)
                {
                    rgbUnit.velocity = firstVelocity;
                    anim.SetBool("isAttack", false);
                    anim.SetBool("isWalk", true);
                }
            }

        }
    }
}

