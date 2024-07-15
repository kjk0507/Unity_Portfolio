using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using EnumStruct;
using UnitStatusStruct;
using UnityEngine.UI;

public class OutpostControl : MonoBehaviour
{
    //bool isWaiting = true; // 대기 상태
    public GameObject body_Black;
    public GameObject body_Blue;
    public GameObject body_Red;

    public Status status;
    public OutPostType curType; // 점령 중인지 아닌지 
    public OutPostState curState; // 대기 공격 상태
    public int occupationGage = 0;
    public bool isOccupation = false; // 점령 중이면 true
    public GameObject barrier;
    public Image hpBar;
    public Image hpBarImage;
    public Image gageBarImage;

    private float elapsedTime = 0f;

    // 주둔 병사 수
    public List<GameObject> unitInAreaList = new List<GameObject>();

    public List<GameObject> warriorList = new List<GameObject>();
    public List<GameObject> archerList = new List<GameObject>();
    public List<GameObject> wizardList = new List<GameObject>();

    public TextMeshProUGUI warriorCount;
    public TextMeshProUGUI archerCount;
    public TextMeshProUGUI wizardCount;

    int warriorNum = 0;
    int archerNum = 0;
    int wizardNum = 0;

    // 대기 공격 버튼
    public GameObject button_Black;
    public GameObject button_Grean;
    public GameObject button_Red;

    // 유닛 체크 중
    public bool isChecked = false;

    public float searchRadius = 18f; // 검사 반경
    public float lastExecutionTime = 0f;
    public float executionInterval = 2f; // 2초마다 검사

    public bool isBroke = false;

    public int curHP;
    public int finalHp;

    void Start()
    {
        Initialize();        
    }

    void Update()
    {
        CheckUnitCount();
        CheckEnterUnitState();
        curHP = this.status.curHp;
        finalHp = this.status.finalHp;
        CheckOutPostHP();
        CheckOccupationGage();
        ChangeOccupationGage();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.blue;
        DrawCircle(transform.position, 18f);
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

    public void Initialize()
    {
        OutPostType type = this.curType;
        ChangeOutPostType(type);

        status = new Status(type);
    }
    public void CheckUnitCount()
    {
        for (int i = warriorList.Count - 1; i >= 0; i--)
        {
            if (warriorList[i] == null)
            {
                warriorList.RemoveAt(i);
            }
        }

        for (int i = archerList.Count - 1; i >= 0; i--)
        {
            if (archerList[i] == null)
            {
                archerList.RemoveAt(i);
            }
        }

        for (int i = wizardList.Count - 1; i >= 0; i--)
        {
            if (wizardList[i] == null)
            {
                wizardList.RemoveAt(i);
            }
        }

        warriorNum = warriorList.Count;
        archerNum = archerList.Count;
        wizardNum = wizardList.Count;

        warriorCount.text = warriorNum.ToString();
        archerCount.text = archerNum.ToString();
        wizardCount.text = wizardNum.ToString();
    }

    public OutPostState CheckOutPostState()
    {
        return curState;
    }

    public void CheckOutPostHP()
    {
        if (!isBroke)
        {
            hpBarImage.fillAmount = (float)this.status.curHp / this.status.finalHp;

            if (this.status.curHp <= 0)
            {
                isBroke = true;
                curType = OutPostType.InActive;
                ChangeOutPostType(curType);
                this.status.curHp = this.status.finalHp;

                foreach (GameObject unit in warriorList)
                {
                    unit.GetComponent<InheriteStatus>().RemoveOutPost();
                }
                foreach (GameObject unit in archerList)
                {
                    unit.GetComponent<InheriteStatus>().RemoveOutPost();
                }
                foreach (GameObject unit in wizardList)
                {
                    unit.GetComponent<InheriteStatus>().RemoveOutPost();
                }
            }            
        }
    }

    public void CheckOccupationGage()
    {
        if(curType == OutPostType.InActive)
        {
            gageBarImage.fillAmount = (float)this.occupationGage / 100;
            if (occupationGage >= 100)
            {
                isBroke = false;
                curType = OutPostType.Active_Player;
                ChangeOutPostType(curType);

                foreach (GameObject unit in warriorList)
                {
                    unit.GetComponent<InheriteStatus>().RegistOutPost(gameObject);
                }
                foreach (GameObject unit in archerList)
                {
                    unit.GetComponent<InheriteStatus>().RegistOutPost(gameObject);
                }
                foreach (GameObject unit in wizardList)
                {
                    unit.GetComponent<InheriteStatus>().RegistOutPost(gameObject);
                }

                occupationGage = 0;
                return;
            }

            if((warriorList.Count != 0 || archerList.Count !=0 || wizardList.Count != 0) && !isOccupation)
            {
                isOccupation = true;
                return;
            }

            if (warriorList.Count == 0 && archerList.Count == 0 && wizardList.Count == 0)
            {
                isOccupation = false;
                return;
            }
        }
        else if(curType == OutPostType.Active_Enemy)
        {
            gageBarImage.fillAmount = 0;
        }
    }

    public void ChangeOccupationGage()
    {
        if (curType == OutPostType.InActive)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= 0.1f)
            {
                if (isOccupation)
                {
                    occupationGage = Mathf.Min(occupationGage + 1, 100);
                }
                else
                {
                    occupationGage = Mathf.Max(occupationGage - 1, 0);
                }

                elapsedTime = 0f; // 시간 초기화
            }
        }
    }

    public void ChangeOutPostType(OutPostType type)
    {
        switch (type)
        {
            case OutPostType.InActive:
                body_Black.SetActive(true);
                body_Blue.SetActive(false);
                body_Red.SetActive(false);
                barrier.SetActive(false);
                hpBar.gameObject.SetActive(false);
                gageBarImage.gameObject.SetActive(true);
                ChangeOutPostState(OutPostState.InActive);
                break;
            case OutPostType.Active_Player:
                body_Black.SetActive(false);
                body_Blue.SetActive(true);
                body_Red.SetActive(false);
                barrier.SetActive(true);
                hpBar.gameObject.SetActive(true);
                gageBarImage.gameObject.SetActive(false);
                ChangeOutPostState(OutPostState.Wait);
                break;
            case OutPostType.Active_Enemy:
                body_Black.SetActive(false);
                body_Blue.SetActive(false);
                body_Red.SetActive(true);
                barrier.SetActive(false);
                ChangeOutPostState(OutPostState.InActive);
                break;
        }        
    }

    public void ChangeOutPostState(OutPostState state)
    {
        // 돌 버튼 색상 변화
        curState = state;

        switch (curState)
        {
            case OutPostState.InActive:
                button_Black.SetActive(true);
                button_Grean.SetActive(false);
                button_Red.SetActive(false);
                break;
            case OutPostState.Move:
                button_Black.SetActive(false);
                button_Grean.SetActive(true);
                button_Red.SetActive(false);

                foreach(GameObject unit in warriorList)
                {
                    unit.GetComponent<InheriteStatus>().isPointMove = true;
                }
                foreach (GameObject unit in archerList)
                {
                    unit.GetComponent<InheriteStatus>().isPointMove = true;
                }
                foreach (GameObject unit in wizardList)
                {
                    unit.GetComponent<InheriteStatus>().isPointMove = true;
                }

                break;
            case OutPostState.Wait:
                button_Black.SetActive(false);
                button_Grean.SetActive(false);
                button_Red.SetActive(true);

                foreach (GameObject unit in warriorList)
                {
                    unit.GetComponent<InheriteStatus>().isPointMove = false;
                }
                foreach (GameObject unit in archerList)
                {
                    unit.GetComponent<InheriteStatus>().isPointMove = false;
                }
                foreach (GameObject unit in wizardList)
                {
                    unit.GetComponent<InheriteStatus>().isPointMove = false;
                }

                break;
        }
    }

    public void CheckEnterUnitState()
    {
        if (!isChecked)
        {
            foreach (GameObject unit in warriorList)
            {
                ChangePointMove(unit);
            }

            foreach (GameObject unit in archerList)
            {
                ChangePointMove(unit);
            }

            foreach (GameObject unit in wizardList)
            {
                ChangePointMove(unit);
            }
        }        
    }

    public void ChangePointMove(GameObject unit)
    {
        if (curState == OutPostState.Move)
        {
            unit.GetComponent<InheriteStatus>().isPointMove = true;
        }
        else
        {
            unit.GetComponent<InheriteStatus>().isPointMove = false;
        }
    }

    //public void CheckOutpostState()
    //{
    //    if(!isWaiting) // 전진 상태 -> 대기 상태
    //    {
    //        waitingImage.SetActive(true);
    //        goImage.SetActive(false);

    //        isWaiting = true;  // 부딪힘과 동시에 다시 변경되면 어짜피 안됨
    //    }
    //    else if(isWaiting) // 대기 상태 -> 전진 상태
    //    {
    //        waitingImage.SetActive(false);
    //        goImage.SetActive(true);

    //        ChangeUnitTarget();

    //        isWaiting = false;
    //    }
    //}

    private void OnTriggerEnter(Collider unit)
    {
        // 플레이어 소유 상태인 경우
        UnitType type;

        if (unit.GetComponent<InheriteStatus>() != null)
        {
            type = unit.GetComponent<InheriteStatus>().unitType;            

            switch (type)
            {
                case UnitType.Warrior:
                    warriorList.Add(unit.gameObject);
                    break;
                case UnitType.Archer:
                    archerList.Add(unit.gameObject);
                    break;
                case UnitType.Wizard:
                    wizardList.Add(unit.gameObject);
                    break;
            }

            if(curType == OutPostType.Active_Player)
            {
                unit.GetComponent<InheriteStatus>().RegistOutPost(gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (Time.time - lastExecutionTime >= executionInterval)
        {
            lastExecutionTime = Time.time;

            Collider[] positiveCollider;
            Collider[] negativeCollider;
            int playerLayerMask = LayerMask.GetMask("Player");
            int enemyLayerMask = LayerMask.GetMask("Enemy");

            if (curType == OutPostType.Active_Player)
            {
                positiveCollider = Physics.OverlapSphere(transform.position, searchRadius, playerLayerMask);
                foreach(Collider unit in positiveCollider)
                {
                    unit.GetComponent<InheriteStatus>().ApplyStatusEffect(StatusEffect.Heal, 10, 10);
                }

                negativeCollider = Physics.OverlapSphere(transform.position, searchRadius, enemyLayerMask);
                foreach (Collider unit in negativeCollider)
                {
                    unit.GetComponent<InheriteStatus>().ApplyStatusEffect(StatusEffect.ElectricShock, 10, -10);
                }
            }
            else if(curType == OutPostType.Active_Enemy) 
            {
                positiveCollider = Physics.OverlapSphere(transform.position, searchRadius, enemyLayerMask);
                foreach (Collider unit in positiveCollider)
                {
                    unit.GetComponent<InheriteStatus>().ApplyStatusEffect(StatusEffect.Heal, 10, 10);
                }
            }
        }

    }

    private void OnTriggerExit(Collider unit)
    {
        UnitType type;

        if (unit.GetComponent<InheriteStatus>() != null)
        {
            type = unit.GetComponent<InheriteStatus>().unitType;

            switch (type)
            {
                case UnitType.Warrior:
                    warriorList.Remove(unit.gameObject);
                    break;
                case UnitType.Archer:
                    archerList.Remove(unit.gameObject);
                    break;
                case UnitType.Wizard:
                    wizardList.Remove(unit.gameObject);
                    break;
            }

            unit.GetComponent<InheriteStatus>().RemoveOutPost();
        }
    }
}
