using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitStatusStruct;
using EnumStruct;
using UnityEngine.UI;


public class UnitStatus : MonoBehaviour
{
    UnitType unitType;
    public Status status;
    public UnitState curState;
    public Animator animator;
    public Transform firePosition;
    public GameObject hpBar;
    Image hpBarImage;
    public GameObject Target;

    bool isAttack = false; // 실제 공격중
    bool isEnemyDeath = false; // 타겟의 생존 여부 false : 생존

    void Start()
    {
        animator = GetComponent<Animator>();
        hpBarImage = hpBar.GetComponent<Image>();

        //ActionStanding();
        //InvokeRepeating("hpCurse", 1.0f, 1.0f);
    }

    void hpCurse()
    {
        this.status.curHp = this.status.curHp - 10;
    }

    void Update()
    {
        //animator.SetInteger("unitState", (int)curState);
        CheckCurHp();
        Target = this.status.curTarget;
        //TestDebugRay();
        CheckEnemyDeath();
    }

    private void FixedUpdate()
    {
        SelectAction();
    }

    public void Initialize(UnitType type, OutPostRow row, GameObject curTarget, GameObject finalTarget)
    {
        status = new Status(type);

        unitType = type;
        this.status.curRow = row;
        this.status.curTarget = curTarget;
        this.status.finalTarget = finalTarget;
    }

    public void ChangeCurTarget(GameObject curTarget)
    {
        this.status.curTarget = curTarget;
    }

    public void SelectAction()
    {
        // todo 대기(0), 이동(1), 공격(2), 사망(3) -> 행동 코드도 따라가야됨 : 애니메이션코드와 상태변경 코드가 같이 실행 될것!
        if(this.status.curHp <= 0)
        {
            ChangeCurState(UnitState.Death);
            return;
        }

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

        animator.SetInteger("unitState", (int)curState);
    }

    public void ChangeCurState(UnitState state)
    {
        curState = state;
        //animator.SetInteger("unitState", (int)state);
    }

    public void ActionIdle()
    {
        // 목표가 있다면 이동 -> 이동에서 공격
        if(this.status.curTarget != null)
        {
            ChangeCurState(UnitState.Move);
            return;
        }

        //if (this.status.curTarget == null)
        //{
        //    FindNewEnemy();
        //    ChangeCurState(UnitState.Running);
        //    //SelectAction();
        //    return;
        //}
    }

    public void ActionMove()
    {
        // 타겟과 거리 측정 후 사정거리 안에 들어오는 경우에 공격으로 전환
        // 아니라면 이동 실행
        GameObject curTarget = this.status.curTarget;

        if (this.status.curTarget != null)
        {
            float distance = Vector3.Distance(curTarget.transform.position, transform.position);

            if(curTarget.tag == "Outpost")
            {
                // 초기 전초기지에 안들어가는 현상 수정
                distance = 100f;
            }

            // 테스트
            //RaycastHit hit;
            //Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out hit, 1f);
            //Debug.DrawRay(gameObject.transform.position, gameObject.transform.forward * this.status.attackRange, Color.red);

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

        //if (this.status.curTarget == null)
        //{
        //    FindNewEnemy();
        //    ChangeCurState(UnitState.Idle);
        //}
    }

    public void ActionAttack()
    {
        // 타겟이 사망하거나 자신이 죽은 경우 바뀌어야함
        isAttack = true;

        // 타겟이 이동했는데 거리가 멀어졌을시
        if(this.status.curTarget != null)
        {
            float distance = Vector3.Distance(this.status.curTarget.transform.position, transform.position);
            
            if(distance > this.status.attackRange)
            {
                isAttack = false;
                ChangeCurState(UnitState.Idle);
                return;
            }
        }
    }

    public void ActionDeath()
    {
        // 유닛의 콜리더 삭제, 회전 고정, 위치 고정
        Collider collider = GetComponent<Collider>();
        collider.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.constraints = RigidbodyConstraints.FreezePosition;
        // 애니메이션 끝난 후 삭제 호출 -> 다른 함수
    }

    public void Attacking()
    {
        // 공격 애니메이션 도중 호출
        // isAttack 이 참인 경우 실행가능
        // 해당 타겟이 사망하는 경우, 유닛이 사망하는 경우에만 공격이 중지
        string tag = this.gameObject.tag;

        switch (tag)
        {
            case "Warrior":
                // 몬스터의 체력을 깍는 함수 실행
                DamageToEnemy();
                break;
            case "Archer":
                // 화살 소환
                // 발사 및 데미지는 다른 스크립트
                // 몹 이동시 방향 이동
                transform.LookAt(this.status.curTarget.transform);

                string prefabPath = "Prefabs/Weapon/Arrow/Arrow";
                GameObject prefab = Resources.Load<GameObject>(prefabPath);

                Quaternion quaternion = Quaternion.identity;

                //Vector3 direction = (this.status.curTarget.transform.position - firePosition.position).normalized;
                Vector3 direction = firePosition.forward;
                Quaternion rotation = Quaternion.LookRotation(direction);

                GameObject arrow = Instantiate(prefab, firePosition.position, quaternion);
                arrow.transform.rotation = rotation;
                arrow.GetComponent<ProjectileControl>().Initialize(this.status.curTarget, DamageType.Target, PlayerDefine.Player, this.status.attackSpeed, this.status.finalAtk);
                break;
            case "Wizard":
                // 불덩이 소환
                // 데미지는 다른 스크립트, 불덩이는 파티클 만들어보기
                break;
            default:
                return;
        }
    }

    public void TestDebugRay()
    {
        Vector3 direction = (this.status.curTarget.transform.position - firePosition.position);
        Quaternion quaternion = Quaternion.LookRotation(direction);

        RaycastHit hit;
        Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out hit, 1f);
        Debug.DrawRay(firePosition.position, firePosition.right * this.status.attackRange, Color.red);
    }

    public void DamageToEnemy()
    {
        status.curTarget.GetComponent<EnemyStatus>().status.Damage(this.status.finalAtk);
    }

    public void Dying()
    {
        // 죽는 애니메이션 끝에 호출
        // 오브젝트 삭제
        string tag = this.gameObject.tag;
        switch (tag)
        {
            case "Warrior":
                UnitManager.um_instance.warriorList.Remove(gameObject);
                break;
            case "Archer":
                UnitManager.um_instance.archerList.Remove(gameObject);
                break;
            case "Wizard":
                UnitManager.um_instance.wizardList.Remove(gameObject);
                break;
        }
        
        Destroy(gameObject);
    }

    public void FindNewEnemy()
    {
        if(this.status.curTarget == null)
        {
            // 해당열 적을 우선 타겟
            // 없다면 가장 가까운 적 타겟            
            OutPostRow row = this.status.curRow;
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
            float distance = Vector3.Distance(transform.position, unit.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                newTarget = unit.gameObject;
            }

            checkUnitCount++;
        }

        if(checkUnitCount == 0)
        {
            List<GameObject> FoundObjects;
            FoundObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));


            foreach (GameObject obj in FoundObjects)
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

    public void CheckEnemyDeath()
    {
        if (this.status.curTarget == null)
        {
            isEnemyDeath = true;
        }

        // 타겟 사망시
        if (isEnemyDeath)
        {
            isEnemyDeath = false;
            isAttack = false;
            FindNewEnemy();
            ChangeCurState(UnitState.Idle);
            return;
        }
    }

    private void OnTriggerEnter(Collider unit)
    {
        // 적과 충돌시 우선 타겟을 입력
        // 무기 충돌은 다른 스크립트로 해결 -> 여긴 충돌하면 타겟을 변경하는 것만
        // 적 충돌시 공격 중이라면 실행 안됨
        GameObject checkObject = unit.gameObject;
        string tag = unit.tag;

        //if(checkObject.layer.Equals("Enemy") && isAttack)
        //{
        //    status.curTarget = checkObject;
        //}

        if (checkObject.CompareTag("Enemy") && isAttack)
        {
            this.status.curTarget = checkObject;
        }
    }
}
