using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//技能静态数据，通过ScriptableObject配置
[CreateAssetMenu(fileName = "NewSkill", menuName ="Skills/Skill Data")]
public class SkillData : ScriptableObject
{
    [Header("基本信息")]
    public string skillName;
    public Sprite icon;
    [TextArea] public string description;

    [Header("战斗参数")]
    public float cooldown = 2.0f;       //冷却时间
    public float manaCost = 10;         //蓝耗
    public float castDuration = 1f;     //施法时间，期间玩家无法移动
    public float damageDelay = 0.5f;    //施法前摇时间
    public int damage;                  //伤害
    public LayerMask targetLayer;       //技能目标层

    [Header("释放规则")]
    public bool requireTarget;          //是否需要选择目标
    public float castRange;             //施法范围

    [Header("表现")]
    public string animTriggerName;
    public GameObject skillEff;         //技能特效
    public GameObject hitEff;           //击中特效

    [Header("投射物技能属性")]
    public float flySpeed;              //投射物技能飞行速度
    public Vector3 spawnOffset = new Vector3(0, 0, 0.5f);       //投射物技能发射位置偏移量

    [Header("核心逻辑 (插卡带)")]
    // 策略模式的精髓
    // 不在这里写逻辑，而是持有一个逻辑策略的引用
    public SkillStrategy strategy;

}

//技能槽技能类型，对应不同的键位
public enum SkillSlotType
{
    Primary = 0,    // 普攻 (通常由武器提供)
    Skill_1 = 1,    // 技能1 (可能是武器技能，如 Q)
    Class_1 = 2,    // 职业技能1 (角色自带，如 E)
    Ultimate = 3    // 大招 (角色自带，如 R)
}
