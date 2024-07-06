using EnumStruct;
using UnityEngine;
using UnitStatusStruct;
using UnityEngine.UI;

public class EnemyStatus : MonoBehaviour
{
    EnemyUnitType enemyType;
    public Status status;
    UnitState curState;
    public Animator animator;
    public Transform firePosition;
    public GameObject hpBar;
    Image hpBarImage;


}
