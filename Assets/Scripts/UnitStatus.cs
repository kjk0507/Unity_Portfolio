using UnityEngine;
using EnumStruct;

public class UnitStatus : InheriteStatus
{
    private void Start()
    {
        playerDefine = PlayerDefine.Player;
        if(unitType == UnitType.Castle)
        {
            Initialize(UnitType.Castle, LineType.Middle, SpawnType.Initial, null, null, null);
        }

        if (this.status.moveType == MoveType.Move)
        {
            animator = GetComponent<Animator>();
        }
        outPostLayerMask = LayerMask.NameToLayer("OutPost");
        enemyLayerMask = LayerMask.NameToLayer("Enemy");
        findLayerMask = LayerMask.GetMask("Enemy");
        ownLayerMask = LayerMask.NameToLayer("Player");

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
        //CheckComingEnemy();
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

        if(this.status.curTarget != null)
        {
            transform.LookAt(this.status.curTarget.transform);
        }

        switch (unitType)
        {
            case UnitType.Warrior:
                DamageToEnemy();
                break;
            case UnitType.Archer:
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
