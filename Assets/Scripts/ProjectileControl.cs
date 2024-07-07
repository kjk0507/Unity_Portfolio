using EnumStruct;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileControl : MonoBehaviour
{
    public GameObject curTarget;
    public int curDamage;
    public List<GameObject> targetList;
    public DamageType curDamageType;
    public PlayerDefine curDefine;
    public float curSpeed;
    public Rigidbody ri;

    bool isDamage = false;
    void Start()
    {
        ri = GetComponent<Rigidbody>();
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        Shoot();
        DamageUnit();
    }


    public void Initialize(GameObject target, DamageType damageType, PlayerDefine playerDefine, float attackSpeed, int damage)
    {
        curTarget = target;
        curDamage = damage;
        curDamageType = damageType;
        curDefine = playerDefine;
        curSpeed = attackSpeed;
    }

    public void Shoot()
    {
        ri.velocity = transform.right * curSpeed;
    }

    public void DamageUnit()
    {
        if (!isDamage && targetList != null)
        {
            isDamage = true;

            List<GameObject> list = new List<GameObject>();
            foreach (GameObject obj in targetList)
            {
                list.Add(obj);
            }

            if(list != null)
            {
                if(curDefine == PlayerDefine.Player)
                {
                    foreach (GameObject unit in list)
                    {
                        unit.GetComponent<EnemyStatus>().status.Damage(curDamage);
                        targetList.Remove(unit);
                    }
                }
                else if(curDefine == PlayerDefine.Enemy)
                {
                    foreach (GameObject unit in list)
                    {
                        unit.GetComponent<UnitStatus>().status.Damage(curDamage);
                        targetList.Remove(unit);
                    }
                }
            }

            isDamage = false;
        }
    }

    private void OnTriggerEnter(Collider unit)
    {
        GameObject checkObject = unit.gameObject;

        // 단일 공격일시
        if(curDamageType == DamageType.Target)
        {
            if(curTarget == checkObject)
            {
                // 플레이어의 공격이면 몬스터에게 데미지
                if (curDefine == PlayerDefine.Player)
                {
                    checkObject.GetComponent<EnemyStatus>().status.Damage(curDamage);
                    Destroy(gameObject);
                }
                else
                {
                    checkObject.GetComponent<UnitStatus>().status.Damage(curDamage);
                    Destroy(gameObject);
                }
            }
        }
        // 광역 공격일시
        else if (curDamageType == DamageType.AOE)
        {
            if (curDefine == PlayerDefine.Player)
            {
                if (checkObject.layer.Equals("Enemy"))
                {
                    targetList.Add(checkObject);
                }
            }
            else
            {
                if (curTarget.CompareTag("FriendlyUnit"))
                {
                    targetList.Add(checkObject);
                }
            }

        }

        //if (!checkObject.layer.Equals("FriendlyUnit"))
        //{
        //    Destroy(gameObject);
        //}
    }
}
