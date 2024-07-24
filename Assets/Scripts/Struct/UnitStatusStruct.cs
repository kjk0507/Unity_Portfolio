using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumStruct;

namespace UnitStatusStruct
{
    public class Status
    {
        // 이름
        public string name;

        // 가격
        public int cost;

        // 스테이터스
        public int finalHp;
        public int curHp = 1;
        public int defaultHp;
        public float finalSpeed;
        public float curSpeed;
        public float defaultSpeed;

        // 공격 관련
        public int finalAtk;
        public int curAtk;
        public int defaultAtk;
        public float attackRange;
        public float attackSpeed; // 투사체의 경우
        public DamageType damageType;

        // 방어 관련
        public int finalDef;
        public int curDef;
        public int defaultDef;

        // 목표 타겟 관련
        public GameObject outPostLocation;
        public LineType curRow; // 해당 라인
        public GameObject curTarget;
        //public GameObject priorityTarget; // 충돌한 타겟
        public GameObject finalTarget;

        // 이동 여부
        public MoveType moveType;

        // 전초기지 소속 여부
        public bool isInOutPost = false;
        public GameObject curOutPost;

        public Status()
        {

        }

        public Status(OutPostType type)
        {
            this.name = "OutPost";
            this.curHp = 100;
            this.curAtk = 10;
            this.curDef = 0;
            this.curSpeed = 0;
            this.attackRange = 0f;
            this.attackSpeed = 0f;
            this.moveType = MoveType.Stand;

            this.finalHp = curHp;
            this.finalAtk = curAtk;
            this.finalDef = curDef;
            this.finalSpeed = curSpeed;

            this.defaultHp = curHp;
            this.defaultAtk = curAtk;
            this.defaultDef = curDef;
            this.defaultSpeed = curSpeed;
        }

        public Status(UnitType unitType)
        {
            if (unitType == UnitType.Warrior)
            {
                this.name = "전사";
                this.cost = 10;
                this.curHp = 100;
                this.curAtk = 10;
                this.curDef = 0;
                this.curSpeed = 10;
                this.attackRange = 10f;
                this.attackSpeed = 1f;
                this.moveType = MoveType.Move;
                this.damageType = DamageType.Target;
            }
            else if(unitType == UnitType.Archer)
            {
                this.name = "궁수";
                this.cost = 50;
                this.curHp = 70;
                this.curAtk = 20;
                this.curDef = 0;
                this.curSpeed = 6;
                this.attackRange = 20f;
                this.attackSpeed = 50f;
                this.moveType = MoveType.Move;
                this.damageType = DamageType.Target;

            }
            else if(unitType == UnitType.Wizard)
            {
                this.name = "마법사";
                this.cost = 100;
                this.curHp = 100;
                this.curAtk = 40;
                this.curDef = 0;
                this.curSpeed = 3;
                this.attackRange = 35f;
                this.attackSpeed= 35f;
                this.moveType = MoveType.Move;
                this.damageType = DamageType.AOE;
            }
            else if (unitType == UnitType.Core)
            {
                this.name = "코어";
                this.curHp = 100;
                this.curAtk = 0;
                this.curDef = 0;
                this.curSpeed = 0;
                this.attackRange = 0f;
                this.attackSpeed = 0f;
                this.moveType = MoveType.Stand;
                this.damageType = DamageType.Target;
            }
            else if (unitType == UnitType.Orc)
            {
                this.name = "Orc";
                this.curHp = 100;
                this.curAtk = 20;
                this.curDef = 0;
                this.curSpeed = 10;
                this.attackRange = 8f;
                this.attackSpeed = 1f;
                this.moveType = MoveType.Move;
            }
            else if (unitType == UnitType.BoneArcher)
            {
                this.name = "BoneArcher";
                this.curHp = 70;
                this.curAtk = 30;
                this.curDef = 0;
                this.curSpeed = 6;
                this.attackRange = 20f;
                this.attackSpeed = 50f;
                this.moveType = MoveType.Move;
            }
            else if (unitType == UnitType.Destroyer)
            {
                this.name = "Destroyer";
                this.curHp = 200;
                this.curAtk = 50;
                this.curDef = 0;
                this.curSpeed = 6;
                this.attackRange = 20f;
                this.attackSpeed = 35f;
                this.moveType = MoveType.Move;
            }
            else if (unitType == UnitType.Door)
            {
                this.name = "Door";
                this.curHp = 1000;
                this.curAtk = 0;
                this.curDef = 0;
                this.curSpeed = 0;
                this.attackRange = 0f;
                this.attackSpeed = 0f;
                this.moveType = MoveType.Stand;
            }
            else if (unitType == UnitType.Tower)
            {
                this.name = "Tower";
                this.curHp = 500;
                this.curAtk = 30;
                this.curDef = 0;
                this.curSpeed = 3;
                this.attackRange = 60f;
                this.attackSpeed = 100f;
                this.moveType = MoveType.Stand;
            }
            else if (unitType == UnitType.Castle)
            {
                this.name = "본성";
                this.curHp = 500;
                this.curAtk = 0;
                this.curDef = 0;
                this.curSpeed = 0;
                this.attackRange = 0f;
                this.attackSpeed = 0f;
                this.moveType = MoveType.Stand;
            }

            this.defaultHp = curHp;
            this.defaultAtk = curAtk;
            this.defaultDef = curDef;
            this.defaultSpeed = curSpeed;

            ChangeStatus(unitType);
        }

        private void ChangeStatus(UnitType unitType)
        {
            if (unitType == UnitType.Warrior || unitType == UnitType.Archer || unitType == UnitType.Wizard)
            {
                this.finalHp = UnitManager.um_instance.GetExtraHp(unitType) + curHp;
                this.finalAtk = UnitManager.um_instance.GetExtraAtk(unitType) + curAtk;
                this.finalDef = UnitManager.um_instance.GetExtraDef(unitType) + curDef;
                this.finalSpeed = UnitManager.um_instance.GetExtraSpeed(unitType) + curSpeed;

                this.curHp = this.finalHp;
                this.curAtk = this.finalAtk;
                this.curDef = this.finalDef;
                this.curSpeed = this.finalSpeed;
            }
            else
            {
                this.finalHp = curHp;
                this.finalAtk = curAtk;
                this.finalDef = curDef;
                this.finalSpeed = curSpeed;
            }
        }

        public void Damage(int attackPoint)
        {
            // 만약 전초기지에 속해 있다면 전초기지가 대신 데미지 받음
            if (isInOutPost && curOutPost != null)
            {
                curOutPost.GetComponent<OutpostControl>().status.Damage(attackPoint);
                return;
            }

            int damagePoint = attackPoint - this.finalDef;
            if (damagePoint < 0)
            {
                damagePoint = 0;
            }

            this.curHp = this.curHp - damagePoint;
        }        
    }


}
