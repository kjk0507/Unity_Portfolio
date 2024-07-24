using EnumStruct;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnitStatusStruct;
using static UnityEngine.Rendering.DebugUI;

public class UIManager : MonoBehaviour
{
    public static UIManager um_instance;

    // UI 순서
    public UIState curState;
    public GameObject titleUi;
    public GameObject mainUi;
    public GameObject playUi;
    public GameObject gameoverUi;
    public GameObject theEndUi;
    public GameObject gameField;

    public TextMeshProUGUI clickedOutpost;

    // 카메라 이동 관련
    public GameObject cameraObject;
    bool arrowLeft = false;
    bool arrowRight = false;
    public float cameraMoveSpeed = 30f;

    // 게임 돈관련
    public TextMeshProUGUI curGold;

    // 상태창 관련
    public GameObject playerInfo;
    public bool isStatusOpen = false;
    // 타워 정보
    public GameObject towerStatus;
    public TextMeshProUGUI outPostNum; // 점령지 수
    public TextMeshProUGUI killCount;
    public TextMeshProUGUI deathCount;
    public TextMeshProUGUI goldChange;

    // 유닛 정보
    public GameObject unitStatus;
    public TextMeshProUGUI unitName;
    public TextMeshProUGUI damageType;
    public TextMeshProUGUI attackRange;
    public TextMeshProUGUI attackSpeed;

    public TextMeshProUGUI hpChange;
    public TextMeshProUGUI hpUpgradeCost;
    public TextMeshProUGUI attackChange;
    public TextMeshProUGUI attackUpgradeCost;
    public TextMeshProUGUI defenceChange;
    public TextMeshProUGUI defencdUpgradeCost;
    public TextMeshProUGUI speedChange;
    public TextMeshProUGUI speedUpgradeCost;

    public int uIUnitTypeNum;
    public UnitType curUnitType;

    // 메시지 관련
    public GameObject messageObj;
    public TextMeshProUGUI curMessage;
    public Coroutine fadeOutCoroutine;



    void Start()
    {
        if (um_instance == null)
        {
            um_instance = this;
        }
        else if (um_instance != this)
        {
            Destroy(gameObject);
        }

        ChangeUIState(0);
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

                //if(clickedObject.tag == "Outpost")
                //{
                //    GameManager.gm_instance.SetPresentOutPost(clickedObject);
                //    clickedOutpost.text = clickedObject.name.ToString();
                //}

                int outPostLayerMask = LayerMask.NameToLayer("OutPost_Check");
                int outPostButtonLayerMask = LayerMask.NameToLayer("OutPost_Button");

                if (clickedObject.layer == outPostLayerMask)
                {
                    string name = clickedObject.transform.parent.name;
                    if(name == "Point_00" || name == "Point_01" || name == "Point_02" || name == "Point_03")
                    {
                        GameManager.gm_instance.SetPresentOutPost(LineType.Top);
                        clickedOutpost.text = "상단";
                    }
                    else if (name == "Point_10" || name == "Point_11" || name == "Point_12" || name == "Point_13")
                    {
                        GameManager.gm_instance.SetPresentOutPost(LineType.Middle);
                        clickedOutpost.text = "중단";
                    }
                    else if (name == "Point_20" || name == "Point_21" || name == "Point_22" || name == "Point_23")
                    {
                        GameManager.gm_instance.SetPresentOutPost(LineType.Bottom);
                        clickedOutpost.text = "하단";
                    }
                }

                Debug.Log("ClickObject : " + clickedObject.layer);
                Debug.Log("layermask : " + outPostButtonLayerMask);


                if (clickedObject.layer == outPostButtonLayerMask)
                {
                    GameObject checkObj = clickedObject.transform.parent.parent.parent.gameObject;
                    Debug.Log("parent : " + checkObj);
                    OutPostState outPostState = checkObj.GetComponent<OutpostControl>().CheckOutPostState();

                    switch (outPostState)
                    {
                        case OutPostState.InActive:
                            break;
                        case OutPostState.Move:
                            checkObj.GetComponent<OutpostControl>().ChangeOutPostState(OutPostState.Wait);
                            break;
                        case OutPostState.Wait:
                            checkObj.GetComponent<OutpostControl>().ChangeOutPostState(OutPostState.Move);
                            break;
                    }
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

    public void ClickedStatusTypeButton(int type)
    {
        if (isStatusOpen)
        {
            CloseStatusTypeButton();
            return;
        }

        isStatusOpen = true;
        playerInfo.SetActive(true);
        UIStatusType sType = (UIStatusType)type;
        ChangeUIType(sType);
    }

    public void ChangeUIType(UIStatusType type)
    {
        switch (type)
        {
            case UIStatusType.Tower:
                towerStatus.SetActive(true);
                unitStatus.SetActive(false);
                ChangeTowerInfo();
                break;
            case UIStatusType.Unit:
                towerStatus.SetActive(false);
                unitStatus.SetActive(true);
                ChangeUnitInfo(0);
                break;
        }
    }

    public void CloseStatusTypeButton()
    {
        isStatusOpen = false;
        playerInfo.SetActive(false);
    }

    public void ChangeTowerInfo()
    {
        outPostNum.text = "점령지 수 : " + GameManager.gm_instance.GetOutPostNum().ToString() + " / 9";
        killCount.text = "처치 수 : " + GameManager.gm_instance.GetKillCount().ToString();
        deathCount.text = "용병 고용 수 : " + GameManager.gm_instance.GetDeathCount().ToString();

        goldChange.text = "(기본) 1\n(강화) " + GameManager.gm_instance.extraGold + "\n(점령지) " + GameManager.gm_instance.GetOutPostNum().ToString() + " * 10" ;
    }

    public void ChangeUnitInfo(int type)
    {
        uIUnitTypeNum = type;
        UIUnitType sType = (UIUnitType)type;
        UnitType uType = UnitType.Warrior;

        Status status = null;
        switch (sType)
        {
            case UIUnitType.Unit0:
                status = new Status(UnitType.Warrior);
                uType = UnitType.Warrior;
                curUnitType = UnitType.Warrior;
                break; 
            case UIUnitType.Unit1:
                status = new Status(UnitType.Archer);
                uType = UnitType.Archer;
                curUnitType = UnitType.Archer;
                break; 
            case UIUnitType.Unit2:
                status = new Status(UnitType.Wizard);
                uType = UnitType.Wizard;
                curUnitType = UnitType.Wizard;
                break;
        }

        unitName.text = status.name.ToString();
        damageType.text = (status.damageType == DamageType.Target ? "단일" : "다수");
        attackRange.text = status.attackRange.ToString();
        attackSpeed.text = status.attackSpeed.ToString();

        hpChange.text = status.defaultHp.ToString() + CheckUnitUpgradeInfo(uType, UpgradeType.Hp);
        attackChange.text = status.defaultAtk.ToString() + CheckUnitUpgradeInfo(uType, UpgradeType.Attack);
        defenceChange.text = status.defaultDef.ToString() + CheckUnitUpgradeInfo(uType, UpgradeType.Defence);
        speedChange.text = status.defaultSpeed.ToString() + CheckUnitUpgradeInfo(uType, UpgradeType.Speed);

        hpUpgradeCost.text = UnitManager.um_instance.GetUnitUpgradeCost(uType, UpgradeType.Hp).ToString();
        attackUpgradeCost.text = UnitManager.um_instance.GetUnitUpgradeCost(uType, UpgradeType.Attack).ToString();
        defencdUpgradeCost.text = UnitManager.um_instance.GetUnitUpgradeCost(uType, UpgradeType.Defence).ToString();
        speedUpgradeCost.text = UnitManager.um_instance.GetUnitUpgradeCost(uType, UpgradeType.Speed).ToString();
    }

    public string CheckUnitUpgradeInfo(UnitType type, UpgradeType uType)
    {
        string result = "";

        switch (type)
        {
            case UnitType.Warrior:
                switch (uType)
                {
                    case UpgradeType.Hp:
                        result = "+ " + UnitManager.um_instance.warrior_upgrade_hp;
                        break;
                    case UpgradeType.Attack:
                        result = "+ " + UnitManager.um_instance.warrior_upgrade_atk;
                        break;
                    case UpgradeType.Defence:
                        result = "+ " + UnitManager.um_instance.warrior_upgrade_def;
                        break;
                    case UpgradeType.Speed:
                        result = "+ " + UnitManager.um_instance.warrior_upgrade_speed;
                        break;
                }
                break;
            case UnitType.Archer:
                switch (uType)
                {
                    case UpgradeType.Hp:
                        result = "+ " + UnitManager.um_instance.archer_upgrade_hp;
                        break;
                    case UpgradeType.Attack:
                        result = "+ " + UnitManager.um_instance.archer_upgrade_atk;
                        break;
                    case UpgradeType.Defence:
                        result = "+ " + UnitManager.um_instance.archer_upgrade_def;
                        break;
                    case UpgradeType.Speed:
                        result = "+" + UnitManager.um_instance.archer_upgrade_speed;
                        break;
                }
                break;
            case UnitType.Wizard:
                switch (uType)
                {
                    case UpgradeType.Hp:
                        result = "+ " + UnitManager.um_instance.wizard_upgrade_hp;
                        break;
                    case UpgradeType.Attack:
                        result = "+ " + UnitManager.um_instance.wizard_upgrade_atk;
                        break;
                    case UpgradeType.Defence:
                        result = "+ " + UnitManager.um_instance.wizard_upgrade_def;
                        break;
                    case UpgradeType.Speed:
                        result = "+ " + UnitManager.um_instance.wizard_upgrade_speed;
                        break;
                }
                break;
        }

        return result;
    }

    public void ClickUnitUpgradeButton(int upgradNum)
    {
        UpgradeType uType = (UpgradeType)upgradNum;
        UnitManager.um_instance.ChangeUpgradeStatus(curUnitType, uType);
        ChangeUnitInfo(uIUnitTypeNum);
    }

    public void ClickTowerUpgradeButton()
    {
        int curGold = GameManager.gm_instance.GetGold();
        int cost = (1 + GameManager.gm_instance.extraGold) * 100;

        if (curGold < cost)
        {
            UIManager.um_instance.ShowMessage("골드가 부족합니다.");
            return;
        }

        GameManager.gm_instance.UpgradeExtraGold();
        ChangeTowerInfo();
    }

    public void CheckCurGold()
    {
        curGold.text = GameManager.gm_instance.GetGold().ToString();
    }

    public void ShowMessage(string message)
    {
        messageObj.gameObject.SetActive(true);
        curMessage.text = message;

        if (fadeOutCoroutine != null)
        {
            StopCoroutine(fadeOutCoroutine);
        }

        curMessage.color = new Color(curMessage.color.r, curMessage.color.g, curMessage.color.b, 1f);

        fadeOutCoroutine = StartCoroutine(FadeOutText(curMessage));
    }

    private IEnumerator FadeOutText(TextMeshProUGUI textMeshPro)
    {
        float fadeOutTime = 4f;

        float startAlpha = textMeshPro.color.a;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeOutTime)
        {
            Color newColor = textMeshPro.color;
            newColor.a = Mathf.Lerp(startAlpha, 0.0f, t);
            textMeshPro.color = newColor;
            yield return null;
        }

        messageObj.gameObject.SetActive(false);
    }

    public void ChangeUIState(int num)
    {
        UIState state = (UIState)num;

        switch (state)
        {
            case UIState.Title:
                curState = UIState.Title;
                titleUi.SetActive(true);
                mainUi.SetActive(false);
                playUi.SetActive(false);
                gameoverUi.SetActive(false);
                theEndUi.SetActive(false);
                //gameField.SetActive(false);
                break;
            case UIState.Main:
                curState = UIState.Main;
                titleUi.SetActive(false);
                mainUi.SetActive(true);
                playUi.SetActive(false);
                gameoverUi.SetActive(false);
                theEndUi.SetActive(false);
                //gameField.SetActive(false);
                break;
            case UIState.Play:
                curState = UIState.Play;
                titleUi.SetActive(false);
                mainUi.SetActive(false);
                playUi.SetActive(true);
                gameoverUi.SetActive(false);
                theEndUi.SetActive(false);
                //gameField.SetActive(true);
                break;
            case UIState.GameOver:
                curState = UIState.GameOver;
                titleUi.SetActive(false);
                mainUi.SetActive(false);
                playUi.SetActive(false);
                gameoverUi.SetActive(true);
                theEndUi.SetActive(false);
                //gameField.SetActive(false);
                break;
            case UIState.TheEnd:
                curState = UIState.TheEnd;
                titleUi.SetActive(false);
                mainUi.SetActive(false);
                playUi.SetActive(false);
                gameoverUi.SetActive(false);
                theEndUi.SetActive(true);
                //gameField.SetActive(false);
                break;
        }
    }
}
