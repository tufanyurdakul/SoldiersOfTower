﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitMovement3 : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D rgbUnit;
    private Vector2 firstVelocity;
    private Animator anim;
    private float atackSpeedTimer, atackSpeedFull;
    private UnitStats stats, enemyStats;
    private UnitFunctions unitFunctions, enemyUnitFunctions;
    private int enemyState;
    public Image skillImage;
    private NetworkClient nc;
    private bool attackBarracs = false;
    private GameObject[] Barracks;
    private float x2;
    private string roomId;
    private float range = 0.25f;
    private GameObject attackEffect,FourthEffect;
    private void Awake()
    {
        GameObject network = GameObject.Find("Network");
        nc = network.GetComponent<NetworkClient>();
    }
    void Start()
    {
        roomId = Room.roomId;
        Barracks = GameObject.FindGameObjectsWithTag("Finish");
        rgbUnit = gameObject.GetComponent<Rigidbody2D>();
        x2 = (float)Screen.width / (float)Screen.height;
        switch (gameObject.transform.rotation.y)
        {
            case 1:
                rgbUnit.velocity = new Vector3(-1, 0) * x2;
                enemyState = 1;
                attackEffect = Resources.Load("Effects/AttackEffectEnemy") as GameObject;
                FourthEffect = Resources.Load("Effects/Unit3EffectEnemy") as GameObject;

                break;
            case 0:
                rgbUnit.velocity = new Vector3(1, 0) * x2;
                enemyState = 0;
                attackEffect = Resources.Load("Effects/AttackEffect") as GameObject;
                FourthEffect = Resources.Load("Effects/Unit3Effect") as GameObject;
                break;
        }
        firstVelocity = rgbUnit.velocity;
        anim = gameObject.GetComponent<Animator>();
        stats = gameObject.GetComponent<UnitStats>();
        unitFunctions = gameObject.GetComponent<UnitFunctions>();
        atackSpeedTimer = (1 / stats.AttackSpeed);
        atackSpeedFull = (1 / stats.AttackSpeed);
        anim.SetFloat("attackSpeed", (anim.runtimeAnimatorController.animationClips[1].length / (1 / stats.AttackSpeed)));
    }
    void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Unit3Attack"))
        {
            atackSpeedTimer -= Time.deltaTime;
            if (atackSpeedTimer <= 0)
            {
                if (enemyUnitFunctions != null)
                {
                    if (enemyState == 0)
                    {
                        SendServerAttack2 ssa = new SendServerAttack2();
                        if (stats.AttackCount == 4)
                        {
                            int physicalDamage = enemyUnitFunctions.HitPhysicalDamage(stats.AttackDamage);
                            int calculatemagic = (int)(5 + ((float)enemyStats.MaxHealth * (0.01f + ((float)stats.AbilityPower * 0.02f))));
                            int magicialDamage = enemyUnitFunctions.HitMagicialDamage(calculatemagic);
                            ssa.damage = physicalDamage + magicialDamage;
                            ssa.id = nc.myId;
                            ssa.roomId = roomId;
                            ssa.UnitId = 3;
                            ssa.CreateId = enemyStats.CreateId;
                            LifeSteal ls = new LifeSteal()
                            {
                                CreateId = stats.CreateId,
                                roomId = roomId,
                                value = magicialDamage
                            };
                            nc.Emit("lifeSteal", new JSONObject(JsonUtility.ToJson(ls)));
                            stats.AttackCount = 0;
                            GameObject effect = Instantiate(FourthEffect, gameObject.transform.position, Quaternion.identity);
                            effect.transform.SetParent(gameObject.transform);
                        }
                        else
                        {
                            int physicalDamage = enemyUnitFunctions.HitPhysicalDamage(stats.AttackDamage);
                            ssa.damage = physicalDamage;
                            ssa.id = nc.myId;
                            ssa.roomId = roomId;
                            ssa.UnitId = 3;
                            ssa.CreateId = enemyStats.CreateId;
                            GameObject effect = Instantiate(attackEffect, gameObject.transform.position, gameObject.transform.rotation);
                            effect.transform.SetParent(gameObject.transform);
                        }
                        nc.Emit("attack", new JSONObject(JsonUtility.ToJson(ssa)));
                        SendAttackCount sac = new SendAttackCount()
                        {
                            AttackCount = stats.AttackCount,
                            roomId = roomId,
                            CreateId = stats.CreateId
                        };
                        nc.Emit("attackCount", new JSONObject(JsonUtility.ToJson(sac)));
                    }
                }
                if (enemyUnitFunctions == null && attackBarracs && enemyState == 0)
                {
                    SendServerAttack2 ssa = new SendServerAttack2();
                    ssa.damage = stats.AttackDamage;
                    ssa.CreateId = stats.CreateId;
                    ssa.UnitId = 3;
                    ssa.id = nc.myId;
                    ssa.roomId = roomId;
                    nc.Emit("attackBase", new JSONObject(JsonUtility.ToJson(ssa)));
                }
                else if (enemyState == 1)
                {
                    if (stats.AttackCount == 4)
                    {
                        GameObject effect = Instantiate(FourthEffect, gameObject.transform.position, Quaternion.identity);
                        effect.transform.SetParent(gameObject.transform);
                    }
                    else
                    {
                        GameObject effect = Instantiate(attackEffect, gameObject.transform.position, gameObject.transform.rotation);
                        effect.transform.SetParent(gameObject.transform);
                    }
                    
                }
                skillImage.fillAmount = ((float)stats.AttackCount) / 4;
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
                        rgbUnit.velocity = new Vector2(0, 0);
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

