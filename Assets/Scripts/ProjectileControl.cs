using EnumStruct;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileControl : MonoBehaviour
{
    public GameObject curTarget;
    public int curDamage;

    void Start()
    {
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        
    }


    public void Initialize(GameObject target, int damage)
    {
        curTarget = target;
        curDamage = damage;
    }


    private void OnTriggerEnter(Collider unit)
    {
        GameObject checkObject = unit.gameObject;

        if(checkObject == curTarget)
        {
            checkObject.GetComponent<UnitStatus>().status.Damage(curDamage);
        }

        if (!checkObject.layer.Equals("FriendlyUnit"))
        {
            Destroy(gameObject);
        }
    }
}
