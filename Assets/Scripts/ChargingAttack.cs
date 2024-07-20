using UnityEngine;
using System.Collections.Generic;
using EnumStruct;
using UnitStatusStruct;
using System.Collections;

public class ChargingAttack : MonoBehaviour
{
    public GameObject parentUnitScript;
    public Transform firePosition;
    public Status status;
    public bool isFire = false;
    public GameObject chargingEffect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.status = parentUnitScript.GetComponent<InheriteStatus>().status;
        Attacking();
    }

    public void Attacking()
    {
        if(!isFire && this.status.curTarget != null)
        {
            float distance = Vector3.Distance(this.transform.position, this.status.curTarget.transform.position);

            if(distance > this.status.attackRange)
            {
                return;
            }

            isFire = true;
            chargingEffect.SetActive(false);

            string prefabPath;
            GameObject prefab;
            Quaternion quaternion = Quaternion.identity;
            Vector3 direction;
            Quaternion rotation;
            Transform target = this.status.curTarget.transform;

            transform.LookAt(target);

            prefabPath = "Prefabs/Skill/Ball";
            prefab = Resources.Load<GameObject>(prefabPath);

            quaternion = Quaternion.identity;
            //direction = -transform.right;
            //direction = -transform.right;
            direction = (target.position - firePosition.position).normalized;
            //direction = firePosition.up;
            rotation = Quaternion.LookRotation(direction);
            Vector3 eulerRotation = rotation.eulerAngles;
            eulerRotation.y -= 90f;
            rotation = Quaternion.Euler(eulerRotation);

            GameObject ball = Instantiate(prefab, firePosition.position, rotation);
            ball.transform.rotation = rotation;
            ball.GetComponent<ProjectileControl>().Initialize(this.status.curTarget, DamageType.Target, PlayerDefine.Enemy, this.status.attackSpeed, this.status.finalAtk);

            StartCoroutine(ChangeFireState());
        }
    }

    IEnumerator ChangeFireState()
    {
        chargingEffect.SetActive(true);
        yield return new WaitForSeconds(2f);
        isFire = false;
    }
}
