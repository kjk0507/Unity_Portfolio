using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumStruct;

public class SpawnManager : MonoBehaviour
{
    public PlayerDefine player;
    public GameObject spawnLocation;
    public GameObject outpostTarget;

    [SerializeField] private GameObject curOutpost;

    public GameObject outpostTop;
    public GameObject outpostMiddle;
    public GameObject outpostBottom;

    public GameObject finalTarget;
    //public bool spawntest = false;

    // 프리팹 위치
    public string prefabPath; 

    void Start()
    {
        
    }

    void Update()
    {
        //CheckPlayer();
    }

    //private void CheckPlayer()
    //{
    //    if(player == PlayerDefine.Player)
    //    {
    //        SpawnPlayerUnit();
    //    }
    //    else
    //    {
    //        SpawnEnemyUnit();
    //    }
    //}

    public void SpawnPlayerUnit(int slotNum)
    {
        UnitType type = (UnitType)System.Enum.ToObject(typeof(UnitType), slotNum);
        Debug.Log("spawn : " + type.ToString());

        //OutPostRow row = (OutPostRow)System.Enum.ToObject(typeof(OutPostRow), slotNum);
        OutPostRow row = 0;

        string outpost_name = GameManager.gm_instance.GetPresentOutPost().name;

        switch (outpost_name)
        {
            case "Outpost_Top":
                row = OutPostRow.Top;
                break;
            case "Outpost_Middle":
                row = OutPostRow.Middle;
                break;
            case "Outpost_Bottom":
                row = OutPostRow.Bottom;
                break;
        }

        if (GameManager.gm_instance.GetPresentOutPost())
        {
            ChangeUnitTypeSpawn(row, type);
        }
    }

    private void SpawnEnemyUnit()
    {

    }

    private void ChangeUnitTypeSpawn(OutPostRow row, UnitType type)
    {
        curOutpost = GameManager.gm_instance.GetPresentOutPost();

        if(row == OutPostRow.Top)
        {
            outpostTarget = outpostTop;
        }
        else if(row == OutPostRow.Middle)
        {
            outpostTarget = outpostMiddle;
        }
        else if(row == OutPostRow.Bottom)
        {
            outpostTarget = outpostBottom;
        }

        if(type == UnitType.Warrior)
        {
            prefabPath = "Prefabs/Unit/Unit_Warrior";
        }
        else if (type == UnitType.Archer)
        {
            prefabPath = "Prefabs/Unit/Unit_Archer";
        }
        else if (type == UnitType.Wizard)
        {
            prefabPath = "Prefabs/Unit/Unit_Wizard";
        }        

        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        GameObject instance = Instantiate(prefab, spawnLocation.transform);
        //instance.GetComponent<UnitStatus>().SetUnitType(UnitType.Warrior);
        instance.GetComponent<UnitStatus>().Initialize(type, row, outpostTarget, finalTarget);

        UnitManager.um_instance.RegistUnit(instance, type);

    }
}
