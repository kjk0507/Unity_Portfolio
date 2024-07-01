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

    void Start()
    {

    }

    void Update()
    {
        CheckClickOutpost();
        CheckUiArrow();
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
                Debug.Log("Clicked on: " + hit.collider.name);
                Debug.Log("Clicked on: " + hit.collider.gameObject.transform.root.gameObject.name);

                clickedObject = hit.collider.gameObject.transform.root.gameObject;

                // 클릭된 오브젝트에서 특정 스크립트를 가져와서 메서드 호출
                //var clickable = hit.collider.gameObject.GetComponent<IClickable>();
                //if (clickable != null)
                //{
                //    clickable.OnClick();
                //}

                GameManager.gm_instance.SetPresentOutPost(clickedObject);
                clickedOutpost.text = clickedObject.name.ToString();
            }

        }

    }

    public void MoveLeft()
    {
        if(cameraObject.transform.position.x > -70)
        {
            cameraObject.transform.position = cameraObject.transform.position - new Vector3(cameraMoveSpeed * Time.deltaTime, 0, 0);
        }
    }

    public void MoveRight()
    {
        if (cameraObject.transform.position.x < 50)
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
}
