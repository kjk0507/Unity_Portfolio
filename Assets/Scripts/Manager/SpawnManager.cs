using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumStruct;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager sm_instance;
    public PlayerDefine player;
    public Transform spawnLocationPlayer;
    public Transform spawnLocationEnemy;
    public GameObject outpostTarget;

    [SerializeField] private GameObject curOutpost;

    public GameObject finalTargetPlayer;
    public GameObject finalTargetEnemy;
    //public bool spawntest = false;

    //public bool isEnemy = false; // true : 플레이어 스폰매니저, false : 적 스폰매니저

    // 아군 스폰 목적지 위치
    public Transform point_00;
    public Transform point_10;
    public Transform point_20;

    // 적 스폰 목적지 위치
    public Transform point_01;
    public Transform point_02;
    public Transform point_03;
    public Transform point_11;
    public Transform point_12;
    public Transform point_13;
    public Transform point_21;
    public Transform point_22;
    public Transform point_23;

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

    // 아군 경로 큐
    Queue<Transform> topRoute = new Queue<Transform>();
    Queue<Transform> middleRoute = new Queue<Transform>();
    Queue<Transform> bottomRoute = new Queue<Transform>();

    void Start()
    {
        if (sm_instance == null)
        {
            sm_instance = this;
        }
        else if (sm_instance != this)
        {
            Destroy(gameObject);
        }

        InitialRegistQueue();
        InitialSpawnEnemy();
        InvokeRepeating("AutoSpawnEnemyUnit", 0f, 1f);
    }

    void Update()
    {
        //AutoSpawnEnemyUnit();
    }

    public void InitialRegistQueue()
    {
        //if (player == PlayerDefine.Player)
        //{
        //    return;
        //}

        // 윗 라인
        RegistUnitQueue(UnitType.Orc, LineType.Top, 3);
        RegistUnitQueue(UnitType.BoneArcher, LineType.Top, 2);
        RegistUnitQueue(UnitType.Orc, LineType.Top, 3);
        RegistUnitQueue(UnitType.BoneArcher, LineType.Top, 2);
        RegistUnitQueue(UnitType.Destroyer, LineType.Top, 1);

        // 중간 라인
        RegistUnitQueue(UnitType.Orc, LineType.Middle, 3);
        RegistUnitQueue(UnitType.BoneArcher, LineType.Middle, 2);
        RegistUnitQueue(UnitType.Orc, LineType.Middle, 3);
        RegistUnitQueue(UnitType.BoneArcher, LineType.Middle, 2);
        RegistUnitQueue(UnitType.Destroyer, LineType.Middle, 1);

        // 밑 라인
        RegistUnitQueue(UnitType.Orc, LineType.Bottom, 3);
        RegistUnitQueue(UnitType.BoneArcher, LineType.Bottom, 2);
        RegistUnitQueue(UnitType.Orc, LineType.Bottom, 3);
        RegistUnitQueue(UnitType.BoneArcher, LineType.Bottom, 2);
        RegistUnitQueue(UnitType.Destroyer, LineType.Bottom, 1);
    }

    public void RegistUnitQueue(UnitType type, LineType line, int count)
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
        //if (player == PlayerDefine.Player)
        //{
        //    return;
        //}

        // 아군 코어
        SpawnUnitPosition(UnitType.Core, point_00, LineType.Top, SpawnType.Initial, 1);
        SpawnUnitPosition(UnitType.Core, point_10, LineType.Middle, SpawnType.Initial, 1);
        SpawnUnitPosition(UnitType.Core, point_20, LineType.Bottom, SpawnType.Initial, 1);

        // 01열        
        SpawnUnitPosition(UnitType.Orc, point_01, LineType.Top, SpawnType.Initial, 3);
        SpawnUnitPosition(UnitType.BoneArcher, point_01, LineType.Top, SpawnType.Initial, 1);

        SpawnUnitPosition(UnitType.Orc, point_11, LineType.Middle, SpawnType.Initial, 3);
        SpawnUnitPosition(UnitType.BoneArcher, point_11, LineType.Middle, SpawnType.Initial, 1);

        SpawnUnitPosition(UnitType.Orc, point_21, LineType.Bottom, SpawnType.Initial, 3);
        SpawnUnitPosition(UnitType.BoneArcher, point_21, LineType.Bottom, SpawnType.Initial, 1);

        // 02열
        SpawnUnitPosition(UnitType.Tower, point_02, LineType.Top, SpawnType.Initial, 1);
        SpawnUnitPosition(UnitType.Orc, point_02, LineType.Top, SpawnType.Initial, 2);
        SpawnUnitPosition(UnitType.BoneArcher, point_02, LineType.Top, SpawnType.Initial, 2);

        SpawnUnitPosition(UnitType.Tower, point_12, LineType.Middle, SpawnType.Initial, 1);
        SpawnUnitPosition(UnitType.Orc, point_12, LineType.Middle, SpawnType.Initial, 2);
        SpawnUnitPosition(UnitType.BoneArcher, point_12, LineType.Middle, SpawnType.Initial, 2);

        SpawnUnitPosition(UnitType.Tower, point_22, LineType.Bottom, SpawnType.Initial, 1);
        SpawnUnitPosition(UnitType.Orc, point_22, LineType.Bottom, SpawnType.Initial, 2);
        SpawnUnitPosition(UnitType.BoneArcher, point_22, LineType.Bottom, SpawnType.Initial, 2);

        // 03열
        SpawnUnitPosition(UnitType.Door, point_03, LineType.Top, SpawnType.Initial, 1);
        SpawnUnitPosition(UnitType.Door, point_13, LineType.Middle, SpawnType.Initial, 1);
        SpawnUnitPosition(UnitType.Door, point_23, LineType.Bottom, SpawnType.Initial, 1);

        // 스폰포인트
        //SpawnUnitPosition(UnitType.Destroyer, point_01, LineType.Middle, SpawnType.Initial, 1);
    }

    public void RegistRoute(Queue<Transform> route, LineType type)
    {
        switch (type)
        {
            case LineType.Top:
                //route.Enqueue(point_00);
                route.Enqueue(point_01);
                route.Enqueue(point_02);
                route.Enqueue(point_03);
                break;
            case LineType.Middle:
                //route.Enqueue(point_10);
                route.Enqueue(point_11);
                route.Enqueue(point_12);
                route.Enqueue(point_13);
                break;
            case LineType.Bottom:
                //route.Enqueue(point_20);
                route.Enqueue(point_21);
                route.Enqueue(point_22);
                route.Enqueue(point_23);
                break;

        }
    }

    public void SpawnUnitPosition(UnitType type, Transform spawnPosition, LineType line, SpawnType spawnType, int count)
    {
        GameObject prefab;
        string prefabPath = "";
        GameObject parentLine = null;
        //Quaternion quaternion = Quaternion.identity;
        GameObject outPostPoint = null;

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
            case UnitType.Core:
                prefabPath = "Prefabs/Building/Core";
                break;
        }

        switch (line)
        {
            case LineType.Top:
                parentLine = topLine;
                outPostPoint = point_03.gameObject;
                break; 
            case LineType.Middle: 
                parentLine = middleLine;
                outPostPoint = point_13.gameObject;
                break; 
            case LineType.Bottom: 
                parentLine = bottomLine;
                outPostPoint = point_23.gameObject;
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
                unit.transform.position = spawnPosition.position + new Vector3(-20, 0, 0);
            }
            else if(type == UnitType.Door)
            {
                unit.transform.position = spawnPosition.position + new Vector3(-25, 0, 0); ;
                unit.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (type == UnitType.Core)
            {
                unit.transform.position = spawnPosition.position + new Vector3(-15, 0, 0);
            }

            unit.GetComponent<InheriteStatus>().Initialize(type, line, spawnType, null, outPostPoint, finalTargetEnemy);
            unit.transform.SetParent(parentLine.transform);
        }
    }

    public void SpawnPlayerUnit(int slotNum)
    {
        UnitType type = (UnitType)System.Enum.ToObject(typeof(UnitType), slotNum);
        LineType line = 0;

        if(GameManager.gm_instance.GetGold() < GetCost(type))
        {
            UIManager.um_instance.ShowMessage("골드가 부족합니다");
            return;
        }

        if(GameManager.gm_instance.GetPresentOutPost() == null)
        {
            UIManager.um_instance.ShowMessage("라인을 선택해주십시오");
            return;
        }

        GameManager.gm_instance.UseGold(GetCost(type));

        string outpost_name = GameManager.gm_instance.GetPresentOutPost().name;

        switch (outpost_name)
        {
            case "Point_00":
                line = LineType.Top;
                break;
            case "Point_10":
                line = LineType.Middle;
                break;
            case "Point_20":
                line = LineType.Bottom;
                break;
        }

        if (GameManager.gm_instance.GetPresentOutPost())
        {
            ChangeUnitTypeSpawn(line, type);
        }
    }

    public int GetCost(UnitType type)
    {
        int result = -1;

        switch (type)
        {
            case UnitType.Warrior:
                result = 10;
                break;
            case UnitType.Archer:
                result = 50;
                break;
            case UnitType.Wizard:
                result = 100;
                break;
        }

        return result;
    }

    private void ChangeUnitTypeSpawn(LineType line, UnitType type)
    {
        string prefabPath = "";

        curOutpost = GameManager.gm_instance.GetPresentOutPost();

        if(line == LineType.Top)
        {
            //outpostTarget = outpostTop.gameObject;
            outpostTarget = point_00.gameObject;
        }
        else if(line == LineType.Middle)
        {
            //outpostTarget = outpostMiddle.gameObject;
            outpostTarget = point_10.gameObject;
        }
        else if(line == LineType.Bottom)
        {
            //outpostTarget = outpostBottom.gameObject;
            outpostTarget = point_20.gameObject;
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
        GameObject instance = Instantiate(prefab, spawnLocationPlayer);
        //instance.GetComponent<UnitStatus>().SetUnitType(UnitType.Warrior);
        instance.GetComponent<UnitStatus>().Initialize(type, line, SpawnType.Spawn, null, outpostTarget, finalTargetPlayer);

        switch (line)
        {
            case LineType.Top:
                instance.transform.SetParent(topLine.transform);
                break;
            case LineType.Middle:
                instance.transform.SetParent(middleLine.transform);
                break;
            case LineType.Bottom:
                instance.transform.SetParent(bottomLine.transform);
                break;
        }

        UnitManager.um_instance.RegistUnit(instance, type);

    }

    public void AutoSpawnEnemyUnit()
    {
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
        if (topLineUnitNum < 5 && topLineQueue.Count > 0)
        {
            UnitType type = topLineQueue.Dequeue();
            SpawnUnitPosition(type, spawnLocationEnemy, LineType.Top, SpawnType.Spawn, 1);

        }

        if (middleLineUnitNum < 5 && middleLineQueue.Count > 0)
        {
            UnitType type = middleLineQueue.Dequeue();
            SpawnUnitPosition(type, spawnLocationEnemy, LineType.Middle, SpawnType.Spawn, 1);
        }

        if (bottomLineUnitNum < 5 && bottomLineQueue.Count > 0)
        {
            UnitType type = bottomLineQueue.Dequeue();
            SpawnUnitPosition(type, spawnLocationEnemy, LineType.Bottom, SpawnType.Spawn, 1);
        }
    }
}
