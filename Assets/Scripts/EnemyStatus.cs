using EnumStruct;
using UnityEngine;
using UnitStatusStruct;
using UnityEngine.UI;

public class EnemyStatus : InheriteStatus
{
    private void Start()
    {
        animator = GetComponent<Animator>();
        enemyLayerMask = LayerMask.NameToLayer("Player");
    }

    private void Update()
    {
        Target = this.status.curTarget;
        CheckCurHp();
        TestDebugRay();
        CheckTargetDeath();
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
                // 불덩이 소환
                // 데미지는 다른 스크립트, 불덩이는 파티클 만들어보기
                transform.LookAt(this.status.curTarget.transform);

                prefabPath = "Prefabs/Skill/Fireball";
                prefab = Resources.Load<GameObject>(prefabPath);

                quaternion = Quaternion.identity;

                //Vector3 direction = (this.status.curTarget.transform.position - firePosition.position).normalized;
                direction = firePosition.forward;
                rotation = Quaternion.LookRotation(direction);

                GameObject fireball = Instantiate(prefab, firePosition.position, quaternion);
                fireball.transform.rotation = rotation;
                fireball.GetComponent<ProjectileControl>().Initialize(this.status.curTarget, DamageType.AOE, PlayerDefine.Player, this.status.attackSpeed, this.status.finalAtk);
                break;
            default:
                return;
        }
    }
}
