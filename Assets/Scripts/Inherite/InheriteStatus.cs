using EnumStruct;
using UnityEngine;
using UnitStatusStruct;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class InheriteStatus : MonoBehaviour
{
    public PlayerDefine playerDefine;
    public UnitType unitType;
    public SpawnType spawnType;
    public Status status;
    public UnitState curState;
    protected Animator animator;
    //public GameObject hpBar;
    public GameObject Target;
    public Transform firePosition;
    public Image hpBarImage;
    public OutPostPoint curPoint;
    public GameObject targetOutPost;

    public bool isAttack = false; // 실제 공격중
    //bool isTargetDeath = false; // 타겟의 생존 여부 false : 생존
    public bool isAttackMotion = false; // true : 모션 실행중 / false : 모션 실행 안하는중

    public int enemyLayerMask; // 적 레이어(번호)
    public int findLayerMask; // 적 레이어(충돌)
    public int ownLayerMask; // 자기 자신의 layer
    public int outPostLayerMask;

    public bool isPointMove = false; // true : 이동 가능 / false : 이동 불가능 

    public bool isDead = false;

    public Coroutine curEffectCoroutine;
    public GameObject curEffect;

    Queue<Transform> routeQueue = new Queue<Transform>();

    // 부서지는 연출
    public GameObject defaultObj;
    public GameObject particleObj;

    // 장비 위치
    public GameObject rightHand;
    public GameObject leftHand;

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

    public void Initialize(UnitType type, LineType row, SpawnType spawn,GameObject curTarget, GameObject point, GameObject finalTarget)
    {
        status = new Status(type);

        unitType = type;
        spawnType = spawn;
        this.status.curRow = row;
        this.status.curTarget = curTarget;
        this.status.finalTarget = finalTarget;

        if(spawnType != SpawnType.Initial)
        {
            this.targetOutPost = point;
            this.curPoint = CheckOutPostPoint(point);
            SpawnManager.sm_instance.RegistRoute(routeQueue, row);
        }

    }

    public void SelectAction()
    {
        // todo 대기(0), 이동(1), 공격(2), 사망(3) -> 행동 코드도 따라가야됨 : 애니메이션코드와 상태변경 코드가 같이 실행 될것!
        // 그 상태가 아니라면 실행 안함


        // 고정형은 애니메이션을 보내지 않음
        if (this.status.moveType != MoveType.Stand)
        {
            animator.SetInteger("unitState", (int)curState);
            animator.SetBool("isAttackMotion", isAttackMotion);
        }

        // 죽었으면 더이상 실행 안함
        if (isDead)
        {
            return;
        }

        if (this.status.curHp <= 0)
        {
            isDead = true;
            ChangeCurState(UnitState.Death);
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
    }

    public void ChangeCurState(UnitState state)
    {
        curState = state;
    }

    public void ActionIdle()
    {
        // 들어오는 경우
        // move : 전초기지에 도착한 경우 -> 유닛의 초기 생성시에는 이를 무시하고 전초기지까진 가야함
        // attack : 목표를 잃은 경우
        // 타겟이 없는 상태라면 타겟을 지정
        //if (this.status.curTarget == null)
        //{
        //    FindNewEnemy();
        //}

        FindNewEnemy();

        // 그래도 없다면 point를 향해 감
        if (this.status.curTarget == null && this.curPoint != OutPostPoint.None && this.spawnType != SpawnType.Initial && isPointMove)
        {
            // 단 도달했다면 멈춤
            float distance = Vector3.Distance(this.targetOutPost.transform.position, transform.position);

            if(distance > 2)
            {
                ChangeCurState(UnitState.Move);
                return;
            }
            
            return;
        }

        // 목표가 있는 경우
        if (this.status.curTarget != null)
        {
            if((curPoint == OutPostPoint.Point_00 || curPoint == OutPostPoint.Point_10 || curPoint == OutPostPoint.Point_20
                || curPoint == OutPostPoint.Point_03 || curPoint == OutPostPoint.Point_13 || curPoint == OutPostPoint.Point_23) && this.spawnType != SpawnType.Initial && isPointMove)
            {
                ChangeCurState(UnitState.Move);
                return;
            }

            GameObject curTarget = this.status.curTarget;
            float distance = Vector3.Distance(this.status.curTarget.transform.position, transform.position);

            // 초기 생성이라면 많이 움직이지 않기
            if (playerDefine == PlayerDefine.Enemy && distance >= 35f && spawnType == SpawnType.Initial) // 35가 가장 먼 공격임
            {
                return;
            }

            // 거리가 멀다면 이동, 거리가 사거리보다 가깝다면 공격, 그냥 가깝다면 idle 유지
            if(distance > this.status.attackRange)
            {
                // point move가 true인 상태에선 이동 아니라면 상태 유지
                if(isPointMove)
                {
                    ChangeCurState(UnitState.Move);
                }
            } else
            {
                isAttack = true; // 공격 중을 의미
                isAttackMotion = true; // 모션의 종료
                ChangeCurState(UnitState.Attack);
            }
        }
    }

    public void ActionMove()
    {
        // 들어오는 경우
        // idle : 목표가 없는 경우(다음 point까지), 거리가 먼경우 타겟에게 가까워질때 까지

        // 고정형인 경우에는 바로 공격으로 전환
        if (this.status.moveType == MoveType.Stand)
        {
            ChangeCurState(UnitState.Attack);
            return;
        }

        if (!isPointMove)
        {
            ChangeCurState(UnitState.Idle); 
            return;
        }

        // 목표가 없는 경우
        if ((this.status.curTarget == null && this.curPoint != OutPostPoint.None)
            || (this.status.curTarget == null && (curPoint == OutPostPoint.Point_00 || curPoint == OutPostPoint.Point_10 || curPoint == OutPostPoint.Point_20)) 
            || (this.status.curTarget == null && (curPoint == OutPostPoint.Point_03 || curPoint == OutPostPoint.Point_13 || curPoint == OutPostPoint.Point_23)))
        {
            float distancePoint = Vector3.Distance(targetOutPost.transform.position, transform.position);
            Vector3 direction = (targetOutPost.transform.position - transform.position).normalized;
            Vector3 newPosition = transform.position + direction * this.status.curSpeed * Time.deltaTime;            

            // 포인터에 접근한 경우(플레이어)
            if (distancePoint < 2 && routeQueue.Count != 0 && playerDefine == PlayerDefine.Player)
            {
                Transform route = routeQueue.Dequeue();
                curPoint = CheckOutPostPoint(route.gameObject);
                targetOutPost = route.gameObject;
                return;
            }

            // 포인터에 접근한 경우(적군)
            if (distancePoint < 2 && routeQueue.Count == 3 && playerDefine == PlayerDefine.Enemy)
            {
                Transform route = routeQueue.Dequeue();
                curPoint = CheckOutPostPoint(route.gameObject);
                targetOutPost = route.gameObject;
                return;
            }

            // 포인터와 먼 경우
            if (distancePoint > 2)
            {
                transform.LookAt(targetOutPost.transform);
                transform.position = newPosition;
                return;
            }

            // 포인터에 도달한 경우
            ChangeCurState(UnitState.Idle);
            return;
        }

        // 목표가 있는 경우
        if (this.status.curTarget != null)
        {
            // 단 초기 전초기지에는 안 간 경우
            if ((curPoint == OutPostPoint.Point_00 || curPoint == OutPostPoint.Point_10 || curPoint == OutPostPoint.Point_20)
                || (curPoint == OutPostPoint.Point_03 || curPoint == OutPostPoint.Point_13 || curPoint == OutPostPoint.Point_23))
            {
                float distanceTarget = Vector3.Distance(this.status.curTarget.transform.position, transform.position);

                float distancePoint = Vector3.Distance(targetOutPost.transform.position, transform.position);
                Vector3 directionTarget = (targetOutPost.transform.position - transform.position).normalized;
                Vector3 newPositionTarget = transform.position + directionTarget * this.status.curSpeed * Time.deltaTime;

                // 포인터 도달 전 적이 더 가까운 경우 그쪽을 대상으로 함
                if(distanceTarget <= distancePoint)
                {
                    curPoint = OutPostPoint.None; 
                    return;
                }

                // 포인터에 접근한 경우(플레이어)
                if (distancePoint < 2 && routeQueue.Count != 0 && playerDefine == PlayerDefine.Player)
                {
                    Transform route = routeQueue.Dequeue();
                    curPoint = CheckOutPostPoint(route.gameObject);
                    targetOutPost = route.gameObject;
                    return;
                } else if(distancePoint < 2 && routeQueue.Count == 0 && playerDefine == PlayerDefine.Player)
                {
                    curPoint = OutPostPoint.None;
                    targetOutPost = null;
                    return;
                }

                // 포인터에 접근한 경우(적군)
                if (distancePoint < 2 && routeQueue.Count == 3 && playerDefine == PlayerDefine.Enemy)
                {
                    Transform route = routeQueue.Dequeue();
                    curPoint = CheckOutPostPoint(route.gameObject);
                    targetOutPost = route.gameObject;
                    return;
                }

                // 포인터와 먼 경우
                if (distancePoint > 2)
                {
                    transform.LookAt(targetOutPost.transform);
                    transform.position = newPositionTarget;
                    return;
                }

                // 포인터에 도달한 경우
                ChangeCurState(UnitState.Idle);
                return;
            }

            GameObject curTarget = this.status.curTarget;

            float distance = Vector3.Distance(curTarget.transform.position, transform.position);
            Vector3 direction = (curTarget.transform.position - transform.position).normalized;
            Vector3 newPosition = transform.position + direction * this.status.curSpeed * Time.deltaTime;           

            // 거리가 사거리보다 작다면 공격으로 전환
            if (distance < this.status.attackRange)
            {
                isAttack = true; // 공격 중을 의미
                isAttackMotion = true; // 모션의 종료
                ChangeCurState(UnitState.Attack);
                return;
            }

            // 전초기지에서 못가게 한 경우 멈춤
            if (!isPointMove && playerDefine == PlayerDefine.Player)
            {
                ChangeCurState(UnitState.Idle);
                return;
            }

            // 거리가 사거리보다 크다면 이동
            transform.LookAt(curTarget.transform);
            transform.position = newPosition;
            return;

            //// isAttackMotion이 false여야 이동;
            //if (isAttackMotion)
            //{
            //    isAttackMotion = false;
            //    if (this.status.moveType == MoveType.Stand)
            //    {
            //        ChangeCurState(UnitState.Idle);
            //        return;
            //    }
            //    ChangeCurState(UnitState.Idle);
            //    return;
            //}            
        }
    }

    public void ActionAttack()
    {
        // 들어오는 경우
        // idle : 타겟과 거리가 사거리보다 작은 경우
        // move : 이동을 통해 거리가 사거리보다 작은 경우


        // 고정형인 경우 다음 타깃 바로 찾음
        if (this.status.moveType == MoveType.Stand)
        {
            ChangeCurState(UnitState.Idle);
            return;
        }

        // 타겟이 사망하는 경우 idle로 전환, collider가 없는 경우에도 사망으로 처리        
        // collider 없다는 건 사망했다는 의미로 타겟을 삭제 처리
        if (isAttack)
        {
            if ((this.status.curTarget == null || this.status.curTarget.GetComponent<Collider>() == null) && !isAttackMotion)
            {
                isAttack = false; // 공격 종료
                this.status.curTarget = null;
                //isPointMove = true;

                ChangeCurState(UnitState.Idle);
                return;
            }
        }

    }

    public void ActionDeath()
    {       
        if(curState != UnitState.Death)
        {
            return;
        }

        if(unitType == UnitType.Castle)
        {
            return;
        }

        if(playerDefine == PlayerDefine.Player)
        {
            GameManager.gm_instance.AddDeathCount();
        } else if (playerDefine == PlayerDefine.Enemy)
        {
            GameManager.gm_instance.AddKillCount();
        }

        if (this.status.moveType == MoveType.Stand)
        {
            if(unitType == UnitType.Core)
            {
                GameManager.gm_instance.enemyMainTarget--;
                defaultObj.SetActive(false);
                particleObj.SetActive(true);

                StartCoroutine(DelayDestroy());
            }
            else if(unitType == UnitType.Door) 
            {
                GameManager.gm_instance.playerMainTarget--;
                Destroy(gameObject);
                return;
            }
            else if(unitType == UnitType.Tower)
            {
                Destroy(gameObject);
                return;
            }
        }

        Collider collider = GetComponent<Collider>();
        Destroy(collider);

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.constraints = RigidbodyConstraints.FreezePosition;
    }
    private IEnumerator DelayDestroy()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    public void TestDebugRay()
    {
        //Vector3 direction = (this.status.curTarget.transform.position - firePosition.position);
        //Quaternion quaternion = Quaternion.LookRotation(direction);

        //RaycastHit hit;
        //Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out hit, 1f);
        Debug.DrawRay(transform.position, transform.forward * this.status.attackRange, UnityEngine.Color.red);
    }

    private void OnDrawGizmos()
    {
        if (firePosition != null)
        {
            Gizmos.color = UnityEngine.Color.blue;
            DrawCircle(firePosition.position, 20f);
        }
    }

    void DrawCircle(Vector3 position, float radius)
    {
        int segments = 100;
        float angle = 0f;

        Vector3 lastPoint = position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
        Vector3 nextPoint = Vector3.zero;

        for (int i = 1; i <= segments; i++)
        {
            angle += 2 * Mathf.PI / segments;
            nextPoint.x = position.x + Mathf.Cos(angle) * radius;
            nextPoint.z = position.z + Mathf.Sin(angle) * radius;
            nextPoint.y = position.y;

            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }
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
    }

    public GameObject CheckLineEnemy(Transform line)
    {
        GameObject newTarget = null;
        float closestDistance = Mathf.Infinity;
        int checkUnitCount = 0;
        float lineDistance = Mathf.Infinity;
        float closeDistance = Mathf.Infinity;
        GameObject lineTarget = null;
        GameObject closeTarget = null;

        foreach (Transform unit in line)
        {
            // Collider가 없다면 적이 아님
            if (unit.GetComponent<Collider>() == null)
            {
                continue;
            }

            if(unit.gameObject.layer == enemyLayerMask)
            {
                float distance = Vector3.Distance(transform.position, unit.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    lineTarget = unit.gameObject;
                    //newTarget = unit.gameObject;
                }

                checkUnitCount++;
            }
        }
        // 라인 적 중 가까운 적
        lineDistance = closestDistance;

        // 레이어를 통한 적 탐지 방법
        float searchRadius = 50f;
        Vector3 currentPosition = transform.position;
        Collider[] colliders = Physics.OverlapSphere(currentPosition, searchRadius, findLayerMask);

        foreach (Collider obj in colliders)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closeTarget = obj.gameObject;
                //newTarget = obj.transform.gameObject;
            }
        }

        // 라인은 아닌데 가까운 적
        closeDistance = closestDistance;

        //if (checkUnitCount == 0)
        //{
        //    // 레이어를 통한 적 탐지 방법
        //    float searchRadius = 50f;
        //    Vector3 currentPosition = transform.position;
        //    Collider[] colliders = Physics.OverlapSphere(currentPosition, searchRadius, findLayerMask);

        //    foreach (Collider obj in colliders)
        //    {
        //        float distance = Vector3.Distance(transform.position, obj.transform.position);
        //        if (distance < closestDistance)
        //        {
        //            closestDistance = distance;
        //            closeTarget = obj.gameObject;
        //            //newTarget = obj.transform.gameObject;
        //        }
        //    }
        //}

        if (lineDistance <= closeDistance)
        {
            newTarget = lineTarget;
        }
        else
        {
            newTarget = closeTarget;
        }

        return newTarget;
    }

    public void DamageToEnemy()
    {
        if(this.status.curTarget == null)
        {
            return;
        }
        this.status.curTarget.GetComponent<InheriteStatus>().status.Damage(this.status.finalAtk);
    }

    public void CheckCurHp()
    {
        if(isDead)
        {
            hpBarImage.fillAmount = 0;
            return;
        }        

        hpBarImage.fillAmount = (float)this.status.curHp / this.status.finalHp;
    }

    public OutPostPoint CheckOutPostPoint(GameObject obj)
    {
        OutPostPoint point = OutPostPoint.None;

        if(obj == null)
        {
            return point;
        }

        string name = obj.name;

        switch (name)
        {
            case "Point_00":
                point = OutPostPoint.Point_00;
                break;
            case "Point_01":
                point = OutPostPoint.Point_01;
                break;
            case "Point_02":
                point = OutPostPoint.Point_02;
                break;
            case "Point_03":
                point = OutPostPoint.Point_03;
                break;
            case "Point_10":
                point = OutPostPoint.Point_10;
                break;
            case "Point_11":
                point = OutPostPoint.Point_11;
                break;
            case "Point_12":
                point = OutPostPoint.Point_12;
                break;
            case "Point_13":
                point = OutPostPoint.Point_13;
                break;
            case "Point_20":
                point = OutPostPoint.Point_20;
                break;
            case "Point_21":
                point = OutPostPoint.Point_21;
                break;
            case "Point_22":
                point = OutPostPoint.Point_22;
                break;
            case "Point_23":
                point = OutPostPoint.Point_23;
                break;
        }

        return point;
    }

    public void RegistOutPost(GameObject outPost)
    {
        this.status.isInOutPost = true;
        this.status.curOutPost = outPost;
    }

    public void RemoveOutPost()
    {
        this.status.isInOutPost = false;
        this.status.curOutPost = null;
    }

    public void ApplyStatusEffect(StatusEffect name, int time, int hp)
    {
        if (curEffectCoroutine != null)
        {
            StopCoroutine(curEffectCoroutine);
            Destroy(curEffect);
        }

        string prefabPath = "";
        GameObject prefab;

        switch (name)
        {
            case StatusEffect.AreaHeal:
                prefabPath = "Prefabs/Effect/AreaHeal";
                break;
            case StatusEffect.Heal:
                prefabPath = "Prefabs/Effect/Heal";
                break;
            case StatusEffect.ElectricShock:
                prefabPath = "Prefabs/Effect/ElectricShock";
                break;
            default:
                break;
        }
        
        prefab = Resources.Load<GameObject>(prefabPath);

        curEffect = Instantiate(prefab, transform);
        curEffectCoroutine = StartCoroutine(ApplyEffect(curEffect, time, hp));
        
    }

    IEnumerator ApplyEffect(GameObject effect, int time, int hp)
    {
        for(int i = 0; i < time; i++)
        {
            yield return new WaitForSeconds(1);
            this.status.curHp += hp;
        }

        Destroy(effect);
    }

    private void OnCollisionEnter(Collision unit)
    {
        GameObject checkObject = unit.gameObject;

        if (checkObject.layer == enemyLayerMask && !isAttack)
        {
            this.status.curTarget = checkObject;
        }        
    }

    private void OnCollisionExit(Collision unit)
    {
        
    }

    private void OnTriggerEnter(Collider unit)
    {
        //GameObject checkObject = unit.gameObject;

        //if (checkObject.layer == outPostLayerMask && playerDefine == PlayerDefine.Player)
        //{
        //    this.curPoint = CheckOutPostPoint(checkObject);
        //    this.targetOutPost = checkObject;
        //}
    }

    private void OnTriggerExit(Collider unit)
    {
        //GameObject checkObject = unit.gameObject;

        //if (checkObject.layer == outPostLayerMask && playerDefine == PlayerDefine.Player)
        //{
        //    this.curPoint = OutPostPoint.None;
        //    this.targetOutPost = null;
        //}
    }
}