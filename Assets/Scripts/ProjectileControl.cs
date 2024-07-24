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

    public int encount;

    int playerLayerMask;
    int enemyLayerMask;

    bool isDamage = false;

    void Start()
    {
        ri = GetComponent<Rigidbody>();

        playerLayerMask = LayerMask.NameToLayer("Player");
        enemyLayerMask = LayerMask.NameToLayer("Enemy");

        Destroy(gameObject, 3f);
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

        if(playerDefine == PlayerDefine.Player) 
        {
            this.gameObject.layer = playerLayerMask;
        }
        else
        {
            this.gameObject.layer = enemyLayerMask;
        }
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
                encount++;
                
                if(encount >= 3)
                {
                    break;
                }
            }

            if(list != null)
            {
                foreach (GameObject unit in list)
                {
                    unit.GetComponent<InheriteStatus>().status.Damage(curDamage);
                    targetList.Remove(unit);
                }
            }

            isDamage = false;

            if(encount >= 3)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider unit)
    {
        GameObject checkObject = unit.gameObject;
        int enemyLayerMask = LayerMask.NameToLayer("Enemy");
        int playerUnitLayerMask = LayerMask.NameToLayer("Player");

        // 단일 공격일시
        if (curDamageType == DamageType.Target)
        {
            if(curTarget == checkObject)
            {
                checkObject.GetComponent<InheriteStatus>().status.Damage(curDamage);
                Destroy(gameObject);
            }
        }
        // 광역 공격일시
        else if (curDamageType == DamageType.AOE)
        {
            if (curDefine == PlayerDefine.Player)
            {
                if (checkObject.layer == enemyLayerMask)
                {
                    targetList.Add(checkObject);
                }
            }
            else
            {
                if (checkObject.layer == playerUnitLayerMask)
                {
                    targetList.Add(checkObject);
                }
            }
        }
    }
}
