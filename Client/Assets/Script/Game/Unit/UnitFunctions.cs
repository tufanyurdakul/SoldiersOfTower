using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitFunctions : MonoBehaviour
{
    private float armour, resistance;
    public int health { get; set; }
    private int maxHealth;
    private byte ar, res;
    private UnitStats unitStats;
    public Image HealthImage;
    private GameObject tmpDamage, tmpRegeneration;
    public Transform hp, ad;
    private List<float> poisonTimer,poisonTime,poisonPerTime;
    private List<int> physic, magic,unitIds;
    public Transform heal;
    private NetworkClient nc;
    public bool canDamage { get; set; }
    private GameObject poisonEffect,copyPoisonEffect;
    private GameObject healing;
    void Start()
    {
        nc = GameObject.Find("Network").GetComponent<NetworkClient>();
        poisonTimer = new List<float>();
        poisonTime = new List<float>();
        poisonPerTime = new List<float>();
        magic = new List<int>();
        physic = new List<int>();
        unitIds = new List<int>();
        tmpRegeneration = (Resources.Load("textRegeneration") as GameObject);
        healing = Resources.Load("Effects/Healing") as GameObject;
        tmpDamage = (Resources.Load("textDamage") as GameObject);
        unitStats = gameObject.GetComponent<UnitStats>();
        armour = (float)(100 / (100 + (float)unitStats.Armour));
        resistance = (float)(100 / (100 + (float)unitStats.Resistance));
        maxHealth = unitStats.MaxHealth;
        ar = unitStats.Armour;
        res = unitStats.Resistance;
        poisonEffect = Resources.Load("Effects/PoisonEffect") as GameObject;

    }
    public void Poisoned(int PhysicalDamage,int MagicialDamage,float perSecond,float total,int unitId)
    {
        physic.Add(PhysicalDamage);
        magic.Add(MagicialDamage);
        poisonTimer.Add(perSecond);
        poisonPerTime.Add(perSecond);
        poisonTime.Add(total);
        unitIds.Add(unitId);
    }
    public void SetArmour()
    {
        armour = (float)(100 / (100 + (float)unitStats.Armour));
    }
    public int HitPhysicalDamage(int AttackDamage)
    {
        int damage = 0;
        if (!canDamage)
        {
            damage = (int)(AttackDamage * armour);
        }
        return damage;
    }
    public int HitMagicialDamage(int AbilityPower)
    {
        int damage = 0;
        if (!canDamage)
        {
            damage = (int)((float)AbilityPower * (float)resistance);
        }
        return damage;
    }

    public void ShowHealthImage()
    {
        health = unitStats.Health;
        maxHealth = unitStats.MaxHealth;
        HealthImage.fillAmount = (float)health / (float)maxHealth;
    }
    public void ShowDamageOnText(int damage)
    {
        GameObject Copy = Instantiate(tmpDamage, ad.transform.position, Quaternion.identity);
        Copy.transform.SetParent(ad.transform);
        TextMeshPro tmp = Copy.GetComponent<TextMeshPro>();
        tmp.SetText($"-{damage}");
        Destroy(Copy, 1);
    }
    public void ShowRegenerationOnText(int regeneration)
    {
        GameObject copyHeal = Instantiate(healing, heal.position, healing.transform.rotation);
        GameObject Copy = Instantiate(tmpRegeneration, hp.transform.position, Quaternion.identity);
        Copy.transform.SetParent(hp.transform);
        copyHeal.transform.SetParent(copyHeal.transform);
        TextMeshPro tmp = Copy.GetComponent<TextMeshPro>();
        tmp.SetText($"+{regeneration}");
        Destroy(Copy, 0.7f);
        Destroy(copyHeal, 1);
    }

    private void Update()
    {
        if (poisonTimer.Count > 0)
        {
            for(int i = 0; i< poisonTimer.Count; i++)
            {
                poisonTime[i] -= Time.deltaTime;
                if (poisonTime[i] > 0)
                {
                    poisonTimer[i] -= Time.deltaTime;
                    if (poisonTimer[i] <= 0)
                    {
                        int damage = HitPhysicalDamage(physic[i]) + HitMagicialDamage(magic[i]);
                        SendServerAttack2 sendServer = new SendServerAttack2()
                        {
                            CreateId = unitStats.CreateId,
                            damage = damage,
                            id = nc.myId,
                            roomId = Room.roomId,
                            UnitId = unitIds[i]
                        };
                        nc.Emit("attack", new JSONObject(JsonUtility.ToJson(sendServer)));
                        poisonTimer[i] = poisonPerTime[i];
                        GameObject PoisonImpact = Instantiate(healing, heal.position, healing.transform.rotation);
                        PoisonImpact.transform.SetParent(gameObject.transform);
                        ParticleSystem ps = PoisonImpact.GetComponent<ParticleSystem>();
                        ps.startColor = Color.red;
                        Destroy(PoisonImpact, 1);
                    }
                }
                else
                {
                    poisonTime.RemoveAt(i);
                    poisonTimer.RemoveAt(i);
                    poisonPerTime.RemoveAt(i);
                    physic.RemoveAt(i);
                    magic.RemoveAt(i);
                }
            }
        }
        if (poisonTimer.Count > 0 && copyPoisonEffect == null)
        {
            copyPoisonEffect = Instantiate(poisonEffect, gameObject.transform.position, Quaternion.identity);
            copyPoisonEffect.transform.SetParent(gameObject.transform);
        }
        else if (poisonTimer.Count == 0 && copyPoisonEffect != null )
        {
            Destroy(copyPoisonEffect);
        }
    }
}

