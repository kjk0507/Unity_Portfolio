using UnityEngine;
using EnumStruct;

public class UnitStatus : InheriteStatus
{
    private void Start()
    {
        playerDefine = PlayerDefine.Player;
        animator = GetComponent<Animator>();
        enemyLayerMask = LayerMask.NameToLayer("Enemy");
        findLayerMask = LayerMask.GetMask("Enemy");
    }

    private void Update()
    {
        Target = this.status.curTarget;
        CheckCurHp();
        //TestDebugRay();
        CheckTargetDeath();
        //SelectAction();
    }

    private void FixedUpdate()
    {
        SelectAction();
    }

    public void Attacking()
    {
        UnitType unitType = this.unitType;
        string prefabPath;
        GameObject prefab;
        Quaternion quaternion = Quaternion.identity;
        Vector3 direction;
        Quaternion rotation;

        switch (unitType)
        {
            case UnitType.Warrior:
                DamageToEnemy();
                break;
            case UnitType.Archer:
                transform.LookAt(this.status.curTarget.transform);

                prefabPath = "Prefabs/Skill/Arrow";
                prefab = Resources.Load<GameObject>(prefabPath);

                quaternion = Quaternion.identity;
                direction = firePosition.forward;
                rotation = Quaternion.LookRotation(direction);

                GameObject arrow = Instantiate(prefab, firePosition.position, quaternion);
                arrow.transform.rotation = rotation;
                arrow.GetComponent<ProjectileControl>().Initialize(this.status.curTarget, DamageType.Target, PlayerDefine.Player, this.status.attackSpeed, this.status.finalAtk);
                break;
            case UnitType.Wizard:
                transform.LookAt(this.status.curTarget.transform);

                prefabPath = "Prefabs/Skill/Fireball";
                prefab = Resources.Load<GameObject>(prefabPath);

                quaternion = Quaternion.identity;
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
