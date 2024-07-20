using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumStruct;

public class UnitManager : MonoBehaviour
{
    public static UnitManager um_instance;

    // 아군 리스트
    public List<GameObject> warriorList;
    public List<GameObject> archerList;
    public List<GameObject> wizardList;

    // 적 리스트
    public List<GameObject> orcList;
    public List<GameObject> boneArcherList;
    public List<GameObject> destroyerList;

    // 소환된 라인
    public Transform topLine; 
    public Transform middleLine;
    public Transform bottomLine;

    // 유닛 변경 수치
    public int warrior_equip_hp;
    public int warrior_equip_atk;
    public int warrior_equip_def;
    public float warrior_equip_speed;
    public int warrior_upgrade_hp;
    public int warrior_upgrade_atk;
    public int warrior_upgrade_def;
    public float warrior_upgrade_speed;

    public int archer_equip_hp;
    public int archer_equip_atk;
    public int archer_equip_def;
    public float archer_equip_speed;
    public int archer_upgrade_hp;
    public int archer_upgrade_atk;
    public int archer_upgrade_def;
    public float archer_upgrade_speed;

    public int wizard_equip_hp;
    public int wizard_equip_atk;
    public int wizard_equip_def;
    public float wizard_equip_speed;
    public int wizard_upgrade_hp;
    public int wizard_upgrade_atk;
    public int wizard_upgrade_def;
    public float wizard_upgrade_speed;

    void Start()
    {
        if (um_instance == null)
        {
            um_instance = this;
        }
        else if (um_instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        
    }

    public int GetExtraHp(UnitType type)
    {
        int extraHp = 0;

        if(type == UnitType.Warrior)
        {
            extraHp = warrior_equip_hp + warrior_upgrade_hp;
        } 
        else if(type == UnitType.Archer)
        {
            extraHp = archer_equip_hp + archer_upgrade_hp;
        }
        else if(type == UnitType.Wizard)
        {
            extraHp = wizard_equip_hp + wizard_upgrade_hp;
        }
        return extraHp;
    }

    public int GetExtraAtk(UnitType type)
    {
        int extraAtk = 0;

        if(type == UnitType.Warrior)
        {
            extraAtk = warrior_equip_atk + warrior_upgrade_atk;
        }
        else if(type == UnitType.Archer)
        {
            extraAtk = archer_equip_atk + archer_upgrade_atk;
        }
        else if(type == UnitType.Wizard)
        {
            extraAtk = wizard_equip_atk + wizard_upgrade_atk;
        }

        return extraAtk;
    }

    public int GetExtraDef(UnitType type)
    {
        int extraDef = 0;

        if (type == UnitType.Warrior)
        {
            extraDef = warrior_equip_def + warrior_upgrade_def;
        }
        else if(type == UnitType.Archer)
        {
            extraDef = archer_equip_atk + archer_upgrade_def;
        }
        else if(type == UnitType.Wizard)
        {
            extraDef = wizard_equip_def + wizard_upgrade_def;
        }

        return extraDef;
    }

    public float GetExtraSpeed(UnitType type)
    {
        float extraSpeed = 0;

        if(type == UnitType.Warrior)
        {
            extraSpeed = warrior_equip_speed + warrior_upgrade_speed;
        }
        else if(type == UnitType.Archer)
        {
            extraSpeed = archer_equip_speed + archer_upgrade_speed;
        }
        else if(type == UnitType.Wizard)
        {
            extraSpeed = wizard_equip_speed + wizard_upgrade_speed;
        }

        return extraSpeed;
    }

    public void RegistUnit(GameObject obj, UnitType type)
    {
        if (type == UnitType.Warrior)
        {
            warriorList.Add(obj);
        }
        else if (type == UnitType.Archer)
        {
            archerList.Add(obj);
        }
        else if (type == UnitType.Wizard)
        {
            wizardList.Add(obj);
        }
    }

    public Transform FindLine(LineType row)
    {
        Transform line = null;

        switch (row)
        {
            case LineType.Top:
                line = topLine;
                break;
            case LineType.Middle:
                line = middleLine;
                break;
            case LineType.Bottom:
                line = bottomLine;
                break;
        }

        return line;
    }

    public int CheckLineNum(LineType type)
    {
        int layerMask = LayerMask.NameToLayer("Enemy");
        int num = 0;

        switch (type)
        {
            case LineType.Top:
                foreach (Transform obj in topLine)
                {
                    if (obj.gameObject.layer == layerMask)
                    {
                        num++;
                    }                    
                }
                break; 
            case LineType.Middle:
                foreach (Transform obj in middleLine)
                {
                    if (obj.gameObject.layer == layerMask)
                    {
                        num++;
                    }
                }
                break; 
            case LineType.Bottom:
                foreach (Transform obj in bottomLine)
                {
                    if (obj.gameObject.layer == layerMask)
                    {
                        num++;
                    }
                }
                break;
        }

        return num;
    }

    public int CheckUnitListNum(GameObject obj, UnitType type)
    {
        int listNum = 0;

        string name = obj.name;

        OutPostPoint point = ChangPointNum(name);

        switch (type)
        {
            case UnitType.Warrior:
                foreach(GameObject unit in warriorList)
                {
                    if(point == unit.GetComponent<InheriteStatus>().curPoint)
                    {
                        listNum++;
                    }                    
                }
                break;
            case UnitType.Archer:
                foreach (GameObject unit in archerList)
                {
                    if (point == unit.GetComponent<InheriteStatus>().curPoint)
                    {
                        listNum++;
                    }
                }
                break;
            case UnitType.Wizard:
                foreach (GameObject unit in wizardList)
                {
                    if (point == unit.GetComponent<InheriteStatus>().curPoint)
                    {
                        listNum++;
                    }
                }
                break;
        }

        return listNum;
    }

    public OutPostPoint ChangPointNum(string name)
    {
        OutPostPoint point = OutPostPoint.None;

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

    public void ChangeUpgradeStatus(UnitType utype, UpgradeType ugType)
    {
        int curGold = GameManager.gm_instance.GetGold();
        float cost = GetUnitUpgradeCost(utype, ugType);

        //if (curGold < cost)
        //{
        //    UIManager.UM_instance.ShowMessage("골드가 부족합니다.");
        //    return;
        //}

        UIManager.UM_instance.ShowMessage("강화에 성공하셨습니다.");

        switch (utype)
        {
            case UnitType.Warrior:
                switch (ugType)
                {
                    case UpgradeType.Hp:
                        warrior_upgrade_hp++;
                        break;
                    case UpgradeType.Attack:
                        warrior_upgrade_atk++;
                        break;
                    case UpgradeType.Defence:
                        warrior_upgrade_def++;
                        break;
                    case UpgradeType.Speed:
                        warrior_upgrade_speed++;
                        break;
                }
                break;
            case UnitType.Archer:
                switch (ugType)
                {
                    case UpgradeType.Hp:
                        archer_upgrade_hp++;
                        break;
                    case UpgradeType.Attack:
                        archer_upgrade_atk++;
                        break;
                    case UpgradeType.Defence:
                        archer_upgrade_def++;
                        break;
                    case UpgradeType.Speed:
                        archer_upgrade_speed++;
                        break;
                }
                break;
            case UnitType.Wizard:
                switch (ugType)
                {
                    case UpgradeType.Hp:
                        wizard_upgrade_hp++;
                        break;
                    case UpgradeType.Attack:
                        wizard_upgrade_atk++;
                        break;
                    case UpgradeType.Defence:
                        wizard_upgrade_def++;
                        break;
                    case UpgradeType.Speed:
                        wizard_upgrade_speed++;
                        break;
                }
                break;
        }
    }

    public float GetUnitUpgradeCost(UnitType utype, UpgradeType ugType)
    {
        float result = 0;

        switch (utype)
        {
            case UnitType.Warrior:
                switch (ugType)
                {
                    case UpgradeType.Hp:
                        result = warrior_upgrade_hp;
                        break;
                    case UpgradeType.Attack:
                        result = warrior_upgrade_atk;
                        break;
                    case UpgradeType.Defence:
                        result = warrior_upgrade_def;
                        break;
                    case UpgradeType.Speed:
                        result = warrior_upgrade_speed;
                        break;
                }
                break;
            case UnitType.Archer:
                switch (ugType)
                {
                    case UpgradeType.Hp:
                        result = archer_upgrade_hp;
                        break;
                    case UpgradeType.Attack:
                        result = archer_upgrade_atk;
                        break;
                    case UpgradeType.Defence:
                        result = archer_upgrade_def;
                        break;
                    case UpgradeType.Speed:
                        result = archer_upgrade_speed;
                        break;
                }
                break;
            case UnitType.Wizard:
                switch (ugType)
                {
                    case UpgradeType.Hp:
                        result = wizard_upgrade_hp;
                        break;
                    case UpgradeType.Attack:
                        result = wizard_upgrade_atk;
                        break;
                    case UpgradeType.Defence:
                        result = wizard_upgrade_def;
                        break;
                    case UpgradeType.Speed:
                        result = wizard_upgrade_speed;
                        break;
                }
                break;
        }

        result = (result + 1) * 100;

        return result;
    }
}
