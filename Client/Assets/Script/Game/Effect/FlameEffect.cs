using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameEffect : MonoBehaviour
{
    public int AbilityPower { get; set; }
    public string id { get; set; }
    private Animator anim;
    private bool isEnded = false;
    public int unitId { get; set; }
    private NetworkClient nc;
    public int FromWho { get; set; }
    private List<GameObject> stayed;
    private List<GameObject> stayedFriend;
    private List<float> timeFriend;
    private List<float> time;
    private float timer = 3;
    private void Start()
    {
        nc = GameObject.Find("Network").GetComponent<NetworkClient>();
        anim = gameObject.GetComponent<Animator>();
        time = new List<float>();
        stayed = new List<GameObject>();
        timeFriend = new List<float>();
        stayedFriend = new List<GameObject>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject col = collision.gameObject;
        UnitStats us = null;
        if (col != null)
        {
            us = col.GetComponent<UnitStats>();
        }
        if (us != null)
        {
            if (us.UnitId != 5)
            {
                if (FromWho == -1)
                {
                    if (anim.GetCurrentAnimatorStateInfo(0).IsName("FlameDamage"))
                    {
                        if (col.tag == "enemyUnit")
                        {
                            if (stayed.Count > 0)
                            {
                                bool isThere = false;
                                GameObject myItem = null;
                                foreach (var item in stayed)
                                {
                                    if (item == col)
                                    {
                                        isThere = true;
                                        myItem = item;
                                        break;

                                    }
                                }
                                if (!isThere && myItem != null)
                                {
                                    SendData(myItem);
                                    stayed.Add(col);
                                    time.Add(1f);
                                }
                            }
                            else
                            {
                                SendData(col);
                                stayed.Add(col);
                                time.Add(1f);
                            }
                        }
                        else if (col.tag == "unit")
                        {
                            if (stayedFriend.Count > 0)
                            {
                                bool isThere = false;
                                GameObject myItem = null;
                                foreach (var item in stayedFriend)
                                {
                                    if (item == col)
                                    {
                                        isThere = true;
                                        myItem = item;
                                        break;

                                    }
                                }
                                if (!isThere)
                                {
                                    SendHeal(myItem);
                                    stayedFriend.Add(col);
                                    timeFriend.Add(2);
                                }
                            }
                            else
                            {
                                SendHeal(col);
                                stayedFriend.Add(col);
                                timeFriend.Add(2);
                            }
                        }
                    }
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject col = collision.gameObject;
        if (FromWho == -1)
        {
            if (col.tag == "enemyUnit")
            {
                for (int i = 0; i < stayed.Count; i++)
                {
                    if (stayed[i] == col)
                    {
                        stayed.RemoveAt(i);
                        time.RemoveAt(i);
                    }
                }
            }
            else if (col.tag == "unit")
            {
                for (int i = 0; i < stayedFriend.Count; i++)
                {
                    if (stayedFriend[i] == col)
                    {
                        stayedFriend.RemoveAt(i);
                        timeFriend.RemoveAt(i);
                    }
                }
            }
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            anim.SetBool("isEnd", true);
        }
        if (!isEnded && anim.GetCurrentAnimatorStateInfo(0).IsName("FlameEnd"))
        {
            isEnded = true;
            Destroy(gameObject, 0.2f);
        }
        if (stayed.Count > 0 && anim.GetCurrentAnimatorStateInfo(0).IsName("FlameDamage"))
        {
            for (int i = 0; i < stayed.Count; i++)
            {
                time[i] -= Time.deltaTime;
                if (time[i] <= 0)
                {
                    SendData(stayed[i]);
                    time[i] = 1f;
                }
            }
        }
        if (stayedFriend.Count > 0 && anim.GetCurrentAnimatorStateInfo(0).IsName("FlameDamage"))
        {
            for (int i = 0; i < stayedFriend.Count; i++)
            {
                timeFriend[i] -= Time.deltaTime;
                if (timeFriend[i] <= 0)
                {
                    SendHeal(stayedFriend[i]);
                    timeFriend[i] = 2;
                }
            }
        }
    }
    private void SendData(GameObject item)
    {
        if (item != null)
        {
            UnitStats us = item.GetComponent<UnitStats>();
            UnitFunctions uf = item.GetComponent<UnitFunctions>();
            int damage = uf.HitMagicialDamage((AbilityPower / 2));
            SendServerAttack2 ssa = new SendServerAttack2()
            {
                CreateId = us.CreateId,
                UnitId = 5,
                id = id,
                damage = damage,
                roomId = Room.roomId
            };
            nc.Emit("attack", new JSONObject(JsonUtility.ToJson(ssa)));
        }
    }
    private void SendHeal(GameObject item)
    {
        if (item != null)
        {
            UnitStats us = item.GetComponent<UnitStats>();
            LifeSteal ssa = new LifeSteal()
            {
                roomId = Room.roomId,
                CreateId = us.CreateId,
                value = (int)((float)AbilityPower / 3)
            };
            nc.Emit("lifeSteal", new JSONObject(JsonUtility.ToJson(ssa)));
        }
    }
}
