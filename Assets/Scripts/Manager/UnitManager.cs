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
}
