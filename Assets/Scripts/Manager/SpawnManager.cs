using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumStruct;

public class SpawnManager : MonoBehaviour
{
    public PlayerDefine player;
    public Transform spawnLocation;
    public GameObject outpostTarget;

    [SerializeField] private GameObject curOutpost;

    public GameObject finalTarget;
    //public bool spawntest = false;

    //public bool isEnemy = false; // true : 플레이어 스폰매니저, false : 적 스폰매니저

    // 아군 스폰 목적지 위치
    public Transform outpostTop;
    public Transform outpostMiddle;
    public Transform outpostBottom;

    // 적 스폰 목적지 위치
    public Transform frontTopPoint;
    public Transform frontMiddlePoint;
    public Transform frontBottomPoint;
    public Transform backTopPoint;
    public Transform backMiddlePoint;
    public Transform backBottomPoint;

    // 라인 : 우선 타겟라인
    public GameObject topLine;
    public GameObject middleLine;
    public GameObject bottomLine;

    // 적 라인 유닛 수
    public int topLineUnitNum;
    public int middleLineUnitNum;
    public int bottomLineUnitNum;

    // 적 유닛 큐
    Queue<UnitType> topLineQueue = new Queue<UnitType>();
    Queue<UnitType> middleLineQueue = new Queue<UnitType>();
    Queue<UnitType> bottomLineQueue = new Queue<UnitType>();

    // 프리팹 위치
    //public string prefabPath; 

    void Start()
    {
        InitialResistQueue();
        InitialSpawnEnemy();
        InvokeRepeating("AutoSpawnEnemyUnit", 0f, 1f);
    }

    void Update()
    {
        //AutoSpawnEnemyUnit();
    }

    public void InitialResistQueue()
    {
        if (player == PlayerDefine.Player)
        {
            return;
        }

        // 윗 라인
        ResistUnitQueue(UnitType.Orc, LineType.Top, 3);
        ResistUnitQueue(UnitType.BoneArcher, LineType.Top, 2);
        ResistUnitQueue(UnitType.Orc, LineType.Top, 3);
        ResistUnitQueue(UnitType.BoneArcher, LineType.Top, 2);
        ResistUnitQueue(UnitType.Destroyer, LineType.Top, 1);

        // 중간 라인
        ResistUnitQueue(UnitType.Orc, LineType.Middle, 3);
        ResistUnitQueue(UnitType.BoneArcher, LineType.Middle, 2);
        ResistUnitQueue(UnitType.Orc, LineType.Middle, 3);
        ResistUnitQueue(UnitType.BoneArcher, LineType.Middle, 2);
        ResistUnitQueue(UnitType.Destroyer, LineType.Middle, 1);

        // 밑 라인
        ResistUnitQueue(UnitType.Orc, LineType.Bottom, 3);
        ResistUnitQueue(UnitType.BoneArcher, LineType.Bottom, 2);
        ResistUnitQueue(UnitType.Orc, LineType.Bottom, 3);
        ResistUnitQueue(UnitType.BoneArcher, LineType.Bottom, 2);
        ResistUnitQueue(UnitType.Destroyer, LineType.Bottom, 1);
    }

    public void ResistUnitQueue(UnitType type, LineType line, int count)
    {
        for(int i = 0; i < count; i++)
        {
            switch (line)
            {
                case LineType.Top:
                    topLineQueue.Enqueue(type);
                    break; 
                case LineType.Middle:
                    middleLineQueue.Enqueue(type);
                    break;
                case LineType.Bottom:
                    bottomLineQueue.Enqueue(type);
                    break;
            }
        }
    }

    public void InitialSpawnEnemy()
    {
        // 적인 경우에만 실행
        if (player == PlayerDefine.Player)
        {
            return;
        }
        // 앞 : 오크 3, 궁병 1
        // 뒤 : 오크 5, 궁병 3
        // 센터 : 파괴자
        // 앞열
        //SpawnUnitPosition(UnitType.Orc, frontTopPoint, LineType.Top, SpawnType.IniInitial, 3);
        //SpawnUnitPosition(UnitType.BoneArcher, frontTopPoint, LineType.Top, SpawnType.IniInitial, 1);
        //SpawnUnitPosition(UnitType.Orc, frontMiddlePoint, LineType.Middle, SpawnType.IniInitial, 3);
        //SpawnUnitPosition(UnitType.BoneArcher, frontMiddlePoint, LineType.Middle, SpawnType.IniInitial, 1);
        //SpawnUnitPosition(UnitType.Orc, frontBottomPoint, LineType.Bottom, SpawnType.IniInitial, 3);
        //SpawnUnitPosition(UnitType.BoneArcher, frontBottomPoint, LineType.Bottom, SpawnType.IniInitial, 1);
        //SpawnUnitPosition(UnitType.Destroyer, frontTopPoint, LineType.Top, SpawnType.IniInitial, 1);
        SpawnUnitPosition(UnitType.Tower, frontTopPoint, LineType.Top, SpawnType.IniInitial, 1);
        SpawnUnitPosition(UnitType.Door, backTopPoint, LineType.Top, SpawnType.IniInitial, 1);

        // 뒷열
        //SpawnUnitPosition(UnitType.Orc, backTopPoint, LineType.Top, SpawnType.IniInitial, 5);
        //SpawnUnitPosition(UnitType.BoneArcher, backTopPoint, LineType.Top, SpawnType.IniInitial, 3);
        //SpawnUnitPosition(UnitType.Orc, backMiddlePoint, LineType.Middle, SpawnType.IniInitial, 5);
        //SpawnUnitPosition(UnitType.BoneArcher, backMiddlePoint, LineType.Middle, SpawnType.IniInitial, 3);
        //SpawnUnitPosition(UnitType.Orc, backBottomPoint, LineType.Bottom, SpawnType.IniInitial, 5);
        //SpawnUnitPosition(UnitType.BoneArcher, backBottomPoint, LineType.Bottom, SpawnType.IniInitial, 3);

        // 스폰포인트
        //SpawnUnitPosition(UnitType.Destroyer, spawnLocation, LineType.Middle, SpawnType.IniInitial, 1);
    }

    public void SpawnUnitPosition(UnitType type, Transform spawnPosition, LineType line, SpawnType spawnType,int count)
    {
        GameObject prefab;
        string prefabPath = "";
        GameObject parentLine = null;
        //Quaternion quaternion = Quaternion.identity;

        switch (type)
        {
            case UnitType.Orc:
                prefabPath = "Prefabs/Unit/Unit_Orc";
                break;
            case UnitType.BoneArcher:
                prefabPath = "Prefabs/Unit/Unit_BoneArcher";
                break;
            case UnitType.Destroyer:
                prefabPath = "Prefabs/Unit/Unit_Destroyer";
                break;
            case UnitType.Door:
                prefabPath = "Prefabs/Building/Door";
                break;
            case UnitType.Tower:
                prefabPath = "Prefabs/Building/Tower";
                break;
        }

        switch (line)
        {
            case LineType.Top:
                parentLine = topLine;
                break; 
            case LineType.Middle: 
                parentLine = middleLine;
                break; 
            case LineType.Bottom: 
                parentLine = bottomLine;
                break;
        }

        prefab = Resources.Load<GameObject>(prefabPath);
        for(int i = 0; i < count; i++)
        {
            Vector3 randomRange = new Vector3(2.0f, 2.0f, 2.0f);
            Vector3 randomOffset = new Vector3(
                Random.Range(-randomRange.x, randomRange.x),
                0,
                Random.Range(-randomRange.z, randomRange.z)
            );

            //GameObject unit = Instantiate(prefab, spawnPosition.position + randomOffset, quaternion);
            GameObject unit = Instantiate(prefab);
            unit.transform.position = spawnPosition.position + randomOffset;
            unit.transform.rotation = Quaternion.Euler(0, -90, 0); 
            if(type == UnitType.Tower)
            {
                unit.transform.position = spawnPosition.position + new Vector3(10, 0, 0);
            }
            if(type == UnitType.Door)
            {
                unit.transform.position = spawnPosition.position;
                unit.transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            unit.GetComponent<EnemyStatus>().Initialize(type, line, spawnType, null, finalTarget);
            unit.transform.SetParent(parentLine.transform);
        }
    }

    public void SpawnPlayerUnit(int slotNum)
    {
        UnitType type = (UnitType)System.Enum.ToObject(typeof(UnitType), slotNum);
        LineType line = 0;

        string outpost_name = GameManager.gm_instance.GetPresentOutPost().name;

        if(outpost_name == null)
        {
            return;
        }

        switch (outpost_name)
        {
            case "Outpost_Top":
                line = LineType.Top;
                break;
            case "Outpost_Middle":
                line = LineType.Middle;
                break;
            case "Outpost_Bottom":
                line = LineType.Bottom;
                break;
        }

        if (GameManager.gm_instance.GetPresentOutPost())
        {
            ChangeUnitTypeSpawn(line, type);
        }
    } 

    private void ChangeUnitTypeSpawn(LineType line, UnitType type)
    {
        string prefabPath = "";

        curOutpost = GameManager.gm_instance.GetPresentOutPost();

        if(line == LineType.Top)
        {
            outpostTarget = outpostTop.gameObject;
        }
        else if(line == LineType.Middle)
        {
            outpostTarget = outpostMiddle.gameObject;
        }
        else if(line == LineType.Bottom)
        {
            outpostTarget = outpostBottom.gameObject;
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
        GameObject instance = Instantiate(prefab, spawnLocation);
        //instance.GetComponent<UnitStatus>().SetUnitType(UnitType.Warrior);
        instance.GetComponent<UnitStatus>().Initialize(type, line, SpawnType.Spawn, outpostTarget, finalTarget);

        UnitManager.um_instance.RegistUnit(instance, type);

    }

    public void AutoSpawnEnemyUnit()
    {
        Debug.Log("topQueue : " + topLineQueue.Count);
        if (player == PlayerDefine.Player)
        {
            return;
        }

        // 해당 라인 유닛 수 체크, 큐에 유닛 등록해두고 유닛 수 줄어들때 마다 생성
        CheckLineUnitNum();
        SpawnUnitLine();
    }

    public void CheckLineUnitNum()
    {
        topLineUnitNum = UnitManager.um_instance.CheckLineNum(LineType.Top);
        middleLineUnitNum = UnitManager.um_instance.CheckLineNum(LineType.Middle);
        bottomLineUnitNum = UnitManager.um_instance.CheckLineNum(LineType.Bottom);
    }

    public void SpawnUnitLine()
    {
        if (topLineUnitNum < 10 && topLineQueue.Count > 0)
        {
            UnitType type = topLineQueue.Dequeue();
            SpawnUnitPosition(type, spawnLocation, LineType.Top, SpawnType.Spawn, 1);

        }

        if (middleLineUnitNum < 10 && middleLineQueue.Count > 0)
        {
            UnitType type = middleLineQueue.Dequeue();
            SpawnUnitPosition(type, spawnLocation, LineType.Middle, SpawnType.Spawn, 1);
        }

        if (bottomLineUnitNum < 10 && bottomLineQueue.Count > 0)
        {
            UnitType type = bottomLineQueue.Dequeue();
            SpawnUnitPosition(type, spawnLocation, LineType.Bottom, SpawnType.Spawn, 1);
        }
    }
}
