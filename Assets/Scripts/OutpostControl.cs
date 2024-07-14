using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using EnumStruct;
using UnitStatusStruct;
using UnityEngine.UI;

public class OutpostControl : MonoBehaviour
{
 
    // 비어있는건 점령 가능 -> 아군은 상대 전초기지나 비어있는 전초기지를 넘어갈수 없음(근데 그렇다고 idle로 변경시키면 적에게 맞아 디질텐데?) -> 넘어갈수 없다고 했으니 ㄱㅊ 사거리가 닿는 경우에 attack으로 변경
    // 게이지 체우면 점령(layer로 구분) -> 점령중 bool 필요, 점령 게이지
    // 방어 건물... 이건 그냥 별도로 설치 하는 걸로
    // 점령상태에선 대기, 전진 상태 전환이 가능, 비점령시 대기
    // 점령상태의 전초기지는 일정 범위 안에 있는 아군에게 힐을 줌
    // 점령상태의 전초기지는 적에게 데미지를 입힘(결계 느낌)

    //bool isWaiting = true; // 대기 상태
    public GameObject body_Black;
    public GameObject body_Blue;
    public GameObject body_Red;

    public Status status;
    public OutPostType curType; // 점령 중인지 아닌지 
    public OutPostState curState; // 대기 공격 상태
    public int occupationGage;
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
        // 이거 안될꺼 같으면 걍 trigger로 처리, list 만들어서 ontrigger랑 exit 트리거로 관리
        //warriorNum = UnitManager.um_instance.CheckUnitListNum(this.gameObject, UnitType.Warrior);
        //archerNum = UnitManager.um_instance.CheckUnitListNum(this.gameObject, UnitType.Archer);
        //wizardNum = UnitManager.um_instance.CheckUnitListNum(this.gameObject, UnitType.Wizard);

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

            unit.GetComponent<InheriteStatus>().RegistOutPost(gameObject);
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
