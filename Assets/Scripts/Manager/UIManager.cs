using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI clickedOutpost;

    // 카메라 이동 관련
    public GameObject cameraObject;
    bool arrowLeft = false;
    bool arrowRight = false;
    public float cameraMoveSpeed = 30f;

    // 게임 돈관련
    public TextMeshProUGUI curGold;

    void Start()
    {

    }

    void Update()
    {
        CheckClickOutpost();
        CheckUiArrow();
        CheckCurGold();
    }

    private void CheckClickOutpost()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject clickedObject;

            // 카메라에서 마우스 클릭 위치로 레이캐스트 쏘기
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 레이캐스트가 충돌한 오브젝트 출력
                //Debug.Log("Clicked on: " + hit.collider.name);
                //Debug.Log("Clicked on: " + hit.collider.gameObject.transform.root.gameObject.name);

                //clickedObject = hit.collider.gameObject.transform.root.gameObject;
                clickedObject = hit.collider.gameObject;

                // 클릭된 오브젝트에서 특정 스크립트를 가져와서 메서드 호출
                //var clickable = hit.collider.gameObject.GetComponent<IClickable>();
                //if (clickable != null)
                //{
                //    clickable.OnClick();
                //}

                if(clickedObject.tag == "Outpost")
                {
                    GameManager.gm_instance.SetPresentOutPost(clickedObject);
                    clickedOutpost.text = clickedObject.name.ToString();
                }
            }
        }

        // todo 클릭시 오브젝트 테두리 활성화
        // 유닛 클릭시 공격 범위 표시
        // 상태창 표시?

    }

    public void MoveLeft()
    {
        if(cameraObject.transform.position.x > -80)
        {
            cameraObject.transform.position = cameraObject.transform.position - new Vector3(cameraMoveSpeed * Time.deltaTime, 0, 0);
        }
    }

    public void MoveRight()
    {
        if (cameraObject.transform.position.x < 65)
        {
            cameraObject.transform.position = cameraObject.transform.position + new Vector3(cameraMoveSpeed * Time.deltaTime, 0, 0);
        }
    }

    public void PressLeftArrow()
    {
        arrowLeft = true;
    }

    public void UpLeftArrow()
    {
        arrowLeft = false;
    }

    public void PressRightArrow()
    {
        arrowRight = true;
    }

    public void UpRightArrow()
    {
        arrowRight = false;
    }

    public void CheckUiArrow()
    {
        if(arrowLeft)
        {
            MoveLeft();
        }

        if (arrowRight)
        {
            MoveRight();
        }
    }

    public void CheckCurGold()
    {
        curGold.text = GameManager.gm_instance.GetGold().ToString();
    }
}
