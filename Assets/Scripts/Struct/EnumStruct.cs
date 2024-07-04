using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnumStruct
{
    public enum UIState
    {
        Title,
        MainMenu,
        Play,
        GameOver,
        TheEnd
    }

    public enum PlayerDefine
    {
        Player,
        Enemy,
    }

    public enum UnitState
    {
        Standing, // 서 있는 중, 생각중
        Running, // 이동
        Attack,  // 공격
        Dying,   // 사망

    }

    public enum UnitType
    {
        Warrior,
        Archer,
        Wizard
    }

    public enum OutPostRow
    {
        Top,
        Middle,
        Bottom,
    }

}
