using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitStatusStruct;
using EnumStruct;

public class UnitStatus : MonoBehaviour
{
    public UnitType UnitType;
    public Status status;
    public UnitState curState = UnitState.Standing;
    public Animator animator;

    bool isAttack = false; // 실제 공격중

    void Start()
    {
        animator = GetComponent<Animator>();
        ActionStanding();
    }

    void Update()
    {
        //Move();        
        //FindNewEnemy();
        animator.SetInteger("unitState", (int)curState);
        //SelectAction();
    }

    private void FixedUpdate()
    {
        SelectAction();
    }

    public void Initialize(UnitType type, OutPostRow row, GameObject curTarget, GameObject finalTarget)
    {
        status = new Status(UnitType);

        UnitType = type;
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
        if(this.status.curHp < 0)
        {
            ChangeCurState(UnitState.Dying);
        }

        switch (curState)
        {
            case UnitState.Standing:
                ActionStanding();
                break;
            case UnitState.Running:
                ActionMove();
                break;
            case UnitState.Attack:
                ActionAttack();
                break;
            case UnitState.Dying:
                ActionDying();
                break;
        }
    }

    public void ChangeCurState(UnitState state)
    {
        curState = state;
        //animator.SetInteger("unitState", (int)state);
    }

    public void ActionStanding()
    {
        // 목표가 있다면 이동 -> 이동에서 공격
        if(status.curTarget != null)
        {
            ChangeCurState(UnitState.Running);
            return;
        }
    }

    public void ActionMove()
    {
        // 타겟과 거리 측정 후 사정거리 안에 들어오는 경우에 공격으로 전환
        // 아니라면 이동 실행
        GameObject curTarget = this.status.curTarget;

        if (curTarget != null)
        {
            float distance = Vector2.Distance(curTarget.transform.position, transform.position);

            // 테스트
            //RaycastHit hit;
            //Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out hit, 1f);
            //Debug.DrawRay(gameObject.transform.position, gameObject.transform.forward * 15f, Color.red);

            // 거리가 사거리보다 작다면 공격으로 전환
            if (distance < status.attackRange)
            {
                ChangeCurState(UnitState.Attack);
                return;
            }

            // 타겟과 현재 위치 사이의 방향을 계산
            Vector3 direction = (curTarget.transform.position - transform.position).normalized;

            // 새로운 위치 계산
            Vector3 newPosition = transform.position + direction * this.status.curSpeed * Time.deltaTime;

            // 캐릭터를 새로운 위치로 이동
            transform.position = newPosition;

            // 캐릭터가 타겟을 바라보게 회전 (선택사항)
            transform.LookAt(curTarget.transform);
        }

        if (this.status.curTarget == null)
        {
            FindNewEnemy();
            ChangeCurState(UnitState.Standing);
        }
    }

    public void ActionAttack()
    {
        // 타겟이 사망하거나 자신이 죽은 경우 바뀌어야함
        isAttack = true;

        // 전사는 무기에 레이케스트 붙여서 거기서 처리
        // 궁수면 화살날리는 코드 여기에 작성 -> 화살 소환
        // 법사도 화염구 날리는 코드 여기에 작성 -> 불덩이 소환
        // 공격 빈도 계산

        // 타겟 사망시
        if(this.status.curTarget == null)
        {
            isAttack = false;
            FindNewEnemy();
            ChangeCurState(UnitState.Standing);
        }
    }

    public void ActionDying()
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
                // 데미지는 다른 스크립트
                break;
            case "Wizard":
                // 불덩이 소환
                // 데미지는 다른 스크립트, 불덩이는 파티클 만들어보기
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
        // 죽는 애니메이션 끝에 호출
        // 오브젝트 삭제

    }

    public void FindNewEnemy()
    {
        if(status.curTarget == null)
        {
            // 해당열 적을 우선 타겟
            // 없다면 가장 가까운 적 타겟
        }
    }

    private void OnTriggerEnter(Collider unit)
    {
        // 적과 충돌시 우선 타겟을 입력
        // 무기 충돌은 다른 스크립트로 해결 -> 여긴 충돌하면 타겟을 변경하는 것만
        // 적 충돌시 공격 중이라면 실행 안됨
        GameObject checkObject = unit.gameObject;
        string tag = unit.tag;

        if(checkObject.layer.Equals("Enemy") && isAttack)
        {
            status.curTarget = checkObject;
        }
    }
}
