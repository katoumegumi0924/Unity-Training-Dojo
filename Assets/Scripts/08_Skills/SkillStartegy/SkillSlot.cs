using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//单个技能栏位
[System.Serializable]
public class SkillSlot
{
    //技能栏位类型，对应哪个键位
    public SkillSlotType slotType;

    //? 为什么使用SerializeField
    [SerializeField]
    private SkillData baseSkill;            //角色原本的技能
    private SkillData overrideSkill;        //武器赋予的技能，覆盖原技能

    //对外只提供当前生效的技能
    public SkillData data => overrideSkill == null ? baseSkill : overrideSkill;

    public float currentCooldown;   //当前冷却时间

    //构造函数
    public SkillSlot(SkillSlotType slotType, SkillData baseSkill)
    {
        this.slotType = slotType;
        this.baseSkill = baseSkill;
    }

    //装备武器时调用
    public void SetOverride(SkillData newSkill)
    {
        overrideSkill = newSkill;
        currentCooldown = 0;            //换武器重置技能冷却
    }

    //卸下武器调用
    public void ClearOverride()
    {
        overrideSkill = null;
        currentCooldown = 0;
    }

    //辅助属性 技能是否冷却
    public bool IsReady => currentCooldown <= 0;

    //计算技能当前冷却时间
    public void Tick(float deltaTime)
    {
        if (currentCooldown > 0)
        {
            currentCooldown -= deltaTime;
        }
    }

    //技能进入冷却时间
    public void StartCooldown()
    {
        if (data != null)
        {
            currentCooldown = data.cooldown;
        }
    }
}
