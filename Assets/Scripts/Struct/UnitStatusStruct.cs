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
        public int curHp;
        public float finalSpeed;
        public float curSpeed;

        // 공격 관련
        public int finalAtk;
        public int curAtk;
        public float attackRange;

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
                this.curDef = 50;
                this.curSpeed = 10;
                this.attackRange = 1f;
            }
            else if(unitType == UnitType.Archer)
            {
                this.name = "Archer";
                this.curHp = 70;
                this.curAtk = 20;
                this.curDef = 20;
                this.curSpeed = 8;
                this.attackRange = 2f;

            }
            else if(unitType == UnitType.Wizard)
            {
                this.name = "Wizard";
                this.curHp = 100;
                this.curAtk = 40;
                this.curDef = 0;
                this.curSpeed = 5;
                this.attackRange = 3f;
            }

            ChangeStatus(unitType);
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
