using EnumStruct;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OutpostControl : MonoBehaviour
{
    // todo1 : (대기 상태일 때) 전초기지에 닿은 유닛을 구분하여 저장 -> 유닛의 숫자는 세되 안보이게 끄기
    // todo2 : 대기 상태가 아니면 그냥 바로 전진 -> 이건 유닛무브의 타겟을 여기서 변경
    // todo3 : 대기 상태에서 공격 상태로 바뀔때 저장하고 있던 유닛들을 랜덤을 통해 약간 위치를 바꾸어 전진

    bool isWaiting = true; // 대기 상태

    public GameObject warriorList;
    public GameObject archerList;
    public GameObject wizardList;

    public TextMeshProUGUI warriorCount;
    public TextMeshProUGUI archerCount;
    public TextMeshProUGUI wizardCount;

    int warriorNum = 0;
    int archerNum = 0;
    int wizardNum = 0;

    public GameObject waitingImage;
    public GameObject goImage;

    public GameObject spawnLocation;

    public GameObject topLine;
    public GameObject middleLine;
    public GameObject bottomLine;

    public GameObject testtarget;

    void Start()
    {

    }

    void Update()
    {
        CheckUnitCount();
    }

    public void CheckUnitCount()
    {
        warriorNum = warriorList.transform.childCount;
        archerNum = archerList.transform.childCount;
        wizardNum = wizardList.transform.childCount;

        warriorCount.text = warriorNum.ToString();
        archerCount.text = archerNum.ToString();
        wizardCount.text = wizardNum.ToString();
    }

    public void ChangeUnitTarget()
    {
        List<Transform> units = new List<Transform>();
        foreach (Transform unit in warriorList.transform)
        {
            units.Add(unit);
        }

        foreach (Transform unit in archerList.transform)
        {
            units.Add(unit);
        }

        foreach (Transform unit in wizardList.transform)
        {
            units.Add(unit);
        }

        foreach (Transform unit in units)
        {
            Vector3 randomRange = new Vector3(2.0f, 2.0f, 2.0f);

            Vector3 randomOffset = new Vector3(
                Random.Range(-randomRange.x, randomRange.x),
                0,
                Random.Range(-randomRange.z, randomRange.z)
            );

            Vector3 finalPosition = spawnLocation.transform.position + randomOffset;

            unit.gameObject.transform.position = finalPosition;
            //unit.gameObject.transform.position = spawnLocation.transform.position;
            unit.gameObject.GetComponent<UnitStatus>().ChangeCurTarget(testtarget);

            OutPostRow row = unit.gameObject.GetComponent<UnitStatus>().status.curRow;

            switch (row)
            {
                case OutPostRow.Top:
                    unit.transform.SetParent(topLine.transform);
                    break;
                case OutPostRow.Middle:
                    unit.transform.SetParent(middleLine.transform);
                    break;
                case OutPostRow.Bottom:
                    unit.transform.SetParent(bottomLine.transform);
                    break;
            }

            unit.gameObject.SetActive(true);
        }
    }

    public void CheckOutpostState()
    {
        Debug.Log("check");

        if(!isWaiting) // 전진 상태 -> 대기 상태
        {
            waitingImage.SetActive(true);
            goImage.SetActive(false);
            
            isWaiting = true;  // 부딪힘과 동시에 다시 변경되면 어짜피 안됨
        }
        else if(isWaiting) // 대기 상태 -> 전진 상태
        {
            waitingImage.SetActive(false);
            goImage.SetActive(true);

            ChangeUnitTarget();

            isWaiting = false;
        }
    }

    private void OnTriggerEnter(Collider unit)
    {
        GameObject checkObject = unit.gameObject;
        string tag = unit.tag;

        switch (tag)
        {
            case "Warrior":
                unit.transform.SetParent(warriorList.transform);
                break;
            case "Archer":
                unit.transform.SetParent(archerList.transform);
                break;
            case "Wizard":
                unit.transform.SetParent(wizardList.transform);
                break;
            default:
                return;
        }

        checkObject.SetActive(false);

        if(!isWaiting )
        {
            ChangeUnitTarget();
        }
    }
}
