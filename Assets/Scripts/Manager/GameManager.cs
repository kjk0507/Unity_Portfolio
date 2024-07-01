using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gm_instance;
    public GameObject presentOutpost;

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

    public GameObject GetPresentOutPost()
    {
        return presentOutpost;
    }

    public void SetPresentOutPost(GameObject clickObject)
    {
        presentOutpost = clickObject;
    }
}
