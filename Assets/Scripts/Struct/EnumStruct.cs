using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnumStruct
{
    public enum UIState
    {
        Title,
        Main,
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
        Core,
        Castle,
    }

    public enum LineType
    {
        Top,
        Middle,
        Bottom,
    }

    public enum DamageType // 단일, 다수
    {
        None,
        Target,
        AOE,
    }

    public enum SpawnType // 초기 생성, 추가 생성
    {
        Initial,
        Spawn
    }

    public enum MoveType // 몬스터 움직임 타입
    {
        Stand,
        Move
    }

    public enum OutPostType // 전초기지 점령 타입
    {
        InActive,
        Active_Player,
        Active_Enemy,
    }

    public enum OutPostState // 전초기지 버튼
    {
        InActive,
        Wait,
        Move
    }

    public enum OutPostPoint // 전초기지 번호
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

    public enum StatusEffect // 이펙트
    {
        Charging,
        AreaHeal,
        Heal,
        ElectricShock
    }

    public enum UIStatusType // 타워, 용병
    {
        Tower,
        Unit
    }

    public enum UIUpgradeButton
    {
        Gold,
        Hp,
        Attack,
        Defence,
        Speed
    }

    public enum UIUnitType
    {
        Unit0,
        Unit1,
        Unit2,
        Unit3
    }

    public enum UpgradeType
    {
        Hp,
        Attack, 
        Defence,
        Speed
    }
}
