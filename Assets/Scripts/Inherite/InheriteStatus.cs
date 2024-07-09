using EnumStruct;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitStatusStruct;
using UnityEngine.UI;
using Unity.VisualScripting;

public class InheriteStatus : MonoBehaviour
{
    public UnitType unitType;
    public Status status;
    public UnitState curState;
    protected Animator animator;
    //public GameObject hpBar;
    public GameObject Target;
    public Transform firePosition;
    public Image hpBarImage;

    public bool isAttack = false; // 실제 공격중
    bool isTargetDeath = false; // 타겟의 생존 여부 false : 생존
    public bool isAttackMotion = false; // true : 모션 실행중 / false : 모션 실행 안하는중

    public int enemyLayerMask;

    void Start()
    {
    }

    void hpCurse()
    {
        this.status.curHp = this.status.curHp - 10;
    }

    void Update()
    {

    }

    private void FixedUpdate()
    {

    }

    public void Initialize(UnitType type, LineType row, GameObject curTarget, GameObject finalTarget)
    {
        status = new Status(type);

        unitType = type;
        this.status.curRow = row;
        this.status.curTarget = curTarget;
        this.status.finalTarget = finalTarget;
    }

    public void SelectAction()
    {
        // todo 대기(0), 이동(1), 공격(2), 사망(3) -> 행동 코드도 따라가야됨 : 애니메이션코드와 상태변경 코드가 같이 실행 될것!
        if (this.status.curHp <= 0)
        {
            ChangeCurState(UnitState.Death);
        }

        //animator.SetInteger("unitState", (int)curState);
        //animator.SetBool("isAttackMotion", isAttackMotion);

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
        animator.SetInteger("unitState", (int)curState);
        animator.SetBool("isAttackMotion", isAttackMotion);
    }
    public void ChangeCurTarget(GameObject curTarget)
    {
        this.status.curTarget = curTarget;
    }

    public void ActionIdle()
    {
        // 목표가 있다면 이동 -> 이동에서 공격
        if (this.status.curTarget != null)
        {
            ChangeCurState(UnitState.Move);
            return;
        }
    }

    public void ActionMove()
    {
        // 타겟과 거리 측정 후 사정거리 안에 들어오는 경우에 공격으로 전환
        // 아니라면 이동 실행
        GameObject curTarget = this.status.curTarget;

        if (this.status.curTarget != null)
        {
            float distance = Vector3.Distance(curTarget.transform.position, transform.position);

            if (curTarget.tag == "Outpost")
            {
                // 초기 전초기지에 안들어가는 현상 수정
                distance = 100f;
            }

            Vector3 direction = (curTarget.transform.position - transform.position).normalized;
            Vector3 newPosition = transform.position + direction * this.status.curSpeed * Time.deltaTime;
            transform.LookAt(curTarget.transform);

            // 거리가 사거리보다 작다면 공격으로 전환
            if (distance < this.status.attackRange)
            {
                ChangeCurState(UnitState.Attack);
                return;
            }

            // isAttackMotion이 false여야 이동;
            if(isAttackMotion)
            {
                isAttackMotion = false;
                animator.SetBool("isAttackMotion", false);
                ChangeCurState(UnitState.Idle);
                return;
            }

            // 거리가 사거리보다 크다면 이동
            transform.position = newPosition;
            return;
        }
    }

    public void ActionAttack()
    {
        // 타겟이 사망하거나 자신이 죽은 경우 바뀌어야함
        isAttack = true;
        isAttackMotion = true;

        // 타겟이 이동했는데 거리가 멀어졌을시
        if (this.status.curTarget != null || this.status.curTarget.GetComponent<Collider>() == null)
        {
            float distance = Vector3.Distance(this.status.curTarget.transform.position, transform.position);

            if (distance > this.status.attackRange)
            {
                ChangeCurState(UnitState.Idle);
                return;
            }
        }
    }

    public void ActionDeath()
    {
        Collider collider = GetComponent<Collider>();
        Destroy(collider);

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.constraints = RigidbodyConstraints.FreezePosition;
    }

    public void TestDebugRay()
    {
        //Vector3 direction = (this.status.curTarget.transform.position - firePosition.position);
        //Quaternion quaternion = Quaternion.LookRotation(direction);

        //RaycastHit hit;
        //Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out hit, 1f);
        Debug.DrawRay(transform.position, transform.forward * this.status.attackRange, Color.red);
    }

    public void Dying()
    {
        // 죽는 애니메이션 끝에 호출
        // 오브젝트 삭제
        //string tag = this.gameObject.tag;
        UnitType unitType = this.unitType;

        switch (unitType)
        {
            case UnitType.Warrior:
                UnitManager.um_instance.warriorList.Remove(gameObject);
                break;
            case UnitType.Archer:
                UnitManager.um_instance.archerList.Remove(gameObject);
                break;
            case UnitType.Wizard:
                UnitManager.um_instance.wizardList.Remove(gameObject);
                break;
            case UnitType.Orc:
                UnitManager.um_instance.orcList.Remove(gameObject);
                break;
            case UnitType.BoneArcher:
                UnitManager.um_instance.boneArcherList.Remove(gameObject);
                break;
            case UnitType.Destroyer:
                UnitManager.um_instance.destroyerList.Remove(gameObject);
                break;
        }

        Destroy(gameObject);
    }

    public void EndAttackMotion()
    {
        isAttackMotion = false;
    }

    public void FindNewEnemy()
    {
        // 해당열 적을 우선 타겟
        // 없다면 가장 가까운 적 타겟

        LineType row = this.status.curRow;
        Transform line = null;
        line = UnitManager.um_instance.FindLine(row);
        this.status.curTarget = CheckLineEnemy(line);

        ChangeCurState(UnitState.Idle);
    }

    public GameObject CheckLineEnemy(Transform line)
    {
        GameObject newTarget = null;
        float closestDistance = Mathf.Infinity;
        int checkUnitCount = 0;

        foreach (Transform unit in line)
        {
            if (unit.GetComponent<Collider>() == null)
            {
                continue;
            }

            //int enemyLayerMask = LayerMask.NameToLayer("Player");
            if (unit.gameObject.layer != enemyLayerMask)
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
            // 태그를 통한 적 탐지 방법
            //List<GameObject> FoundObjects;
            //FoundObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));


            //foreach (GameObject obj in FoundObjects)
            //{
            //    float distance = Vector3.Distance(transform.position, obj.transform.position);
            //    if (distance < closestDistance)
            //    {
            //        closestDistance = distance;
            //        newTarget = obj.transform.gameObject;
            //    }
            //}

            // 레이어를 통한 적 탐지 방법
            float searchRadius = 50f;
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
            //isAttackMotion = true; // 모션은 무조건 false에만 나오는것이 가능
            FindNewEnemy();
        }
    }

    public void DamageToEnemy()
    {
        status.curTarget.GetComponent<InheriteStatus>().status.Damage(this.status.finalAtk);
    }

    public void CheckCurHp()
    {
        hpBarImage.fillAmount = (float)this.status.curHp / this.status.finalHp;
    }

    private void OnCollisionEnter(Collision unit)
    {
        GameObject checkObject = unit.gameObject;

        if (checkObject.layer == enemyLayerMask && !isAttack)
        {
            this.status.curTarget = checkObject;
        }
    }
}
