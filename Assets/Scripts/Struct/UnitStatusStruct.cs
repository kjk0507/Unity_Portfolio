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

        // 스테이터스
        public int finalHp;
        public int curHp = 1;
        public float finalSpeed;
        public float curSpeed;

        // 공격 관련
        public int finalAtk;
        public int curAtk;
        public float attackRange;
        public float attackSpeed; // 투사체의 경우

        // 방어 관련
        public int finalDef;
        public int curDef;

        // 목표 타겟 관련
        public GameObject outPostLocation;
        public OutPostRow curRow;
        public GameObject curTarget;
        //public GameObject priorityTarget; // 충돌한 타겟
        public GameObject finalTarget;

        public Status()
        {

        }

        public Status(UnitType unitType)
        {
            if (unitType == UnitType.Warrior)
            {
                this.name = "Warrior";
                this.curHp = 100;
                this.curAtk = 10;
                this.curDef = 0;
                this.curSpeed = 10;
                this.attackRange = 8f;
                this.attackSpeed = 1f;
            }
            else if(unitType == UnitType.Archer)
            {
                this.name = "Archer";
                this.curHp = 70;
                this.curAtk = 20;
                this.curDef = 0;
                this.curSpeed = 6;
                this.attackRange = 20f;
                this.attackSpeed = 50f;

            }
            else if(unitType == UnitType.Wizard)
            {
                this.name = "Wizard";
                this.curHp = 100;
                this.curAtk = 40;
                this.curDef = 0;
                this.curSpeed = 3;
                this.attackRange = 35f;
                this.attackSpeed= 35f;
            }

            ChangeStatus(unitType);
        }

        public Status(EnemyUnitType type)
        {
            if (type == EnemyUnitType.Orc)
            {
                this.name = "Orc";
                this.curHp = 100;
                this.curAtk = 20;
                this.curDef = 0;
                this.curSpeed = 10;
                this.attackRange = 8f;
                this.attackSpeed = 1f;
            }
            else if (type == EnemyUnitType.BoneSoldier)
            {
                this.name = "BoneSoldier";
                this.curHp = 70;
                this.curAtk = 30;
                this.curDef = 0;
                this.curSpeed = 6;
                this.attackRange = 20f;
                this.attackSpeed = 50f;

            }
            else if (type == EnemyUnitType.Destroyer)
            {
                this.name = "Destroyer";
                this.curHp = 200;
                this.curAtk = 50;
                this.curDef = 0;
                this.curSpeed = 6;
                this.attackRange = 8f;
                this.attackSpeed = 35f;
            }
            else if (type == EnemyUnitType.Door)
            {
                this.name = "Door";
                this.curHp = 1000;
                this.curAtk = 0;
                this.curDef = 0;
                this.curSpeed = 3;
                this.attackRange = 0f;
                this.attackSpeed = 0f;
            }
            else if (type == EnemyUnitType.Tower)
            {
                this.name = "Tower";
                this.curHp = 500;
                this.curAtk = 30;
                this.curDef = 0;
                this.curSpeed = 3;
                this.attackRange = 50f;
                this.attackSpeed = 0f;
            }

            //ChangeStatus(type);
        }

        private void ChangeStatus(UnitType unitType)
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

        public void Damage(int attackPoint)
        {
            int damagePoint = attackPoint - this.finalDef;
            if (damagePoint < 0)
            {
                damagePoint = 0;
            }

            this.curHp = this.curHp - damagePoint;
        }
    }


}
