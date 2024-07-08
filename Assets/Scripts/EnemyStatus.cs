using EnumStruct;
using UnityEngine;
using UnitStatusStruct;
using UnityEngine.UI;

public class EnemyStatus : MonoBehaviour
{
    EnemyUnitType enemyUnitType;
    public Status status;
    public UnitState curState;
    public Animator animator;
    public Transform firePosition;
    public GameObject hpBar;
    Image hpBarImage;
    //public GameObject Target;

    bool isAttack = false; // 실제 공격중
    bool isTargetDeath = false; // 타겟의 생존 여부 false : 생존

    private void Start()
    {
        //status = new Status(EnemyUnitType.Orc);
        animator = GetComponent<Animator>();
        hpBarImage = hpBar.GetComponent<Image>();
        //InvokeRepeating("hpCurse", 1.0f, 1.0f);
    }

    private void Update()
    {
        CheckCurHp();
        CheckTargetDeath();
        //Target = this.status.curTarget;        
    }

    private void FixedUpdate()
    {
        SelectAction();
    }

    void hpCurse()
    {
        this.status.curHp = this.status.curHp - 10;
    }

    public void Initialize(EnemyUnitType type, LineType row, GameObject curTarget, GameObject finalTarget)
    {
        status = new Status(type);

        enemyUnitType = type;
        this.status.curRow = row;
        this.status.curTarget = curTarget;
        this.status.finalTarget = finalTarget;
    }

    public void SelectAction()
    {
        if (this.status.curHp <= 0)
        {
            ChangeCurState(UnitState.Death);
        }

        animator.SetInteger("unitState", (int)curState);

        switch (curState)
        {
            case UnitState.Idle:
                ActionIdle();
                break;
            case UnitState.Move:
                ActionMove();
                break;
            case UnitState.Attack:
                ActionAttack();
                break;
            case UnitState.Death:
                ActionDeath();
                break;
        }

    }

    public void ChangeCurState(UnitState state)
    {
        curState = state;
    }

    public void ChangeCurTarget(GameObject curTarget)
    {
        this.status.curTarget = curTarget;
    }

    public void ActionIdle()
    {
        if (this.status.curTarget != null)
        {
            ChangeCurState(UnitState.Move);
            return;
        }
    }

    public void ActionMove()
    {
        GameObject curTarget = this.status.curTarget;

        if (this.status.curTarget != null)
        {
            float distance = Vector3.Distance(curTarget.transform.position, transform.position);

            Vector3 direction = (curTarget.transform.position - transform.position).normalized;
            Vector3 newPosition = transform.position + direction * this.status.curSpeed * Time.deltaTime;
            transform.LookAt(curTarget.transform);

            // 거리가 사거리보다 작다면 공격으로 전환
            if (distance < this.status.attackRange)
            {
                ChangeCurState(UnitState.Attack);
                return;
            }
            // 거리가 사거리보다 크다면 이동
            transform.position = newPosition;
            return;
        }
    }

    public void ActionAttack()
    {
        isAttack = true;

        // 타겟이 이동했는데 거리가 멀어졌을시
        if (this.status.curTarget != null)
        {
            float distance = Vector3.Distance(this.status.curTarget.transform.position, transform.position);

            if (distance > this.status.attackRange)
            {
                isAttack = false;
                ChangeCurState(UnitState.Idle);
                return;
            }
        }
    }

    public void ActionDeath()
    {
        //Destroy(gameObject);
        Collider collider = GetComponent<Collider>();
        //collider.enabled = false;
        Destroy(collider);

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.constraints = RigidbodyConstraints.FreezePosition;
    }

    public void Attacking()
    {
        //string tag = this.gameObject.tag;
        EnemyUnitType unitType = this.enemyUnitType;
        string prefabPath;
        GameObject prefab;
        Quaternion quaternion = Quaternion.identity;
        Vector3 direction;
        Quaternion rotation;

        switch (unitType)
        {
            case EnemyUnitType.Orc:
                // 몬스터의 체력을 깍는 함수 실행
                DamageToEnemy();
                break;
            case EnemyUnitType.BoneArcher:
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
                arrow.GetComponent<ProjectileControl>().Initialize(this.status.curTarget, DamageType.Target, PlayerDefine.Player, this.status.attackSpeed, this.status.finalAtk);
                break;
            case EnemyUnitType.Destroyer:
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

    public void DamageToEnemy()
    {
        status.curTarget.GetComponent<UnitStatus>().status.Damage(this.status.finalAtk);
    }

    public void Dying()
    {
        EnemyUnitType unitType = this.enemyUnitType;

        switch (unitType)
        {
            case EnemyUnitType.Orc:
                UnitManager.um_instance.orcList.Remove(gameObject);
                break;
            case EnemyUnitType.BoneArcher:
                UnitManager.um_instance.archerList.Remove(gameObject);
                break;
            case EnemyUnitType.Destroyer:
                UnitManager.um_instance.destroyerList.Remove(gameObject);
                break;
        }

        Destroy(gameObject);
    }

    public void FindNewEnemy()
    {
        if (this.status.curTarget == null || this.status.curTarget.GetComponent<Collider>() == null)
        {
            // 해당열 적을 우선 타겟
            // 없다면 가장 가까운 적 타겟            
            LineType row = this.status.curRow;
            Transform line = null;
            line = UnitManager.um_instance.FindLine(row);
            this.status.curTarget = CheckLineEnemy(line);
        }
    }

    public GameObject CheckLineEnemy(Transform line)
    {
        GameObject newTarget = null;
        float closestDistance = Mathf.Infinity;
        int checkUnitCount = 0;

        foreach (Transform unit in line)
        {
            int enemyLayerMask = LayerMask.NameToLayer("Enemy");
            if (unit.gameObject.layer == enemyLayerMask)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, unit.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                newTarget = unit.gameObject;
            }

            checkUnitCount++;
        }

        if (checkUnitCount == 0)
        {
            // 레이어를 통한 적 탐지 방법
            float searchRadius = 50f;
            int enemyLayerMask = LayerMask.GetMask("Player");
            Vector3 currentPosition = transform.position;
            Collider[] colliders = Physics.OverlapSphere(currentPosition, searchRadius, enemyLayerMask);
            //Collider closestEnemy = null;
            //float closestDistanceSqr = Mathf.Infinity;

            foreach (Collider obj in colliders)
            {
                float distance = Vector3.Distance(transform.position, obj.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    newTarget = obj.transform.gameObject;
                }
            }
        }

        return newTarget;
    }

    public void CheckCurHp()
    {
        hpBarImage.fillAmount = (float)this.status.curHp / this.status.finalHp;
    }

    public void CheckTargetDeath()
    {
        if (this.status.curTarget == null || this.status.curTarget.GetComponent<Collider>() == null)
        {
            isTargetDeath = true;
        }

        // 타겟 사망시
        if (isTargetDeath)
        {
            isTargetDeath = false;
            isAttack = false;
            FindNewEnemy();
            ChangeCurState(UnitState.Idle);
            return;
        }
    }

    private void OnTriggerEnter(Collider unit)
    {
        GameObject checkObject = unit.gameObject;

        int enemyLayerMask = LayerMask.NameToLayer("Enemy");

        if (checkObject.layer == enemyLayerMask && !isAttack)
        {
            this.status.curTarget = checkObject;
        }
    }
}
