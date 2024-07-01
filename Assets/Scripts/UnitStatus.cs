using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitStatusStruct;
using EnumStruct;

public class UnitStatus : MonoBehaviour
{
    public UnitType UnitType;
    public Status status;

    void Start()
    {
        status = new Status(UnitType);
    }

    void Update()
    {
        Debug.Log("status: " + status.curHp);
    }

    public void SetUnitType(UnitType type)
    {
        UnitType = type;
    }
}
