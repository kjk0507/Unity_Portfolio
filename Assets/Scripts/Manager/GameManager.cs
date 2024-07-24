using EnumStruct;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager gm_instance;
    public GameObject presentOutpost;
    public int ruby; // 상점 재화
    public int gold; // 인게임 생성 재화
    public int extraGold; // 추가 골드
    public bool isPlay = false;

    public GameObject topOutPost;
    public GameObject middleOutPost;
    public GameObject bottomOutPost;

    // 타워 정보
    public int beforOutPostNum = 0;
    public int outPostNum;
    public int killCount;
    public int deathCount;

    // 중요 건물
    public GameObject playerCenter;
    public GameObject enemyCenter;
    public int playerMainTarget = 3;
    public int enemyMainTarget = 3;
    public bool isPlayerCanAttack = false;
    public bool isEnemyCanAttack = false;

    // 본성 체력바
    public GameObject playerCastleHp;
    public GameObject EnemyCastlehp;

    // 게임 재시작 여부
    public bool isRestart = false;


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
        
    }

    private void Update()
    {
        UpdateTowerInfo();
        CheckCenterAttacking();
        CheckGameStart();
        CheckCastleDestroy();
    }

    public void CheckGameStart()
    {
        if(!isPlay && UIManager.um_instance.curState == UIState.Play)
        {
            Debug.Log("test");
            isPlay = true;
            StartCoroutine(AddGoldRoutine());
        }

    }

    public void CheckCenterAttacking()
    {
        if(playerMainTarget == 0 && !isPlayerCanAttack)
        {
            isPlayerCanAttack = true;
            enemyCenter.layer = LayerMask.NameToLayer("Enemy");
            EnemyCastlehp.SetActive(true);
        }

        if (enemyMainTarget == 0 && !isEnemyCanAttack)
        {
            isEnemyCanAttack = true;
            playerCenter.layer = LayerMask.NameToLayer("Player");
            playerCastleHp.SetActive(true);
        }
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
            UIManager.um_instance.ChangeTowerInfo();
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

    public void CheckCastleDestroy()
    {
        if (isRestart)
        {
            return;
        }

        if(playerCenter.GetComponent<InheriteStatus>().status.curHp <= 0)
        {
            isRestart = true;
            UIManager.um_instance.ChangeUIState(3);
            StartCoroutine(RestartGame());
        }

        if (enemyCenter.GetComponent<InheriteStatus>().status.curHp <= 0)
        { 
            isRestart = true;
            UIManager.um_instance.ChangeUIState(4);
            StartCoroutine(RestartGame());
        }
    }
    private IEnumerator RestartGame()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

}
