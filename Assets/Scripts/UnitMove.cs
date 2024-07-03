using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumStruct;
using UnitStatusStruct;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class UnitMove : MonoBehaviour
{
    Status status;
    public float moveSpeed;
    public GameObject curTarget;
    bool isInitialize = false;

    NavMeshAgent nmAgent;

    void Start()
    {
        nmAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        Initialize();
        Move();
        //nmAgent.SetDestination(curTarget.transform.position);
    }

     public void Move()
    {
        if (curTarget != null)
        {
            float distance = Vector2.Distance(curTarget.transform.position, transform.position);
            if (distance < 1f)
            {
                return;
            }

            // 타겟과 현재 위치 사이의 방향을 계산
            Vector3 direction = (curTarget.transform.position - transform.position).normalized;            

            // 새로운 위치 계산
            Vector3 newPosition = transform.position + direction * moveSpeed * Time.deltaTime;

            // 캐릭터를 새로운 위치로 이동
            transform.position = newPosition;

            // 캐릭터가 타겟을 바라보게 회전 (선택사항)
            transform.LookAt(curTarget.transform);
        }
    }

    public void Initialize()
    {
        if (!isInitialize)
        {
            status = GetComponent<UnitStatus>().status;

            moveSpeed = status.curSpeed;
            curTarget = status.curTarget;

            isInitialize = true;
        }
    }
}
