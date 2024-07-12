using EnumStruct;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    public GameObject Body_Black;
    public GameObject Body_Blue;
    public GameObject Body_Red;

    public OutPostType curType; // 점령 중인지 아닌지 
    public OutPostState curState; // 대기 공격 상태
    public int occupationGage;
    public bool isOccupation = false; // 점령 중이면 true

    // 주둔 병사 수
    public TextMeshProUGUI warriorCount;
    public TextMeshProUGUI archerCount;
    public TextMeshProUGUI wizardCount;

    int warriorNum = 0;
    int archerNum = 0;
    int wizardNum = 0;

    // 대기 공격 버튼
    public GameObject Button_Black;
    public GameObject Button_Grean;
    public GameObject Button_Red;

    //public GameObject warriorList;
    //public GameObject archerList;
    //public GameObject wizardList;

    // 이미지로 안하고 게임 오브젝트로 함
    //public GameObject waitingImage;
    //public GameObject goImage;


    //public GameObject spawnLocation;

    //public GameObject topLine;
    //public GameObject middleLine;
    //public GameObject bottomLine;

    //public GameObject testtarget;

    void Start()
    {

    }

    void Update()
    {
        //CheckUnitCount();
    }

    public void CheckUnitCount()
    {
        // 이거 안될꺼 같으면 걍 trigger로 처리, list 만들어서 ontrigger랑 exit 트리거로 관리
        warriorNum = UnitManager.um_instance.CheckUnitListNum(this.gameObject, UnitType.Warrior);
        archerNum = UnitManager.um_instance.CheckUnitListNum(this.gameObject, UnitType.Archer);
        wizardNum = UnitManager.um_instance.CheckUnitListNum(this.gameObject, UnitType.Wizard);

        warriorCount.text = warriorNum.ToString();
        archerCount.text = archerNum.ToString();
        wizardCount.text = wizardNum.ToString();
    }

    public OutPostState CheckOutPostState()
    {
        return curState;
    }

    public void ChangeOutPostType(OutPostType type)
    {
        switch (type)
        {
            case OutPostType.InActive:
                Body_Black.SetActive(true);
                Body_Blue.SetActive(false);
                Body_Red.SetActive(false);
                break;
            case OutPostType.Active_Player:
                Body_Black.SetActive(false);
                Body_Blue.SetActive(true);
                Body_Red.SetActive(false);
                break;
            case OutPostType.Active_Enemy:
                Body_Black.SetActive(false);
                Body_Blue.SetActive(false);
                Body_Red.SetActive(true);
                break;
        }        
    }

    public void ChangeOutPostState(OutPostState state)
    {
        // 돌 버튼 색상 변화
        //curState = (OutPostState)System.Enum.ToObject(typeof(OutPostState), state); ;
        curState = state;

        switch (curState)
        {
            case OutPostState.InActive:
                Button_Black.SetActive(true);
                Button_Grean.SetActive(false);
                Button_Red.SetActive(false);
                break;
            case OutPostState.Move:
                Button_Black.SetActive(false);
                Button_Grean.SetActive(true);
                Button_Red.SetActive(false);
                break;
            case OutPostState.Wait:
                Button_Black.SetActive(false);
                Button_Grean.SetActive(false);
                Button_Red.SetActive(true);
                break;
        }
    }


    //public void CheckUnitCount()
    //{
    //    warriorNum = warriorList.transform.childCount;
    //    archerNum = archerList.transform.childCount;
    //    wizardNum = wizardList.transform.childCount;

    //    warriorCount.text = warriorNum.ToString();
    //    archerCount.text = archerNum.ToString();
    //    wizardCount.text = wizardNum.ToString();
    //}

    //public void ChangeUnitTarget()
    //{
    //    List<Transform> units = new List<Transform>();
    //    foreach (Transform unit in warriorList.transform)
    //    {
    //        units.Add(unit);
    //    }

    //    foreach (Transform unit in archerList.transform)
    //    {
    //        units.Add(unit);
    //    }

    //    foreach (Transform unit in wizardList.transform)
    //    {
    //        units.Add(unit);
    //    }

    //    foreach (Transform unit in units)
    //    {
    //        Vector3 randomRange = new Vector3(2.0f, 2.0f, 2.0f);

    //        Vector3 randomOffset = new Vector3(
    //            Random.Range(-randomRange.x, randomRange.x),
    //            0,
    //            Random.Range(-randomRange.z, randomRange.z)
    //        );

    //        Vector3 finalPosition = spawnLocation.transform.position + randomOffset;

    //        unit.gameObject.transform.position = finalPosition;
    //        // 원래는 타겟을 강제로 바꾸는 코드였으나 unitstatus에서 실행
    //        unit.gameObject.GetComponent<UnitStatus>().ChangeCurTarget(null);

    //        LineType row = unit.gameObject.GetComponent<UnitStatus>().status.curRow;

    //        switch (row)
    //        {
    //            case LineType.Top:
    //                unit.transform.SetParent(topLine.transform);
    //                break;
    //            case LineType.Middle:
    //                unit.transform.SetParent(middleLine.transform);
    //                break;
    //            case LineType.Bottom:
    //                unit.transform.SetParent(bottomLine.transform);
    //                break;
    //        }

    //        unit.gameObject.SetActive(true);
    //    }
    //}

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
        //GameObject checkObject = unit.gameObject;
        //string tag = unit.tag;

        //switch (tag)
        //{
        //    case "Warrior":
        //        unit.transform.SetParent(warriorList.transform);
        //        break;
        //    case "Archer":
        //        unit.transform.SetParent(archerList.transform);
        //        break;
        //    case "Wizard":
        //        unit.transform.SetParent(wizardList.transform);
        //        break;
        //    default:
        //        return;
        //}

        //checkObject.SetActive(false);

        //if(!isWaiting )
        //{
        //    ChangeUnitTarget();
        //}
    }
}
