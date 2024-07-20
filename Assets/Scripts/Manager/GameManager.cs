using EnumStruct;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gm_instance;
    public GameObject presentOutpost;
    public int ruby; // 상점 재화
    public int gold; // 인게임 생성 재화
    public int extraGold; // 추가 골드

    public GameObject topOutPost;
    public GameObject middleOutPost;
    public GameObject bottomOutPost;

    // 타워 정보
    public int beforOutPostNum = 0;
    public int outPostNum;
    public int killCount;
    public int deathCount;

    void Awake()
    {
        if (gm_instance == null)
        {
            gm_instance = this;
        }
        else if (gm_instance != this)
        {
            Destroy(gameObject);
        }

        // 이 오브젝트를 씬 전환 시에도 유지하도록 설정
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartCoroutine(AddGoldRoutine()); // 골드 획득 코루틴
    }

    private void Update()
    {
        UpdateTowerInfo();
    }

    public GameObject GetPresentOutPost()
    {
        return presentOutpost;
    }

    public void SetPresentOutPost(LineType line)
    {
        //presentOutpost = clickObject;
        switch (line)
        {
            case LineType.Top:
                presentOutpost = topOutPost;
                break;
            case LineType.Middle:
                presentOutpost = middleOutPost;
                break;
            case LineType.Bottom:
                presentOutpost = bottomOutPost;
                break;
        }
    }

    public void AddGold()
    {
        //gold++;
        gold = gold + 1 + extraGold + outPostNum * 10;
        //Debug.Log("Gold: " + gold);
    }

    private IEnumerator AddGoldRoutine()
    {
        while (true)
        {
            // 1초 대기
            yield return new WaitForSeconds(1f);
            // gold 증가
            AddGold();
        }
    }

    public void UpgradeExtraGold()
    {
        extraGold++;
    }

    public void UseGold(int cost)
    {
        gold -= cost;
    }

    public int GetGold()
    {
        return gold;
    }

    public void UpdateTowerInfo()
    {
        if(beforOutPostNum != outPostNum)
        {
            beforOutPostNum = outPostNum;
            UIManager.UM_instance.ChangeTowerInfo();
        }
    }

    public void AddKillCount()
    {
        killCount++;
    }

    public void AddDeathCount()
    {
        deathCount++;
    }

    public void AddOutPostNum()
    {
        outPostNum++;
    }

    public void RemoveOutPostNum()
    {
        if(outPostNum > 0)
        {
            outPostNum--;
        }
    }

    public int GetKillCount()
    {
        return killCount;
    }

    public int GetDeathCount()
    {
        return deathCount;
    }

    public int GetOutPostNum()
    {
        return outPostNum;
    }
}
