using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockEffect : MonoBehaviour
{
    public int AbilityPower { private get; set; }
    public string SocketId { private get; set; }
    public bool FromWho { private get; set; }
    private NetworkClient nc;
    private void Start()
    {
        nc = GameObject.Find("Network").GetComponent<NetworkClient>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "enemyUnit")
        {
            if (FromWho)
            {
                UnitFunctions hitFunc = collision.GetComponent<UnitFunctions>();
                UnitStats unitStats = collision.GetComponent<UnitStats>();
                int damage = hitFunc.HitMagicialDamage(AbilityPower * 6);
                SendServerAttack2 ss2 = new SendServerAttack2()
                {
                    CreateId = unitStats.CreateId,
                    damage = damage,
                    id = SocketId,
                    roomId = Room.roomId,
                    UnitId = 9
                };
                nc.Emit("attack", new JSONObject(JsonUtility.ToJson(ss2)));
            }
        }
    }
}
