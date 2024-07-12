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

    public GameObject topOutPost;
    public GameObject middleOutPost;
    public GameObject bottomOutPost;

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
        gold++;
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

    public int GetGold()
    {
        return gold;
    }
}
