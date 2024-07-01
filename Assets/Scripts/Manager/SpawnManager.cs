using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumStruct;
using UnitStatusStruct;

public class SpawnManager : MonoBehaviour
{
    public PlayerDefine player;
    public GameObject spawnLocation;
    [SerializeField] private GameObject outpost;

    //public bool spawntest = false;

    // 프리팹 위치
    public string prefabPath = "Prefabs/Unit/Unit_Warrior";

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
        //if (spawntest)
        //{
        //    return;
        //}
        //UnitType type = (UnitType)System.Enum.ToObject(typeof(UnitType), slotNum);
        //Debug.Log("spawn : " + type.ToString());
        Debug.Log("test" + slotNum);
        if (GameManager.gm_instance.GetPresentOutPost())
        {
            outpost = GameManager.gm_instance.GetPresentOutPost();
            GameObject prefab = Resources.Load<GameObject>(prefabPath);
            GameObject instance = Instantiate(prefab);
            instance.GetComponent<UnitStatus>().SetUnitType(UnitType.Warrior);

            //spawntest = true;
        }
    }

    private void SpawnEnemyUnit()
    {

    }
}
