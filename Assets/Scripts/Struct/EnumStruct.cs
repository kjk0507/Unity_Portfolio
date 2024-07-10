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
}
