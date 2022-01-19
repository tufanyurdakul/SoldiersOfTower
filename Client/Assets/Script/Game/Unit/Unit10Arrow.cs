using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit10Arrow : MonoBehaviour
{
    public int AttackDamage { get; set; }
    public int AbilityPower { get; set; }
    public int enemy { get; set; }
    public int Count { get; set; }
    public string CreateId { get; set; }
    public bool AttackBarracks { get; set; }
    public string EnemyCreateId { get; set; }
    private bool hit = false;
    private NetworkClient nc;
    private void Start()
    {
        nc = GameObject.Find("Network").GetComponent<NetworkClient>();

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "enemyUnit")
        {
            UnitStats enemyStats = collision.gameObject.GetComponent<UnitStats>();
            if (!hit && enemy == 0 && enemyStats.CreateId == EnemyCreateId)
            {
                hit = true;
                UnitFunctions enemyUnitFunctions = collision.gameObject.GetComponent<UnitFunctions>();
                SendServerAttack2 ssa = new SendServerAttack2();
                int physicalDamage = enemyUnitFunctions.HitPhysicalDamage(AttackDamage + (AbilityPower * Count));
                ssa.damage = physicalDamage;
                ssa.CreateId = enemyStats.CreateId;
                ssa.id = nc.myId;
                ssa.roomId = Room.roomId;
                ssa.UnitId = 10;
                nc.Emit("attack", new JSONObject(JsonUtility.ToJson(ssa)));
                Destroy(gameObject);
            }
        }
        if (collision.gameObject.tag == "unit")
        {
            if (enemy == 1)
            {
                Destroy(gameObject);
            }
        }
        if (collision.gameObject.tag == "Finish")
        {
            if (enemy == 0 && AttackBarracks)
            {
                SendServerAttack2 ssa = new SendServerAttack2();
                ssa.damage = AttackDamage;
                ssa.CreateId = CreateId;
                ssa.UnitId = 10;
                ssa.id = nc.myId;
                ssa.roomId = Room.roomId;
                nc.Emit("attackBase", new JSONObject(JsonUtility.ToJson(ssa)));
            }
        }
        
    }
}
