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

    // 프리팹 위치
    //public string prefabPath; 

    void Start()
    {
        InstantiateSpawnEnemy();
    }

    void Update()
    {

    }

    public void InstantiateSpawnEnemy()
    {
        // 적인 경우에만 실행
        if (player != PlayerDefine.Enemy)
        {
            return;
        }
        // 앞 : 오크 3, 궁병 1
        // 뒤 : 오크 5, 궁병 3
        // 센터 : 파괴자
        // 앞열
        SpawnUnitPosition(EnemyUnitType.Orc, frontTopPoint, LineType.Top, 3);
        SpawnUnitPosition(EnemyUnitType.BoneArcher, frontTopPoint, LineType.Top, 1);
        //SpawnUnitPosition(EnemyUnitType.Orc, frontMiddlePoint, LineType.Middle, 3);
        //SpawnUnitPosition(EnemyUnitType.BoneArcher, frontMiddlePoint, LineType.Middle, 1);
        //SpawnUnitPosition(EnemyUnitType.Orc, frontBottomPoint, LineType.Bottom, 3);
        //SpawnUnitPosition(EnemyUnitType.BoneArcher, frontBottomPoint, LineType.Bottom, 1);

        // 뒷열
        //SpawnUnitPosition(EnemyUnitType.Orc, backTopPoint, LineType.Top, 5);
        //SpawnUnitPosition(EnemyUnitType.BoneArcher, backTopPoint, LineType.Top, 3);
        //SpawnUnitPosition(EnemyUnitType.Orc, backMiddlePoint, LineType.Middle, 5);
        //SpawnUnitPosition(EnemyUnitType.BoneArcher, backMiddlePoint, LineType.Middle, 3);
        //SpawnUnitPosition(EnemyUnitType.Orc, backBottomPoint, LineType.Bottom, 5);
        //SpawnUnitPosition(EnemyUnitType.BoneArcher, backBottomPoint, LineType.Bottom, 3);

        // 스폰포인트
        SpawnUnitPosition(EnemyUnitType.Destroyer, spawnLocation, LineType.Middle, 1);
    }

    public void SpawnUnitPosition(EnemyUnitType type, Transform spawnPosition, LineType line, int count)
    {
        GameObject prefab;
        string prefabPath = "";
        GameObject parentLine = null;
        //Quaternion quaternion = Quaternion.identity;

        switch (type)
        {
            case EnemyUnitType.Orc:
                prefabPath = "Prefabs/Unit/Unit_Orc";
                break;
            case EnemyUnitType.BoneArcher:
                prefabPath = "Prefabs/Unit/Unit_BoneArcher";
                break;
            case EnemyUnitType.Destroyer:
                prefabPath = "Prefabs/Unit/Unit_Destroyer";
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
            unit.transform.rotation = Quaternion.Euler(0, -90, 0); ;
            unit.GetComponent<EnemyStatus>().Initialize(type, line, null, finalTarget);
            unit.transform.SetParent(parentLine.transform);
        }
    }

    public void SpawnPlayerUnit(int slotNum)
    {
        UnitType type = (UnitType)System.Enum.ToObject(typeof(UnitType), slotNum);
        LineType line = 0;

        string outpost_name = GameManager.gm_instance.GetPresentOutPost().name;

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

    private void SpawnEnemyUnit()
    {

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
        instance.GetComponent<UnitStatus>().Initialize(type, line, outpostTarget, finalTarget);

        UnitManager.um_instance.RegistUnit(instance, type);

    }
}
