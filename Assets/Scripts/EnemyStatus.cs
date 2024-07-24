using UnityEngine;
using EnumStruct;
using System.Collections.Generic;

public class EnemyStatus : InheriteStatus
{
    private void Start()
    {
        playerDefine = PlayerDefine.Enemy;
        if (unitType == UnitType.Castle)
        {
            Initialize(UnitType.Castle, LineType.Middle, SpawnType.Initial, null, null, null);
        }

        if (this.status.moveType == MoveType.Move)
        {
            animator = GetComponent<Animator>();
        }
        outPostLayerMask = LayerMask.NameToLayer("OutPost");
        enemyLayerMask = LayerMask.NameToLayer("Player");
        findLayerMask = LayerMask.GetMask("Player");
        ownLayerMask = LayerMask.NameToLayer("Enemy");

        isPointMove = true;

        randomPositionRange = new Vector3(0f, 0f, 2f);
        randomPosition = new Vector3(0, 0, Random.Range(-randomPositionRange.z, randomPositionRange.z));
        //SelectAction();
    }

    private void Update()
    {
        Target = this.status.curTarget;
        CheckCurHp();
        //TestDebugRay();
        //SelectAction();
    }

    private void FixedUpdate()
    {
        SelectAction();
    }

    public void Attacking()
    {
        //string tag = this.gameObject.tag;
        UnitType unitType = this.unitType;
        string prefabPath;
        GameObject prefab;
        Quaternion quaternion = Quaternion.identity;
        Vector3 direction;
        Quaternion rotation;

        switch (unitType)
        {
            case UnitType.Orc:
                // 몬스터의 체력을 깍는 함수 실행
                DamageToEnemy();
                break;
            case UnitType.BoneArcher:
                // 화살 소환
                // 발사 및 데미지는 다른 스크립트
                // 몹 이동시 방향 이동
                transform.LookAt(this.status.curTarget.transform);

                prefabPath = "Prefabs/Skill/Arrow";
                prefab = Resources.Load<GameObject>(prefabPath);

                quaternion = Quaternion.identity;
                direction = firePosition.forward;
                rotation = Quaternion.LookRotation(direction);

                GameObject arrow = Instantiate(prefab, firePosition.position, quaternion);
                arrow.transform.rotation = rotation;
                arrow.GetComponent<ProjectileControl>().Initialize(this.status.curTarget, DamageType.Target, PlayerDefine.Enemy, this.status.attackSpeed, this.status.finalAtk);
                break;
            case UnitType.Destroyer:
                List<GameObject> DamageList = new List<GameObject>();

                Collider[] hitColliders = Physics.OverlapSphere(firePosition.position, 20f, findLayerMask);                

                foreach (Collider unitCollider in hitColliders)
                {
                    DamageList.Add(unitCollider.gameObject);
                }

                foreach (GameObject unit in DamageList)
                {
                    unit.GetComponent<InheriteStatus>().status.Damage(this.status.finalAtk);
                }
                break;
            default:
                return;
        }
    }
}
