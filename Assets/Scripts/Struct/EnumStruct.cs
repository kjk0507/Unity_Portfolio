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
        Standing,
        Running,
        Attack,

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
