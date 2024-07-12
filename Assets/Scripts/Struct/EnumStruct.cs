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
        Idle, // 서 있는 중, 생각중
        Move, // 이동
        Attack,  // 공격
        Death,   // 사망
    }

    public enum UnitType
    {
        Warrior,
        Archer,
        Wizard,
        Orc,
        BoneArcher,
        Destroyer,
        Tower,
        Door,
    }

    public enum LineType
    {
        Top,
        Middle,
        Bottom,
    }

    public enum DamageType
    {
        Target,
        AOE,
    }

    public enum SpawnType
    {
        IniInitial,
        Spawn
    }

    public enum MoveType
    {
        Stand,
        Move
    }

    public enum OutPostType
    {
        InActive,
        Active_Player,
        Active_Enemy,
    }

    public enum OutPostState
    {
        InActive,
        Wait,
        Move
    }

    public enum OutPostPoint
    {
        None,
        Point_00,
        Point_01,
        Point_02,
        Point_03,
        Point_10,
        Point_11,
        Point_12,
        Point_13,
        Point_20,
        Point_21,
        Point_22,
        Point_23,
    }
}
